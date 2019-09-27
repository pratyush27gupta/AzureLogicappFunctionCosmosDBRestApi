using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosAPI.OrdersHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;

namespace CosmosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private string endpoint;
        private string masterKey;
        OrdersCosmosDBHelper ordersHelper;

        public OrdersController(IConfiguration config)
        {
            configuration = config;

            endpoint = configuration.GetConnectionString("CosmosDBEndpoint");
            masterKey = configuration.GetConnectionString("CosmosDBMasterKey");
            ordersHelper = new OrdersCosmosDBHelper();
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<IList<Order>>> GetAllOrders()
        {
            using (var client = new DocumentClient(new Uri(endpoint), masterKey))
            {
                var response = await ordersHelper.GetAllOrders(client);
                return Ok(response);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            using (var client = new DocumentClient(new Uri(endpoint), masterKey))
            {
                var response = await ordersHelper.CreateOrder(client, order);
                return Ok(response);
            }
        }
    }
}
