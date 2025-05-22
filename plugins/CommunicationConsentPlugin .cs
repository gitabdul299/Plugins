
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
    public class CommunicationConsentPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            

            {
                // Obtain tracing, context, and organization service
                ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Ensure plugin is triggered on Lead entity
                    if (context.PrimaryEntityName.ToLower() != "lead")
                        return;

                    // Execute for Create and Update messages
                    if (context.MessageName.ToLower() != "create" && context.MessageName.ToLower() != "update")
                        return;

                    // Get Lead ID
                    Guid leadId = context.PrimaryEntityId;

                    // Retrieve Input Parameters
                    Entity leadEntity = (Entity)context.InputParameters["Target"];

                    // Check if consent fields exist
                    bool hasConsentFields = leadEntity.Attributes.Contains("aa_allowphone") ||
                                            leadEntity.Attributes.Contains("aa_allowemail") ||
                                            leadEntity.Attributes.Contains("aa_allowsms") ||
                                            leadEntity.Attributes.Contains("aa_allowpost") ||
                                            leadEntity.Attributes.Contains("aa_allowfax");

                    if (!hasConsentFields)
                        return;

                    // Check if a Communication Consent record already exists for this Lead
                    QueryExpression query = new QueryExpression("aa_communicationconsent")
                    {
                        ColumnSet = new ColumnSet("aa_communicationconsentid"),
                        Criteria =
                {
                    Conditions =
                    {
                        new ConditionExpression("aa_leadid", ConditionOperator.Equal, leadId)
                    }
                }
                    };

                    EntityCollection existingConsents = service.RetrieveMultiple(query);

                    Entity consentRecord;

                    if (existingConsents.Entities.Count > 0)
                    {
                        // Update existing consent record
                        consentRecord = existingConsents.Entities[0];
                    }
                    else
                    {
                        // Create new consent record
                        consentRecord = new Entity("aa_communicationconsent")
                        {
                            ["aa_leadid"] = new EntityReference("lead", leadId)
                        };
                    }

                    // Map values from Lead to Communication Consent
                    if (leadEntity.Attributes.Contains("aa_allowphone"))
                        consentRecord["aa_allowphone"] = leadEntity["aa_allowphone"];

                    if (leadEntity.Attributes.Contains("aa_allowemail"))
                        consentRecord["aa_allowemail"] = leadEntity["aa_allowemail"];

                    if (leadEntity.Attributes.Contains("aa_allowsms"))
                        consentRecord["aa_allowsms"] = leadEntity["aa_allowsms"];

                    if (leadEntity.Attributes.Contains("aa_allowpost"))
                        consentRecord["aa_allowpost"] = leadEntity["aa_allowpost"];

                    if (leadEntity.Attributes.Contains("aa_allowfax"))
                        consentRecord["aa_allowfax"] = leadEntity["aa_allowfax"];

                    // Save the record
                    if (existingConsents.Entities.Count > 0)
                        service.Update(consentRecord);
                    else
                        service.Create(consentRecord);
                }
                catch (Exception ex)
                {
                    throw new InvalidPluginExecutionException($"Error in CommunicationConsentPlugin: {ex.Message}");
                }
            }
        }

    }
}