using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IProducer<string, string> producer;
        public OrderController() 
        {
            var config = new ProducerConfig
            {
                BootstrapServers = "localhost:9093", // Replace with your Kafka broker address
            };
            producer = new ProducerBuilder<string, string>(config).Build();
        }



        [HttpGet]
        public async Task<IActionResult> OrderStatusUpdate() 
        {
            try
            {
                var dr = await producer.ProduceAsync("order_status_updated", new Message<string, string>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = "ALLAH is One",
                });

                return Ok($"Produced message: {dr.Value}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error producing message: {ex.Message}");
            }
        }


    }
}
