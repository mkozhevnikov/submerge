using System;
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Some arguments are expected");
        }
        else
        {
            foreach (var arg in args)
            {
                Console.WriteLine($"You passed: {arg}");
            }
        }
    }
}
