using System;
using Automatonymous;
using MassTransit.Messages;
using MassTransit.Saga;

namespace MassTransit.Publisher
{

    class Program
    {
        static void Main(string[] args)
        {

            var repository = new InMemorySagaRepository<AnalysisStateMachineInstance>();
            var machine = new AnalysisStateMachine();

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                //sbc.ReceiveEndpoint(host, "analysis_state", e =>
                //{
                //    e.PrefetchCount = 8;
                //    e.StateMachineSaga(machine, repository);
                //}); ;

            });

            bus.Start();

            bus.Publish(new YourMessage { Text = "Hi" });

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            bus.Stop();
        }
    }
}
