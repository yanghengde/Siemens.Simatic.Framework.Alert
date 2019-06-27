using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.ALT.BusinessLogic
{
    public class Http_Request
    {
        public string HttpPost(string url, string body)
        {
            string responseText = "";
            try
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                Encoding encoding = Encoding.UTF8;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.ContentType = "application/json";

                byte[] buffer = encoding.GetBytes(body);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                //{
                //    responseText = reader.ReadToEnd();
                //}
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                responseText = reader.ReadToEnd();

                reader.Close();                
            }
            catch (Exception ex)
            {
                responseText = "HttpPost异常：" + ex.Message;
            }
            return responseText;
        }

    }
}
