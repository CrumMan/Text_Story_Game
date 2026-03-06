//I plan to create a class of character here that can create a character
namespace textSim
{
    public partial class Character
    {
        string _name { get; }
        protected int _cha { get; }
        protected int _str { get; }
        protected int _con { get; }
        protected int _int { get; }
        protected int _wis { get; }
        protected int _dex { get; }

        protected string _class { get; }
        Attack _attack { get; }

        protected public int _hitPoints { get; set; }
        protected int _maxHitPoints { get; }
        protected string _armor { get; }
        protected int _armorClass { get; }
        protected int? _maxFireballs { get; }
        protected int? _fireballs { get; }
        //rollRand will save the modifier to the character stats using the custom dice number as a max number.
        private int rollRandMod(int dice)
        {
            Random random = new Random();
            int num = random.Next(1, dice + 1);
            if (num > 10 || num < 10)
            {
                mod = (num - 10) / 2;
            }
            else mod = 0;
            Console.WriteLine($"You rolled a {num}/{dice}! You will have a {mod} modifier to your rolls in this stat.");
            Console.ReadKey();
            return mod;
        }
        private void getArmorAC(string charClass)
        {
            if (charClass == "warrior")
            {

                Console.WriteLine($"What Armor would you like to wear? (It will add to a base of 10.) \nUnarmored +{_con} (normal clothing) \n Robes +0 \n Leather Armor +2");
                _armor = Console.ReadLine().ToLower();
                while (_armor != "unarmored" || _armor != "robes" || _armor != "leather armor")
                {
                    Console.WriteLine("Im sorry, the armor available to play with are: \n Unarmored, Robes,  \n please choose one of those 4 classes by entering the class name.");
                    _armor = Console.ReadLine().ToLower();
                }
                if (_armor == "unarmored") _armorClass = 10 + _con;
                else if (_armor == "robes") _armorClass = 0;
                else if (_armor == "leather armor") _armorClass = 12;
                else _armorClass = 10;
            }
            else
            {
                Console.WriteLine($"What Armor would you like to wear? (It will add to a base of 10.) \nUnarmored +{_dex} (normal clothing) \n Robes +0 \n Leather Armor +2");
                _armor = Console.ReadLine().ToLower();
                while (_armor != "unarmored" || _armor != "robes" || _armor != "leather armor")
                {
                    Console.WriteLine("Im sorry, the armor available to play with are: \n Unarmored, Robes,  \n please choose one of those 4 classes by entering the class name.");
                    _armor = Console.ReadLine().ToLower();
                }
                if (_armor == "unarmored") _armorClass = 10 + _dex;
                else if (_armor == "robes") _armorClass = 0;
                else if (_armor == "leather armor") _armorClass = 12;
                else _armorClass = 10;
            }
            Console.WriteLine($"Your Armor Class(AC) is {_armorClass}");
            Console.ReadKey();
        }
        private static Random _rand = new Random();
        private int rollRand(int dice)
        {
            int num = _rand.Next(1, dice + 1);
            return num;
        }
    }
}