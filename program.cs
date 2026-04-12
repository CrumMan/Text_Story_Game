using System.Runtime.InteropServices;
using textSim.database;
namespace text_Sim
{
    class Program
    {
        static async Task Main()
        {
            await SupabaseService.Initialize();
            overall_Methods Overall_Methods = new();
            storyMethods StoryMethods = new();
            creatureMethods CreatureMethods = new();
            attackMethods AttackMethods = new();

            Console.WriteLine("Welcome to your personal Text Story Game! Where we create an amazing immersive story!");
            int selection = 1;
            bool numb = false;
            Console.Clear();
            Console.WriteLine("\x1b[3J");

            int choice = 3;
            int chosen = 1;
            while (choice != 0)
            {
                System.Console.WriteLine($"Enter the number for the following choice. \n1.Creation Mode\n2.Play a Story Mode");
                choice = Overall_Methods.returnInt();
                if (choice == 1)
                {
                    while (true)
                    {
                        System.Console.WriteLine("Choose to:\n1. Build or Delete a Story \n 2. Build, Edit, or Delete a Creature \n3. Build or Delete an attack \n0. To go back to select either Reading or Create mode");
                        selection = Overall_Methods.returnInt();
                        if (selection == 1)
                        {
                            System.Console.WriteLine("1.Build a story \n2.Delete a story\n3. Add creatures and Attacks to Creatures in a story.");
                            chosen = Overall_Methods.returnInt();
                            if (chosen == 1)
                            {
                                System.Console.WriteLine("Does this story have a seprate vagrant storyline? \n 1.Yes 2.No");
                                bool selectBool = false;
                                bool isVagrant = false;
                                while (!selectBool)
                                {
                                    int select = Overall_Methods.returnInt();
                                    if (select == 1) { selectBool = true; isVagrant = true; }
                                    else if (select == 2) { selectBool = true; }
                                    else { System.Console.WriteLine("The only options are 1 or 2 to add a vagrant storyline"); }
                                }
                                await StoryMethods.CreateStory(isVagrant);
                            }
                            if (chosen == 2) await StoryMethods.DeleteStory();
                            if (chosen == 3) await StoryMethods.DoStoryHomework();

                        }
                        if (selection == 2)
                        {
                            System.Console.WriteLine("Choose to:\n1.Build a new Creature \n2.Edit a Creature \n3.Delete a creature");
                            chosen = Overall_Methods.returnInt();
                            if (chosen == 1) await CreatureMethods.insertCreature();
                            if (chosen == 2) await CreatureMethods.EditCreature();
                            if (chosen == 3) await CreatureMethods.DeleteCreature();
                        }
                        if (selection == 3)
                        {
                            System.Console.WriteLine("Choose to:\n1.Build an attack\n2.Delete an Attack");
                            chosen = Overall_Methods.returnInt();
                            if (chosen == 1) await AttackMethods.CreateAttack();
                            if (chosen == 2) await AttackMethods.DeleteAttack();
                        }
                        if (selection == 0) break;
                    }
                }
            }


            if (choice == 2)
            {
                while (selection != 0 || numb == false)
                {
                    Character character = new Character();
                    Story story = new Story(character);
                    await story.Start();
                    System.Console.WriteLine("Enter any number except 0 to restart the story.");
                    numb = int.TryParse(Console.ReadLine(), out selection);
                }
            }
        }
    }

}