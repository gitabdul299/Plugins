using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
    public class accountretrieve : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Get the context and organization service
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);


            // Check if the Target is provided and is an Entity
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity curdEntity = (Entity)context.InputParameters["Target"];


                // Check if the new_organization attribute exists
                if (curdEntity.Attributes.Contains("new_organization"))
                {
                    EntityReference accountLookup = curdEntity.GetAttributeValue<EntityReference>("new_organization");
                    Guid accountId = accountLookup.Id;


                    // Retrieve account information
                    Entity account = service.Retrieve(accountLookup.LogicalName, accountId, new Microsoft.Xrm.Sdk.Query.ColumnSet("name", "telephone1", "fax"));

                    curdEntity["new_accountname"] = account.GetAttributeValue<string>("name");
                    curdEntity["new_accountphone"] = account.GetAttributeValue<string>("telephone1");
                    curdEntity["new_accountfax"] = account.GetAttributeValue<string>("fax");
                    service.Update(curdEntity);
                }
            }
        }
    }
}
