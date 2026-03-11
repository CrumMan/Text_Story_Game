namespace textSim
{
    public class Creature
    {
        public string _name { get; set; }
        public int _hp { get; set; }
        public int _ac { get; set; }
        public int _initiaveMod { get; set; }
        public Attack _attack { get; set; }
        List<Attack> multiAttacks { get; } = new List<Attack>();

        public Creature(dynamic creature)
        {
            _name = (string)creature["name"];
            _hp = (int)creature["hp"];
            _ac = (int)creature["ac"];
            _initiaveMod = (int)creature["initiaveMod"];
            if (creature["attack"] != null && creature["attack"]["multiAttack"] != null)
            {
                foreach (var atk in creature["attack"]["multiAttack"]["_multiAttack"])
                {
                    multiAttacks.Add(new Attack(atk));
                }
                _attack = new Attack("multiAttack", multiAttacks);
            }
            else if (creature["attack"] != null)
            {
                foreach (var atk in creature["attack"])
                {
                    _attack = new Attack(atk.First);
                    break;
                }
            }
            else
            {
                throw new Exception($"Creature '{_name}' in creature.json is missing an 'attack' field.");
            }
        }


    }
}