using System;
using Microsoft.Xrm.Sdk;

namespace plugins
{
    public class SetAccountTierPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Ensure the plugin is triggered on Opportunity creation
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity && entity.LogicalName == "opportunity")
                {
                    // Get custom estimated revenue and account reference
                    Money estimatedRevenue = entity.GetAttributeValue<Money>("aa_estimatedvalue");
                    EntityReference accountRef = entity.GetAttributeValue<EntityReference>("aa_accountlookup");

                    // Proceed only if both values exist
                    if (estimatedRevenue != null && accountRef != null)
                    {
                        if (estimatedRevenue.Value > 100000)
                        {
                            // Update the Account Tier field on the related account
                            Entity accountToUpdate = new Entity("account", accountRef.Id);
                            accountToUpdate["aa_accounttier"] = new OptionSetValue(954000001); // Example: 100000001 = Premium
                            service.Update(accountToUpdate);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException("Error in SetAccountTierPlugin: " + ex.Message, ex);
            }
        }
    }
}
