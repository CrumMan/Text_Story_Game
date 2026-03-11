//I plan to create a class of character here that can create a character
namespace textSim
{
    public partial class Character
    {
        string _name { get; set; }
        public int _cha { get; set; }
        public int _str { get; set; }
        public int _con { get; set; }
        public int _int { get; set; }
        public int _wis { get; set; }
        public int _dex { get; set; }

        public string _class { get; set; }
        public Attack _attack { get; set; }

        public int _hitPoints { get; set; }
        public int _maxHitPoints { get; set; }
        public string _armor { get; set; }
        public int _armorClass { get; set; }
        public int _maxFireballs { get; set; }
        public int _fireballs { get; set; }
        private int mod;
        private dynamic chosenWeapon;
        private int modifier;
        //rollRand will save the modifier to the character stats using the custom dice number as a max number.
        private static Random _rand = new Random();
        private int rollRand(int dice)
        {
            int num = _rand.Next(1, dice + 1);
            return num;
        }
    }
}