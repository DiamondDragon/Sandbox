using System;
using System.Threading.Tasks;
using MassTransit.Messages;

//using MassTransit.Messages;

namespace MassTransit.Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, "test_queue", ep =>
                {
                    ep.Consumer<YourMessageConsumer>();

                    ep.Handler<YourMessage>(context => Console.Out.WriteLineAsync($"Received: {context.Message.Text}"));
                });
            });

            bus.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            bus.Stop();
        }
    }

    public class YourMessageConsumer : IConsumer<YourMessage>
    {
        public async Task Consume(ConsumeContext<YourMessage> context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var retryCount = context.GetRetryAttempt();

            await Console.Out.WriteLineAsync("Consumer: " + context.Message.Text);
        }
    }
}
