namespace textSim
{
    class Creature(var creature)
    {
        string _name = creature.name;
        int _hp = creature.hp;
        int _ac = creature.ac;
        int _initiaveMod = creature.initiaveMod;
        Attack _attack = creature.attack;

    }
}