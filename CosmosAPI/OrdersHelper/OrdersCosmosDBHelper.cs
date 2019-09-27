using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace CosmosAPI.OrdersHelper
{
    public class OrdersCosmosDBHelper
    {
        public static Uri myDBUri => UriFactory.CreateDatabaseUri("orders");
        public static Uri MyOrdersCollectionUri =>
           UriFactory.CreateDocumentCollectionUri("orders", "orders");
       
        public async Task<Document> CreateOrder(DocumentClient client, object order)
        {
            var result = await client.CreateDocumentAsync(MyOrdersCollectionUri, order);
            var document = result.Resource;
            return document;
        }


        public async Task<IList<Order>> GetAllOrders(DocumentClient client)
        {
            IList<Order> orders = new List<Order>();
            try
            {

                var sql = "SELECT * FROM c";
                var options = new FeedOptions { EnableCrossPartitionQuery = true };

                var query = client
                    .CreateDocumentQuery(MyOrdersCollectionUri, sql, options)
                    .AsDocumentQuery();

                while (query.HasMoreResults)
                {
                    var documents = await query.ExecuteNextAsync();
                    foreach (var document in documents)
                    {
                        orders.Add(JsonConvert.DeserializeObject<Order>(document.ToString()));
                    }
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return orders;
        }
    }
}
