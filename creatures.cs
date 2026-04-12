using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Supabase.Postgrest;
using textSim.database;
using System.Data.Common;
using Newtonsoft.Json;
using Websocket.Client;
using System.Reactive.Subjects;
namespace text_Sim
{
    [Table("creatures")]
    public class Creature : BaseModel
    {
        [PrimaryKey("name")]
        [Column("name")]
        public string Name { get; set; }
        [Column("hp")]
        public int Hp { get; set; }
        [Column("ac")]
        public int Ac { get; set; }
        [Column("initiaveMod")]
        public int InitiaveMod { get; set; }
        [Column("attackString")]
        public string AttackString { get; set; }
        [Column("creatureMultiAttack")]
        public bool CreatureMultiattack { get; set; }
        [Column("deleteable")]
        public bool Deleteable { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public Attack AttackObj { get; set; }

        //Multiattack may or may not be null. This builds the creature's attacks.
        [Newtonsoft.Json.JsonIgnore]
        List<Attack> MultiAttacks { get; } = new List<Attack>();

        //I made this a dynamic class so I can have either a mulitattack or a regular attack passed through.
        public Creature() { }
        public Creature(dynamic creature)
        {
            Create(creature);
        }
        public static async Task<Creature> Create(dynamic creature)
        {
            var c = new Creature();
            c.Name = creature.name;
            c.Hp = creature.hp;
            c.Ac = creature.ac;
            c.InitiaveMod = creature.initiaveMod;
            string[] attackArray = creature.AttackString.Replace(" ", "").Split(',').ToArray();
            if (c.CreatureMultiattack == true)
            {
                // TODO: Provide required arguments for Attack constructor
                c.AttackObj = await Attack.AttackCreate(attackArray.ToList(), c.Name, c.InitiaveMod);
            }
            else
            {
                c.AttackObj = await Attack.AttackCreate(attackArray[0], c.InitiaveMod);
            }
            return c;
        }
    }

    public class creatureMethods
    {
        overall_Methods overallMethods = new();
        public async Task<Creature> getCreature(string creatureString)
        {
            var response = await SupabaseService.SupabaseClient
                .From<Creature>()
                .Where(x => x.Name == creatureString)
                .Get();
            var creatures = response.Models.FirstOrDefault();
            if (creatures == null)
            {
                creatures = await insertCreatureReturnCreature(creatureString);
            }
            return new Creature(creatures);
        }
        public async Task insertCreature(string name)
        {
            name = name.ToLower();
            //check for a creature by name
            var response = await SupabaseService.SupabaseClient
                .From<Creature>()
                .Where(x => x.Name == name)
                .Get();
            if (response.Models.Any())
            {
                System.Console.WriteLine("please enter a unique creature name.");
                name = overallMethods.solveNull();
                response = await SupabaseService.SupabaseClient
                .From<Creature>()
                .Where(x => x.Name.ToLower() == name.ToLower())
                .Get();
            }
            System.Console.WriteLine($"Enter the amount of HP the {name} has");
            int hp = overallMethods.returnInt();
            System.Console.WriteLine($"Enter the Armor class the creature named {name} has.");
            int ac = overallMethods.returnInt();
            System.Console.WriteLine("Add the dice roller adder the creature has");
            int initiaveMod = overallMethods.returnInt();
            int selection = 0;
            int select = 1;
            bool mulitattack = false;
            System.Console.WriteLine("Does this creature use a multiattack? 1.Yes 2.No");
            string attackstring = "";
            while (selection != 1 && selection != 2)
            {
                selection = overallMethods.returnInt();
            }
            if (selection == 1)
            {
                mulitattack = true;
                System.Console.WriteLine("Enter the first attack");
                attackstring = overallMethods.solveNull();
                System.Console.WriteLine("Enter the second attack");
                attackstring = overallMethods.solveNull(attackstring);
                while (select != 2)
                {
                    System.Console.WriteLine("Would you like to add another attack: \n1. Yes\n2.No");
                    select = overallMethods.returnInt();
                    if (select == 1) attackstring = overallMethods.solveNull(attackstring);
                }
            }
            if (selection == 2)
            {
                mulitattack = false;
                System.Console.WriteLine("Enter the attack name (this must be associated with an attack proeviously created to properly load.)");
                attackstring = overallMethods.solveNull();
            }
                ;

            var crtr = new Creature
            {
                Name = name,
                Hp = hp,
                Ac = ac,
                InitiaveMod = initiaveMod,
                AttackString = attackstring,
                Deleteable = true,
                CreatureMultiattack = mulitattack
            };
            await SupabaseService.SupabaseClient.From<Creature>().Insert(crtr);
            Console.WriteLine($"Creature named {crtr.Name} has been added");
        }

        public async Task insertCreature()
        {
            System.Console.WriteLine("You Chose to Insert a new Creature for a future story Make sure you have all the attacks the creature will use updated in the attack part of the updates.");
            System.Console.WriteLine("Enter a name for the creature");
            string name = overallMethods.solveNull();
            await insertCreature(name);
        }
        public async Task<Creature> insertCreatureReturnCreature(string name)
        {
            //check for a creature by name
            var response = await SupabaseService.SupabaseClient
                .From<Creature>()
                .Where(x => x.Name.ToLower() == name.ToLower())
                .Get();
            var creatures = response.Models.FirstOrDefault(m => m.Name == name);
            if (creatures != null)
            {
                return creatures;
            }
            System.Console.WriteLine("It seems the creature was not found and needs to be added to your database.");
            System.Console.WriteLine("Enter the amount of HP the creature has");
            int hp = overallMethods.returnInt();
            System.Console.WriteLine("Enter the Armor class the creature has.");
            int ac = overallMethods.returnInt();
            System.Console.WriteLine("Add the dice roller adder the creature has");
            int initiaveMod = overallMethods.returnInt();
            System.Console.WriteLine("Enter a list of attacks. If the attack is a multiattack enter multiattack for the first attack. Each attack must be seperated by a comma to.");
            string attackstring = overallMethods.solveNull();
            var crtr = new Creature
            {
                Name = name,
                Hp = hp,
                Ac = ac,
                InitiaveMod = initiaveMod,
                AttackString = attackstring,
                Deleteable = true
            };
            await SupabaseService.SupabaseClient.From<Creature>().Insert(crtr);
            Console.WriteLine($"Creature named {crtr.Name} has been added");
            return crtr;
        }

        public async Task DeleteCreature()
        {
            var response = await SupabaseService.SupabaseClient
                .From<Creature>()
                .Where(x => x.Deleteable == true)
                .Get();

            var creatures = response.Models;
            foreach (var creature in creatures)
            {
                System.Console.WriteLine($"Name:{creature.Name}");
                System.Console.WriteLine("------------------");
            }
            System.Console.WriteLine("Enter the Name that you wish to delete:");
            string selectedName = overallMethods.solveNull();
            var selectedCreature = creatures.FirstOrDefault(m => m.Name.ToLower() == selectedName.ToLower());
            if (selectedCreature == null)
            {
                System.Console.WriteLine("Selected Creature is null");
                return;
            }
            await SupabaseService.SupabaseClient
                .From<Creature>()
                .Where(x => x.Name == selectedName)
                .Delete();
        }
        public async Task GetAllCreatures()
        {
            var response = await SupabaseService.SupabaseClient
            .From<Creature>()
            .Where(x => x.Name != "")
            .Get();
            var creatures = response.Models;
            foreach (var creature in creatures)
            {
                System.Console.WriteLine($"Creature Name:{creature.Name}");
                System.Console.WriteLine($"Creature Attacks:{creature.AttackString}");
                System.Console.WriteLine("------------------");
            }
        }
        public async Task<Creature> GetCreature(string name)
        {
            name = name.ToLower();
            var response = await SupabaseService.SupabaseClient
            .From<Creature>()
            .Where(x => x.Name == name)
            .Get();
            var creature = response.Models.FirstOrDefault(m => m.Name == name);
            if (creature == null)
            {

                return await insertCreatureReturnCreature(name);
            }
            else return creature;
        }
        public async Task DoCreatureHomework()
        {
            System.Console.WriteLine("Enter the creature's name");
            string name = overallMethods.solveNull();
            await DoCreatureHomework(name);
        }
        public async Task DoCreatureHomework(String name)
        {
            var response = await SupabaseService.SupabaseClient
            .From<Creature>()
            .Where(x => x.Name == name)
            .Get();
            if (!response.Models.Any()) await insertCreature(name);
        }
        public async Task EditCreature()
        {
            await GetAllCreatures();
            string name = "";
            var responseBool = false;
            int choice = 777;
            int selection = 0;
            string attack = "";
            List<string> attackList = new();

            Creature creature;
            while (!responseBool)
            {
                System.Console.WriteLine("Enter the name of your selected Creature.");
                name = overallMethods.solveNull();
                var response = await SupabaseService.SupabaseClient
                .From<Creature>()
                .Where(x => x.Name == name)
                .Get();
                creature = response.Models.FirstOrDefault(m => m.Name == name);

                if (creature != null)
                {
                    responseBool = true;

                    while (choice != 0)
                    {
                        System.Console.WriteLine($"You selected the {creature.Name} Creature, Current Stats:\nArmor Class: {creature.Ac}\nAttack String: {creature.AttackString}\nTotal HitPoints:{creature.Hp}\nInitiative Modifier:{creature.InitiaveMod}");
                        System.Console.WriteLine("Please enter the following choice to edit the creature.\n1. Armor Class\n2.Attack String \n3.Total HP\n4.Initiave Modifier\nMulti Attack switch\n0.Finish editing the creature");
                        choice = overallMethods.returnInt();
                        if (choice == 1)
                        {
                            System.Console.WriteLine($"Enter the {creature.Name}'s Armor Class");
                            creature.Ac = overallMethods.returnInt();
                        }

                        if (choice == 2)
                        {
                            System.Console.WriteLine($"Enter the {creature.Name}'s Attack String it uses");
                            if (!creature.CreatureMultiattack) creature.AttackString = overallMethods.solveNull();
                            if (creature.CreatureMultiattack)
                            {
                                System.Console.WriteLine("Enter names of attacks the creature will use, Enter 0 when you are done");
                                while (attack != "0")
                                {
                                    attack = overallMethods.solveNull().Trim();
                                    if (attack != "0")
                                    {
                                        attackList.Add(attack);
                                    }
                                }
                                creature.AttackString = string.Join(", ", attackList);
                            }
                        }

                        if (choice == 3)
                        {
                            System.Console.WriteLine($"Enter the {creature.Name}'s total Health Points");
                            creature.Hp = overallMethods.returnInt();
                        }

                        if (choice == 4)
                        {
                            System.Console.WriteLine($"Enter the {creature.Name}'s Initiave Modifier");
                            creature.InitiaveMod = overallMethods.returnInt();
                        }

                        if (choice == 5)
                        {
                            System.Console.WriteLine("Is this a multiattack? 1.Yes 2.No");
                            while (selection != 1 && selection != 2)
                            {
                                selection = overallMethods.returnInt();
                            }
                            if (selection == 1) creature.CreatureMultiattack = true;
                            if (selection == 2) creature.CreatureMultiattack = false;
                        }
                    }
                    await creature.Update<Creature>();
                }
            }
        }

    }
}