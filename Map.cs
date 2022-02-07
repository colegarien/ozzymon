using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Ozzymon
{
    internal class Map
    {
        // contains all game areas
        List<Area> areas = new List<Area>();
        // load textures
        ContentManager content;
        // the player
        Player player;

        // gametime clock
        GameClock worldTime;

        // temporary font
        SpriteFont debug;

        // first area in the list
        int currentArea = 0;
        // change area to toChangeTo
        bool changingArea = true;
        // area to goto
        string toChangeTo = "";

        // for transitioning from area to area or from between battling and not
        Transition transition;

        Inventory startMenu;

        // this will hold all npc's in the game and will be ordered by area
        List<NPC> npcList = new List<NPC>();
        // npcs in current area so update em
        List<int> currNpcs = new List<int>();

        public Map(ContentManager cM, SpriteBatch b)
        {
            // so change load content
            content = cM;

            // load temp font
            debug = content.Load<SpriteFont>("Font/debugFont");

            //FORTESTIN
            List<string> temp = new List<string>();
            temp.Add("moveup");
            temp.Add("idle1");
            temp.Add("moveleft");
            temp.Add("idle1");
            temp.Add("movedown");
            temp.Add("idle1");
            temp.Add("moveright");
            temp.Add("idle1");
            temp.Add("moveup");
            temp.Add("idle1");
            temp.Add("lookup");
            temp.Add("idle1");
            temp.Add("lookleft");
            temp.Add("idle1");
            temp.Add("lookdown");
            temp.Add("idle1");
            temp.Add("lookright");
            temp.Add("idle1");
            npcList.Add(new NPC(cM, "Lil Timmy", "roomone", "OldMale1", new Vector2(1, 1), 3, temp));
            npcList.Add(new NPC(cM, "Big ol Bastard", "roomone", "Male1", new Vector2(1, 2), 3, temp));
            //FORTESTIN

            // sets up players animations and shifts them so they look right
            player = new Player(cM);

            // MAKE IT ADD ALL AREAS TO LIST
            // Will set the name of area, load layout and textures and everything
            areas.Add(new Area("buildingtest", cM));
            areas.Add(new Area("roomone", cM));
            areas.Add(new Area("roomtwo", cM));
            areas.Add(new Area("roomthree", cM));

            // makes a new clock that starts at midnight (in seconds) and the timefactor
            worldTime = new GameClock(12 * 3600, 10);

            // give it content manager so it can load transition things
            transition = new Transition(cM);

            startMenu = new Inventory(cM);
        }

        public bool Update(GameTime gameTime)
        {
            if (startMenu.isClosed())
            {
                // if the area needs to change then change it
                if (changingArea)
                {
                    changeArea();
                }

                if (transition.isDone())
                {
                    // updates time
                    worldTime.Update(gameTime);

                    // set the animation and updates it and use the wall layout and stuff to navigate
                    player.Update(gameTime, areas[currentArea], npcList, currNpcs);
                    // updates npc's in the area
                    for (int i = 0; i < currNpcs.Count; ++i)
                        npcList[currNpcs[i]].Update(gameTime, areas[currentArea], player, npcList, currNpcs);
                    // this will update the area to see if the player is on a door and
                    // if it will move the area around the player and if he is on a door
                    // then it will change area to the doors area it leads to
                    changingArea = areas[currentArea].Update(gameTime, player);



                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start))
                        startMenu.Start();
                }
                // if the area is an outdoor area then light according to the worlds light BASED ON TIME
                if (areas[currentArea].isOutdoor())
                    areas[currentArea].setSky(worldTime.getColor());

                transition.Update(gameTime);
            }
            else
            {
                if (startMenu.Update(gameTime))
                    return true;
            }

            // end of method and not exiting;
            return false;
        }

        // WILL HAVE DOOR LOC AND SEND WHERE PLAYER COMES FROM
        private void changeArea()
        {
            // the follwing crap is just to check if the area im headed is outdoors vvv
            int nextArea = 0;
            bool founding = false;
            string temp = areas[currentArea].getTileLayout()[player.getX(), player.getY()].getWhereTo();
            while (nextArea < areas.Count && !founding)
            {
                if (temp == areas[nextArea].getName())
                    founding = true;
                else
                    nextArea++;
            }
            if (!founding)
                nextArea = currentArea;
            // end of crap to check if im headed outdoors or not ^^^

            if (transition.isDone() && (!areas[currentArea].isOutdoor() || !areas[nextArea].isOutdoor()))
            {
                transition.Start();
            }
            else if (transition.isInbetween() || (areas[currentArea].isOutdoor() && areas[nextArea].isOutdoor()))
            {
                // gets where the door the player is on leads to
                toChangeTo = areas[currentArea].getTileLayout()[player.getX(), player.getY()].getWhereTo();

                // no longer need to change area because im doing it right now
                changingArea = false;
                // the area hasnt been found yet
                bool found = false;
                // prime the pump
                int i = 0;
                // run through till i found the area or im at the end of the list
                while (i < areas.Count && !found)
                {
                    // if the area has the name im looking for then i found it IF NOT then look at the next item
                    if (areas[i].getName() == toChangeTo)
                        found = true;
                    else
                        ++i;
                }

                // IF WHEREFROM NO SET THEN MAKE THE POS TO START FROM MIDDLE
                // if area not in the list then just goto the first area on the list
                if (!found) { i = 0; }
                // the current area is the area where we are going to
                currentArea = i;
                // move to the new area and move the player and tiles to the right place
                areas[currentArea].ChangeTo(ref player);
                // now that we are here, it is where we are from if you go to another area
                player.setWhereFrom(areas[currentArea].getName());

                // get the list of npcs in this area
                currNpcs.Clear();
                for (int npcs = 0; npcs < npcList.Count; ++npcs)
                {
                    if (npcList[npcs].getArea() == areas[currentArea].getName())
                        currNpcs.Add(npcs);
                }

                transition.setState("entering");
            }
        }

        // gets the skyColor of the current area
        public Color getSky() { return areas[currentArea].getSky(); }

        // for drawing in rigth order
        private void organizeNpcs()
        {
            for (int i = 0; i < currNpcs.Count - 1; ++i)
            {
                for (int k = i + 1; k < currNpcs.Count; ++k)
                {
                    if (npcList[currNpcs[i]].getY() > npcList[currNpcs[k]].getY())
                    {
                        int temp = currNpcs[i];
                        currNpcs[i] = currNpcs[k];
                        currNpcs[k] = temp;
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch b)
        {
            // this is so they willn't draw wrongs
            organizeNpcs();
            // draw the background the the player then the foreground
            areas[currentArea].Draw(b, player);
            // draw the ones behind player
            for (int i = 0; i < currNpcs.Count; ++i)
                npcList[currNpcs[i]].Draw(b, areas[currentArea].getSky());
            player.Draw(b, areas[currentArea].getSky());
            // ones in front of player
            areas[currentArea].DrawWalls(b, player, npcList, currNpcs);

            // THIS WILL BE HUD S---
            b.Begin();
            // DROP SHADOW
            b.DrawString(debug, "Player X: " + player.getX() + ", Player Y: " + player.getY(), Vector2.Zero + new Vector2(0.5f, 0.5f), /*Color.Black*/Color.White);
            b.DrawString(debug, "R: " + areas[currentArea].getSky().R + " G: " + areas[currentArea].getSky().G + " B: " + areas[currentArea].getSky().B, new Vector2(0.5f, 25.5f), /*Color.Black*/Color.White);
            b.DrawString(debug, "Area: " + areas[currentArea].getName(), new Vector2(0.5f, 50.5f), /*Color.Black*/Color.White);
            b.DrawString(debug, "Where Player From: " + player.getWhereFrom(), new Vector2(0.5f, 75.5f), Color.White);

            // NORMAL TEXT
            b.DrawString(debug, "Player X: " + player.getX() + ", Player Y: " + player.getY(), Vector2.Zero, /*Color.White*/ new Color(255 - areas[currentArea].getSky().R, 255 - areas[currentArea].getSky().G, 255 - areas[currentArea].getSky().B));
            b.DrawString(debug, "R: " + areas[currentArea].getSky().R + " G: " + areas[currentArea].getSky().G + " B: " + areas[currentArea].getSky().B, new Vector2(0, 25), /*Color.White*/new Color(255 - areas[currentArea].getSky().R, 255 - areas[currentArea].getSky().G, 255 - areas[currentArea].getSky().B));
            b.DrawString(debug, "Area: " + areas[currentArea].getName(), new Vector2(0f, 50), /*Color.White*/new Color(255 - areas[currentArea].getSky().R, 255 - areas[currentArea].getSky().G, 255 - areas[currentArea].getSky().B));
            b.DrawString(debug, "Where Player From: " + player.getWhereFrom(), new Vector2(0f, 75), /*Color.White*/new Color(255 - areas[currentArea].getSky().R, 255 - areas[currentArea].getSky().G, 255 - areas[currentArea].getSky().B));


            // Other S---
            // time
            b.DrawString(debug, DateTime.Now.ToString("HH:mm tt"), new Vector2(0f, 100f), Color.White);
            // stuff
            b.DrawString(debug, "" + worldTime.getTime(), new Vector2(0f, 125f), Color.White);
            // FPS
            b.DrawString(debug, "FPS: " + Math.Round(1.0 / gameTime.ElapsedGameTime.TotalSeconds), new Vector2(0f, 150f), Color.White);

            b.End();

            if (!startMenu.isClosed())
            {
                // draw the Inventory and such
                startMenu.Draw(b, worldTime.getTime());
            }

            // Draw Transitions
            transition.Draw(b);
        }
    }
}
