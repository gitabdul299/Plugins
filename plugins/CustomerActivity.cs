using Microsoft.Xrm.Sdk;
using System;

namespace plugins
{
    public class CustomerActivity : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity curdEntity = (Entity)context.InputParameters["Target"];
                OptionSetValue customerActivities = curdEntity.GetAttributeValue<OptionSetValue>("new_customeractivity");

                if (customerActivities != null)
                {
                    int customerActivitiesValue = customerActivities.Value;
                    Entity activity = null;

                    if (customerActivitiesValue == 100000000) // Phone Call
                    {
                        activity = CreatePhoneCall(curdEntity);
                    }
                    else if (customerActivitiesValue == 100000001) // Email
                    {
                        activity = CreateEmail(curdEntity);
                    }
                    else if (customerActivitiesValue == 100000002) // Appointment
                    {
                        activity = CreateAppointment(curdEntity);
                    }
                    else if (customerActivitiesValue == 100000003) // Task
                    {
                        activity = CreateTask(curdEntity);
                    }

                    if (activity != null)
                    {
                        service.Create(activity);
                    }
                }
            }
        }

        private Entity CreatePhoneCall(Entity curdEntity)
        {
            Entity phoneCall = new Entity("phonecall");
            phoneCall["subject"] = "Phone Call regarding " + curdEntity.GetAttributeValue<string>("new_timeline");
            phoneCall["description"] = "Description for Phone Call";
            phoneCall["regardingobjectid"] = curdEntity.ToEntityReference();
            return phoneCall;
        }

        private Entity CreateEmail(Entity curdEntity)
        {
            Entity email = new Entity("email");
            email["subject"] = "Email regarding " + curdEntity.GetAttributeValue<string>("new_timeline");
            email["description"] = "Description for Email";
            email["regardingobjectid"] = curdEntity.ToEntityReference();
            return email;
        }

        private Entity CreateAppointment(Entity curdEntity)
        {
            Entity appointment = new Entity("appointment");
            appointment["subject"] = "Appointment regarding " + curdEntity.GetAttributeValue<string>("new_timeline");
            appointment["description"] = "Description for Appointment";
            appointment["scheduledstart"] = DateTime.Now.AddDays(1);
            appointment["scheduledend"] = DateTime.Now.AddDays(1).AddHours(1);
            appointment["regardingobjectid"] = curdEntity.ToEntityReference();
            return appointment;
        }

        private Entity CreateTask(Entity curdEntity)
        {
            Entity task = new Entity("task");
            task["subject"] = "Task regarding " + curdEntity.GetAttributeValue<string>("new_timeline");
            task["description"] = "Description for Task";
            task["scheduledstart"] = DateTime.Now;
            task["scheduledend"] = DateTime.Now.AddHours(2);
            task["regardingobjectid"] = curdEntity.ToEntityReference();
            return task;
        }
    }
}
