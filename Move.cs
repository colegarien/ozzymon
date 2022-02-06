namespace Ozzymon
{
    // INTEGRATE AND AILMENT THING FOR BURN POISON PARALYSIS
    // ANIMATION OF ATTACK AND DEFEND EFFECT IN THE BATTLE CLASS

    public enum AttackStyle { Slash, Bash, None };

    internal class Move
    {
        // ozzy points for attacks
        int op = 0, maxop = 0;

        // accuracy of the move
        int accuracy = 0;

        // style of attack
        AttackStyle style;
        // element tied on attack
        OzzymonElement element;

        // name of attack
        string name = "";
    }
}
