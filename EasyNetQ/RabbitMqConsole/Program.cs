using System;
using EasyNetQ;
using EasyNetQ.Loggers;
using RabbitMq.Messages;

namespace RabbitMqConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost,product=MyCustomProduct", x => x.Register<IEasyNetQLogger>(_ => new ConsoleLogger())))
            {
                var input = "";
                Console.WriteLine("Enter a message. 'Quit' to quit.");
                while ((input = Console.ReadLine()) != "Quit")
                {
                    bus.Publish(new TextMessage { Text = input });

                   // bus.Send("build.queue",  new TextMessage { Text = input });
                }
            }
        }
    }
}
