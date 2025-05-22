using Microsoft.Xrm.Sdk;
using System;

namespace Plugins
{
    public class CallPowerAutomateActionPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Define input parameters for the action
                var request = new OrganizationRequest("aa_TestActionAccountUpdate");

                // Example: Adding parameters to the request if required
                // request["ParameterName"] = parameterValue;

                // Execute the action
                OrganizationResponse response = service.Execute(request);

                // Optionally, handle the response if needed
                // var result = response["ResultField"];
            }
            catch (InvalidPluginExecutionException ex)
            {
                throw; // Rethrow InvalidPluginExecutionException as is
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in the CallPowerAutomateActionPlugin.", ex);
            }
        }
    }
}
