using Azure.Storage.Queues;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestQueueICE3
{

    internal class Program
    {

        static async Task Main(string[] args)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=st10449407cldv6212;AccountKey=YOmFkaZ2GNWZSq7TUhojCynDCA9jS1pirVsp4UdEvdbAnirT08WL47dD4rgUrHFcURlvI9PHMOuC+AStgNekWQ==;EndpointSuffix=core.windows.net";

            var queueClient = new QueueClient(
                connectionString,
                "ice3",
                new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 }
                );
            await queueClient.CreateIfNotExistsAsync();
            var person = new { Name = "General Kenobi", Email = "Obi-1@jedi-we-are.com" };
            string json = JsonSerializer.Serialize(person);
            await queueClient.SendMessageAsync(json);
            Console.WriteLine($"Message sent : {json}");
        }

    }
}
