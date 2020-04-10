using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace durablefunc
{
    public static class processorder
    {
        [FunctionName("processorder_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req, [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            string instanceId = await starter.StartNewAsync("processorder", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("processorder")]
        public static async Task<List<string>> RunOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            outputs.Add(await context.CallActivityAsync<string>("CheckProductAvailability", "Mi Band 4, Optical Mouse - red"));
            outputs.Add(await context.CallActivityAsync<string>("CreateInvoice", true));
            outputs.Add(await context.CallActivityAsync<string>("ShipProducts", 10000));

            return outputs;
        }

        [FunctionName("CheckProductAvailability")]
        public static string ProductAvailability([ActivityTrigger] string productList, ILogger log)
        {
            log.LogInformation($"Product List - {productList}.");
            return $"Product List -  {productList}!";
        }

        [FunctionName("CreateInvoice")]
        public static string CreateInvoice([ActivityTrigger] bool productAvailable, ILogger log)
        {
            if(productAvailable)
            {
                log.LogInformation("Products are available");
                return "Products are available";
            }
            else
            {
                log.LogInformation("Products are not available");
                return "Products are not available";
            }
        }

        [FunctionName("ShipProducts")]
        public static string ShipProducts([ActivityTrigger] double totalAmount, ILogger log)
        {
            log.LogInformation($"Invoice Amount {totalAmount}.");
            return $"Invoice Amount {totalAmount}.";
        }

        
    }
}