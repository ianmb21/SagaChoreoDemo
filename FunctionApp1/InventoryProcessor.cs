using System;
using Azure.Messaging.ServiceBus;
using InventoryService.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderService.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace InventoryService
{
    public class InventoryProcessor
    {
        private readonly IInventoryUpdator inventoryUpdator;

        public InventoryProcessor(IInventoryUpdator inventoryUpdator)
        {
            this.inventoryUpdator = inventoryUpdator;
        }
        [FunctionName("OrderCreatedListener")]
        public void Run([ServiceBusTrigger("order-created", "order-response", Connection = "SBConnectionString")]string mySbMsg, ILogger log)
        {
            //log.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");

            var sbConnection = Environment.GetEnvironmentVariable("SBConnectionString");
            var client = new ServiceBusClient(sbConnection);
            var sender = client.CreateSender("inventory-updated");

            var response = JsonConvert.DeserializeObject<OrderRequest>(mySbMsg);
            try
            {
                inventoryUpdator.Update(response.ProductId, response.Quantity).GetAwaiter().GetResult();

                var message = new InventoryResponse
                {
                    OrderId = response.OrderId,
                    IsSuccess = true
                };

                var body = JsonSerializer.Serialize(message);
                var sbMessage = new ServiceBusMessage(body);
                sender.SendMessageAsync(sbMessage);
            }
            catch (Exception)
            {
                var message = new InventoryResponse
                {
                    OrderId = response.OrderId,
                    IsSuccess = false
                };

                var body = JsonSerializer.Serialize(message);
                var sbMessage = new ServiceBusMessage(body);
                sender.SendMessageAsync(sbMessage);
            }
        }
    }
}
