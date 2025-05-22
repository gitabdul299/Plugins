using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{

    public class D365option : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)

        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity curdEntity = (Entity)context.InputParameters["Target"];
                bool areYouDeveloperBool = curdEntity.GetAttributeValue<bool>("new_areyoudeveloper");
                string areYouDeveloperText = areYouDeveloperBool ? "Yes" : "No";

                OptionSetValue regionValue = curdEntity.GetAttributeValue<OptionSetValue>("new_region");
                int regionRealValue = regionValue.Value;
                string regionText = regionValue.Value.ToString(); //south



                RetrieveAttributeRequest attributeRequest = new RetrieveAttributeRequest 
                {
                    EntityLogicalName = curdEntity.LogicalName, // logical name of entity
                    LogicalName = "new_region",  //field name
                    RetrieveAsIfPublished = true
                };

                RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)service.Execute(attributeRequest);
                PicklistAttributeMetadata picklistMetadata = (PicklistAttributeMetadata)attributeResponse.AttributeMetadata;


                // step3: get the label of selected optionset value
                string optionLabel = picklistMetadata.OptionSet.Options
                    .Where(o => o.Value == regionRealValue)
                    .Select(o => o.Label.UserLocalizedLabel.Label)
                    .FirstOrDefault();

                string multiLineResult = "Are you developer? Bool" + " '" + areYouDeveloperBool +
                    "' Are you developerText '" + " '" + areYouDeveloperText + 
                    "' Region Value'" + regionRealValue + "'Region Text'" + optionLabel + " '";


                curdEntity.Attributes["new_multiline"] = multiLineResult;
                service.Update(curdEntity);

            }
        }
    }
}