using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace plugins
{
    public class CheckForMap : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity contact = (Entity)context.InputParameters["Target"];

            if (contact.Attributes.Contains("parentcustomerid"))
            {
                EntityReference accountLookup = contact.GetAttributeValue<EntityReference>("parentcustomerid");
                Guid accountId = accountLookup.Id;

                Entity accountResult = service.Retrieve(accountLookup.LogicalName, accountId, new Microsoft.Xrm.Sdk.Query. ColumnSet("new_maps"));

            if (accountResult.Attributes.Contains("new_maps"))
            {
                OptionSetValue checkForMap = accountResult.GetAttributeValue<OptionSetValue>("new_maps");

            if (checkForMap.Value == 100000000) // assuming 100000000 represents "Yes"
            {
                            // The account is ready for map, so allow contact creation.
            }
             else if (checkForMap.Value == 100000001) // assuming 100000001 represents "No"
            {
                            // The account is not ready for map, throw an exception to prevent contact creation.
             throw new InvalidPluginExecutionException("The contact 'Parent Customer (Account)' does not have mapping permission. Please create the contact with another 'Parent Customer (Account)' record.");
            }
            }
            }
            }
        }
    }
}