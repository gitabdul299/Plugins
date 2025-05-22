using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
    public class multipleRetrieve : IPlugin
    {

        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity contact = (Entity)context.InputParameters["Target"];

                if (contact.Attributes.Contains("emailaddress1"))
                {
                    string contactEmail = contact.GetAttributeValue<string>("emailaddress1");
                    QueryExpression query = new QueryExpression("contact");
                    query.ColumnSet = new ColumnSet("emailaddress1");
                    query.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, contactEmail);
                    EntityCollection coll = service.RetrieveMultiple(query);

                    if (coll.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("your entered email address is already in use, please add new emai address");                    }
                }
            }
        }
    }
}
// triggered on pre-validation on create on contact entity