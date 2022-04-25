using Azure.Messaging.ServiceBus;
using InventoryService.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using OrderService.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService
{
    public class InventoryResponseListener : IHostedService
    {
        private readonly IOrderDeletor orderDeletor;
        private readonly IConfiguration config;

        public InventoryResponseListener(IOrderDeletor orderDeletor, IConfiguration config)
        {
            this.orderDeletor = orderDeletor;
            this.config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var sbConnection = config.GetConnectionString("SBConnectionString");
            //var client = new ServiceBusClient(sbConnection);
            //var receiver = client.CreateReceiver("inventory-updated","inventory-response");
            //var message = receiver.ReceiveMessageAsync();

            //this.Subscribe(message.ToString());

            var subscriptionClient = new SubscriptionClient(sbConnection, "inventory-updated", "inventory-response");

            try
            {
                subscriptionClient.RegisterMessageHandler(
                    async (message, token) =>
                    {
                        var messageJson = Encoding.UTF8.GetString(message.Body);
                        var response = JsonConvert.DeserializeObject<InventoryResponse>(messageJson);

                        if (!response.IsSuccess)
                        {
                            orderDeletor.Delete(response.OrderId).GetAwaiter().GetResult();
                        }

                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    },
                    new MessageHandlerOptions(async args => Console.WriteLine(args.Exception))
                    { MaxConcurrentCalls = 1, AutoComplete = false });
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return Task.CompletedTask;
        }

        //private bool Subscribe(string message)
        //{
        //    var response = JsonConvert.DeserializeObject<InventoryResponse>(message);
        //    if (!response.IsSuccess)
        //    {
        //        orderDeletor.Delete(response.OrderId).GetAwaiter().GetResult();
        //    }
        //    return true;
        //}

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
