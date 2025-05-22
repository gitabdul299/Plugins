using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using System.Diagnostics;

public class ValidateVehicleInfoPlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
        IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
        IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
        IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

        try
        {
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity targetEntity = (Entity)context.InputParameters["Target"];
                if (targetEntity.LogicalName != "lead") return; // Ensure plugin runs only on the Lead entity

                string makeId = targetEntity.Contains("aa_makeid") ? targetEntity["aa_makeid"].ToString() : string.Empty;
                string makeName = targetEntity.Contains("aa_makename") ? targetEntity["aa_makename"].ToString() : string.Empty;

                if (string.IsNullOrEmpty(makeId) || string.IsNullOrEmpty(makeName))
                {
                    throw new InvalidPluginExecutionException("Both Make Id and Make Name are required.");
                }

                // Call API and validate the vehicle data
                bool isValid = ValidateVehicleDataAsync(makeId, makeName, tracingService).GetAwaiter().GetResult();

                if (!isValid)
                {
                    throw new InvalidPluginExecutionException("The Make Id and Make Name combination is not valid.");
                }
            }
        }
        catch (Exception ex)
        {
            tracingService.Trace("ValidateVehicleInfoPlugin Error: " + ex.Message);
            throw new InvalidPluginExecutionException("An error occurred while validating vehicle data: " + ex.Message);
        }
    }

    private async Task<bool> ValidateVehicleDataAsync(string makeId, string makeName, ITracingService tracingService)
    {
        string apiUrl = "https://vpic.nhtsa.dot.gov/api/vehicles/getallmakes?format=json";

        using (HttpClient httpClient = new HttpClient())
        {
            try
            {
                string response = await httpClient.GetStringAsync(apiUrl);
                tracingService.Trace("API Response: " + response);

                var makesData = JsonConvert.DeserializeObject<MakeApiResponse>(response);

                if (makesData == null || makesData.Results == null)
                {
                    tracingService.Trace("API returned null or invalid data.");
                    return false;
                }

                // Check if Make Id and Make Name exist in API data
                var make = makesData.Results.FirstOrDefault(m =>
                    m.MakeId.ToString() == makeId && m.MakeName.Equals(makeName, StringComparison.OrdinalIgnoreCase));

                return make != null;
            }
            catch (Exception ex)
            {
                tracingService.Trace("API Call Failed: " + ex.Message);
                return false;
            }
        }
    }
}

// Wrapper class to map API response
public class MakeApiResponse
{
    public List<Make> Results { get; set; }
}

// Make entity model from API response
public class Make
{
    [JsonProperty("Make_ID")]
    public int MakeId { get; set; }

    [JsonProperty("Make_Name")]
    public string MakeName { get; set; }
}
