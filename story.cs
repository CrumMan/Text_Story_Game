using Newtonsoft.Json;
namespace textSim
{
    public class Story
    {
        Dictionary<string, dynamic> _creatures;
        Character _character;
        Dictionary<int, Section> _sections;
        string _storyText;
        public Story()
        {
            _character = new Character();
            LoadCreatures();
            _storyText = "story.txt";
            if (_character._class == "vagrant") _storyText = "vagrant.txt";
            _sections = buildSections(_storyText);

        }
        public Story(Character character)
        {
            _character = character;
            LoadCreatures();
            string storyText = "story.txt";
            if (_character._class == "vagrant") storyText = "vagrant.txt";
            _sections = buildSections(storyText);
        }
        public void Start()
        {
            int currentSelection = 1;
            while (currentSelection != _sections.Count + 1)
            {
                currentSelection = _sections[currentSelection].Run();
                if (_character._class == "vagrant" && _character._hitPoints == 0)
                {
                    System.Console.WriteLine("You dont get to die in this story, you just go hoho back to the start! As you find yourself slipping back into the time warp, You get a fireball! (use a fireball to end an encounter!)");
                    _character._hitPoints = _character._maxHitPoints;
                    _character._maxFireballs = _character._maxFireballs + 1;
                    _character._fireballs = _character._maxFireballs;
                    System.Console.WriteLine("Press any Key to continue.");
                    currentSelection = 1;
                    Console.ReadKey();
                }
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("\x1b[3J");
            }
        }

        private void LoadCreatures()
        {
            var json = File.ReadAllText(@"creature.json");
            _creatures = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
        }
        private Dictionary<int, Section> buildSections(string storyText)
        {
            var sections = new Dictionary<int, Section>();
            var lines = File.ReadAllLines(storyText);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                int sectionNumber = int.Parse(parts[0]);
                string text = parts[1];
                string[] outcomes = parts[2].Split(',');
                int[] outcomeSelections = parts[3].Split(',').Select(int.Parse).ToArray();
                Creature creature = null;
                if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
                {
                    creature = new Creature(_creatures[parts[4]]);
                }
                Section section = new Section(sectionNumber, text, outcomes, outcomeSelections, _character, creature);
                sections.Add(sectionNumber, section);
            }
            return sections;
        }
    }

}