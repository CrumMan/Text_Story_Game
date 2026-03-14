using System.Runtime.InteropServices;
namespace textSim
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Welcome to your personal Text Story Game! Where we create an amazing immersive story!");
            int selection = 1;
            bool numb = false;
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            while (selection != 0 || numb == false)
            {

                Character character = new Character();
                Story story = new Story(character);
                story.Start();
                System.Console.WriteLine("Enter any number except 0 to restart the story.");
                numb = int.TryParse(Console.ReadLine(), out selection);
            }
        }
    }
}