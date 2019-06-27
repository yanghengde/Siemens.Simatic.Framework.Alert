using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.IO;

namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    class MailHelper
    {
        public static bool SendNetMail(string to, string subject, string body, IList<string> attachments, string from, string senderusername, string senderpassword, string smtpserver, string SmtpPort, bool EnableSsl)
        {
            string[] sep = new string[] { ",", ";" };
            List<string> toList = new List<string>();
            toList.AddRange(to.Split(sep, StringSplitOptions.RemoveEmptyEntries));

            return SendNetMail(toList, null, null, subject, body, attachments, from, senderusername, senderpassword, smtpserver, SmtpPort, EnableSsl);
        }

        public static bool SendNetMail(string to, string cc, string bcc, string subject, string body, string attachments, string from, string senderusername, string senderpassword, string smtpserver, string SmtpPort, bool EnableSsl)
        {
            string[] sep = new string[] { ",", ";" };

            List<string> toList = new List<string>();
            if (!string.IsNullOrEmpty(to))
                toList.AddRange(to.Split(sep, StringSplitOptions.RemoveEmptyEntries));

            List<string> ccList = new List<string>();
            if (!string.IsNullOrEmpty(cc))
                ccList.AddRange(cc.Split(sep, StringSplitOptions.RemoveEmptyEntries));

            List<string> bccList = new List<string>();
            if (!string.IsNullOrEmpty(bcc))
                bccList.AddRange(bcc.Split(sep, StringSplitOptions.RemoveEmptyEntries));

            List<string> attachedList = new List<string>();
            if (!string.IsNullOrEmpty(attachments))
                attachedList.AddRange(attachments.Split(sep, StringSplitOptions.RemoveEmptyEntries));

            return SendNetMail(toList, ccList, bccList, subject, body, attachedList, from, senderusername, senderpassword, smtpserver, SmtpPort, EnableSsl);
        }

        public static bool SendNetMail(IList<string> toList, IList<string> ccList, IList<string> bccList, string subject, string body, IList<string> attachments, string senderemail, string senderusername, string senderpassword, string smtpserver, string SmtpPort, bool EnableSsl)
        {
            try
            {
                MailMessage mail = new MailMessage();

                if (toList == null || toList.Count == 0)
                {
                    throw new ArgumentNullException("toList can be null or empty.");
                }

                foreach (string s in toList)
                {
                    mail.To.Add(s);
                }

                if (ccList != null && ccList.Count != 0)
                {
                    foreach (string s in ccList)
                    {
                        mail.CC.Add(s);
                    }
                }

                if (bccList != null && bccList.Count != 0)
                {
                    foreach (string s in bccList)
                    {
                        mail.Bcc.Add(s);
                    }
                }

                if (attachments != null && attachments.Count > 0)
                {
                    foreach (string fileName in attachments)
                    {
                        if (File.Exists(fileName))
                        {
                            mail.Attachments.Add(new Attachment(fileName));
                        }
                    }
                }

               // MailAddress ma = new MailAddress(ConfigurationManager.AppSettings["SenderEmail"], ConfigurationManager.AppSettings["SenderUserId"], Encoding.UTF8);
               MailAddress ma = new MailAddress(senderemail, senderusername, Encoding.UTF8);
              
                mail.From = ma;
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                mail.Body = body;


                SmtpClient client = new SmtpClient(smtpserver, Convert.ToInt32(SmtpPort));
                client.Credentials = new System.Net.NetworkCredential(senderusername, senderpassword);
                client.EnableSsl = EnableSsl;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(mail);
            }
            catch (SmtpFailedRecipientsException sfres)
            {
                throw new Exception("SmtpFailedRecipientsException:[FailedRecipient]" + sfres.FailedRecipient + ",[Message]" + sfres.Message);
            }
            catch (SmtpException sex)
            {
                //add by wesley@20130813
                if (sex.StatusCode != SmtpStatusCode.MailboxUnavailable)
                {
                    throw new Exception("SmtpException:[StatusCode]" + sex.StatusCode + ",[Message]" + sex.Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Other Exception:" + ex.Message);
            }

            return true;
        }
    }
}
