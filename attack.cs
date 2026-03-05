using System.Security.Authentication;

namespace textSim
{
    class Attack(dynamic attack, bool? advantage)
    {
        private string _name { set; get; }
        private int _hitDice { set; get; }
        private int _hitdiceNum { set; get; }
        private int _hitDiceMod { set; get; }
        private int toHitModifier { set; get; }
        private Attack? One
        { set; get; }
        private Attack? Two { set; get; }
        private Run()
        {
            _name = attack.name;
            if (_name == "multiAttack")
            {
                One = attack["1"];
                Two = attack["2"];
            }
            else
            {
                _name = attack.name;
                _hitDice = attack.hitDice;
                _hitDiceMod = attack.hitDice;
            }
        }
        public void calculateDamage(bool user, Character character, Creature? creature1, Battle battle)
        {
            if (user)
            {
                int n = rollRand(20) + toHitModifier;
                int n2 = 0;
                if (character._class == "warrior")
                {
                    n2 = rollRand(20) + toHitModifier;
                }
                if (creature1.ac <= n || n2 >= creature1.ac)
                {
                    int hitroll = rollRand(_hitdice);
                    int damage = hitroll + _hitDiceMod;
                    creature1._hp = creature1._hp - damage;

                    Console.WriteLine($"Your attack with the {_name} caused {damage} damage! It has {creature._hp} hp left");
                    if (creature1._hp == 0)
                    {
                        battle.end(true);
                    }
                }
            }
            else if (!user)
            {
                if (_name = "multiAttack")
                {
                    One.calculateDamage(user, character, creature1);
                    if (character._hitPoints > 0) Two.calculateDamage(user, character, creature1);
                }
                else
                {
                    int n = rollRand(20) + toHitModifier;
                    if (n >= character._armorClass)
                    {
                        int hitRoll = rollRand(_hitDice, _hitdiceNum);
                        character._hitPoints = character.hitpoints - hitRoll + _hitDiceMod;
                        if (character.hitpoints < 0) character.hitpoints = 0;
                        Console.WriteLine($"The {creature1._name} hit you with a {attack._name} for {hitRoll + _hitDiceMod} damage you now are at {character._hitpoints}/{character._maxHitPoints}HP.");
                    }
                }
            }

        }


        private int rollRand(int dice, int? count)
        {
            Random random = new Random();
            int num = random.Next(1, dice + 1);
            count--;
            if (count != 0) num += rollRand(dice, count);
            return num;
        }
    }
}