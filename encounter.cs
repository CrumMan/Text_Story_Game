using System.Reflection.PortableExecutable;

namespace textSim
{
    public class Encounter
    {
        private Character _character;
        private Creature _creature;

        public Encounter(Character character, Creature creature)
        {
            _character = character;
            _creature = creature;
        }

        public void Combat()
        {
            int charInitiave;
            int creInitiative;
            do
            {
                charInitiave = rollRand(20) + _character._dex;
                creInitiative = rollRand(20) + _creature._initiaveMod;
            }
            while (creInitiative == charInitiave);

            if (creInitiative > charInitiave)
            {
                System.Console.WriteLine($"The {_creature._name} attacks first!");
                turnCycle(false);
            }
            else
            {
                System.Console.WriteLine("You do your turn first!");
                turnCycle(true);
            }
        }
        private void turnCycle(bool userFirst)
        {
            if (userFirst)
            {
                while (_character._hitPoints > 0 && _creature._hp > 0)
                {
                    userTurn();
                    if (_creature._hp > 0) attack(false, _creature, _character);

                }
            }
            else
            {
                while (_character._hitPoints > 0 && _creature._hp > 0)
                {
                    attack(false, _creature, _character);
                    if (_character._hitPoints > 0) userTurn();
                }
            }
        }
        private void userTurn()
        {
            int choice = 0;
            System.Console.WriteLine($"Choose to: \n1.Attack\n2.Flee");
            if (_character._fireballs > 0)
            {
                System.Console.WriteLine("3.Use a fireball");
                while (choice < 1 || choice > 3)
                {
                    System.Console.WriteLine("Make a choice");
                    int.TryParse(Console.ReadLine(), out choice);
                    if (choice == 3)
                    {
                        _character._fireballs--;
                        _creature._hp = 0;
                        System.Console.WriteLine($"So you blew the {_creature._name} up? Thats actually impressive!");
                        return;
                    }
                }
            }
            else
            {
                while (choice != 1 || choice != 2)

                {
                    System.Console.WriteLine("Make a choice");
                    int.TryParse(Console.ReadLine(), out choice);
                }
            }
            if (choice == 1)
            {
                attack(true, _character, _creature);
            }
            if (choice == 2)
            {
                int run = rollRand(20) + _character._dex;
                int creatureRun = 15 + _creature._initiaveMod;
                if (run > creatureRun)
                    System.Console.WriteLine($"So you ran? said the old man, why?");
                break;
            }
        }
        private void attack(bool user, dynamic attacker, dynamic defender)
        {

            attacker._attack.getAttack(user, defender);
        }
        public void endCombat(bool win)
        {
            if (!win)
            {
                story.end(false);
            }
            else
            {
                System.Console.WriteLine($"Describe how you kill the {_creature._name} or press enter for a general input!");
                string description = Console.ReadLine();
                if (description == "")
                {
                    System.Console.WriteLine($"You explain that you slayed the {_creature._name}!");
                }
                else { System.Console.WriteLine($"\nYou describe to the man \"{description}\""); }
            }
        }
        private static Random _rand = new Random();
        private int rollRand(int dice)
        {
            int num = _rand.Next(1, dice + 1);
            return num;
        }
    }
}
