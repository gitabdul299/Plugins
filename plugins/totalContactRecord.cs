using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
     public class totalContactRecord : IPlugin

    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                tracingService.Trace("inside the target entity");
                Entity contact = (Entity)context.InputParameters["Target"];
                tracingService.Trace("target entity completed");

                if (contact.Attributes.Contains("parentcustomerid"))
                {
                    tracingService.Trace("getting inside account name lookup");
                    EntityReference accountLookup = contact.GetAttributeValue<EntityReference>("parentcustomerid");
                    Guid accountId = accountLookup.Id;


                    QueryExpression query = new QueryExpression("contact");
                    query.ColumnSet = new ColumnSet("parentcustomerid");
                    query.Criteria.AddCondition("parentcustomerid", ConditionOperator.Equal,accountId); //on create
                  //  query.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);  //statecode=0 active     //on update
                    EntityCollection coll = service.RetrieveMultiple(query);

                    if (coll.Entities.Count > 0)
                    {
                        Entity accountObj = new Entity("account", accountId); //update total count in account entity
                        accountObj["new_totalcontact"] = coll.Entities.Count;
                        service.Update(accountObj);
                        tracingService.Trace("tracing completed");
                    }


                }
            }
        }
    }
}

