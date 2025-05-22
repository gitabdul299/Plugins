using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace plugins
{
    public class activeAndDeactivateContact : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("EntityMoniker") && context.InputParameters["EntityMoniker"] is EntityReference)
            {
                
                EntityReference contactRef = (EntityReference)context.InputParameters["EntityMoniker"];

                // Retrieve the contact to get the parent account
                Entity contact = service.Retrieve(contactRef.LogicalName, contactRef.Id, new ColumnSet("parentcustomerid"));
                if (contact.Attributes.Contains("parentcustomerid"))
                {
                    
                    EntityReference accountLookup = contact.GetAttributeValue<EntityReference>("parentcustomerid");
                    Guid accountId = accountLookup.Id;

                    // Retrieve only active contacts associated with this account
                    QueryExpression query = new QueryExpression("contact")
                    {
                        ColumnSet = new ColumnSet("parentcustomerid"),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                            {
                                new ConditionExpression("parentcustomerid", ConditionOperator.Equal, accountId),
                                new ConditionExpression("statecode", ConditionOperator.Equal, 0) // statecode=0 active
                            }
                        }
                    };

                    EntityCollection contacts = service.RetrieveMultiple(query);
                    tracingService.Trace("total contact is" + contacts.Entities.Count);
                    // Count the active contacts
                    int totalActiveContacts = contacts.Entities.Count;
                    
                    // Update the Total Contact field on the Account entity
                    Entity accountToUpdate = new Entity(accountLookup.LogicalName, accountLookup.Id);
                    accountToUpdate["new_totalcontact"] = totalActiveContacts;
                    service.Update(accountToUpdate);
                    tracingService.Trace("Executed successfully");


                }
            }
        }
    }
}
