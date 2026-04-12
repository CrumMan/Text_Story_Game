using Microsoft.VisualBasic;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Supabase.Postgrest;
using textSim.database;
using text_Sim;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.Design;
namespace text_Sim
{
    [Table("attack")]
    public class Attack : BaseModel
    {
        [PrimaryKey("name")]
        [Column("name")]
        public string Name { get; set; }
        [Column("hitDice")]
        public int HitDice { get; set; }
        [Column("numberOfHitdice")]
        public int NumberOfHitDice { get; set; }
        [Column("hitAdder")]
        public int HitAdder { get; set; }
        [Column("toHitModifier")]
        public int ToHitModifier { get; set; }
        [Column("multiattackbool")]
        public bool MultiAttackBool { get; set; }
        [Column("characterAccessable")]
        public bool CharacterAccessable { get; set; } = false;
        [Column("characterString")]
        public string CharacterString { get; set; } = "";
        [Newtonsoft.Json.JsonIgnore]
        private List<Attack> MultiAttack { get; set; } = new List<Attack>();

        public Attack(Attack attack, int mod)
        {
            Name = attack.Name;
            HitDice = attack.HitDice;
            NumberOfHitDice = attack.NumberOfHitDice;
            HitAdder = attack.HitAdder;
            ToHitModifier = mod;
        }
        public Attack(string name, int hitDice, int numberOfHitDice, int hitAdder, bool forchar = false, string characterString = "")
        {
            Name = name;
            HitDice = hitDice;
            NumberOfHitDice = numberOfHitDice;
            HitAdder = hitAdder;
            CharacterAccessable = forchar;
            CharacterString = characterString;

        }
        public Attack()
        {

        }
        public static async Task<Attack> AttackCreate(List<string> multiAttack, string creatureName, int mod, bool forChar = false)
        {
            Attack attackObj = new Attack();
            string attackNamed = $"{creatureName} {string.Join(",", multiAttack)}";
            var response = await SupabaseService.SupabaseClient
                .From<Attack>()
                .Where(x => x.Name == attackNamed)
                .Get();
            var attack = response.Models;
            if (attack.Count == 0)
            {
                attackMethods methods = new attackMethods();
                foreach (string attackname in multiAttack)
                {
                    response = await SupabaseService.SupabaseClient
                        .From<Attack>()
                        .Where(x => x.Name == attackNamed)
                        .Get();
                    attack = response.Models;
                    if (attack.Count == 0)
                    {
                        Attack attackToAdd = await methods.CreateAttack(attackname, mod);
                        attackObj.MultiAttack.Add(attackToAdd);
                    }
                    else
                    {
                        Attack attack1 = attack.FirstOrDefault();
                        attack1.ToHitModifier = mod;
                        return attack1;
                    }
                }
                return attackObj;
            }
            else
            {
                Attack attack1 = attack.FirstOrDefault();
                attack1.ToHitModifier = mod;
                return attack1;
            }
        }
        public static async Task<Attack> AttackCreate(string name, int mod)
        {
            attackMethods methods = new();
            var response = await SupabaseService.SupabaseClient
                .From<Attack>()
                .Where(x => x.Name == name)
                .Get();
            var attack = response.Models;
            if (attack.Count == 0)
            {
                Attack attackToAdd = await methods.CreateAttack(name, mod);
                return attackToAdd;
            }
            Attack attack1 = attack.FirstOrDefault();
            attack1.ToHitModifier = mod;
            return attack1;
        }

        public void getAttack(bool isUser, dynamic target)
        {
            if (isUser)
            {
                int attackRoll = rollRand(20, ToHitModifier);
                if (attackRoll >= target.Ac)
                {
                    int damage = rollRandDamage(HitDice, NumberOfHitDice) + HitAdder;
                    target.Hp -= damage;
                    if (target._hp <= 0)
                    {
                        return;
                    }
                    System.Console.WriteLine($"You hit with your {Name} for {damage} Damage!");
                }
                else { System.Console.WriteLine($"You missed your attack."); }
            }
            if (!isUser)
            {
                if (MultiAttackBool)
                {
                    foreach (var atk in MultiAttack)
                    {
                        atk.getAttack(false, target);
                    }
                }
                else
                {
                    if (target._hitPoints == 0) { return; }
                    int attackRoll = rollRand(20, ToHitModifier);
                    if (attackRoll >= target._armorClass)
                    {
                        int damage = rollRandDamage(HitDice, NumberOfHitDice) + HitAdder;
                        System.Console.WriteLine($"The Damage for the {Name} Hit! You take *{damage} damage!");
                        target._hitPoints -= damage;
                        if (target._hitPoints < 0) { target.HitPoints = 0; }
                        System.Console.WriteLine($"You now have {target.HitPoints}/{target.MaxHitPoints}HP!");
                        Console.ReadKey();
                    }
                    else
                    {
                        System.Console.WriteLine($"The Creature missed their {Name} attack.");
                    }
                }
            }
        }
        private static Random _rand = new Random();
        private int rollRandDamage(int dice, int? diceNum = 1)
        {
            int num = 0;
            while (diceNum != 0)
            {
                num += _rand.Next(1, dice + 1);
                diceNum--;
            }
            return num;
        }
        private int rollRand(int dice, int modifier)
        {
            int num = _rand.Next(1, dice + 1) + modifier;
            return num;
        }
    }
}

public class attackMethods
{
    overall_Methods Overall_Methods = new();
    public async Task<Attack> CreateAttack(string name, int mod)
    {
        int select;
        System.Console.WriteLine($"Lets add the attack {name} with a mod of {mod}");
        System.Console.WriteLine("------------------------");
        System.Console.WriteLine("We need the hit dice that is being used for a successful attack, Please enter the highest number that can be made on a hit. (for a max of 8 min of 1 enter 8 )");
        int hitDice;
        do
        {
            hitDice = Overall_Methods.returnInt();
        } while (hitDice < 1);

        System.Console.WriteLine("Enter the number of Hitdice");
        int numberOfHitDice;
        do
        {
            numberOfHitDice = Overall_Methods.returnInt();
        } while (numberOfHitDice < 1);

        System.Console.WriteLine("Enter the amount of damage a hit does.");

        bool CharacterAccessable = false;
        do
        {
            System.Console.WriteLine("Is this attack accessable to a character?\n1.Yes\n2.No ");
            select = Overall_Methods.returnInt();
            if (select == 1) CharacterAccessable = true;
            if (select == 2) CharacterAccessable = false;
        } while (select < 1 && select > 2);
        string characterString = "";
        if (CharacterAccessable)
        {

            System.Console.WriteLine("The character has a diffrent hit adder according to their attribute Choose the attribute:\n1.Strength\n2.Charisma\n3.Constitution\n4.Intelligence\n5.Wisdom\n6.Dexterity");
            do
            {
                select = Overall_Methods.returnInt();
                if (select == 1) characterString = "str";
                if (select == 2) characterString = "cha";
                if (select == 3) characterString = "con";
                if (select == 4) characterString = "int";
                if (select == 5) characterString = "wis";
                if (select == 6) characterString = "dex";

            } while (select < 0 && select > 7);
        }
        System.Console.WriteLine("If the attack hits how much extra damage will it do?");
        int hitAdder;
        do
        {
            hitAdder = Overall_Methods.returnInt();
        }
        while (hitAdder < 0);

        Attack createdAttack = new(name, hitDice, numberOfHitDice, hitAdder, CharacterAccessable, characterString);
        await SupabaseService.SupabaseClient.From<Attack>().Insert(createdAttack);
        createdAttack.ToHitModifier = mod;
        Console.WriteLine($"{name} Attack has been created, press any key to clear unnessary text. to continue.");
        Console.ReadKey();
        Console.Clear();
        return createdAttack;

    }
    public async Task DoAttackHomework(string name, int mod)
    {
        var response = await SupabaseService.SupabaseClient
        .From<Attack>()
        .Where(x => x.Name == name)
        .Get();
        if (!response.Models.Any()) await CreateAttack(name, mod);
        else { return; }
    }
    public async Task<Attack> CreateAttack()
    {
        System.Console.WriteLine("Please enter the attacks name");
        string name;
        while (true)
        {
            name = Overall_Methods.solveNull();
            var response = await SupabaseService.SupabaseClient.From<Attack>().Where(x => x.Name == name).Get();
            if (!response.Models.Any() && name != "") break;
            else System.Console.WriteLine("Im sorry this attack was found in the database. Please assign another name to your attack. (You can edit your creature if already created to represent the new attack string.)");
        }

        System.Console.WriteLine("Next we need to find a hit modifier (amount to add to hit roll for creatures using the attack)");
        int mod;
        while (true)
        {
            mod = Overall_Methods.returnInt();
            if (mod >= 0) break;
            else System.Console.WriteLine("The attack modification must be more than or equal to 0.");
        }
        Attack attack = await CreateAttack(name, mod);
        return attack;
    }
    public async Task DeleteAttack()
    {
        var response = await SupabaseService.SupabaseClient
                        .From<Attack>()
                        .Where(x => x.Name != "")
                        .Get();

        var attacks = response.Models;
        foreach (var atk in attacks)
        {
            System.Console.WriteLine($"Attack Name:{atk.Name}");
            System.Console.WriteLine("------------------");
        }
        System.Console.WriteLine("Enter the Name that you wish to delete:");

        string selectedName = Overall_Methods.solveNull();
        selectedName.ToLower();

        var selectedAttack = attacks.FirstOrDefault(m => m.Name.Equals(selectedName));
        if (selectedAttack.Name == null)
        {
            System.Console.WriteLine("Selected Attack is null");
            return;
        }
        await SupabaseService.SupabaseClient
               .From<Attack>()
               .Where(x => x.Name == selectedName)
               .Delete();
    }
    public async Task<Attack> GetAttack(string name)
    {
        var response = await SupabaseService.SupabaseClient
            .From<Attack>()
               .Where(x => x.Name.ToLower() == name.ToLower())
               .Get();
        var attack = response.Models.FirstOrDefault(m => m.Name == name.ToLower());
        return attack;
    }
    public async Task<Attack> GetAttack()
    {
        System.Console.WriteLine("Enter attack name");
        string name = Overall_Methods.solveNull();
        var response = await SupabaseService.SupabaseClient
            .From<Attack>()
               .Where(x => x.Name.ToLower() == name.ToLower())
               .Get();
        var attack = response.Models.FirstOrDefault(m => m.Name == name.ToLower());
        if (attack == null) await GetAttack();
        return attack;
    }
}