using Microsoft.VisualBasic;

namespace textSim
{
    public class Attack
    {
        protected string _name { get; init; }
        private int _hitDice { get; init; }
        private int _numberOfHitDice { get; init; }
        private int _hitAdder { get; init; }
        private int _toHitModifier { get; init; }
        private List<Attack>? _multiAttack { get; init; }

        public Attack(dynamic attack)
        {
            _name = (string)attack["_name"];
            _hitDice = (int)attack["_hitDice"];
            _numberOfHitDice = (int)attack["_numberOfHitDice"];
            _hitAdder = (int)attack["_hitAdder"];
            _toHitModifier = (int)attack["_toHitModifier"];
        }
        public Attack(string name, int hitDice, int numberOfHitDice, int hitAdder, int toHitModifier)
        {
            _name = name;
            _hitDice = hitDice;
            _numberOfHitDice = numberOfHitDice;
            _hitAdder = hitAdder;
            _toHitModifier = toHitModifier;
        }
        public Attack(string name, List<Attack> multiAttacks)
        {
            _name = name;
            _multiAttack = multiAttacks;
        }

        public void getAttack(bool isUser, dynamic target)
        {
            if (isUser)
            {
                int attackRoll = rollRand(20, _toHitModifier);
                if (attackRoll >= target._ac)
                {
                    int damage = rollRandDamage(_hitDice, _numberOfHitDice) + _hitAdder;
                    target._hp -= damage;
                    if (target._hp <= 0)
                    {
                        return;
                    }
                    System.Console.WriteLine($"You hit with your {_name} for {damage} Damage!");
                }
            }
            if (!isUser)
            {
                if (_name == "multiAttack")
                {
                    foreach (var atk in _multiAttack)
                    {
                        atk.getAttack(false, target);
                    }
                }
                else
                {
                    if (target._hitPoints == 0) { return; }
                    int attackRoll = rollRand(20, _toHitModifier);
                    if (attackRoll >= target._armorClass)
                    {
                        int damage = rollRandDamage(_hitDice, _numberOfHitDice) + _hitAdder;
                        System.Console.WriteLine($"The Damage for the {_name} Hit! You take *{damage} damage!");
                        target._hitPoints -= damage;
                        if (target._hitPoints < 0) { target._hitPoints = 0; }
                        System.Console.WriteLine($"You now have {target._hitPoints}/{target._maxHitPoints}HP!");
                        Console.ReadKey();
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