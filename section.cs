using System;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Supabase.Postgrest;
using textSim.database;
using Newtonsoft.Json;

namespace text_Sim
{
    [Table("sections")]
    public class Section : BaseModel
    {
        [PrimaryKey("id")]
        [Column("id")]
        public int Id { get; set; }
        [Column("storyId")]
        public int StoryId { get; set; }
        [Column("sectionNumber")]
        public int SectionNumber { get; set; }
        [Column("outcomeSelections")]
        public string OutcomeSelections { get; set; }
        [Column("outcomes")]
        public string Outcomes { get; set; }
        [Column("text")]
        public string Text { get; set; }
        [Column("creatureString")]
        public string CreatureString { get; set; }
        [Column("isVagrant")]
        public bool isVagrant { get; set; } = false;
        [Newtonsoft.Json.JsonIgnore]
        Character Character;


        //a section holds an encounter if any. If there is one the encounter is played through. I chose than after for text to be displayed and for the user to select from a list of outcomes. Than after selection the outcome is given through the run function.
        public Section() { }
        public Section(int sectionNumber, string text)
        {
            SectionNumber = sectionNumber;
            Text = text;
            CreatureString = "";

        }
        public Section(int id, int storyId, int sectionNumber, string text, string[] outcomes, int[] outcomeSelections, string creature = "", bool IsVagrant = false)
        {
            Id = id;
            StoryId = storyId;
            SectionNumber = sectionNumber;
            Text = text;
            Outcomes = string.Join(",", outcomes);
            if (sectionNumber == 0) outcomeSelections = [9999999];
            OutcomeSelections = string.Join(",", outcomeSelections);
            if (CreatureString != "") CreatureString = creature;
            if (IsVagrant) isVagrant = IsVagrant;
        }
        public Section(int storyId, int sectionNumber, string text, string[] outcomes, int[] outcomeSelections, Character character, string creature = "", bool vagrant = false)
        {
            StoryId = storyId;
            SectionNumber = sectionNumber;
            Text = text;
            Outcomes = string.Join(",", outcomes);
            OutcomeSelections = string.Join(",", outcomeSelections);
            Character = character;
            if (CreatureString != "") CreatureString = creature;
        }
        public async Task<int> Run()
        {
            if (CreatureString == null)
            {
                System.Console.WriteLine(Text);
                return displayAndChooseChoices();
            }
            else
            {
                Creature creature = new Creature(CreatureString);
                Encounter encounter = new Encounter(Character, creature);
                encounter.entireEncounter();
                if (Character._hitPoints == 0)
                {
                    return 0;
                }
                else
                {
                    Console.ReadKey();
                    Console.Clear();
                    Console.WriteLine(Text);
                    return displayAndChooseChoices();
                }
            }
        }
        private int displayAndChooseChoices()
        {
            int count = 1;
            int selection = 0;
            string[] outcomesArray = Outcomes.Split(",");
            if (OutcomeSelections.Length > 1)
            {
                System.Console.WriteLine("Would you like to");
            }
            else { System.Console.WriteLine("You feel a power forcing you to... "); }
            foreach (string choice in outcomesArray)
            {
                Console.WriteLine($"{count}. {choice}");
                count++;
            }
            while (selection == 0 || selection > Outcomes.Length)
            {
                System.Console.WriteLine("Please choose an outcome");
                int.TryParse(Console.ReadLine(), out selection);
            }
            //outcome is in index notation hence the -1
            return OutcomeSelections[selection - 1];
        }
    }
    public class sectionMethods()
    {
        overall_Methods Overall_Methods = new();
        creatureMethods CreatureMethods = new();
        public async Task<int> InsertAndReturnSectionId(int storyId, int sectionId, bool isVagrant)
        {
            var response = await SupabaseService.SupabaseClient
                .From<Section>()
                .Where(x => x.Id == sectionId)
                .Get();
            while (response.Models.Any())
            {
                await CreateSection(storyId, sectionId, isVagrant);
                response = await SupabaseService.SupabaseClient
                .From<Section>()
                .Where(x => x.Id == sectionId)
                .Get();

            }
            var section = response.Models.FirstOrDefault(m => m.Id == sectionId);
            return section.SectionNumber;
        }
        public async Task CreateSection(Section section)
        {
            await SupabaseService.SupabaseClient.From<Section>().Insert(section);
        }

        public async Task CreateSection(int storyId, int sectionId, bool isVagrant)
        {
            Section section = await BuildSection(storyId, sectionId, isVagrant);
            await SupabaseService.SupabaseClient.From<Section>().Insert(section);
        }
        public async Task<Section> BuildSection(int storyId, int sectionId, bool isVagrant)
        {
            var sectionResponse = await SupabaseService.SupabaseClient
                            .From<Section>()
                            .Where(x => x.Id > 0)
                            .Get();

            int sectionPrimaryId;
            if (!sectionResponse.Models.Any()) sectionPrimaryId = 0;
            else sectionPrimaryId = sectionResponse.Models.Max(m => m.StoryId) + 1;
            System.Console.WriteLine($"\nYour New section number will be {sectionId}");
            Section currentSection = new Section
            {
                Id = sectionPrimaryId,
                SectionNumber = sectionId,
                StoryId = storyId
            };
            if (isVagrant) currentSection.isVagrant = true;
            int currentChoice = 999;
            System.Console.WriteLine("The section always starts with a possible creature encounter. Is there a creature encounter? \n 1.Yes \n2.No");
            do
            {
                currentChoice = Overall_Methods.returnInt();
            } while (currentChoice < 1 && currentChoice > 2);
            if (currentChoice == 1)
            {
                System.Console.WriteLine("What is the name of the creature the character will face?");
                currentSection.CreatureString = Overall_Methods.solveNull();
                // Creature creature = await CreatureMethods.GetCreature(currentSection.CreatureString);
            }

            System.Console.WriteLine("Enter the situation the character will have to choose after any encounter. Example: That was a intense encounter! A pathway is before you stretching right and left do you: ");
            currentSection.Text = Overall_Methods.solveNull();
            List<int> outcomeSelections = new();
            currentChoice = 1;
            while (currentChoice != 0)
            {
                System.Console.WriteLine("Enter an option for the player to choose Example: Go Left");
                currentSection.Outcomes = Overall_Methods.solveNull(currentSection.Outcomes);
                System.Console.WriteLine("What Outcome Selection will this option inhibit?(Remember, This option does not need to be currently available)");
                outcomeSelections.Add(Overall_Methods.returnInt());
                System.Console.WriteLine("Are you done adding outcomes and their selection types? 1.No 0.Yes");
                currentChoice = Overall_Methods.returnInt();
            }
            currentSection.OutcomeSelections = string.Join(",", outcomeSelections);
            return currentSection;
        }
    }
}