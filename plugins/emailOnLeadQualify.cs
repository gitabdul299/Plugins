using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Xrm.Sdk;

namespace LeadQualificationPlugin
{
    public class LeadQualificationPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            try
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity entity)
                {
                    if (entity.LogicalName == "lead" && entity.Contains("statuscode") && entity["statuscode"].ToString() == "Qualified")
                    {
                        string recipientEmail = entity.GetAttributeValue<string>("emailaddress1") ?? string.Empty;

                        if (!string.IsNullOrWhiteSpace(recipientEmail))
                        {
                            string subject = "Congratulations, you're qualified!";
                            string message = "Dear customer, your lead has been qualified!";
                            SendEmail(recipientEmail, subject, message, tracingService);
                        }
                        else
                        {
                            tracingService.Trace("Email address is missing or invalid.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tracingService.Trace($"An error occurred: {ex.Message}");
                throw;
            }
        }

        private void SendEmail(string recipientEmail, string subject, string message, ITracingService tracingService)
        {
            string senderEmail = Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL");
            string senderPassword = Environment.GetEnvironmentVariable("SMTP_SENDER_PASSWORD");
            string smtpServerAddress = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? "smtp.gmail.com";

            if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(senderPassword))
            {
                tracingService.Trace("SMTP credentials are not configured.");
                throw new InvalidOperationException("SMTP credentials are missing.");
            }

            MailMessage mail = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = message
            };
            mail.To.Add(recipientEmail);

            SmtpClient smtpServer = new SmtpClient(smtpServerAddress)
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true
            };

            try
            {
                smtpServer.Send(mail);
                tracingService.Trace("Email sent successfully.");
            }
            catch (Exception ex)
            {
                tracingService.Trace($"Failed to send email. Error: {ex.Message}");
                throw;
            }
        }
    }
}
