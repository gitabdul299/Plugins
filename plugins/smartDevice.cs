using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace Plugins
{
    public class ValidateWarrantyStatus : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service (for debugging)
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Ensure the plugin is running on Create of Service Request
            if (context.MessageName.ToLower() != "create" || !context.InputParameters.Contains("Target") || !(context.InputParameters["Target"] is Entity))
                return;

            Entity serviceRequest = (Entity)context.InputParameters["Target"];

            // Check if 'Device' (aa_device) is linked in the Service Request
            if (!serviceRequest.Attributes.Contains("aa_device") || !(serviceRequest["aa_device"] is EntityReference))
            {
                tracingService.Trace("Device is not linked to the Service Request. Exiting plugin.");
                return;
            }

            // Obtain the organization service
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Fetch the linked Device record using the Device Name entity (aa_devicename)
                Guid deviceId = ((EntityReference)serviceRequest["aa_device"]).Id;
                Entity device = service.Retrieve("aa_devicename", deviceId, new ColumnSet("aa_warrantyexpirationdate", "aa_name"));

                if (device != null && device.Attributes.Contains("aa_warrantyexpirationdate"))
                {
                    DateTime warrantyExpirationDate = (DateTime)device["aa_warrantyexpirationdate"];
                    tracingService.Trace($"Warranty Expiration Date: {warrantyExpirationDate}");

                    // Check if the warranty has expired
                    if (warrantyExpirationDate < DateTime.UtcNow)
                    {
                        // Update the warranty status field to indicate "Out of Warranty"
                        serviceRequest["aa_warranty_status"] = new OptionSetValue(954000002); // "Out of Warranty"

                        // Send a notification to the customer (create an email activity)
                        if (serviceRequest.Attributes.Contains("aa_customer") && serviceRequest["aa_customer"] is EntityReference)
                        {
                            Guid accountId = ((EntityReference)serviceRequest["aa_customer"]).Id; // Assuming 'aa_customer' is the Account lookup
                            string deviceName = device.Attributes.Contains("aa_name") ? device["aa_name"].ToString() : "your device";

                            SendNotification(service, accountId, deviceName);
                        }
                        else
                        {
                            tracingService.Trace("Customer is not linked to the Service Request. No notification sent.");
                        }
                    }
                }
                else
                {
                    tracingService.Trace("Warranty Expiration Date not found on the linked Device.");
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace("ValidateWarrantyStatus Plugin Error: {0}", ex.ToString());
                throw new InvalidPluginExecutionException("An error occurred while validating the warranty status.", ex);
            }
        }

        private void SendNotification(IOrganizationService service, Guid accountId, string deviceName)
        {
            // Create and send an email activity to notify the customer
            Entity email = new Entity("email");
            email["subject"] = "Warranty Expired for Your Device";
            email["description"] = $"The warranty for your device '{deviceName}' has expired. Please contact support for further assistance.";
            email["to"] = new EntityCollection(new List<Entity>
            {
                new Entity("activityparty") { ["partyid"] = new EntityReference("account", accountId) }
            });

            service.Create(email);
        }
    }
}
