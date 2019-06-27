using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.IO;
using System.Text;

namespace Siemens.Simatic.Util.Utilities.Mail
{
    public abstract class AbstractMailService
    {
        public string Host;
        
        public virtual bool IsHtmlFormat
        {
            get
            {
                return true;
            }
        }

        public static ArrayList imageCollection = new ArrayList();

        //member of IMailService
        //public abstract void Send();

        public void SendMail(string to, string cc, string from, string subject, string body, string attachment)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(this.Host);
                MailMessage mailMsg = new MailMessage();
                //msg.Body = body;

                if (String.IsNullOrEmpty(to))
                    return;
                mailMsg.To.Add(to);

                if (!String.IsNullOrEmpty(cc))
                    mailMsg.CC.Add(cc);

                mailMsg.From = new MailAddress(from);
                mailMsg.Subject = subject;
                mailMsg.IsBodyHtml = IsHtmlFormat;

                if (!String.IsNullOrEmpty(attachment))
                {
                    System.Net.Mail.Attachment attach = new Attachment(attachment);
                    mailMsg.Attachments.Add(attach);
                }

                //this.AttachImages(ref body, ".bmp");
                //this.AttachImages(ref body, ".jpg");
                //this.AttachImages(ref body, ".gif");

                AlternateView plainView = AlternateView.CreateAlternateViewFromString("This is my plain text content, viewable by those clients that don't support html", 
                                                                                        null, 
                                                                                        "text/plain");

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                //add the views
                mailMsg.AlternateViews.Add(plainView);
                mailMsg.AlternateViews.Add(htmlView);

                foreach (LinkedResource lr in imageCollection)
                {
                    htmlView.LinkedResources.Add(lr);
                }

                smtpClient.Send(mailMsg);
                //StreamWriter writer = new StreamWriter(@"d:\Mail\" + System.Guid.NewGuid().ToString() + ".htm");
                //writer.Write(msg.Body);
                //writer.Close();
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public void SendMail(string to, string cc, string bcc, string from, string subject, string body, string attachment)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient(this.Host);
                MailMessage mailMsg = new MailMessage();
                //msg.Body = body;

                if (String.IsNullOrEmpty(to))
                    return;
                mailMsg.To.Add(to);

                if (!String.IsNullOrEmpty(cc))
                    mailMsg.CC.Add(cc);

                if (!String.IsNullOrEmpty(bcc))
                    mailMsg.Bcc.Add(bcc);

                mailMsg.From = new MailAddress(from);
                mailMsg.Subject = subject;
                mailMsg.IsBodyHtml = IsHtmlFormat;

                if (!string.IsNullOrEmpty(attachment))
                {
                    System.Net.Mail.Attachment attach = new Attachment(attachment);
                    mailMsg.Attachments.Add(attach);
                }

                //AttachImages(ref body, ".bmp");
                //AttachImages(ref body, ".jpg");
                //AttachImages(ref body, ".gif");

                AlternateView plainView = AlternateView.CreateAlternateViewFromString("This is my plain text content, viewable by those clients that don't support html", 
                                                                                        null,
                                                                                        "text/plain");

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                //add the views
                mailMsg.AlternateViews.Add(plainView);
                mailMsg.AlternateViews.Add(htmlView);

                foreach (LinkedResource lr in imageCollection)
                {
                    htmlView.LinkedResources.Add(lr);
                }

                smtpClient.Send(mailMsg);
                //StreamWriter writer = new StreamWriter(@"d:\Mail\" + System.Guid.NewGuid().ToString() + ".htm");
                //writer.Write(msg.Body);
                //writer.Close();
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        //public void AttachImages(ref string body, string imageType)
        //{
        //    int b = 1;

        //    string tempBody = body;
        //    int startPoint = 0;

        //    while (b > 0)
        //    {
        //        b = tempBody.IndexOf(imageType, startPoint);
        //        if (b > 0)
        //        {
        //            string part = tempBody.Substring(startPoint, b - startPoint);

        //            if (part.LastIndexOf("<") > part.LastIndexOf(">"))
        //            {
        //                string imagePath = part.Substring(part.LastIndexOf("\"") + 1).Replace("/EMISWeb", Configuration.ImagePath).Replace("\\", "/") + imageType;

        //                LinkedResource logo = new LinkedResource(imagePath);
        //                body = body.Replace(part.Substring(part.LastIndexOf("\"") + 1) + imageType, "cid:" + logo.ContentId);
        //                imageCollection.Add(logo);
        //            }

        //            startPoint = b + 1;
        //        }

        //    }
        //}

        
    }
}
