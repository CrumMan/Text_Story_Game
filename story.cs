using System;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Supabase.Postgrest;
using textSim.database;
using Newtonsoft.Json;
using System.ComponentModel.Design;
namespace text_Sim
{

    [Table("story")]
    public class Story : BaseModel
    {
        // a story has 3 things, A list of creatures a list of sections and the character. I chose to make creatures dynamic because they may have a multiattack attack that in itself is dynamic to be built.
        [PrimaryKey("storyId")]
        [Column("storyId")]
        public int StoryId { get; set; }
        [Column("storyName")]
        public string storyName { get; set; }
        [Column("deleteable")]
        public bool Deleteable { get; set; } = false;

        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<string, dynamic> Creatures;
        [Newtonsoft.Json.JsonIgnore]
        public Character Character;
        [Newtonsoft.Json.JsonIgnore]
        public Dictionary<int, Section> Sections;


        overall_Methods Overall_Methods = new();
        public Story() { }
        public Story(int storyId, string name, bool deleteable = true)
        {
            StoryId = storyId;
            storyName = name;
            Deleteable = deleteable;
            Sections = new Dictionary<int, Section>();
        }
        public Story(int storyId)
        {
            Character = new Character();
            Character.CharacterCreate().Wait();
            LoadCreatures().Wait();
            Sections = buildSections(storyId).Result;
        }
        public Story(Character character)
        {
            Character = character;
            Character.CharacterCreate().Wait();
            var response = SupabaseService.SupabaseClient
            .From<Story>()
            .Where(x => x.StoryId != 0)
            .Get()
            .Result;
            Story StoryModel = null;
            int storyId = 0;
            foreach (var Story in response.Models) { System.Console.WriteLine($"Story Id: {Story.StoryId}\nStory name: {Story.storyName}\n"); }
            do
            {
                System.Console.WriteLine("Please select which story you wish to read");
                storyId = Overall_Methods.returnInt();
                StoryModel = new Story(storyId);
            } while (StoryModel != null && StoryId != 0);

            LoadCreatures().Wait();

            Sections = buildSections(StoryId).Result;
        }
        public async Task Start()
        {
            int currentSelection = 1;
            while (currentSelection != Sections.Count + 1)
            {
                currentSelection = await Sections[currentSelection].Run();
                if (Character._class == "vagrant" && Character._hitPoints == 0)
                {
                    System.Console.WriteLine("You dont get to die in this story, you just go hoho back to the start! As you find yourself slipping back into the time warp, You get a fireball! (use a fireball to end an encounter!)");
                    Character._hitPoints = Character._maxHitPoints;
                    Character._maxFireballs = Character._maxFireballs + 1;
                    Character._fireballs = Character._maxFireballs;
                    System.Console.WriteLine("Press any Key to continue.");
                    currentSelection = 1;
                    Console.ReadKey();
                }
                Console.ReadKey();
                Console.Clear();
                Console.WriteLine("\x1b[3J");
            }
        }

        private async Task LoadCreatures()
        {
            var sections = new Dictionary<int, Section>();
            var response = await SupabaseService.SupabaseClient
            .From<Creature>()
            .Where(x => x.Name != "")
            .Get();
            foreach (var creature in response.Models)
            {
                Creatures.Add(creature.Name, creature);
            }
        }
        private async Task<Dictionary<int, Section>> buildSections(int Id, bool IsVagrant = false)
        {
            var sections = new Dictionary<int, Section>();
            var response = await SupabaseService.SupabaseClient
            .From<Section>()
            .Where(x => x.StoryId == Id && x.isVagrant == IsVagrant)
            .Get();
            var sectionList = response.Models;
            foreach (var sectionInstance in sectionList)
            {
                int sectionNumber = sectionInstance.SectionNumber;
                string text = sectionInstance.Text;
                string[] outcomes = sectionInstance.Outcomes.Replace(" ", "").Split(',');
                int[] outcomeSelections = sectionInstance.OutcomeSelections.Split(",").Select(int.Parse).ToArray();
                int storyId = Id;
                string creatureString = "";
                if (sectionInstance.CreatureString != "")
                {
                    creatureString = sectionInstance.CreatureString;
                }
                Section section = new Section(storyId, sectionNumber, text, outcomes, outcomeSelections, Character, creatureString);
                sections.Add(sectionNumber, section);
            }
            return sections;
        }
    }
    public class storyMethods
    {
        overall_Methods Overall_Methods = new();
        sectionMethods Section_Methods = new();
        creatureMethods CreatureMethods = new();
        sectionMethods SectionMethods = new();
        attackMethods AttackMethods = new();
        public async Task CreateStory(bool isVagrant = false)
        {
            System.Console.WriteLine("You are creating a new story please start by entering the story name.");
            string name = Overall_Methods.solveNull().ToLower();
            var storyResponse = await SupabaseService.SupabaseClient
                            .From<Story>()
                            .Where(x => x.storyName != "")
                            .Get();
            int storyId;
            if (!storyResponse.Models.Any()) storyId = 0;
            else storyId = storyResponse.Models.Max(m => m.StoryId) + 1;

            Story createdStory = new Story(storyId, name);
            await SupabaseService.SupabaseClient.From<Story>().Insert(createdStory);

            var response = await SupabaseService.SupabaseClient
                            .From<Story>()
                            .Where(x => x.storyName == name)
                            .Get();
            var story = response.Models.FirstOrDefault(m => m.storyName == name);
            var allSections = createdStory.Sections;
            System.Console.WriteLine("Now we are going to create sections for your story, First enter a death message.");
            string deathMsg = Overall_Methods.solveNull();


            List<string> outcomes = new();
            System.Console.WriteLine("What is the option to be given to the user upon death?");
            outcomes.Add(Overall_Methods.solveNull());



            int[] outcomeList = { 1 };



            var sectionResponse = await SupabaseService.SupabaseClient
                             .From<Section>()
                             .Where(x => x.Id > 0)
                             .Get();
            int sectionPrimaryId;
            if (sectionResponse.Models.Any()) sectionPrimaryId = sectionResponse.Models.Max(m => m.Id) + 1;
            else { sectionPrimaryId = 0; }
            Section deathsection = new Section(sectionPrimaryId, storyId, 0, deathMsg, outcomes.ToArray(), outcomeList, "", false);
            await Section_Methods.CreateSection(deathsection);
            deathsection.isVagrant = true;
            deathsection.Id = sectionPrimaryId + 1;
            await Section_Methods.CreateSection(deathsection);

            int choice = 1;
            int count = 1;
            createdStory.Sections.Add(0, deathsection);
            System.Console.WriteLine("The death messages have been added.");
            while (choice != 0)
            {
                if (createdStory.Sections.Count() != 0)
                {
                    foreach (var section in createdStory.Sections)
                    {
                        if (section.Value.SectionNumber == 0) continue;
                        System.Console.WriteLine($"Your current built sections (death section is not included):\n Section Id in relation to story: {section.Value.SectionNumber} \n The text choice is {section.Value.Text} \nChoice Ids to this section: {section.Value.OutcomeSelections} \n ---------------------------------------------------\n");
                    }
                }
                Section currentSection = await SectionMethods.BuildSection(storyId, count, isVagrant);
                createdStory.Sections.Add(count, currentSection);

                System.Console.WriteLine("\nWould you like to finish the story? 1.No 0.Yes.\n (the vagrant side of the story if true will start)");
                choice = Overall_Methods.returnInt();
                while (choice != 1 && choice != 0)
                {
                    choice = Overall_Methods.returnInt();
                }
                Console.Clear();
                ;

                count++;
            }
            int sectionId;

            sectionResponse = await SupabaseService.SupabaseClient
                        .From<Section>()
                        .Where(x => x.Id > 0)
                        .Get();
            sectionId = sectionResponse.Models.Any() ? sectionResponse.Models.Max(m => m.Id) + 1 : 0;
            foreach (var entry in createdStory.Sections)
            {
                if (entry.Key == 0) continue;
                entry.Value.Id = sectionId;
                entry.Value.isVagrant = false;
                await Section_Methods.CreateSection(entry.Value);
                sectionId++;
            }
            if (isVagrant)
            {
                System.Console.WriteLine("There is a diffrent story for vagrant players.They are inherently able to revive till the story ends with a fireball, please start creating their story.");
                createdStory.Sections.Clear();
                count = 1;
                choice = 1;
                while (choice != 0)
                {
                    if (createdStory.Sections.Count() != 0)
                    {
                        foreach (var section in createdStory.Sections)
                        {
                            if (section.Value.SectionNumber == 0) continue;
                            System.Console.WriteLine($"Your current built sections (death section is not included):\n Section Id in relation to story: {section.Value.SectionNumber} \n The text choice is: {section.Value.Text} \nChoice Ids to this section: {section.Value.OutcomeSelections} \n ---------------------------------------------------\n");
                        }
                    }
                    sectionResponse = await SupabaseService.SupabaseClient
                            .From<Section>()
                            .Where(x => x.Id > 0)
                            .Get();
                    sectionId = sectionResponse.Models.Max(m => m.Id) + 1;
                    Section currentSection = await SectionMethods.BuildSection(story.StoryId, count, isVagrant);
                    createdStory.Sections.Add(count, currentSection);
                    System.Console.WriteLine("Would you like to finish the story? (continue making sections) 1.no 0.yes");
                    choice = Overall_Methods.returnInt();
                    while (choice != 1 && choice != 0)
                    {
                        choice = Overall_Methods.returnInt();
                    }
                    Console.Clear();
                    count++;
                }
                sectionResponse = await SupabaseService.SupabaseClient
                            .From<Section>()
                            .Where(x => x.Id > 0)
                            .Get();
                sectionId = sectionResponse.Models.Any() ? sectionResponse.Models.Max(m => m.Id) + 1 : 0;

            }
            foreach (var entry in createdStory.Sections)
            {
                if (entry.Key == 0) continue;
                entry.Value.isVagrant = true;
                entry.Value.Id = sectionId;
                sectionId++;

                await Section_Methods.CreateSection(entry.Value);
            }

        }
        public async Task DoStoryHomework(int storyId)
        {
            var response = await SupabaseService.SupabaseClient
                .From<Section>()
                .Where(x => x.StoryId == storyId && x.CreatureString !=
            "")
                .Get();
            var sections = response.Models.Where(c => c.CreatureString !=
            "");

            foreach (Section section in sections)
            {
                await CreatureMethods.DoCreatureHomework(section.CreatureString);
                var CreatureResponse = await SupabaseService.SupabaseClient
                .From<Creature>()
                .Where(x => x.Name == section.CreatureString)
                .Get();
                var Creatures = CreatureResponse.Models;
                foreach (Creature creature in Creatures)
                {
                    string creatureAttack = creature.AttackString;
                    if (creature.CreatureMultiattack)
                    {
                        string[] creatureAttackArray = creatureAttack.Split(",");
                        foreach (string attack in creatureAttackArray)
                        {
                            await AttackMethods.DoAttackHomework(attack, creature.InitiaveMod);
                        }
                    }
                    else
                    {
                        await AttackMethods.DoAttackHomework(creatureAttack, creature.InitiaveMod);
                    }
                }
            }

        }
        public async Task DoStoryHomework()
        {
            var storyResponse = await SupabaseService.SupabaseClient
                .From<Story>()
                .Where(x => x.StoryId != 0)
                .Get();
            int storyId;
            System.Console.WriteLine("Select a story to enter the creatures and attacks for");
            foreach (var model in storyResponse.Models)
            {
                System.Console.WriteLine($"Story ID:{model.StoryId}\nStory Name:{model.storyName}\n------------------------------------------\n");
            }
            int StoryID;
            do
            {
                StoryID = Overall_Methods.returnInt();
            } while (StoryID == 0);
            var StoryBuilder = storyResponse.Models.Where(x => x.StoryId == StoryID).Any();
            if (StoryBuilder) await DoStoryHomework(StoryID);


        }
        public async Task DeleteStory()
        {
            var response = await SupabaseService.SupabaseClient
                .From<Story>()
                .Where(x => x.Deleteable == true)
                .Get();

            var stories = response.Models;
            foreach (var story in stories)
            {
                System.Console.WriteLine($"Id:{story.StoryId}");
                System.Console.WriteLine($"Name: {story.storyName}");
                System.Console.WriteLine("------------------");
            }
            System.Console.WriteLine("Enter the Id of the story that you wish to delete:");
            int selectedId = Overall_Methods.returnInt();
            var selectedStory = stories.FirstOrDefault(m => m.StoryId == selectedId);
            if (selectedStory == null || selectedStory.Deleteable == false)
            {
                System.Console.WriteLine("Selected Story is null");
                return;
            }
            await SupabaseService.SupabaseClient
                .From<Story>()
                .Where(x => x.StoryId == selectedId)
                .Delete();
            await SupabaseService.SupabaseClient
            .From<Section>()
            .Where(x => x.StoryId == selectedId)
            .Delete();
            System.Console.WriteLine($"The Story entity and section entities linked to story Name:{selectedStory.storyName} have been deleted");
        }
    }

}