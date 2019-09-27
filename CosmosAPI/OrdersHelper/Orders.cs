using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosAPI.OrdersHelper
{
    public class Order
    {
        public string OrderId { get; set; }
        public string Product { get; set; }
        public string Email { get; set; }
        public string Price { get; set; }
        public string CustomerName { get; set; }

    }
}
