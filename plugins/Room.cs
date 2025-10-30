using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
    public class SetPropertyNamePlugin : IPlugin
        {
            public void Execute(IServiceProvider serviceProvider)
            {
                // Setup
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity && entity.LogicalName == "aa_property")
                {
                    // Get values
                    string city = entity.GetAttributeValue<string>("aa_city");
                    string propertyNumber = entity.GetAttributeValue<string>("aa_propertynumber");
                    EntityReference ownerRef = entity.GetAttributeValue<EntityReference>("aa_propertyowner");

                    string ownerName = string.Empty;

                    if (ownerRef != null)
                    {
                        Entity owner = service.Retrieve("contact", ownerRef.Id, new ColumnSet("fullname"));
                        ownerName = owner.GetAttributeValue<string>("fullname");
                    }

                    // Construct name
                    string propertyName = $"{city} - {propertyNumber} - {ownerName}";

                    // Set the name field (replace below with your schema name for Name field if not default)
                    entity["aa_name"] = propertyName;
                }
            }
        }
    }

