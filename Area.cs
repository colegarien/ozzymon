using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;

namespace Ozzymon
{
    internal class Area
    {
        // basic 0 1 layout
        int[,] layout;
        // texture and important info
        Tile[,] backTiles, wallTiles;
        // size of area
        int width = 0, height = 0;
        // name of area
        string name = "";
        ContentManager content;
        // indooor or outdooor
        bool isIndoors = false;

        // if not from anywhere then put player here
        Vector2 defaultPos = new Vector2(1, 1);

        // ADJUST BASED ON CURRENT TIME IF !isInDoors
        Color skyColor = Color.White;

        public Area(string n, ContentManager cM)
        {
            name = n;
            content = cM;

            // load up the areas info from file
            loadArea(n);
        }

        public bool isOutdoor() { return !isIndoors; }
        public void setSky(Color clr)
        {
            skyColor = clr;
        }

        public void loadArea(string name)
        {
            // open up the area file
            StreamReader file = new StreamReader(@"Content/Areas/" + name + ".ozz");

            // Width
            width = int.Parse(file.ReadLine());

            // Height
            height = int.Parse(file.ReadLine());

            // isIndoors
            isIndoors = bool.Parse(file.ReadLine());

            // if its indoors then get R then G then B
            if (isIndoors)
                skyColor = new Color(int.Parse(file.ReadLine()), int.Parse(file.ReadLine()), int.Parse(file.ReadLine()));

            // get default start pos
            defaultPos = new Vector2(int.Parse(file.ReadLine()), int.Parse(file.ReadLine()));

            // get the layout
            layout = new int[width, height];
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    layout[x, y] = int.Parse(file.ReadLine());

            // load the backTiles
            backTiles = new Tile[width, height];

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int numFrames = 0;
                    bool animated = bool.Parse(file.ReadLine());
                    string tileType = file.ReadLine();
                    Vector2 spritePos = new Vector2(int.Parse(file.ReadLine()), int.Parse(file.ReadLine()));
                    if (animated)
                        numFrames = int.Parse(file.ReadLine());
                    bool water = bool.Parse(file.ReadLine());
                    bool wall = bool.Parse(file.ReadLine());
                    bool wild = bool.Parse(file.ReadLine());
                    bool door = bool.Parse(file.ReadLine());
                    int minL = int.Parse(file.ReadLine());
                    int maxL = int.Parse(file.ReadLine());
                    string whereTo = file.ReadLine();
                    Vector2 doorAdjust = new Vector2(int.Parse(file.ReadLine()), int.Parse(file.ReadLine()));
                    if (animated)
                    {
                        backTiles[x, y] = new Tile(new AnimatedSprite(content.Load<Texture2D>(getTexture(tileType, water, wall, wild, door, x, y)), spritePos, numFrames),
                            water, wall, wild, door, minL, maxL, whereTo, doorAdjust);
                        backTiles[x, y].getAniSprite().setFPS(20);
                    }
                    else
                    {
                        backTiles[x, y] = new Tile(new Sprite(content.Load<Texture2D>(getTexture(tileType, water, wall, wild, door, x, y)), spritePos),
                        water, wall, wild, door, minL, maxL, whereTo, doorAdjust);
                    }
                }
            }


            // load the wallTiles
            wallTiles = new Tile[width, height];
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    int numFrames = 0;
                    bool animated = bool.Parse(file.ReadLine());
                    string tileType = file.ReadLine();
                    Vector2 spritePos = new Vector2(int.Parse(file.ReadLine()), int.Parse(file.ReadLine()));
                    if (animated)
                        numFrames = int.Parse(file.ReadLine());
                    bool water = bool.Parse(file.ReadLine());
                    bool wall = bool.Parse(file.ReadLine());
                    bool wild = bool.Parse(file.ReadLine());
                    bool door = bool.Parse(file.ReadLine());
                    int minL = int.Parse(file.ReadLine());
                    int maxL = int.Parse(file.ReadLine());
                    string whereTo = file.ReadLine();
                    Vector2 doorAdjust = new Vector2(int.Parse(file.ReadLine()), int.Parse(file.ReadLine()));
                    if (animated)
                    {
                        wallTiles[x, y] = new Tile(new AnimatedSprite(content.Load<Texture2D>(getTexture(tileType, water, wall, wild, door, x, y)), spritePos, numFrames),
                            water, wall, wild, door, minL, maxL, whereTo, doorAdjust);
                        wallTiles[x, y].getAniSprite().setFPS(20);
                    }
                    else
                    {
                        wallTiles[x, y] = new Tile(new Sprite(content.Load<Texture2D>(getTexture(tileType, water, wall, wild, door, x, y)), spritePos),
                        water, wall, wild, door, minL, maxL, whereTo, doorAdjust);
                    }
                }
            }

            // seal it up
            file.Close();

        }

        private string getTexture(string n, bool water, bool wall, bool wild, bool door, int x, int y)
        {
            // set the default folder for tiles
            string type = "Tiles/";

            // if tile is empty
            if (n == "empty")
                type += "empty";
            else if (water) // if the tile isWater
                type += "Water/waterTile";
            else if (wild) // if the tile is Wilderness
                type += "Grass/wildGrassTile";
            else if (n == "grass") // if it is grass
            {
                // change to grass folder
                type += "Grass/";
                if (door || layout[x, y] == 0) // if is a door or empty space
                    type += "grassTile";
                else if (wall) // if its a wall then make it to fence
                    if (x + 1 < width && y + 1 < height && layout[x, y] == 1 && layout[x + 1, y] == 1 && layout[x, y + 1] == 1)// top left
                        type += "rightdownFence";
                    else if (x - 1 >= 0 && y + 1 < height && layout[x, y] == 1 && layout[x - 1, y] == 1 && layout[x, y + 1] == 1)// top right
                        type += "leftdownFence";
                    else if (x + 1 < width && y - 1 >= 0 && layout[x, y] == 1 && layout[x + 1, y] == 1 && layout[x, y - 1] == 1)// bottom left
                        type += "rightupFence";
                    else if (x - 1 >= 0 && y - 1 >= 0 && layout[x, y] == 1 && layout[x - 1, y] == 1 && layout[x, y - 1] == 1)// bottom left
                        type += "leftupFence";
                    else if ((y - 1 >= 0 && layout[x, y] == 1 && layout[x, y - 1] == 1) || (y + 1 < height && layout[x, y] == 1 && layout[x, y + 1] == 1)) // vertical
                        type += "vertFence";
                    else if (layout[x, y] == 1)
                        type += "horizFence";
                    else
                        type += "grassTile";
                else
                    type += "grassTile";
            }
            else if (n == "room") // if it is the room tex set
            {
                if (!wall) // if it is a piece of floor
                    type += "Building/tileFloor";
                else // otherwise it's a wall
                    type += "Building/plasterWall";
            }
            else if (n == "buildingred") // outdoor red building set
            {
                if (door) // if is the door
                    type += "Building/buildingRedDoor";
                else // if is wall
                    type += "Building/buildingRedWall";


            }
            else if (n == "buildingblue") // outdoor blue set
            {
                if (door) // is door
                    type += "Building/buildingBlueDoor";
                else // is wall
                    type += "Building/buildingBlueWall";


            }
            else if (n == "buildinggreen") // outdoor green set
            {
                if (door) // is door
                    type += "Building/buildingGreenDoor";
                else // is wall
                    type += "Building/buildingGreenWall";
            }
            else if (n == "water") // if is water set
                type += "Water/waterTile";
            else if (n == "misc") // if is misc set
                type += "Misc/untitled";
            else // then make it empty if nothing else
                type += "empty";

            return type;
        }

        public Tile[,] getWallLayout() { return wallTiles; }
        public Color getSky() { return skyColor; }

        // change the area
        public void ChangeTo(ref Player p)
        {
            // if the door where the player came from is found
            bool found = false;
            int x = 0;
            int y = 0;

            // run through till the door is found and it is where the player came from
            while (x < width && !found)
            {
                while (y < height && !found)
                {
                    if (backTiles[x, y].isADoor() && backTiles[x, y].getWhereTo() == p.getWhereFrom())
                        found = true;
                    else
                        ++y;
                }
                if (!found)
                {
                    ++x;
                    y = 0;
                }
            }

            // if the door was found then move the player to the door and shift it, then change direction of player
            // ELSE move player to default position and make him face south
            if (found)
            {
                p.setPos(new Vector2(x, y) + backTiles[x, y].getDoorShift());
                // set direction like came through the door
                Vector2 dir = backTiles[x, y].getDoorShift();
                if (dir.X == 1)
                    p.setDirection("east");
                else if (dir.X == -1)
                    p.setDirection("west");
                else if (dir.Y == 1)
                    p.setDirection("south");
                else if (dir.Y == -1)
                    p.setDirection("north");
                else
                    p.setDirection("south");
            }
            else
            { // LATER MAKE AN AREA HAVE A START POS WHICH WHILL BE SET HERE
                //p.setPos(new Vector2(width / 2, height / 2));
                p.setPos(defaultPos);
                p.setDirection("south");
            }

            // move the tile around the player into the proper positions
            for (int i = -8; i <= 8; i++)
            {
                for (int k = -8; k <= 8; k++)
                {
                    int xIndex = p.getX() + 1 + i;
                    int yIndex = p.getY() + 1 + k;
                    if (xIndex >= 0 && yIndex >= 0 && xIndex < width && yIndex < height)
                    {
                        // Postion is from bottom vert and middle horiz THEN add player raw pos
                        Vector2 baseDraw = new Vector2(((4 + i) * 75 + (75 / 2)) + (75 * p.getRawX()), ((6 + k) * 52 + (52)) + (52 * p.getRawY()));
                        if (layout[xIndex, yIndex] != 0)
                            wallTiles[xIndex, yIndex].setPos(baseDraw);

                        backTiles[xIndex, yIndex].setPos(baseDraw);
                    }
                }
            }
        }

        public string getName()
        {
            return name;
        }
        public int[,] getLayout()
        {
            return layout;
        }
        public int getWidth()
        {
            return width;
        }
        public int getHeight()
        {
            return height;
        }
        public Tile[,] getTileLayout() { return backTiles; }

        public bool Update(GameTime gameTime, Player p)
        {

            #region playing with sky color
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad7) && skyColor.R + 1 < 256)
                skyColor.R += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4) && skyColor.R - 1 >= 0)
                skyColor.R -= 1;

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad9) && skyColor.B + 1 < 256)
                skyColor.B += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6) && skyColor.B - 1 >= 0)
                skyColor.B -= 1;

            if (Keyboard.GetState().IsKeyDown(Keys.NumPad8) && skyColor.G + 1 < 256)
                skyColor.G += 1;
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5) && skyColor.G - 1 >= 0)
                skyColor.G -= 1;
            #endregion

            // update the tiles if they are animated
            for (int i = 0; i < width; i++)
                for (int k = 0; k < height; k++)
                    if (wallTiles[i, k].isAnimated())
                        wallTiles[i, k].Update(gameTime);
                    else if (backTiles[i, k].isAnimated())
                        backTiles[i, k].Update(gameTime);

            //change to true if the player walks onto a door
            bool changing = false;

            // update if the players animation is just changed aka TICKED
            // if it has ticked then shift the map by the players shift or rawX
            if (p.aniTick())
            {
                //for (int i = -8; i <= 8; i++)
                for (int xIndex = 0; xIndex < width; xIndex++)
                {
                    //for (int k = -8; k <= 8; k++)
                    for (int yIndex = 0; yIndex < height; yIndex++)
                    {

                        //int xIndex = p.getX() + 1 + i;
                        //int yIndex = p.getY() + 1 + k;
                        int i = xIndex - p.getX() - 1;
                        int k = yIndex - p.getY() - 1;
                        if (xIndex >= 0 && yIndex >= 0 && xIndex < width && yIndex < height)
                        {

                            // if it is a door
                            if (p.getX() == xIndex && p.getY() == yIndex && backTiles[xIndex, yIndex].isADoor())
                                changing = true;
                            // Postion is from bottom vert and middle horiz THEN add player raw pos
                            Vector2 baseDraw = new Vector2(((4 + i) * 75 + (75 / 2)) + (75 * p.getRawX()), ((6 + k) * 52 + (52)) + (52 * p.getRawY()));
                            if (layout[xIndex, yIndex] != 0)
                                wallTiles[xIndex, yIndex].setPos(baseDraw);

                            backTiles[xIndex, yIndex].setPos(baseDraw);
                        }
                    }
                }
            }
            return changing;
        }

        public void Draw(SpriteBatch b, Player p)
        {
            // draw everything around player
            for (int i = p.getY() - 8; i <= p.getY() + 8; i++)
            {
                for (int k = p.getX() - 8; k <= p.getX() + 8; k++)
                {
                    if (i >= 0 && k >= 0 && i < height && k < width)
                    {
                        backTiles[k, i].Draw(b, skyColor);
                        if (layout[k, i] != 0)
                            wallTiles[k, i].Draw(b, skyColor);
                    }
                }
            }
        }

        public void DrawWalls(SpriteBatch b, Player p, List<NPC> allNPCS, List<int> currNPCS)
        {
            for (int i = 0; i < currNPCS.Count; i++)
            {
                NPC temp = allNPCS[currNPCS[i]];


                if (Math.Abs((double)temp.getX() - (double)p.getX()) < 10 && Math.Abs((double)temp.getY() - (double)p.getY()) < 10)
                    for (int x = 0; x <= 18; ++x)
                        for (int y = -2; y <= 2; y++)
                            if ((temp.getX() + y >= 0 && temp.getX() + y < width && temp.getY() + x < height) && (wallTiles[temp.getX() + y, temp.getY() + x].isADoor() || wallTiles[temp.getX() + y, temp.getY() + x].isAWall() || wallTiles[temp.getX() + y, temp.getY() + x].isAWild()))
                            {
                                if (!(y == 0 && x == 0 && wallTiles[temp.getX(), temp.getY()].isADoor()))
                                    wallTiles[temp.getX() + y, temp.getY() + x].Draw(b, skyColor);
                            }
                if (Vector2.Distance(new Vector2(temp.getX(), temp.getY()), p.getTargetPos()) < 18 && p.getY() > temp.getX())
                {
                    p.Draw(b, skyColor);
                }
            }


            // draw things south of player and NPCS?
            // CHANGE TO CHECK IF WILD OR WALL OR DOOR
            for (int k = 0; k <= 8; ++k)
                for (int i = -2; i <= 2; i++)
                    if ((p.getX() + i >= 0 && p.getX() + i < width && p.getY() + k < height))
                    {
                        // FIGURE OUT Wut IS WITH THE WALL S--- AND FIX SO NO WALK ON DOORS

                        if (!(i == 0 && k == 0 && wallTiles[p.getX(), p.getY()].isADoor()))
                            wallTiles[p.getX() + i, p.getY() + k].Draw(b, skyColor);

                        for (int m = 0; m < currNPCS.Count; ++m)
                            if (allNPCS[currNPCS[m]].getY() >= p.getY() + k)
                                allNPCS[currNPCS[m]].Draw(b, skyColor);
                    }



        }
    }
}
