using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace ReadOrdersQueueFunctionApp
{
    public static class ReadFromOrdersQueue
    {
        private static HttpClient _client = new HttpClient();

        [FunctionName("ReadFromOrdersQueue")]
        public static async Task RunAsync(
            [QueueTrigger("orders", Connection = "AzureWebJobsStorage")]Order order,
            [SendGrid(ApiKey = "SendGridApiKey")] ICollector<SendGridMessage> sender,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {order.OrderId}");

            var email = order.Email;
            log.LogInformation($"Got order from {email} \n Order Id:{order.OrderId}");

            var cosmosAPICreateOrderUrl = Environment.GetEnvironmentVariable("CosmosAPIUrl") + "CreateOrder";
            
            _client.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _client.PostAsJsonAsync(cosmosAPICreateOrderUrl, order);
            var message = new SendGridMessage();
            message.From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender"));
            if (response.IsSuccessStatusCode)
            {                
                message.AddTo(email);
                message.Subject = $"Order placed || Product Name: {order.Product}";
                message.HtmlContent = $"Hello {order.CustomerName}, " +
                    $"Thank you for your order!" +
                    $"Your order is successfully placed with us for Product: {order.Product} for the purchase amount {order.Price}" +
                    $"\n Regards, Pratyush&Company";
                if (email.EndsWith("@publicissapient.com"))
                    sender.Add(message);
            }
            else
            {
                message.AddTo("pgupta116@sapient.com");
                message.Subject = $"Order failed || Product Name: {order.Product}";
                message.HtmlContent = $"Order failed. Please check the issue.";
                if (email.EndsWith("@publicissapient.com"))
                    sender.Add(message);
            }
        }
    }

    public class Order
    {
        public string OrderId { get; set; }
        public string Product { get; set; }
        public string Email { get; set; }
        public string Price { get; set; }
        public string CustomerName { get; set; }
    }
}
