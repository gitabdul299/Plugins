using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    public class DelectingCountGetTheAccountAssociatedContactCount : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //throw new NotImplementedException();
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            //Initializes the IOrganizationService,which is used to interact with CRM data (query,retrieve,update,etc.).
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            //The (context.UserId) ensures that actions performed by this service is created in the context of the user who triggered the plugin.
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            service = serviceFactory.CreateOrganizationService(context.UserId);
            //The plugin verifies that it was triggered by a Delete event on the Contact entity:
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference entityRef &&
                entityRef.LogicalName == "contact" && context.MessageName == "Delete")
            //If the conditions are met, the plugin proceeds to execute 
            {
                try
                {
                    if (context.PreEntityImages.Contains("PreImage") && context.PreEntityImages["PreImage"] is Entity preImageContact)
                    {
                        //Retrieves a pre-image of the contact record.
                        //Pre-images are needed here to access the contact’s parentcustomerid field (account) before deletion.
                        if (preImageContact.Attributes.Contains("parentcustomerid") && preImageContact["parentcustomerid"] is EntityReference parentCustomerRef)
                        {
                            Guid accountId = parentCustomerRef.Id;

                            // Query to count the remaining active contacts associated with the account
                            QueryExpression query = new QueryExpression("contact")
                            {
                                ColumnSet = new ColumnSet("contactid"),
                                Criteria = new FilterExpression
                                {
                                    Conditions =
                                    {
                                        new ConditionExpression("parentcustomerid", ConditionOperator.Equal, accountId),
                                        new ConditionExpression("statecode", ConditionOperator.Equal, 0) // Active contacts only
                                    }
                                }
                            };

                            EntityCollection remainingContacts = service.RetrieveMultiple(query);
                            int totalContactCount = remainingContacts.Entities.Count;

                            // Update the Parent Account with the new total contact count
                            Entity accountToUpdate = new Entity("account", accountId)
                            {
                                ["new_totalcontact"] = totalContactCount // Assuming this is the field on Account to store contact count
                            };
                            service.Update(accountToUpdate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    tracingService.Trace("DeleteContactRecordAndTotalContactUpdated Plugin: {0}", ex.ToString());
                    throw new InvalidPluginExecutionException("An error occurred in the DeleteContactRecordAndTotalContactUpdated plugin.", ex);
                }
            }
        }
    }
}
