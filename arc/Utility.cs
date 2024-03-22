using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Benaa2018
{
    public class Utility
    {
        public static async void SendMail(string fromEmail, string subject, string emailBody)
        {
            try
            {
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential("dream.sumit18@gmail.com", "swapnasumit")
                };
                using (var message = new MailMessage("dream.sumit18@gmail.com", "sumitganguly1989@gmail.com")
                {
                    Subject = subject,
                    Body = emailBody
                })
                {
                    await smtpClient.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static async void SendAssignedMail(string todoName, string projectName, string userName, 
            IHostingEnvironment hostingEnvironment, List<Tuple<string, string>> userTuple)
        {
            var htmlBody = string.Empty;
            using (StreamReader reader = File.OpenText(hostingEnvironment.ContentRootPath + "/EmailTemplate/AssignUserTemplate.html"))
            {
                htmlBody = await reader.ReadToEndAsync();
            }
            htmlBody = htmlBody.Replace("{ToDoTaskName}", todoName);
            htmlBody = htmlBody.Replace("{ProjectName}", projectName);
            htmlBody = htmlBody.Replace("{AssignedByUser}", userName);
            htmlBody = htmlBody.Replace("{AssignedDate}", DateTime.Now.ToString());
            string assignedUserHtml = string.Empty;
            foreach (var item in userTuple)
            {
                assignedUserHtml = "</tr>" +
                                   "<tr>" +
                                   "<td><img src='check.jpg>t1</td>" +
                                   "</tr>" +
                                   "<tr>" +
                                   "<td><p><span>Added By:</span>"+ item.Item2 + " </p></td>" +
                                   "</tr>";
            }
            htmlBody = htmlBody.Replace("{AssignedUserText}", assignedUserHtml);
            string emailSubject = "Assigned user Template";
            foreach (var item in userTuple)
            {
                SendMail(item.Item1, emailSubject, htmlBody);
            }
        }      
    }
}
