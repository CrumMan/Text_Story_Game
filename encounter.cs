using System.Diagnostics;

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

        public void entireEncounter()
        {
            //I chose to do a skill check with wisdom to give the user the ability to start a combat if they pass the skill check they will enter a minigame cause its fairly straightforward.
            bool seen = skillCheck("wisdom", _character._wis);
            if (seen)
            {
                int choice = 0;
                Console.WriteLine($"A {_creature._name} is seen what do you do?\n1.Attack First\n2.Sneak Past(Dexterity Check)\n3.Hide (You must wait till you as the user decide is \"long enough\")");
                while (choice != 1 && choice != 2 && choice != 3)
                {
                    System.Console.WriteLine("Please do one of the provided decisions.");
                    string entered = Console.ReadLine();
                    int.TryParse(entered, out choice);
                }
                if (choice == 1)
                {
                    turnCycle(true);
                }
                else if (choice == 2)
                {
                    bool escape = skillCheck("dexterity", _character._dex);
                    if (escape)
                    {
                        System.Console.WriteLine("You Escaped.");
                        return;
                    }
                    else
                    {
                        Combat();
                    }
                }
                else if (choice == 3)
                {
                    System.Console.WriteLine("You Choose to Hide! Do not hit enter in the console until you think the beast has left, press any key to start the stopwatch.");
                    Console.ReadKey();
                    System.Console.WriteLine($"You have found a hiding spot in the general area, dont leave until you think the {_creature._name} has left.");
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    Console.ReadLine();
                    timer.Stop();
                    int waitedTime = (int)timer.Elapsed.TotalSeconds;
                    int creatureWaited = rollRand(20) + _creature._initiaveMod;
                    if (waitedTime > creatureWaited)
                    {
                        System.Console.WriteLine($"When you waited for {waitedTime} you emerge from your hiding spot and the creature was no where to be seen.");
                        return;
                    }
                    else
                    {
                        System.Console.WriteLine($"When you waited for {waitedTime} you emerge from your hiding spot the creature sees you!");
                        Console.ReadKey();
                        Combat();
                    }

                }
            }
            else
            {
                System.Console.WriteLine($"A {_creature._name} attacks!");
                Combat();
            }
        }
        //skill check checks if a desired outcome happens.
        private bool skillCheck(string modString, int mod)
        {

            int charRoll = rollRand(20) + mod;
            if (_character._class == "ranger" && (modString == "dexterity" || modString == "wisdom"))
            {
                int r1 = rollRand(20) + mod;
                if (r1 > charRoll) { charRoll = r1; }
            }
            int creatureRoll = rollRand(20) + _creature._initiaveMod;
            if (creatureRoll > charRoll) { return false; }
            return true;
        }
        //combat will randomly roll initiative to go first, the combat will commence randomly. I did this so there is still randomness.
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
        //turn cycle I build for a while so we can keep an eye on two conditiions (or a return) that will continue combat otherwise.
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
                while (choice != 1 && choice != 2 && choice != 3)
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
                while (choice != 1 && choice != 2)

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
                    if (_character._class == "vagrant") System.Console.WriteLine("You ran away.");
                    else System.Console.WriteLine($"So you ran? said the old man, why?");
                return;
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
                return;
            }
            else
            {
                System.Console.WriteLine($"Describe how you kill the {_creature._name} or press enter for a general input!");
                string description = Console.ReadLine();
                if (description == "")
                {
                    System.Console.WriteLine($"You slayed the {_creature._name}!");
                }
                else
                {
                    if (_character._class == "vagrant") System.Console.WriteLine($"Thinking you remember that \"{description}\"");
                    System.Console.WriteLine($"\nYou describe to the man \"{description}\"");
                }
                return;
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
