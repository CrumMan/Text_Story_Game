namespace textSim
{
    class Creature
    {
        protected string _name { get; }
        public int _hp { get; set; }
        protected int _ac { get; }
        protected int _initiaveMod { get; }
        protected Attack _attack { get; }

        public Creature(dynamic creature)
        {
            _name = creature.name;
            _hp = creature.hp;
            _ac = creature.ac;
            _initiaveMod = creature.initiaveMod;
            if (creature.attack.multiAttack != null)
            {
                List<Attack> multiAttacks = new List<Attack>();
                foreach (var atk in creature.attack.multiAttack._multiAttack)
                {
                    multiAttacks.Add(new Attack(atk));
                }
                _attack = new Attack("multiAttack", multiAttacks);
            }
            else
            {
                var firstAttackKey = creature.attack.Keys.First();
                var firstAttack = creature.attack[creature.attack.Keys.First()];
                _attack = new Attack(firstAttack);

            }
        }


    }
}