using Microsoft.Xrm.Sdk;
using System;


namespace plugins
{
    public class CreateContactPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Ensure the target is an Entity of type Contact
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity contact = (Entity)context.InputParameters["Target"];

                // Check if the ParentCustomerId exists
                if (contact.Attributes.Contains("aa_acountname"))
                {
                    // Retrieve ParentCustomerId
                    EntityReference parentCustomerId = (EntityReference)contact.Attributes["aa_acountname"];

                    // Store ParentCustomerId in Shared Variables
                    context.SharedVariables["aa_acountname"] = parentCustomerId;
                }
            }
        }
    }
}