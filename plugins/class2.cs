using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
public class SalesOrderIntegrationPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Ensure the plugin runs only on the Sales Order entity
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity targetEntity)
                {
                    if (targetEntity.LogicalName != "salesorder") return;

                    // Extract required fields
                    if (!targetEntity.Attributes.Contains("productid") || !targetEntity.Attributes.Contains("quantity"))
                    {
                        throw new InvalidPluginExecutionException("Product ID and Quantity are required to create a Sales Order.");
                    }

                    Guid productId = ((EntityReference)targetEntity["productid"]).Id;
                    int quantity = (int)targetEntity["quantity"];
                    Guid orderId = targetEntity.Id;

                    // Prepare the payload for the API
                    var payload = new
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        OrderId = orderId
                    };

                    string apiUrl = "https://external-inventory-api.com/reserve";

                    // Send the HTTP POST request
                    using (HttpClient httpClient = new HttpClient())
                    {
                        string jsonPayload = JsonConvert.SerializeObject(payload);
                        StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = httpClient.PostAsync(apiUrl, content).GetAwaiter().GetResult();

                        if (!response.IsSuccessStatusCode)
                        {
                            string errorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            throw new InvalidPluginExecutionException($"Inventory reservation failed: {errorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace("SalesOrderIntegrationPlugin Error: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred in the SalesOrderIntegrationPlugin.", ex);
            }
        }
    }
}

