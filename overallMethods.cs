namespace text_Sim
{
    public class overall_Methods
    {
        public string solveNull(string previous = "")
        {
            string issue = null;
            int count = 0;
            while (issue == null)
            {
                if (count > 0) System.Console.WriteLine("You must enter text");
                issue = Console.ReadLine();
                count++;
            }
            if (previous != "")
            {
                issue = $"{previous},{issue}";
            }
            return issue;
        }
        public int returnInt()
        {
            string numString = Console.ReadLine();
            int number;
            while (!int.TryParse(numString, out number))
            {
                numString = Console.ReadLine();
            }
            return number;
        }
    }
}