using System;

namespace PasswordHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Please specify the password as argument.");
                return;
            }

            var pwd = RabbitMqPasswordHelper.EncodePassword(args[0]);
            Console.WriteLine(pwd);
        }
    }
}
