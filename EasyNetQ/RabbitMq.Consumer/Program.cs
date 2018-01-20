using System;
using System.Reflection;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using EasyNetQ.Loggers;
using RabbitMq.Messages;

namespace RabbitMq.Consumer
{
    public class Message2
    {
        public string Text { get; set; }
    }

    //public class MyConsumer : IConsume<MessageA>, IConsume<MessageB>, IConsumeAsync<MessageC>
    //{
    //    public void Consume(MessageA message) {...}

    //    public void Consume(MessageB message) {...}

    //    public Task Consume(MessageC message) {...}
    //}

    public class TextMessageConsumer : IConsume<TextMessage>
    {
        public void Consume(TextMessage message)
        {
            Console.WriteLine("Consumer -> Message: " + message.Text);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var bus = RabbitHutch.CreateBus("host=localhost", x => x.Register<IEasyNetQLogger>(_ => new ConsoleLogger())))
            {
                //bus.Subscribe<TextMessage>("test", HandleTextMessage);

                //bus.Receive<TextMessage>("build.queue", x => HandleTextMessage(x));

                //var subscriber = new AutoSubscriber(bus, "my_applications_subscriptionId_prefix");

                var subscriber = new AutoSubscriber(bus, "My_subscription_id_prefix")
                {
                    AutoSubscriberMessageDispatcher = new WindsorMessageDispatcher()
                };
                //autoSubscriber.Subscribe(GetType().Assembly);

                subscriber.Subscribe(Assembly.GetExecutingAssembly());

                Console.WriteLine("Listening for messages. Hit <return> to quit.");
                Console.ReadLine();
            }
        }

        static void HandleTextMessage(TextMessage textMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Got message: {0}", textMessage.Text);
            Console.ResetColor();
        }
    }


    public class WindsorMessageDispatcher : IAutoSubscriberMessageDispatcher
    {
        //private readonly IWindsorContainer container;

        //public WindsorMessageDispatcher(IWindsorContainer container)
        //{
        //    this.container = container;
        //}

        public void Dispatch<TMessage, TConsumer>(TMessage message) where TMessage : class where TConsumer : IConsume<TMessage>
        {
            Console.WriteLine("Dispatcher");
                new TextMessageConsumer().Consume((TextMessage)(object)message);

        }

        public Task DispatchAsync<TMessage, TConsumer>(TMessage message) where TMessage : class where TConsumer : IConsumeAsync<TMessage>
        {
            Console.WriteLine("Dispatcher");
            new TextMessageConsumer().Consume((TextMessage)(object)message);
            return Task.FromResult(123);

            //var consumer = _container.Resolve<TConsumer>();
            //return consumer.Consume(message).ContinueWith(t => _container.Release(consumer));
        }
    }
}
