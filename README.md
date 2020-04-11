# Durable function on .NET Core 3.1 and function v3
This is a durable function made using durale function framework on Visual Studio 2019. You have to get the durable function template and change it as per the application requirements

In the sample appication, it has a orchestrator function and calls three activity functions in a given order

## Installed Packages
Microsoft.NET.Sdk.Functions version 3 (3.0.5) and Microsoft.Azure.WebJobs.Extensions.DurableTask 2 (2.2.0) 

## Code snippets
### Http trigger function
This is a http trigger function and the entry point for the application
```
[FunctionName("processorder_HttpStart")]
public static async Task<HttpResponseMessage> HttpStart(
  [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]HttpRequestMessage req, 
  [DurableClient] IDurableOrchestrationClient starter, ILogger log)
{
    string instanceId = await starter.StartNewAsync("processorder", null);
    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
    return starter.CreateCheckStatusResponse(req, instanceId);
 }
```

### Orchestrator function to call the activity functions in a specific order
```
[FunctionName("processorder")]
public static async Task<List<string>> RunOrchestrator(
  [OrchestrationTrigger] IDurableOrchestrationContext context)
{
    var outputs = new List<string>();

    outputs.Add(await context.CallActivityAsync<string>("CheckProductAvailability", 
                                                        "Mi Band 4, Optical Mouse - red"));
    outputs.Add(await context.CallActivityAsync<string>("CreateInvoice", true));
    outputs.Add(await context.CallActivityAsync<string>("ShipProducts", 10000));

    return outputs;
  }
  ```
  
  ### Activity functions
  ```
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
```
