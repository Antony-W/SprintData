using System;

namespace SprintData
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Sprint Data Analyzer");
            Console.WriteLine("--------------------");
            Console.WriteLine();

            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length < 3)
            {
                Console.WriteLine("Please specify the filename to compare.");
            }
            else
            {
                var pd = new ProcessData(arguments[1], arguments[2]);
                pd.Start();
            }
        }
    }
}
