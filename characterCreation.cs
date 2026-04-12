using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Supabase.Postgrest;
using textSim.database;
using text_Sim;
namespace text_Sim
{
    public partial class Character
    {
        //i seperated this from the character class cause this file's purpose is to let the character be created but to access variables directly as well.
        public Character()
        {
        }
        public async Task CharacterCreate()
        {
            Console.Clear();

            while (_name == null) { Console.WriteLine("Hello! what is your name?"); _name = Console.ReadLine(); }
            Console.Clear();
            Console.WriteLine($"What class would you like to be? \n The Warrior has initial advantages on attack rolls and damage is halved when they are hit, but low character rolls on all rolls when creating stats except strength and constitution. (D12 is rolled instead of a D20) \n The Ranger has high perception so they have avantage stats on Wisdom, Dexterity and Intelligence rolls but disadvantage on strength and constitution saving throws \n The Mage has low rolls for stats on all other rolls except for Intelligence but he has a fireball spell he can use two times a day that will win him hostile encounters.\n The Vagrant has no stat avantages or disavantages. He rolls a 20 sided dice for all of his character stats. Being a vagrant may incure hidden dialoge (be it good or bad is a mystery).");
            _class = Console.ReadLine().ToLower();
            while (_class != "warrior" && _class != "ranger" && _class != "mage" && _class != "vagrant")
            {
                Console.WriteLine("Im sorry, the classes available to play with are: \n Warrior, Ranger, Mage, Vagrant \n please choose one of those 4 classes by entering the class name.");
                _class = Console.ReadLine().ToLower();
            }
            if (_class == "warrior")
            {
                Console.WriteLine("We will first roll for Strength.");
                Console.ReadKey();
                _str = rollRandMod(20);

                Console.WriteLine("We will now roll for Charisma.");
                Console.ReadKey();
                _cha = rollRandMod(12);

                Console.WriteLine("We will now roll for Constitution.");
                Console.ReadKey();
                _con = rollRandMod(20);

                Console.WriteLine("We will now roll for Intelligence.");
                Console.ReadKey();
                _int = rollRandMod(12);

                Console.WriteLine("We will now roll for Wisdom.");
                Console.ReadKey();
                _wis = rollRandMod(12);

                Console.WriteLine("We will now roll for Dexterity.");
                Console.ReadKey();
                _dex = rollRandMod(12);

                Console.WriteLine("We will finally roll for Max HP.");
                Console.ReadKey();
                _maxHitPoints = rollRand(16) + _con;
                _hitPoints = _maxHitPoints;
                Console.WriteLine($"You got a {_hitPoints} for your max hitpoints.");
            }
            else if (_class == "ranger")
            {
                Console.WriteLine("We will first roll for Strength.");
                Console.ReadKey();
                _str = rollRandMod(12);

                Console.WriteLine("We will now roll for Charisma.");
                Console.ReadKey();
                _cha = rollRandMod(12);

                Console.WriteLine("We will now roll for Constitution.");
                Console.ReadKey();
                _con = rollRandMod(12);

                Console.WriteLine("We will now roll for Intelligence.");
                Console.ReadKey();
                _int = rollRandMod(20);

                Console.WriteLine("We will now roll for Wisdom.");
                Console.ReadKey();
                _wis = rollRandMod(20);

                Console.WriteLine("We will now roll for Dexterity.");
                Console.ReadKey();
                _dex = rollRandMod(20);

                Console.WriteLine("We will finally roll for Max HP.");
                Console.ReadKey();
                _maxHitPoints = rollRand(10) + _con;
                _hitPoints = _maxHitPoints;
                Console.WriteLine($"You got a {_hitPoints} for your max hitpoints.");
            }
            else if (_class == "mage")
            {
                Console.WriteLine("We will first roll for Strength.");
                Console.ReadKey();
                _str = rollRandMod(12);

                Console.WriteLine("We will now roll for Charisma.");
                Console.ReadKey();
                _cha = rollRandMod(12);

                Console.WriteLine("We will now roll for Constitution.");
                Console.ReadKey();
                _con = rollRandMod(20);

                Console.WriteLine("We will now roll for Intelligence.");
                Console.ReadKey();
                _int = rollRandMod(12);

                Console.WriteLine("We will now roll for Wisdom.");
                Console.ReadKey();
                _wis = rollRandMod(12);

                Console.WriteLine("We will now roll for Dexterity.");
                Console.ReadKey();
                _dex = rollRandMod(12);

                Console.WriteLine("We will finally roll for Max HP.");
                Console.ReadKey();
                _maxHitPoints = rollRand(6) + _con;
                _hitPoints = _maxHitPoints;
                Console.WriteLine($"You got a {_hitPoints} for your max hitpoints.");
                _maxFireballs = 2;
                _fireballs = _maxFireballs;

            }
            else if (_class == "vagrant")
            {
                Console.WriteLine("We will first roll for Strength.");
                Console.ReadKey();
                _str = rollRandMod(20);

                Console.WriteLine("We will now roll for Charisma.");
                Console.ReadKey();
                _cha = rollRandMod(20);

                Console.WriteLine("We will now roll for Constitution.");
                Console.ReadKey();
                _con = rollRandMod(20);

                Console.WriteLine("We will now roll for Intelligence.");
                Console.ReadKey();
                _int = rollRandMod(20);

                Console.WriteLine("We will now roll for Wisdom.");
                Console.ReadKey();
                _wis = rollRandMod(20);

                Console.WriteLine("We will now roll for Dexterity.");
                Console.ReadKey();
                _dex = rollRandMod(20);

                Console.WriteLine("We will finally roll for Max HP.");
                Console.ReadKey();
                _maxHitPoints = rollRand(12) + _con;
                _hitPoints = _maxHitPoints;
                Console.WriteLine($"You got a {_hitPoints} for your max hitpoints.");

            }
            getArmorAC(_class);
            _attack = await GetAttack();
            System.Console.WriteLine("Take note next time you enter a key this will be deleted.");
            Console.ReadKey();
            Console.Clear();
        }
        attackMethods AttackMethods = new();
        overall_Methods Overall_Methods = new();
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

        //this installs the attack trait for the character. I chose to build a dynamic result that can be loaded into and returned into an attack class. I chose to use dynamic cause it will dynamically build the weapon for the user.
        private async Task<Attack> GetAttack()
        {
            var response = await SupabaseService.SupabaseClient
               .From<Attack>()
               .Where(x => x.CharacterAccessable == true)
               .Get();
            var attacks = response.Models;
            do
            {
                attacks = response.Models;
                if (!response.Models.Any())
                {
                    System.Console.WriteLine("Error cant find any character attacks");
                    await AttackMethods.CreateAttack();
                    response = await SupabaseService.SupabaseClient
                        .From<Attack>()
                        .Where(x => x.CharacterAccessable == true)
                        .Get();
                }
            } while (!response.Models.Any());

            foreach (var Attack in attacks)
            {
                System.Console.WriteLine($"Name:{Attack.Name} | Stat Modifier: {Attack.CharacterString}");
            }
            System.Console.WriteLine("Enter the an attack name youd like to have your character use.");
            string name = Overall_Methods.solveNull();
            Attack chosenWeapon = await AttackMethods.GetAttack(name);



            var stat = chosenWeapon.CharacterString;
            switch (stat)
            {
                case "str":
                    modifier = _str;
                    break;
                case "cha":
                    modifier = _cha;
                    break;
                case "con":
                    modifier = _con;
                    break;
                case "dex":
                    modifier = _dex;
                    break;
                case "wis":
                    modifier = _wis;
                    break;
                case "int":
                    modifier = _int;
                    break;

            }

            Attack attack1 = chosenWeapon;
            attack1.ToHitModifier = modifier;
            return attack1;
        }

        private void getArmorAC(string charClass)
        {
            if (charClass == "warrior")
            {

                Console.WriteLine($"What Armor would you like to wear? (It will add to a base of 10.) \nUnarmored +{_con} (normal clothing) \n Robes +0 \n Leather Armor +2");
                _armor = Console.ReadLine().ToLower();
                while (_armor != "unarmored" && _armor != "robes" && _armor != "leather armor")
                {
                    Console.WriteLine("Im sorry, the armor available to play with are: \n Unarmored, Robes,  \n please choose one of those 3 armors by entering the armor type.");
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
                while (_armor != "unarmored" && _armor != "robes" && _armor != "leather armor")
                {
                    Console.WriteLine("Im sorry, the armor available to play with are: \n Unarmored, Robes,  \n please choose one of those 3 armors by entering the armor type.");
                    _armor = Console.ReadLine().ToLower();
                }
                if (_armor == "unarmored") _armorClass = 10 + _dex;
                else if (_armor == "robes") _armorClass = 0;
                else if (_armor == "leather armor") _armorClass = 12;
                else _armorClass = 10;
            }
            Console.WriteLine($"Your Armor Class(AC) is {_armorClass}");
            Console.ReadKey();
            return;
        }
    }
}