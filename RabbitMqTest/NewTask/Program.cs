using System;
using RabbitMQ.Client;
using System.Text;

class NewTask
{
    public static void Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost", DispatchConsumersAsync = true };
        using(var connection = factory.CreateConnection()) // create connection
        using(var channel = connection.CreateModel())      // creatae channel (AMQP Model)
        {
            // declare exchange
            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);
            // declare queue: test1
            channel.QueueDeclare("test1", durable: true, autoDelete: false, exclusive: false);
            // declare queue: test2
            channel.QueueDeclare("test2", durable: true, autoDelete: false, exclusive: false);

            for (int i = 0; i < 31; i++)
            {
                var message = GetMessage(new string[]{i.ToString()});
                var body = Encoding.UTF8.GetBytes(message);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: properties, body: body);
                Console.WriteLine(" [x] Sent {0}", message);
            }
        }

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    private static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}