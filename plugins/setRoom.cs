using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace plugins
{
    public class SetRoomNamePlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity)
            {
                try
                {
                    // Ensure it's the correct entity
                    if (entity.LogicalName != "aa_room") return;

                    // Check for required fields
                    if (!entity.Attributes.Contains("aa_roomtype") || entity["aa_roomtype"] == null)
                        throw new InvalidPluginExecutionException("Room Type (OptionSet) is required.");
                    if (!entity.Attributes.Contains("aa_property") || entity["aa_property"] == null)
                        throw new InvalidPluginExecutionException("Property (lookup) is required.");
                    if (!entity.Attributes.Contains("aa_area") || entity["aa_area"] == null)
                        throw new InvalidPluginExecutionException("Area (text) is required.");

                    // Get Room Type (OptionSet label)
                    OptionSetValue roomTypeOption = (OptionSetValue)entity["aa_roomtype"];
                    string roomTypeLabel = GetOptionSetText(service, "aa_room", "aa_roomtype", roomTypeOption.Value);

                    // Get Property Name from lookup
                    EntityReference propertyRef = (EntityReference)entity["aa_property"];
                    string propertyName = propertyRef.Name; // Should already be populated

                    // Get Area text
                    string area = entity["aa_area"].ToString();

                    // Set the formatted name
                    string roomName = $"{roomTypeLabel} - {propertyName} - {area}";
                    entity["name"] = roomName;
                }
                catch (Exception ex)
                {
                    tracingService.Trace("SetRoomNamePlugin Error: {0}", ex.ToString());
                    throw new InvalidPluginExecutionException("An error occurred in SetRoomNamePlugin: " + ex.Message);
                }
            }
        }

        private string GetOptionSetText(IOrganizationService service, string entityLogicalName, string attributeLogicalName, int optionSetValue)
        {
            var request = new Microsoft.Xrm.Sdk.Messages.RetrieveAttributeRequest
            {
                EntityLogicalName = entityLogicalName,
                LogicalName = attributeLogicalName,
                RetrieveAsIfPublished = true
            };

            var response = (Microsoft.Xrm.Sdk.Messages.RetrieveAttributeResponse)service.Execute(request);
            var attributeMetadata = (Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata)response.AttributeMetadata;

            foreach (var option in attributeMetadata.OptionSet.Options)
            {
                if (option.Value == optionSetValue)
                {
                    return option.Label.UserLocalizedLabel?.Label ?? option.Value.ToString();
                }
            }

            return optionSetValue.ToString(); // fallback if label not found
        }
    }
}
