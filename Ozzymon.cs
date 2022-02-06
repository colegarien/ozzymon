using System.Collections.Generic;

namespace Ozzymon
{
    public enum OzzymonElement { Fire, Water, Wind, Earth, Light, Dark, None };
    public enum OzzymonSpecies { Rat, Snake, Ghost, Plant, Bird, Fish, Bug, Horse };
    public enum OzzymonExpType { Slow, Normal, Fast };

    internal class Ozzymon
    {
        #region variables and such
        // health
        int hp = 0, maxhp = 0;

        // experience
        int exp = 0, maxexp = 0;
        // level
        int level = 0;

        // defence
        int def = 0, maxdef = 0;
        // agility to move outta the way
        int agi = 0, maxagi = 0;
        // speed to hit em before they use agility
        int spd = 0, maxspd = 0;
        // attack strenght
        int atck = 0, maxatck = 0;

        // elemental type
        OzzymonElement element;
        // species of ozzymon
        OzzymonSpecies species;
        // rate which they gain exp
        OzzymonExpType expType;

        // only false for special ozzymon
        bool nameable = true;
        // ozzymon actual animal name
        string name = "";
        // ozzymon nickname
        string slavename = "";

        // current attacks
        Move[] moves = new Move[4];
        // attacks gained over levels
        List<Move> moveArchive = new List<Move>();

        // front and back images
        Sprite front;
        Sprite back;


        #endregion

    }
}
