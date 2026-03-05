using System.Security.Authentication;

namespace textSim
{
    class Attack(dynamic attack, bool? advantage)
    {
        private string _name { set; get; }
        private int _hitDice { set; get; }
        private int _hitdiceNum { set; get; }
        private int _hitDiceMod { set; get; }
        private int _toHitModifier { set; get; }
        private Attack? One
        { set; get; }
        private Attack? Two { set; get; }
        private Run()
        {
            _name = attack.name;
            if (_name == "multiAttack")
            {
                //I chose to build here with a recursive function so that I can build multiple multi attacks. Be it, A creature does 1 attack it goes to the "else" if not, it loads just one of the multiAttacks that may be their. For example, if multi attack has 3 attacks it would load a multi attack attack 1 would be named multiattack than would load 1 and two. Than it would load the second attack all as one attack action.
                if (attack["1"]) One = attack["1"];
                if (attack["2"]) Two = attack["2"];
            }
            else
            {
                //The else is if the name is not multiAttack it builds the individulal attack in the lower statement.
                _hitDice = attack.hitDice;
                _hitDiceMod = attack.hitDice;
                _hitdiceNum = attack.hitDiceNum;

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
                        battle.end(false);
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