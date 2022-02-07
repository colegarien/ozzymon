using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Ozzymon
{
    internal class NPC
    {
        string name;
        string areaName;

        // the current tile the player is on
        Vector2 pos = new Vector2(1, 1);
        // where the player is walking to
        Vector2 target = new Vector2(1, 1);

        // content loader
        ContentManager content;
        // animations for the player
        List<AnimatedSprite> walk = new List<AnimatedSprite>();
        List<AnimatedSprite> idle = new List<AnimatedSprite>();
        List<AnimatedSprite> action = new List<AnimatedSprite>();

        // player isn't walking and isn't trying to interact
        bool walking = false, actioning = false;

        // N E S W : 0 1 2 3
        short direction = 0;
        // walk, idle, action
        string currentAni = "idle";

        // skin of npc
        string tex;

        // what the npc will do
        // moveup movedown moveleft moveright
        // lookup lookdown lookleft lookright
        // idleSECONDStoBEidle ex: idle10, this is idle for 10 seconds
        // doaction
        List<string> instructions = new List<string>();

        float delayTime = 0;
        float currTime = 0;

        // current instruction to do
        int currAction = 0;

        public NPC(ContentManager cM, string namen, string area, string texture, Vector2 positon, short direct, List<string> instruct)
        {
            // npc name
            name = namen;

            // area where the npc tromps aboot
            areaName = area;

            // npc skin
            tex = texture;

            // where the npc is in its tromping grounds
            target = pos = positon;

            // where the npc is facing
            direction = direct;

            // what it do
            instructions = instruct;

            // content loader
            content = cM;

            // the middle of the screen where the the player is drawn
            Vector2 baseDraw = new Vector2(4 * 75 + (75f / 2), 6 * 52 + (52));

            // texture IS name of npc skin

            // sets all the animations and sets the appropriate shift so it looks right
            walk.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/North/Walk"), baseDraw, 20));
            walk.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/East/Walk"), baseDraw, 20));
            walk[1].shiftOffset(new Vector2(8, 0));
            walk.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/South/Walk"), baseDraw, 20));
            walk.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/West/Walk"), baseDraw, 20));
            walk[3].shiftOffset(new Vector2(-8, 0));

            idle.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/North/Idle"), baseDraw, 25, 2));
            idle.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/East/Idle"), baseDraw, 25, 2));
            idle.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/South/Idle"), baseDraw, 25, 2));
            idle.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/West/Idle"), baseDraw, 25, 2));

            action.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/North/Action"), baseDraw, 11));
            action.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/East/Action"), baseDraw, 11));
            action[1].shiftOffset(new Vector2(-3, 0));
            action.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/South/Action"), baseDraw, 11));
            action.Add(new AnimatedSprite(content.Load<Texture2D>("NPC/" + texture + "/West/Action"), baseDraw, 11));
            action[3].shiftOffset(new Vector2(3, 0));



            // SHIFT THE ANIMATIONS SO THEY R RIGHT
            for (int i = 0; i < 4; i++)
            {
                walk[i].setShifter(-1);
                idle[i].setShifter(-1);
                action[i].setShifter(-1);
            }
        }

        public void setDirection(string dir)
        {
            // checks which direction to change to and translates it to 0 1 2 3
            if (dir == "north")
                direction = 0;
            else if (dir == "east")
                direction = 1;
            else if (dir == "south")
                direction = 2;
            else if (dir == "west")
                direction = 3;
            else
                direction = 0;
        }

        // sets players position
        public void setPos(Vector2 p)
        {
            pos = p;
            target = p;
        }

        // gets  the X and Y
        public int getX()
        {
            return (int)Math.Round((double)target.X);
        }
        public int getY()
        {
            return (int)Math.Round((double)target.Y);
        }

        // MODIFY
        public void Update(GameTime gameTime, Area a, Player player, List<NPC> npcList, List<int> curNpcs)
        {
            // checks which animation the player is doing and updates it
            if (currentAni == "walk")
                walk[direction].Update(gameTime);
            else if (currentAni == "idle")
                idle[direction].Update(gameTime);
            else if (currentAni == "action")
                action[direction].Update(gameTime);

            if (currTime >= delayTime)
            {
                // MAKE MORE DETAILED TO NOT WALK OUTT MAP AND TO NOT ONLY WALK THROUGH 0's ON LAYOUT
                // MOVE THE MAP

                // checks where player can move the moves him if he can then do
                // also checks if player can interact and if he can it does
                if (CanMoveUp(a.getLayout(), a.getWallLayout(), player.getTargetPos()) && notNPC(new Vector2(0, -1), npcList, curNpcs))
                {
                    MoveUp();
                }
                else if (!actioning && !walking && (instructions[currAction] == "moveup" || instructions[currAction] == "lookup")) { nextAction(); direction = 0; }
                else if (CanMoveDown(a.getLayout(), a.getWallLayout(), a.getHeight(), player.getTargetPos()) && notNPC(new Vector2(0, 1), npcList, curNpcs))
                {
                    MoveDown();
                }
                else if (!actioning && !walking && ((instructions[currAction] == "movedown" || instructions[currAction] == "lookdown"))) { nextAction(); direction = 2; }
                else if (CanMoveLeft(a.getLayout(), a.getWallLayout(), player.getTargetPos()) && notNPC(new Vector2(-1, 0), npcList, curNpcs))
                {
                    MoveLeft();
                }
                else if (!actioning && !walking && (instructions[currAction] == "moveleft" || instructions[currAction] == "lookleft")) { nextAction(); direction = 3; }
                else if (CanMoveRight(a.getLayout(), a.getWallLayout(), a.getWidth(), player.getTargetPos()) && notNPC(new Vector2(1, 0), npcList, curNpcs))
                {
                    MoveRight();
                }
                else if (!actioning && !walking && (instructions[currAction] == "moveright" || instructions[currAction] == "lookright")) { nextAction(); direction = 1; }
                else if (CanAction())
                {
                    DoAction();
                }
                else if (!actioning && !walking && instructions[currAction].Substring(0, 4) == "idle")
                {
                    // try and gather time to delay and s---
                    try
                    {
                        delayTime = float.Parse(instructions[currAction].Substring(4));
                        currTime = 0;
                    }
                    catch (Exception e)
                    {
                        string temp = e.Message;
                        delayTime = 0;
                        currTime = 0;
                        nextAction();
                    }
                }

                if (walking && (walk[direction].tick))
                    Walk();
                if (actioning)
                {
                    Action();
                }
            }
            else
            {
                currentAni = "idle";
                updateTimer(gameTime);
            }

            Vector2 baseDraw = new Vector2(4 * 75 + (75f / 2), 6 * 52 + (52)) + (new Vector2((pos.X - 1) * 75 + (player.getRawX() * 75), (pos.Y - 1) * 52 + (player.getRawY() * 52)) - new Vector2(player.getX() * 75, player.getY() * 52));
            for (int m = 0; m <= 3; ++m)
            {
                walk[m].pos = baseDraw;
                action[m].pos = baseDraw;
                idle[m].pos = baseDraw;
            }
        }

        private bool notNPC(Vector2 nextLoc, List<NPC> npcList, List<int> curNpcs)
        {
            for (int i = 0; i < curNpcs.Count; ++i)
            {
                if (npcList[curNpcs[i]].getName() != name && npcList[curNpcs[i]].getX() == pos.X + nextLoc.X && npcList[curNpcs[i]].getY() == pos.Y + nextLoc.Y)
                    return false;
            }

            return true;
        }

        public string getName() { return name; }

        // where is this guy at
        public string getArea() { return areaName; }

        private void updateTimer(GameTime gameTime)
        {
            currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currTime >= delayTime)
                nextAction();
        }
        private void nextAction()
        {
            ++currAction;
            if (currAction >= instructions.Count)
                currAction = 0;
        }

        // MODIFY
        private void Action()
        {
            // EVENTUALLY HAVE PICKING ITEMS UP AND TALKING TO PEOPLE HERE
            // if the action is over then stop actioning, reset the animation, return to idle, and reset it
            if (action[direction].getFrame() >= 10)
            {
                actioning = false;
                action[direction].Reset();
                nextAction();
                currentAni = "idle";
                idle[direction].Reset();
            }
        }

        // MODIFY
        private bool CanAction()
        {
            // EVENTUALLY ADD IF A NPC OR ITEM IS IN FRONT OF THE PLAYER
            // if the player is idle and the appropriate buttons are pressed
            bool b = !actioning && !walking && (instructions[currAction] == "doaction");
            return b;
        }

        // starts the animation of actioning
        public void DoAction()
        {
            actioning = true;
            currentAni = "action";
        }

        // true if animation has changed this game tick
        public bool aniTick()
        {
            if (!walking)
                return false;
            else
                return walk[direction].tick;
        }

        //translates the 0 1 2 3 to north east south west
        public string getDirection()
        {
            if (direction == 0)
                return "north";
            else if (direction == 1)
                return "east";
            else if (direction == 2)
                return "south";
            else if (direction == 3)
                return "west";
            else
                return "unknown";
        }

        // LOOK AT THIS AND LOOK IN AREA WHERE IT IS SHIFTED
        public float getRawX()
        {
            float raw = 1 - ((float)pos.X - (float)target.X);
            if (direction == 3)
                raw = 1 - Math.Abs((float)target.X - (float)pos.X);
            if (raw != 0)
                return raw;
            else return 1;
        }
        public float getRawY()
        {
            float raw = 1f - ((float)pos.Y - (float)target.Y);
            if (direction == 0)
                raw = 1f - Math.Abs((float)target.Y - (float)pos.Y);
            if (raw != 0)
                return raw;
            else return 1;
        }

        private void Walk()
        {
            // the walk rate is move 1 unit in 10 frames
            float walkRateY = 1f / (10f), walkRateX = 1f / (10f);
            // if havent reached the target yet then move else stop moving
            if (target != pos)
            {
                if (direction == 2)
                {
                    if (pos.Y + walkRateY <= target.Y)
                        pos.Y += walkRateY;
                    else
                        pos.Y = target.Y;
                }
                else if (direction == 1)
                {
                    if (pos.X + walkRateX <= target.X)
                        pos.X += walkRateX;
                    else
                        pos.X = target.X;
                }
                else if (direction == 0)
                {
                    if (pos.Y - walkRateY >= target.Y)
                        pos.Y -= walkRateY;
                    else
                        pos.Y = target.Y;
                }
                else if (direction == 3)
                {
                    if (pos.X - walkRateX >= target.X)
                        pos.X -= walkRateX;
                    else
                        pos.X = target.X;
                }
            }
            else
            {
                // fixes left left foot walk
                if (walk[direction].getFrame() == 11)
                    walk[direction].setFrame(10);
                walking = false;
                nextAction();
                currentAni = "idle";
                idle[direction].Reset();
            }
        }

        // Checks layouts and controls to see if the player can move and if they can then move em
        #region CanMoveANDMove


        // MODIFY
        private bool CanMoveUp(int[,] l, Tile[,] walls, Vector2 playerPos)
        {
            bool r = (!actioning && !walking && pos.Y - 1 >= 0 && !(pos.Y - 1 == playerPos.Y && pos.X == playerPos.X) && !walls[(int)pos.X, (int)pos.Y - 1].isADoor() && !(l[(int)pos.X, (int)pos.Y - 1] == 1 && (walls[(int)pos.X, (int)pos.Y - 1].isAWall() || walls[(int)pos.X, (int)pos.Y - 1].isAWater())) && (instructions[currAction] == "moveup"));

            return r;
        }
        private void MoveUp()
        {
            currentAni = "walk";
            walking = true;
            direction = 0;
            target = new Vector2(pos.X, pos.Y - 1);
            if (walk[direction].getFrame() != 0 && walk[direction].getFrame() != 10)
                walk[direction].Reset();
        }

        // MODIFY
        private bool CanMoveDown(int[,] l, Tile[,] walls, int h, Vector2 playerPos)
        {
            bool r = (!actioning && !walking && pos.Y + 1 < h && !(pos.Y + 1 == playerPos.Y && pos.X == playerPos.X) && !(l[(int)pos.X, (int)pos.Y + 1] == 1 && !walls[(int)pos.X, (int)pos.Y + 1].isADoor() && (walls[(int)pos.X, (int)pos.Y + 1].isAWall() || walls[(int)pos.X, (int)pos.Y + 1].isAWater())) && (instructions[currAction] == "movedown"));

            return r;
        }
        private void MoveDown()
        {
            currentAni = "walk";
            walking = true;
            direction = 2;
            target = new Vector2(pos.X, pos.Y + 1);
            if (walk[direction].getFrame() != 0 && walk[direction].getFrame() != 10)
                walk[direction].Reset();
        }

        // MODIFY
        private bool CanMoveLeft(int[,] l, Tile[,] walls, Vector2 playerPos)
        {
            bool r = (!actioning && !walking && pos.X - 1 >= 0 && !(pos.X - 1 == playerPos.X && pos.Y == playerPos.Y) && !walls[(int)pos.X - 1, (int)pos.Y].isADoor() && !(l[(int)pos.X - 1, (int)pos.Y] == 1 && (walls[(int)pos.X - 1, (int)pos.Y].isAWall() || walls[(int)pos.X - 1, (int)pos.Y].isAWater())) && (instructions[currAction] == "moveleft"));

            return r;
        }
        private void MoveLeft()
        {
            currentAni = "walk";
            walking = true;
            direction = 3;
            target = new Vector2(pos.X - 1, pos.Y);
            if (walk[direction].getFrame() != 0 && walk[direction].getFrame() != 10)
                walk[direction].Reset();
        }


        // MODIFY
        private bool CanMoveRight(int[,] l, Tile[,] walls, int w, Vector2 playerPos)
        {
            bool r = (!actioning && !walking && pos.X + 1 < w && !(pos.X + 1 == playerPos.X && pos.Y == playerPos.Y) && !walls[(int)pos.X + 1, (int)pos.Y].isADoor() && !(l[(int)pos.X + 1, (int)pos.Y] == 1 && (walls[(int)pos.X + 1, (int)pos.Y].isAWall() || walls[(int)pos.X + 1, (int)pos.Y].isAWater()))) && (instructions[currAction] == "moveright");

            return r;
        }
        private void MoveRight()
        {
            currentAni = "walk";
            walking = true;
            direction = 1;
            target = new Vector2(pos.X + 1, pos.Y);
            if (walk[direction].getFrame() != 0 && walk[direction].getFrame() != 10)
                walk[direction].Reset();
        }
        #endregion

        public void Draw(SpriteBatch b, Color sky)
        {

            // check which animation to draw then draw it
            if (currentAni == "walk")
            {
                walk[direction].Draw(b, sky);
            }
            else if (currentAni == "idle")
            {
                idle[direction].Draw(b, sky);
            }
            else if (currentAni == "action")
            {
                action[direction].Draw(b, sky);
            }
        }
    }
}
