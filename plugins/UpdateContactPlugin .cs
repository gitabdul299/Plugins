using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;


namespace plugins
{
    public class UpdateContactPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // Check if ParentCustomerId exists in Shared Variables
            if (context.SharedVariables.Contains("aa_acountname"))
            {
                EntityReference parentCustomerId = (EntityReference)context.SharedVariables["aa_acountname"];

                // Retrieve ParentCustomerId Entity Record
                if (parentCustomerId.LogicalName == "account")
                {
                    Entity account = service.Retrieve("account", parentCustomerId.Id, new ColumnSet("name", "fax", "emailaddress1"));

                    // Prepare Contact update entity
                    Entity contactUpdate = new Entity("contact", context.PrimaryEntityId);

                    if (account.Attributes.Contains("name"))
                        contactUpdate["aa_accountname"] = account.Attributes["name"];
                    if (account.Attributes.Contains("fax"))
                        contactUpdate["fax"] = account.Attributes["fax"];
                    if (account.Attributes.Contains("emailaddress1"))
                        contactUpdate["emailaddress1"] = account.Attributes["emailaddress1"];

                    // Update the Contact
                    service.Update(contactUpdate);
                }
            }
        }
    }
}