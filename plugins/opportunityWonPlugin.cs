using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Crm.Sdk.Messages;
using plugins;

namespace OpportunityWonPlugin
{
    public class SendEmailOnOpportunityWon : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            tracingService.Trace("SendEmailOnOpportunityWon plugin triggered.");

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity opportunity)
            {
                try
                {
                    if (opportunity.LogicalName == "opportunity" && opportunity.Contains("status") && ((OptionSetValue)opportunity["status"]).Value == 3)
                    {
                        IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                        IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                        tracingService.Trace("Opportunity is in won state.");

                        // Check for Account and Contact references
                        EntityReference customerRef = opportunity.GetAttributeValue<EntityReference>("customerid");
                        EntityReference contactRef = opportunity.GetAttributeValue<EntityReference>("parentcontactid");

                        tracingService.Trace("customerid: " + (customerRef != null ? customerRef.Id.ToString() : "null"));
                        tracingService.Trace("parentcontactid: " + (contactRef != null ? contactRef.Id.ToString() : "null"));

                        if (customerRef != null)
                        {
                            tracingService.Trace("Customer reference found: " + customerRef.Id);
                            SendEmail(service, context.UserId, customerRef, tracingService);
                        }
                        else if (contactRef != null)
                        {
                            tracingService.Trace("Contact reference found: " + contactRef.Id);
                            SendEmail(service, context.UserId, contactRef, tracingService);
                        }
                        else
                        {
                            tracingService.Trace("Neither customer nor contact reference found.");
                        }
                    }
                    else
                    {
                        tracingService.Trace("Opportunity is not in won state or status is not present.");
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    tracingService.Trace("OrganizationServiceFault: " + ex.Message);
                    throw new InvalidPluginExecutionException($"An error occurred in the SendEmailOnOpportunityWon plugin: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    tracingService.Trace("Exception: " + ex.Message);
                    throw new InvalidPluginExecutionException($"An unexpected error occurred in the SendEmailOnOpportunityWon plugin: {ex.Message}", ex);
                }
            }
            else
            {
                tracingService.Trace("Target is not present or not an Entity.");
            }
        }

        private void SendEmail(IOrganizationService service, Guid userId, EntityReference targetRef, ITracingService tracingService)
        {
            tracingService.Trace("Creating email entity.");
            Entity email = new Entity("email");
            email["subject"] = "Congratulations on Winning the Opportunity!";
            email["description"] = "We are excited to inform you that you have won the opportunity.";

            tracingService.Trace("Setting email parties.");
            Entity fromParty = new Entity("activityparty");
            fromParty["partyid"] = new EntityReference("systemuser", userId);

            Entity toParty = new Entity("activityparty");
            toParty["partyid"] = targetRef;

            email["from"] = new EntityCollection(new Entity[] { fromParty });
            email["to"] = new EntityCollection(new Entity[] { toParty });

            tracingService.Trace("Creating email in CRM.");
            Guid emailId = service.Create(email);

            tracingService.Trace("Sending email with ID: " + emailId);
            SendEmailRequest sendEmailRequest = new SendEmailRequest
            {
                EmailId = emailId,
                IssueSend = true
            };

            tracingService.Trace("Executing send email request.");
            service.Execute(sendEmailRequest);
            tracingService.Trace("Email sent successfully.");
        }
    }
}
