using System.Net.Mail;
using System.Net;
using AcademiaSys.Models;

namespace AcademiaSys.Services
{
    public class EmailService
    {
        private SmtpClient smtpClient;

        public EmailService()
        {
            // Configure your SMTP settings here
            smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("19co58@aitdgoa.edu.in", "Sujay9823"),
                EnableSsl = true,
            };
        }

        public Response SendOtpEmail(string toEmail, string? password)
        {
            try
            {
                // Create the email message
                MailMessage mailMessage = new MailMessage("19co58@aitdgoa.edu.in", toEmail)
                {
                    Subject = "Your Password is Generated for Login",
                    Body = $"<html><body><div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">\r\n <div style=\"margin:50px auto;width:70%;padding:20px 0\">\r\n <div style=\"border-bottom:1px solid #eee\">\r\n <a href=\"\" style=\"font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600\">Admin Access</a>\r\n </div>\r\n <p style=\"font-size:1.1em\">Hi,</p>\r\n <p>You have been apoointed as Teacher, Kindly Use the following Password to Login In.</p>\r\n <h2 style=\"background:  #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;\">{password}</h2>\r\n <p style=\"font-size:0.9em;\">Regards,<br />Your Admin</p>\r\n <hr style=\"border:none;border-top:1px solid #eee\" />\r\n <div style=\"float:right;padding:8px 0;color:#aaa;font-size:0.8em;line-height:1;font-weight:300\">\r\n <p>Your Admin Inc <3</p>\r\n <p>1690 Ambravati Parkway</p>\r\n <p>Knowhere,Space.</p>\r\n </div>\r\n   </div>\r\n</div></body></html>",
                    IsBodyHtml = true
                };

                // Send the email
                smtpClient.Send(mailMessage);

                var response = new Response
                {
                    Success = true,
                    Message = $"Email with OTP sent to {toEmail}"
                };


                return (response);
            }
            catch (Exception ex)
            {
                var response = new Response
                {
                    Success = false,
                    Message = $"Error sending email: {ex.Message}"
                };
                return (response);
            }
        }
    }
}
