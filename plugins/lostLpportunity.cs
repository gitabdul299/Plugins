using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
public class CloseOpportunityActivities : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity opportunity)
            {
                if (opportunity.LogicalName != "opportunity") return;

                // Check if Opportunity is being closed as Lost
                if (opportunity.Attributes.Contains("statecode") && (OptionSetValue)opportunity["statecode"] == new OptionSetValue(2))
                {
                    Guid opportunityId = opportunity.Id;

                    // Retrieve and close all open activities related to the Opportunity
                    QueryExpression query = new QueryExpression("activitypointer")
                    {
                        ColumnSet = new ColumnSet("activityid"),
                        Criteria = new FilterExpression
                        {
                            Conditions =
                        {
                            new ConditionExpression("regardingobjectid", ConditionOperator.Equal, opportunityId),
                            new ConditionExpression("statecode", ConditionOperator.Equal, 0) // Open activities
                        }
                        }
                    };

                    EntityCollection activities = service.RetrieveMultiple(query);
                    foreach (Entity activity in activities.Entities)
                    {
                        Entity closeActivity = new Entity(activity.LogicalName, activity.Id)
                        {
                            ["statecode"] = new OptionSetValue(1), // Closed
                            ["statuscode"] = new OptionSetValue(3)  // Canceled
                        };
                        service.Update(closeActivity);
                    }

                    // Ensure Opportunity is set to "Closed as Lost"
                    Entity updateOpportunity = new Entity("opportunity", opportunityId)
                    {
                        ["statecode"] = new OptionSetValue(2), // Lost
                        ["statuscode"] = new OptionSetValue(4)  // Lost status
                    };
                    service.Update(updateOpportunity);
                }
            }
        }
    }

}
