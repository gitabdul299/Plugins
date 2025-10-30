using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Xrm.Sdk;

namespace LeadCreatePlugin
{
    public class SendEmailOnLeadCreate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            try
            {
                if (context.InputParameters.Contains("Target") &&
                    context.InputParameters["Target"] is Entity entity &&
                    entity.LogicalName == "lead")
                {
                    string email = entity.GetAttributeValue<string>("emailaddress1");

                    if (!string.IsNullOrEmpty(email))
                    {
                        
                        
                       string firstName = entity.GetAttributeValue<string>("firstname") ?? "there";
                        string subject = "Thank you for your interest!";
                        string body = $"Hello {firstName},\n\nThanks for registering as a lead. We'll get in touch with you soon.";

                        SendEmail(email, subject, body, tracingService);
                    }
                    else
                    {
                        tracingService.Trace("Email not found in lead record.");
                    }
                }
            }
            catch
            {
                tracingService.Trace("Error occurred in SendEmailOnLeadCreate plugin.");
                throw new InvalidPluginExecutionException("Error in SendEmailOnLeadCreate plugin.");
            }
        }

        private void SendEmail(string toEmail, string subject, string body, ITracingService tracer)
        {
            try
            {
                string fromEmail = Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL");
                string fromPassword = Environment.GetEnvironmentVariable("SMTP_SENDER_PASSWORD");
                string smtpHost = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? "smtp.gmail.com";

                if (string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(fromPassword))
                {
                    tracer.Trace("SMTP credentials not set in environment variables.");
                    return;
                }

                MailMessage mail = new MailMessage(fromEmail, toEmail, subject, body);
                SmtpClient smtpClient = new SmtpClient(smtpHost)
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, fromPassword),
                    EnableSsl = true
                };

                smtpClient.Send(mail);
                tracer.Trace("Email sent successfully to " + toEmail);
            }
            catch (Exception emailEx)
            {
                tracer.Trace("Error sending email: " + emailEx.Message);
                throw;
            }
        }
    }
}