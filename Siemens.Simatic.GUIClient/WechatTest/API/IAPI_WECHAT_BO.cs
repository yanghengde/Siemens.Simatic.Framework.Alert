using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Newtonsoft.Json;
using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Wechat.Common;
using Siemens.Simatic.Wechat.Enterprise;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Siemens.Simatic.WechatTest
{
    [DefaultImplementationAttreibute(typeof(Siemens.Simatic.WechatTest.BusinessLogic.API_WECHAT_BO))]
    public partial interface IAPI_WECHAT_BO
    {
        //IALT_BSC_BO bscbo, 
        void Gettoken(string corpid, string corpsecret);
        //void Settoken(string access_token);
        ReturnValue SendImage(ImageMessage message);
        ReturnValue SendImage(int agentID, string media_id, string tousers, bool isFetchChild);
        ReturnValue SendImage(int agentID, string title, string bodyText, string tousers, bool isFetchChild);
        ReturnValue SendImage(int agentID, string media_id, string toParty, string toTag, string toUser);
        ReturnValue SendImage(Bitmap m_Bitmap, int agentID, string title, string bodyText, string toParty, bool isFetchChild);
        ReturnValue SendImage(int agentID, string title, string bodyText, string toParty, string toTag, string toUser);
        ReturnValue SendImage(string url, int width, int hight, int agentID, string title, string toParty, string toTag, string toUser);
        ReturnValue SendQRImage(int agentID, string title, string bodyText, string tousers, bool isFetchChild);
        ReturnValue SendText(TextMessage message);
        ReturnValue SendText(int agentID, string content, string tousers, bool isFetchChild);
        ReturnValue SendText(int agentID, string content, string toParty, string toTag, string toUser);
    }
}

namespace Siemens.Simatic.WechatTest.BusinessLogic
{
    public partial class API_WECHAT_BO : IAPI_WECHAT_BO
    {
        private WeChatEnterprise enterprise;
        private const string selectTokenSql = "select Access_token from PM_WECHAT_TOKEN where DATEDIFF(MINUTE,Date,GETDATE()) <= 100";
        private const string updateSql = "update PM_WECHAT_TOKEN set Access_token = '{0}' , Date = GETDATE() where PK = 1 ";

        public API_WECHAT_BO()
        {
            enterprise = new WeChatEnterprise();
        }

        //获得access_token
        //public void Gettoken(string corpid, string corpsecret)
        //{
        //    //获得数据库中的access_token
        //    DataTable dt = bscbo.GetDataTableBySql(selectTokenSql);
        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        enterprise.Access_token = dt.Rows[0]["Access_token"].ToString();
        //    }
        //    else
        //    {
        //        //获得新的token
        //        ReturnValue val = enterprise.Gettoken(corpid, corpsecret);
        //        if (val.Success)
        //        {
        //            //并更新token
        //            string sql = string.Format(CultureInfo.InvariantCulture, updateSql, val.Result);
        //            bscbo.ExecuteNonQueryBySql(sql);
        //        }
        //    }
        //}

        //获得access_token
        public void Gettoken(string corpid, string corpsecret)
        {
            ReturnValue val = enterprise.Gettoken(corpid, corpsecret);
        }

        ////设置access_token
        //public void Settoken(string access_token) 
        //{
        //    enterprise.Access_token = access_token;       
        //}
        public ReturnValue SendImage(ImageMessage message)
        {
            return enterprise.sendMessage(JsonConvert.SerializeObject(message));
        }

        public ReturnValue SendImage(int agentID, string media_id, string tousers, bool isFetchChild)
        {
            ReturnValue val = new ReturnValue
            {
                Success = false
            };
            if (string.IsNullOrEmpty(tousers))
            {
                val.Message = "无用户信息，无法发送";
                return val;
            }
            return this.SendImage(agentID, media_id, string.Empty, string.Empty, tousers);
        }

        public ReturnValue SendImage(int agentID, string title, string bodyText, string toUsers, bool isFetchChild)
        {
            ReturnValue val = new ReturnValue
            {
                Success = false
            };
            try
            {
                string path = "C:/Images/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string str2 = title + ".html";
                if (!File.Exists(path + str2))
                {
                    File.Create(path + str2);
                }
                Bitmap webSiteThumbnail = WebSiteThumbnail.GetWebSiteThumbnail(path + str2, bodyText);
                string str3 = title + ".Png";
                webSiteThumbnail.Save(path + str3, ImageFormat.Png);
                val = this.enterprise.UploadMedia("image", path + str3);
                if (!val.Success)
                {
                    return val;
                }
                return this.SendImage(agentID, val.Result.ToString(), toUsers, isFetchChild);
            }
            catch (Exception exception)
            {
                val.Message = exception.Message + exception.StackTrace;
            }
            return val;
        }

        public ReturnValue SendImage(int agentID, string media_id, string toParty, string toTag, string toUser)
        {
            Siemens.Simatic.Wechat.Common.Image image = new Siemens.Simatic.Wechat.Common.Image
            {
                media_id = media_id
            };
            ImageMessage message = new ImageMessage
            {
                agentid = agentID,
                image = image,
                msgtype = "image",
                safe = 0,
                toparty = toParty,
                totag = toTag,
                touser = toUser
            };
            return this.SendImage(message);
        }


        public ReturnValue SendImage(Bitmap m_Bitmap, int agentID, string title, string bodyText, string toParty, bool isFetchChild)
        {
            ReturnValue val = new ReturnValue
            {
                Success = false
            };
            try
            {
                string path = "C:/Images/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string str2 = title + ".html";
                if (!File.Exists(path + str2))
                {
                    File.Create(path + str2);
                }
                string str3 = title + ".Png";
                m_Bitmap.Save(path + str3, ImageFormat.Png);
                val = enterprise.UploadMedia("image", path + str3);
                if (!val.Success)
                {
                    return val;
                }
                return this.SendImage(agentID, val.Result.ToString(), toParty, isFetchChild);
            }
            catch (Exception exception)
            {
                val.Message = exception.Message + exception.StackTrace;
            }
            return val;
        }


        public ReturnValue SendImage(int agentID, string title, string bodyText, string toParty, string toTag, string toUser)
        {
            ReturnValue val = new ReturnValue
            {
                Success = false
            };
            try
            {
                string path = "C:/Images/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string str2 = title + ".html";
                if (!File.Exists(path + str2))
                {
                    File.Create(path + str2);
                }
                Bitmap webSiteThumbnail = WebSiteThumbnail.GetWebSiteThumbnail(path + str2, bodyText);
                string str3 = title + ".Png";
                webSiteThumbnail.Save(path + str3, ImageFormat.Png);
                val = this.enterprise.UploadMedia("image", path + str3);
                if (!val.Success)
                {
                    return val;
                }
                return this.SendImage(agentID, val.Result.ToString(), toParty, toTag, toUser);
            }
            catch (Exception exception)
            {
                val.Message = exception.Message + exception.StackTrace;
            }
            return val;
        }


        public ReturnValue SendImage(string url, int width, int hight, int agentID, string title, string toParty, string toTag, string toUser)
        {
            ReturnValue val = new ReturnValue
            {
                Success = false
            };
            try
            {
                string path = "C:/Images/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string str2 = title + ".html";
                if (!File.Exists(path + str2))
                {
                    File.Create(path + str2);
                }
                Bitmap bitmap = WebSiteBrowser.GetWebSiteThumbnail(url, width, hight);
                string str3 = title + ".Png";
                bitmap.Save(path + str3, ImageFormat.Png);
                val = this.enterprise.UploadMedia("image", path + str3);
                if (!val.Success)
                {
                    return val;
                }
                return this.SendImage(agentID, val.Result.ToString(), toParty, toTag, toUser);
            }
            catch (Exception exception)
            {
                val.Message = exception.Message + exception.StackTrace;
            }
            return val;
        }

        public ReturnValue SendQRImage(int agentID, string title, string bodyText, string tousers, bool isFetchChild)
        {
            ReturnValue val = new ReturnValue
            {
                Success = false
            };
            try
            {
                string path = "C:/Images/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Bitmap bitmap = new QRBarCode().GenerateQRCode47(bodyText);
                string str2 = title + ".Png";
                bitmap.Save(path + str2, ImageFormat.Png);
                val = this.enterprise.UploadMedia("image", path + str2);
                if (!val.Success)
                {
                    return val;
                }
                return this.SendImage(agentID, val.Result.ToString(), tousers, isFetchChild);
            }
            catch (Exception exception)
            {
                val.Message = exception.Message + exception.StackTrace;
            }
            return val;
        }

        public ReturnValue SendText(TextMessage message)
        {
            return enterprise.sendMessage(JsonConvert.SerializeObject(message));
        }

        public ReturnValue SendText(int agentID, string content, string toUsers, bool isFetchChild)
        {
            ReturnValue val = new ReturnValue
            {
                Success = false
            };
            if (string.IsNullOrEmpty(toUsers))
            {
                val.Message = "无用户信息，无法发送";
                return val;
            }
            return this.SendText(agentID, content, string.Empty, string.Empty, toUsers);
        }

        public ReturnValue SendText(int agentID, string content, string toParty, string toTag, string toUser)
        {
            Text text = new Text
            {
                content = content
            };
            TextMessage message = new TextMessage
            {
                agentid = agentID,
                text = text,
                msgtype = "text",
                safe = 0,
                toparty = toParty,
                totag = toTag,
                touser = toUser
            };
            return this.SendText(message);
        }

        private WeChatHttpWebReuest request = new WeChatHttpWebReuest();
        


    }
}
