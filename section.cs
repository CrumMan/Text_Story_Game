using System;
namespace textSim
{
    public class Section
    {
        public int _sectionNumber { get; set; }
        int _outcome { get; set; }
        int[] _outcomeSelections;
        string[] _outcomes { get; set; }
        string _text { get; set; }
        Creature _creature;
        Character _character;
        public Section(int sectionNumber, string text, string[] outcomes, int[] outcomeSelections, Character character, Creature? creature = null)
        {
            _sectionNumber = sectionNumber;
            _text = text;
            _outcomes = outcomes;
            _outcomeSelections = outcomeSelections;
            _character = character;
            if (creature != null) _creature = creature;
            else _creature = creature;
        }
        public int Run()
        {
            if (_creature == null)
            {
                System.Console.WriteLine(_text);
                return displayAndChooseChoices();
            }
            else
            {
                Encounter encounter = new Encounter(_character, _creature);
                encounter.entireEncounter();
                if (_character._hitPoints == 0)
                {
                    return 0;
                }
                else
                {
                    Console.ReadKey();
                    Console.Clear();
                    Console.WriteLine(_text);
                    return displayAndChooseChoices();
                }
            }
        }
        private int displayAndChooseChoices()
        {
            int count = 1;
            int selection = 0;
            System.Console.WriteLine("Would you like to");
            foreach (string choice in _outcomes)
            {
                Console.WriteLine($"{count}. {choice}");
                count++;
            }
            while (selection == 0 || selection > _outcomes.Length)
            {
                System.Console.WriteLine("Please choose an outcome");
                int.TryParse(Console.ReadLine(), out selection);
            }
            return _outcomeSelections[selection - 1];
        }
    }
}