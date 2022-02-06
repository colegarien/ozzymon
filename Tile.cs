using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ozzymon
{
    internal class Tile
    {
        // is it water or a wall or wilderness or a door? no
        bool isWater = false, isWall = false, isWilderness = false, isDoor = false;
        // is it animated such as water or maybe lava
        bool animated = false;
        // -1 for no monsters
        int minLvl = -1, maxLvl = -1;
        // if not animated
        Sprite texture;
        // if animated
        AnimatedSprite aniTex;
        // if its a door that goes somewhere
        string whereTo = "";
        // where to set the player relative to door pos
        Vector2 doorAdjust = Vector2.Zero;

        // if it is NOT animated set everything
        public Tile(Sprite t, bool water, bool wall, bool wild, bool door, int minL, int maxL, string wT, Vector2 da)
        {
            texture = t;
            isWater = water;
            isWall = wall;
            isWilderness = wild;
            isDoor = door;
            minLvl = minL;
            maxLvl = maxL;
            whereTo = wT;
            doorAdjust = da;
            animated = false;
        }

        // if it is animated set everything
        public Tile(AnimatedSprite t, bool water, bool wall, bool wild, bool door, int minL, int maxL, string wT, Vector2 da)
        {
            aniTex = t;
            isWater = water;
            isWall = wall;
            isWilderness = wild;
            isDoor = door;
            minLvl = minL;
            maxLvl = maxL;
            whereTo = wT;
            doorAdjust = da;
            animated = true;
        }

        // get the animated sprite
        public AnimatedSprite getAniSprite() { return aniTex; }
        // get if is animated
        public bool isAnimated() { return animated; }
        public bool isAWild()
        {
            return isWilderness;
        }
        public string getWhereTo()
        {
            return whereTo;
        }
        public Vector2 getDoorShift()
        {
            return doorAdjust;
        }
        public bool isAWater() { return isWater; }
        public bool isADoor() { return isDoor; }

        // set position of tile
        public void setPos(Vector2 p)
        {
            if (!animated)
                texture.pos = p;
            else
                aniTex.pos = p;
        }

        // update animation if animated
        public void Update(GameTime gameTime)
        {
            if (animated)
                aniTex.Update(gameTime);
        }

        // if it is a wall return true
        public bool isAWall() { return isWall; }

        // draw everything
        public void Draw(SpriteBatch b, Color sky)
        {
            if (!animated)
                texture.Draw(b, sky);
            else
                aniTex.Draw(b, sky);
        }

    }
}
