using System;
using System.Text.Json;
using Azure.Data.Tables;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace QueueFunctionICE3;

public class Function1
{
    private readonly ILogger<Function1> _logger;
    private readonly string _storageConnectionString;
    private TableClient _tableclient;


    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
        _storageConnectionString = Environment.GetEnvironmentVariable("connection");
        var serviceClient = new TableServiceClient(_storageConnectionString);
        _tableclient = serviceClient.GetTableClient("PeopleTable");
    }

    //[Function(nameof(Function1))]
    //public void Run([QueueTrigger("ice3", Connection = "connection")] QueueMessage message)
    //{
    //    _logger.LogInformation("C# Queue trigger function processed: {messageText}", message.MessageText);
    //}

    [Function(nameof(QueuePeopleSender))]

    public async Task QueuePeopleSender([QueueTrigger("ice3", Connection ="connection")] QueueMessage message)
    {

        _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

        await _tableclient.CreateIfNotExistsAsync();

        var person = JsonSerializer.Deserialize<PersonEntity>(message.MessageText);

        if (person == null)
        {
            _logger.LogError("Failed to deserialize JSON message.");
            return;
        }
        person.RowKey = Guid.NewGuid().ToString();
        person.PartitionKey = "People";

        _logger.LogInformation($"Saving entity with RowKey: {person.RowKey}");

        await _tableclient.AddEntityAsync(person);
        _logger.LogInformation("Successfully saved person to table.");


    }


}