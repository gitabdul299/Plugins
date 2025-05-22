using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plugins
{
    public class GetSetValues : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                
                Entity curdEntity = (Entity)context.InputParameters["Target"];
                if (curdEntity.Attributes.Contains("new_name"))
                {    //we can store values directly
                    curdEntity.Attributes["new_name1"] = curdEntity.GetAttributeValue<string>("new_name");
                }
                if (curdEntity.Attributes.Contains("new_dob"))
                {
                    curdEntity.Attributes["new_dob1"] = curdEntity.GetAttributeValue<DateTime>("new_dob");
                }
                if (curdEntity.Attributes.Contains("new_phoneno"))
                {
                    curdEntity.Attributes["new_phoneno1"] = curdEntity.GetAttributeValue<int>("new_phoneno");
                }
                if (curdEntity.Attributes.Contains("new_address"))
                {
                    curdEntity.Attributes["new_address1"] = curdEntity.GetAttributeValue<string>("new_address");
                }
                if (curdEntity.Attributes.Contains("new_gender"))
                {
                    curdEntity.Attributes["new_gender1"] = curdEntity.GetAttributeValue<bool>("new_gender");
                }
                if (curdEntity.Attributes.Contains("new_fees"))
                {
                    curdEntity.Attributes["new_fees1"] = curdEntity.GetAttributeValue<Money>("new_fees");
                }
                if(curdEntity.Attributes.Contains("new_result"))
                {
                    curdEntity.Attributes["new_result1"] = curdEntity.GetAttributeValue<OptionSetValue>("new_result");
                }
                if (curdEntity.Attributes.Contains("new_search"))
                {
                    curdEntity.Attributes["new_search1"] = curdEntity.GetAttributeValue<EntityReference>("new_search");
                }
                if (curdEntity.Attributes.Contains("new_subject"))
                {
                    curdEntity.Attributes["new_subject1"] = curdEntity.GetAttributeValue<OptionSetValueCollection>("new_subject");
                }

                service.Update(curdEntity);



                /*
                //get data/ value and store in variables
                string CRUDName = curdEntity.GetAttributeValue<string>("new_name");
                DateTime DOB = curdEntity.GetAttributeValue<DateTime>("new_dob");
                int PN = curdEntity.GetAttributeValue<int>("new_phoneno");
                string add = curdEntity.GetAttributeValue<string>("new_address");
                bool gender = curdEntity.GetAttributeValue<bool>("new_gender");
                Money fee = curdEntity.GetAttributeValue<Money>("new_fees");
                OptionSetValue resultvalue = curdEntity.GetAttributeValue<OptionSetValue>("new_result");
                EntityReference searchValue = curdEntity.GetAttributeValue<EntityReference>("new_search");//lookup
                OptionSetValueCollection subjectValues = curdEntity.GetAttributeValue<OptionSetValueCollection>("new_subject");//multi option set
                
                
                //update field
                curdEntity.Attributes["new_name1"] = CRUDName;
                curdEntity.Attributes["new_dob1"] = DOB;
                curdEntity.Attributes["new_phoneno1"] = PN;
                curdEntity.Attributes["new_address1"] = add;
                curdEntity.Attributes["new_gender1"] = gender;
                curdEntity["new_fees1"] = fee;
                curdEntity.Attributes["new_result1"] = resultvalue;
                curdEntity["new_search1"] = searchValue;
                curdEntity["new_subject1"] = subjectValues;
                
                service.Update(curdEntity);
                */
            }
        }
    }
}