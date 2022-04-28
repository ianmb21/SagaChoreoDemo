using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OrderService.Models;
using OrderService.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderCreator orderCreator;
        private readonly IConfiguration config;

        public OrderController(IOrderCreator orderCreator, IConfiguration config)
        {
            this.orderCreator = orderCreator;
            this.config = config;
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public async Task Post([FromBody] OrderDetail orderDetail)
        {
            var id = await orderCreator.Create(orderDetail);

            var message = new OrderRequest
            {
                OrderId = id,
                ProductId = orderDetail.ProductId,
                Quantity = orderDetail.Quantity
            };

            var sbConnection = config.GetConnectionString("SBConnectionString");
            var client = new ServiceBusClient(sbConnection);
            var sender = client.CreateSender("order-created");
            var body = JsonSerializer.Serialize(message);
            var sbMessage = new ServiceBusMessage(body);
            await sender.SendMessageAsync(sbMessage);
        }
    }
}
