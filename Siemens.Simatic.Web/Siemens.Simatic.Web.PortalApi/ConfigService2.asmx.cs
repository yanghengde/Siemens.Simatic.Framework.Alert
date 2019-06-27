//using SITCAB.Framework.Providers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;
using System.Web.Services;
//using SITCAB.Framework.UserManager;
using log4net;

namespace Siemens.Simatic.Web.PortalApi
{


    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1), ToolboxItem(false), WebService(Namespace = "http://tempuri.org/")]
    public class ConfigService2 : WebService
    {
        ILog log = LogManager.GetLogger(typeof(ConfigService));

        private static object _lockObject = new object();
        private string _resourceCacheKey = "SS_ResourceCultureCache";
        private string _systemCacheKey = "SS_SystemConfigCache";
        private string _userCacheKey = "SS_UserConfigCache";

        [WebMethod(EnableSession = true)]
        public bool CABAuthentication(string userName, string password)
        {
            base.Session.Timeout = 30;
            //return CABUsers.ValidateUser(userName, password);
            return true;
        }

        [WebMethod]
        public bool DeleteApplication(Guid applicationGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("delete from dbo.Plat_cfg_Application where ApplicationGuid = '{0}'", applicationGuid);
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool DeleteConfigItem(Guid configItemGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("delete from dbo.Plat_cfg_UserConfig where SystemConfigItemGuid = '{0}' \r\n                                                 delete from dbo.Plat_cfg_SystemConfigValueLog where ConfigItemGuid = '{0}'\r\n                                                 delete from dbo.Plat_cfg_SystemConfigValue where ConfigItemGuid = '{0}'\r\n                                                 delete from dbo.Plat_cfg_SystemConfigOption where ConfigItemGuid = '{0}'\r\n\r\n                                                 delete from dbo.Plat_cfg_SystemConfigItem where ConfigItemGuid = '{0}'", configItemGuid);
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool DeleteConfigOption(Guid configOptionGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = string.Format(" delete from dbo.Plat_cfg_SystemConfigValue where ConfigOptionGuid = '{0}'\r\n                                                         delete from dbo.Plat_cfg_SystemConfigOption where ConfigOptionGuid = '{0}'", configOptionGuid)
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool DeleteConfigVersion(Guid configVersionGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = string.Format(" delete from dbo.Plat_cfg_SystemConfigValueVersionDetail where ConfigVersionGuid = '{0}'\r\n                                                         delete from dbo.Plat_cfg_SystemConfigValueVersion where ConfigVersionGuid = '{0}'", configVersionGuid)
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool DeleteEnvironment(Guid environmentGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("delete from dbo.Plat_cfg_Environment where EnvironmentGuid = '{0}'", environmentGuid);
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool DeleteResourceCultureByKey(string applicationName, string environmentName, string resourceHost, string resourceKey)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("--declare @ApplicationGuid uniqueidentifier,\r\n\t                                             --        @EnvironmentGuid uniqueidentifier\r\n\t    \r\n                                                --select @ApplicationGuid = ApplicationGuid from Plat_cfg_Application where ApplicationName = '{0}'\r\n                                                --select @EnvironmentGuid = EnvironmentGuid from Plat_cfg_Environment where EnvironmentName = '{1}'\r\n\r\n                                                delete from Plat_cfg_CultureResource\r\n                                                from Plat_cfg_CultureResource c\r\n                                                inner join Plat_cfg_Application a on c.ApplicationGuid = a.ApplicationGuid\r\n                                                inner join Plat_cfg_Environment e on c.EnvironmentGuid = e.EnvironmentGuid\r\n                                                where a.ApplicationName = '{0}'\r\n                                                and e.EnvironmentName = '{1}'\r\n                                                --and c.CultureCode = '{2}'\r\n                                                and c.ResourceHost = '{2}'\r\n                                                and c.ResourceKey = '{3}'", new object[] { applicationName, environmentName, resourceHost, resourceKey });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public DataTable GetActiveApplications()
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_Application\r\n                                        where IsActive = 1\r\n                                        order by CreateDatetime asc", new object[0]);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetActiveConfigItemsByApp(Guid applicationGuid)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_SystemConfigItem\r\n                                        where ConfigItemStatus = 1\r\n                                        and ApplicationGuid = '{0}'\r\n                                        order by ConfigItemName asc", applicationGuid);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetActiveEnvironments()
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_Environment\r\n                                        where IsActive = 1\r\n                                        order by CreateDatetime asc", new object[0]);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetActiveOptionsByItem(Guid configItemGuid)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_SystemConfigOption\r\n                                        where OptionStatus = 1\r\n                                        and ConfigItemGuid = '{0}'\r\n                                        order by OptionSequence asc", configItemGuid);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetActiveValuesByItem(Guid configItemGuid, Guid environmentGuid)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select sco.*,scv.ConfigValue,scv.IsSelected as IsSelectedValue,scv.ResetDatetime,\r\n                                        sci.ConfigItemType,sci.IsEncrypt,sci.IsGlobal,sci.IsReadOnly\r\n                                        from dbo.Plat_cfg_SystemConfigOption sco\r\n                                        left outer join dbo.Plat_cfg_SystemConfigValue scv on scv.ConfigOptionGuid = sco.ConfigOptionGuid\r\n                                        left outer join dbo.Plat_cfg_SystemConfigItem sci on sci.ConfigItemGuid = sco.ConfigItemGuid\r\n                                        where sco.OptionStatus = 1\r\n                                        and sco.ConfigItemGuid = '{0}'\r\n                                        and scv.EnvironmentGuid = '{1}'\r\n                                        order by sco.OptionSequence asc", configItemGuid, environmentGuid);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetActiveValuesByOption(Guid configOptionGuid, Guid environmentGuid)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select sco.*,scv.ConfigValue,scv.IsSelected as IsSelectedValue,scv.ResetDatetime,\r\n                                        sci.ConfigItemType,sci.ConfigItemType,sci.IsEncrypt,sci.IsGlobal,sci.IsReadOnly\r\n                                        from dbo.Plat_cfg_SystemConfigOption sco\r\n                                        left outer join dbo.Plat_cfg_SystemConfigValue scv on scv.ConfigOptionGuid = sco.ConfigOptionGuid\r\n                                        left outer join dbo.Plat_cfg_SystemConfigItem sci on sci.ConfigItemGuid = sco.ConfigItemGuid\r\n                                        where sco.OptionStatus = 1\r\n                                        and sco.ConfigOptionGuid = '{0}'\r\n                                        and scv.EnvironmentGuid = '{1}'\r\n                                        order by sco.OptionSequence asc", configOptionGuid, environmentGuid);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        //[WebMethod(EnableSession = true)]
        //public string[] GetAllCABUsers()
        //{
        //    Collection<CABMembershipUser> allUsers = CABUsers.GetAllUsers();
        //    if (allUsers != null)
        //    {
        //        string[] strArray = new string[allUsers.Count];
        //        for (int i = 0; i < allUsers.Count; i++)
        //        {
        //            strArray[i] = allUsers[i].UserId;
        //        }
        //        return strArray;
        //    }
        //    return new string[0];
        //}

        //[WebMethod(EnableSession = true)]
        //public string[] GetAllGroupsByUser(string userName)
        //{
        //    Collection<CABGroupUser> allGroups = CABUsers.GetAllGroups(userName);
        //    if ((allGroups != null) && (allGroups.Count > 0))
        //    {
        //        string[] strArray = new string[allGroups.Count];
        //        for (int i = 0; i < allGroups.Count; i++)
        //        {
        //            strArray[i] = allGroups[i].GroupName;
        //        }
        //        return strArray;
        //    }
        //    return new string[0];
        //}

        private DataTable GetAllResourceCulture()
        {
            lock (_lockObject)
            {
                DataTable table;
                if (HttpRuntime.Cache[this._resourceCacheKey] == null)
                {
                    string configDBConnString = this.GetConfigDBConnString();
                    string str2 = "SELECT c.[ResourceGuid]\r\n                                          ,c.[ApplicationGuid]\r\n                                          ,a.ApplicationName\r\n                                          ,c.[EnvironmentGuid]\r\n                                          ,e.EnvironmentName\r\n                                          ,c.[ResourceHost]\r\n                                          ,c.[ResourceKey]\r\n                                          ,c.[CultureCode]\r\n                                          ,c.[ResourceValue]\r\n                                          ,c.[ResourceLocation]\r\n                                          ,c.[ResourceSize]\r\n                                          ,c.[IsActive]\r\n                                          ,c.[MyMark]\r\n                                      FROM [Plat_cfg_CultureResource] c\r\n                                      inner join dbo.Plat_cfg_Application a on c.ApplicationGuid = a.ApplicationGuid\r\n                                      inner join dbo.Plat_cfg_Environment e on c.EnvironmentGuid = e.EnvironmentGuid\r\n                                      WHERE c.[IsActive] = 1\r\n                                      order by ApplicationName,EnvironmentName,CultureCode,ResourceHost,ResourceKey";
                    SqlConnection connection = new SqlConnection(configDBConnString);
                    try
                    {
                        SqlCommand selectCommand = new SqlCommand
                        {
                            Connection = connection,
                            CommandType = CommandType.Text,
                            CommandText = str2
                        };
                        connection.Open();
                        DataSet dataSet = new DataSet();
                        new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                        connection.Close();
                        table = dataSet.Tables["tb1"];
                        HttpRuntime.Cache.Insert(this._resourceCacheKey, table, null, DateTime.Now.AddMinutes((double)int.Parse(ConfigurationManager.AppSettings["ResourceCultureCacheExpirationMinutes"].ToString())), Cache.NoSlidingExpiration);
                    }
                    catch (Exception exception)
                    {
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                        throw new Exception(exception.Message);
                    }
                }
                else
                {
                    table = (DataTable)HttpRuntime.Cache[this._resourceCacheKey];
                }
                return table;
            }
        }

        private DataTable GetAllSystemConfig()
        {
            lock (_lockObject)
            {
                DataTable table;
                if (HttpRuntime.Cache[this._systemCacheKey] == null)
                {
                    string configDBConnString = this.GetConfigDBConnString();
                    string str2 = "SELECT [ApplicationGuid]\r\n                                          ,[ApplicationName]\r\n                                          ,[ApplicationDesc]\r\n                                          ,[Architecture]\r\n                                          ,[ApplicationGroup]\r\n                                          ,[CreateDatetimeApplication]\r\n                                          ,[CreateUserApplication]\r\n                                          ,[IsActiveApplication]\r\n                                          ,[EnvironmentName]\r\n                                          ,[EnvironmentDesc]\r\n                                          ,[CreateDatetimeEnvironment]\r\n                                          ,[CreateUserEnvironment]\r\n                                          ,[ConfigItemGuid]\r\n                                          ,[ConfigItemName]\r\n                                          ,[ConfigItemDesc]\r\n                                          ,[ConfigItemType]\r\n                                          ,[ConfigItemStatus]\r\n                                          ,[IsEncrypt]\r\n                                          ,[IsGlobal]\r\n                                          ,[IsReadOnly]\r\n                                          ,[ConfigValueGuid]\r\n                                          ,[ValueName]\r\n                                          ,[ConfigValue]\r\n                                          ,[ValueDesc]\r\n                                          ,[IsSelected]\r\n                                          ,[ResetDatetime]\r\n                                          ,[IsActiveConfigValue]\r\n                                          ,[EnvironmentGuid]\r\n                                          ,[IsActiveEnvironment]\r\n                                          ,[ConfigValueSequence]\r\n                                      FROM [View_Plat_cfg_SystemConfig]\r\n                                      WHERE [IsActiveApplication] = 1\r\n                                      AND [IsActiveEnvironment] = 1\r\n                                      AND [ConfigItemStatus] = 1\r\n                                      AND [IsActiveConfigValue] = 1\r\n                                      order by [ValueName],[ConfigValueSequence]";
                    SqlConnection connection = new SqlConnection(configDBConnString);
                    try
                    {
                        SqlCommand selectCommand = new SqlCommand
                        {
                            Connection = connection,
                            CommandType = CommandType.Text,
                            CommandText = str2
                        };
                        connection.Open();
                        DataSet dataSet = new DataSet();
                        new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                        connection.Close();
                        table = dataSet.Tables["tb1"];
                        HttpRuntime.Cache.Insert(this._systemCacheKey, table, null, DateTime.Now.AddMinutes((double)int.Parse(ConfigurationManager.AppSettings["SystemConfigCacheExpirationMinutes"].ToString())), Cache.NoSlidingExpiration);
                    }
                    catch (Exception exception)
                    {
                        if (connection.State != ConnectionState.Closed)
                        {
                            connection.Close();
                        }
                        throw new Exception(exception.Message);
                    }
                }
                else
                {
                    table = (DataTable)HttpRuntime.Cache[this._systemCacheKey];
                }
                return table;
            }
        }

        [WebMethod]
        public DataTable GetByApplicationGuid(Guid applicationGuid)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_Application\r\n                                        where ApplicationGuid = '{0}'", applicationGuid);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetByApplicationName(string applicationName)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_Application\r\n                                        where ApplicationName = '{0}'", applicationName);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetByConfigItemGuid(Guid configItemGuid)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_SystemConfigItem\r\n                                        where ConfigItemGuid = '{0}'", configItemGuid);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetByConfigItemName(Guid applicationGuid, string configItemName)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_SystemConfigItem\r\n                                        where ApplicationGuid = '{0}' \r\n                                        and ConfigItemName = '{1}' ", applicationGuid, configItemName);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetByConfigOptionGuid(Guid configOptionGuid)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_SystemConfigOption\r\n                                        where ConfigOptionGuid = '{0}'", configOptionGuid);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetByConfigOptionName(Guid configItemGuid, string configOptionName)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_SystemConfigOption\r\n                                        where ConfigItemGuid = '{0}' \r\n                                        and OptionName = '{1}' ", configItemGuid, configOptionName);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetByEnvironmentGuid(Guid environmentGuid)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_Environment\r\n                                        where EnvironmentGuid = '{0}'", environmentGuid);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetByEnvironmentName(string environmentName)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_Environment\r\n                                        where EnvironmentName = '{0}'", environmentName);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        private string GetConfigDBConnString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["Siemens.Simatic.Platform.ConfigConnectionString"].ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException("ConnectionStrings.Siemens.Simatic.Platform.ConfigConnectionString 配置项不存在。");
            }
            return connectionString;
        }

        [WebMethod]
        public DataTable GetConfigValueByOptionAndEnvironment(Guid configOptionGuid, Guid environmentGuid)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *\r\n                                        from dbo.Plat_cfg_SystemConfigValue\r\n                                        where ConfigOptionGuid = '{0}'\r\n                                        and EnvironmentGuid = '{1}'", configOptionGuid, environmentGuid);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetConfigVersions(Guid applicationGuid, Guid environmentGuid)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select *,app.ApplicationName,env.EnvironmentName from dbo.Plat_cfg_SystemConfigValueVersion ver\r\n                                        inner join dbo.Plat_cfg_Application app on ver.ApplicationGuid = app.ApplicationGuid\r\n                                        inner join dbo.Plat_cfg_Environment env on ver.EnvironmentGuid = env.EnvironmentGuid\r\n                                        where app.ApplicationGuid = '{0}'\r\n                                        and env.EnvironmentGuid = '{1}'\r\n                                        order by ver.VersionDatetime desc", applicationGuid, environmentGuid);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        private DataTable GetLocalConfigItemName(string applicationName, string environment)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select distinct ConfigItemName\r\n                                        from dbo.View_Plat_cfg_SystemConfig\r\n                                        where ApplicationName = '{0}'\r\n                                        and EnvironmentName = '{1}'\r\n                                        and IsGlobal = 0\r\n                                        and IsActiveApplication = 1\r\n                                        and IsActiveEnvironment = 1\r\n                                        and ConfigItemStatus = 1", applicationName, environment);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public string[] GetLocalConfigItemNameArray(string applicationName, string environment)
        {
            string[] strArray = new string[0];
            DataTable localConfigItemName = this.GetLocalConfigItemName(applicationName, environment);
            if ((localConfigItemName != null) || (localConfigItemName.Rows.Count > 0))
            {
                strArray = new string[localConfigItemName.Rows.Count];
                for (int i = 0; i < localConfigItemName.Rows.Count; i++)
                {
                    strArray[i] = localConfigItemName.Rows[i][0].ToString();
                }
            }
            return strArray;
        }

        [WebMethod]
        public DataTable GetResourceCultureByHost_Control(string applicationName, string environmentName, string cultureCode, string resourceHost)
        {
            DataTable allResourceCulture = this.GetAllResourceCulture();
            if ((allResourceCulture == null) || (allResourceCulture.Rows.Count <= 0))
            {
                return null;
            }
            string filterExpression = string.Format("ApplicationName='{0}' and EnvironmentName='{1}' and CultureCode = '{2}' and ResourceHost='{3}' and ResourceValue not like 'msg_%'", new object[] { applicationName, environmentName, cultureCode, resourceHost });
            DataRow[] rowArray = allResourceCulture.Select(filterExpression);
            DataTable table2 = allResourceCulture.Clone();
            foreach (DataRow row in rowArray)
            {
                table2.ImportRow(row);
            }
            table2.AcceptChanges();
            return table2;
        }

        [WebMethod]
        public DataTable GetResourceCultureByHost_Message(string applicationName, string environmentName, string cultureCode, string resourceHost)
        {
            DataTable allResourceCulture = this.GetAllResourceCulture();
            if ((allResourceCulture == null) || (allResourceCulture.Rows.Count <= 0))
            {
                return null;
            }
            string filterExpression = string.Format("ApplicationName='{0}' and EnvironmentName='{1}' and CultureCode = '{2}' and ResourceHost='{3}' and ResourceValue like 'msg_%'", new object[] { applicationName, environmentName, cultureCode, resourceHost });
            DataRow[] rowArray = allResourceCulture.Select(filterExpression);
            DataTable table2 = allResourceCulture.Clone();
            foreach (DataRow row in rowArray)
            {
                table2.ImportRow(row);
            }
            table2.AcceptChanges();
            return table2;
        }

        [WebMethod]
        public DataTable GetResourceCultureByKey(string applicationName, string environmentName, string cultureCode, string resourceHost, string resourceKey)
        {
            DataTable allResourceCulture = this.GetAllResourceCulture();
            if ((allResourceCulture == null) || (allResourceCulture.Rows.Count <= 0))
            {
                return null;
            }
            string filterExpression = string.Format("ApplicationName='{0}' and EnvironmentName='{1}' and CultureCode = '{2}' and ResourceHost='{3}' and ResourceKey = '{4}'", new object[] { applicationName, environmentName, cultureCode, resourceHost, resourceKey });
            DataRow[] rowArray = allResourceCulture.Select(filterExpression);
            DataTable table2 = allResourceCulture.Clone();
            foreach (DataRow row in rowArray)
            {
                table2.ImportRow(row);
            }
            table2.AcceptChanges();
            return table2;
        }

        [WebMethod]
        public DataTable GetSelectedSystemConfig(string applicationName, string environment)
        {
            DataTable allSystemConfig = this.GetAllSystemConfig();
            if ((allSystemConfig == null) || (allSystemConfig.Rows.Count <= 0))
            {
                throw new Exception("未设置任何系统配置信息，请联系管理员！");
            }
            string filterExpression = string.Format("ApplicationName='{0}' and EnvironmentName='{1}' and IsSelected = 1 ", applicationName, environment);
            DataRow[] rowArray = allSystemConfig.Select(filterExpression);
            DataTable table2 = allSystemConfig.Clone();
            foreach (DataRow row in rowArray)
            {
                table2.ImportRow(row);
            }
            table2.AcceptChanges();
            return table2;
        }

        [WebMethod]
        public DataTable GetSelectedSystemConfigByItem(string applicationName, string environment, string configItemName)
        {
            DataTable allSystemConfig = this.GetAllSystemConfig();
            if ((allSystemConfig == null) || (allSystemConfig.Rows.Count <= 0))
            {
                throw new Exception("未设置任何系统配置信息，请联系管理员！");
            }
            string filterExpression = string.Format("ApplicationName='{0}' and EnvironmentName='{1}' and ConfigItemName='{2}' and IsSelected = 1", applicationName, environment, configItemName);
            DataRow[] rowArray = allSystemConfig.Select(filterExpression);
            DataTable table2 = allSystemConfig.Clone();
            foreach (DataRow row in rowArray)
            {
                table2.ImportRow(row);
            }
            table2.AcceptChanges();
            return table2;
        }

        [WebMethod]
        public DataTable GetSelectedSystemConfigForLocal(string applicationName, string environment)
        {
            DataTable allSystemConfig = this.GetAllSystemConfig();
            if ((allSystemConfig == null) || (allSystemConfig.Rows.Count <= 0))
            {
                throw new Exception("未设置任何系统配置信息，请联系管理员！");
            }
            string filterExpression = string.Format("ApplicationName='{0}' and EnvironmentName='{1}' and IsSelected = 1 and IsGlobal = 0 ", applicationName, environment);
            DataRow[] rowArray = allSystemConfig.Select(filterExpression);
            DataTable table2 = allSystemConfig.Clone();
            foreach (DataRow row in rowArray)
            {
                table2.ImportRow(row);
            }
            table2.AcceptChanges();
            return table2;
        }

        [WebMethod]
        public DataTable GetSystemConfig(string applicationName, string environment)
        {
            DataTable allSystemConfig = this.GetAllSystemConfig();
            if ((allSystemConfig == null) || (allSystemConfig.Rows.Count <= 0))
            {
                throw new Exception("未设置任何系统配置信息，请联系管理员！");
            }
            string filterExpression = string.Format("ApplicationName='{0}' and EnvironmentName='{1}' ", applicationName, environment);
            DataRow[] rowArray = allSystemConfig.Select(filterExpression);
            DataTable table2 = allSystemConfig.Clone();
            foreach (DataRow row in rowArray)
            {
                table2.ImportRow(row);
            }
            table2.AcceptChanges();
            return table2;
        }

        [WebMethod]
        public DataTable GetSystemConfigByItem(string applicationName, string environment, string configItemName)
        {
            DataTable allSystemConfig = this.GetAllSystemConfig();
            if ((allSystemConfig == null) || (allSystemConfig.Rows.Count <= 0))
            {
                throw new Exception("未设置任何系统配置信息，请联系管理员！");
            }
            string filterExpression = string.Format("ApplicationName='{0}' and EnvironmentName='{1}' and ConfigItemName='{2}'", applicationName, environment, configItemName);
            DataRow[] rowArray = allSystemConfig.Select(filterExpression);
            DataTable table2 = allSystemConfig.Clone();
            foreach (DataRow row in rowArray)
            {
                table2.ImportRow(row);
            }
            table2.AcceptChanges();
            return table2;
        }

        [WebMethod]
        public DataTable GetSystemConfigForLocal(string applicationName, string environment)
        {
            DataTable allSystemConfig = this.GetAllSystemConfig();
            if ((allSystemConfig == null) || (allSystemConfig.Rows.Count <= 0))
            {
                throw new Exception("未设置任何系统配置信息，请联系管理员！");
            }
            string filterExpression = string.Format("ApplicationName='{0}' and EnvironmentName='{1}' and IsGlobal = 0 ", applicationName, environment);
            DataRow[] rowArray = allSystemConfig.Select(filterExpression);
            DataTable table2 = allSystemConfig.Clone();
            foreach (DataRow row in rowArray)
            {
                table2.ImportRow(row);
            }
            table2.AcceptChanges();
            return table2;
        }

        private DataTable GetSystemConfigItemName(string applicationName, string environment)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("select distinct ConfigItemName\r\n                                        from dbo.View_Plat_cfg_SystemConfig\r\n                                        where ApplicationName = '{0}'\r\n                                        and EnvironmentName = '{1}'\r\n                                        and IsActiveApplication = 1\r\n                                        and IsActiveEnvironment = 1\r\n                                        and ConfigItemStatus = 1", applicationName, environment);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public string[] GetSystemConfigItemNameArray(string applicationName, string environment)
        {
            string[] strArray = new string[0];
            DataTable systemConfigItemName = this.GetSystemConfigItemName(applicationName, environment);
            if ((systemConfigItemName != null) || (systemConfigItemName.Rows.Count > 0))
            {
                strArray = new string[systemConfigItemName.Rows.Count];
                for (int i = 0; i < systemConfigItemName.Rows.Count; i++)
                {
                    strArray[i] = systemConfigItemName.Rows[i][0].ToString();
                }
            }
            return strArray;
        }

        private DataTable GetUserConfig(Guid userGuid, string applicationName, string environment)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("SELECT [UserConfigGuid]\r\n                                              ,[UserGuid]\r\n                                              ,[SystemConfigItemGuid]\r\n                                              ,[ResetDatetime]\r\n                                              ,[IsActiveUserConfig]\r\n                                              ,[EnvironmentGuid]\r\n                                              ,[UserConfigName]\r\n                                              ,[UserConfigValue]\r\n                                              ,[EnvironmentName]\r\n                                              ,[IsActiveEnvironment]\r\n                                              ,[ConfigItemName]\r\n                                              ,[ConfigItemDesc]\r\n                                              ,[ConfigItemType]\r\n                                              ,[ConfigItemStatus]\r\n                                              ,[IsEncrypt]\r\n                                              ,[IsGlobal]\r\n                                              ,[IsReadOnly]\r\n                                              ,[ApplicationName]\r\n                                              ,[IsActiveApplication]\r\n                                          FROM [View_Plat_cfg_UserConfig]\r\n                                          WHERE [IsActiveUserConfig] = 1\r\n                                          AND [IsActiveApplication] = 1\r\n                                          AND [IsActiveEnvironment] = 1\r\n                                          AND [ConfigItemStatus] = 1\r\n                                          AND [UserGuid] = '{0}'\r\n                                          AND [ApplicationName] = '{1}'\r\n                                          AND [EnvironmentName] = '{2}'\r\n                                          order by [ConfigItemName]", userGuid, applicationName, environment);
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetUserConfigByItem(Guid userGuid, string applicationName, string environment, string configItemName)
        {
            return this.GetUserConfigByItem_Private(userGuid, applicationName, environment, configItemName);
        }

        private DataTable GetUserConfigByItem_Private(Guid userGuid, string applicationName, string environment, string configItemName)
        {
            DataTable table = new DataTable();
            string configDBConnString = this.GetConfigDBConnString();
            string str2 = string.Format("SELECT [UserConfigGuid]\r\n                                              ,[UserGuid]\r\n                                              ,[SystemConfigItemGuid]\r\n                                              ,[ResetDatetime]\r\n                                              ,[IsActiveUserConfig]\r\n                                              ,[EnvironmentGuid]\r\n                                              ,[UserConfigName]\r\n                                              ,[UserConfigValue]\r\n                                              ,[EnvironmentName]\r\n                                              ,[IsActiveEnvironment]\r\n                                              ,[ConfigItemName]\r\n                                              ,[ConfigItemDesc]\r\n                                              ,[ConfigItemType]\r\n                                              ,[ConfigItemStatus]\r\n                                              ,[IsEncrypt]\r\n                                              ,[IsGlobal]\r\n                                              ,[IsReadOnly]\r\n                                              ,[ApplicationName]\r\n                                              ,[IsActiveApplication]\r\n                                          FROM [View_Plat_cfg_UserConfig]\r\n                                          WHERE [IsActiveUserConfig] = 1\r\n                                          AND [IsActiveApplication] = 1\r\n                                          AND [IsActiveEnvironment] = 1\r\n                                          AND [ConfigItemStatus] = 1\r\n                                          AND [UserGuid] = '{0}'\r\n                                          AND [ApplicationName] = '{1}'\r\n                                          AND [EnvironmentName] = '{2}'\r\n                                          AND [ConfigItemName] = '{3}'\r\n                                          order by [ConfigItemName]", new object[] { userGuid, applicationName, environment, configItemName });
            SqlConnection connection = new SqlConnection(configDBConnString);
            try
            {
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        [WebMethod]
        public DataTable GetUserConfigByUser(Guid userGuid, string applicationName, string environment)
        {
            return this.GetUserConfig(userGuid, applicationName, environment);
        }

        [WebMethod]
        public bool InsertApplication(string applicationName, string applicationDesc, string architecture, string applicationGroup, Guid createUserGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                DataTable byApplicationName = this.GetByApplicationName(applicationName);
                if ((byApplicationName != null) && (byApplicationName.Rows.Count > 0))
                {
                    throw new Exception("Current application has exist in database!");
                }
                string str2 = string.Format("INSERT INTO [Plat_cfg_Application]\r\n                                               ([ApplicationGuid]\r\n                                               ,[ApplicationName]\r\n                                               ,[ApplicationDesc]\r\n                                               ,[Architecture]\r\n                                               ,[ApplicationGroup]\r\n                                               ,[CreateDatetime]\r\n                                               ,[CreateUser]\r\n                                               ,[IsActive]\r\n                                               ,[MyMark])\r\n                                         VALUES\r\n                                               (newid()\r\n                                               ,'{0}'\r\n                                               ,'{1}'\r\n                                               ,'{2}'\r\n                                               ,'{3}'\r\n                                               ,getdate()\r\n                                               ,'{4}'\r\n                                               ,1\r\n                                               ,0)", new object[] { applicationName, applicationDesc, architecture, applicationGroup, createUserGuid });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool InsertConfigItem(Guid applicationGuid, string configItemName, string configItemDesc, string configItemType, string configItemStatus, bool isEncrypt, bool isGlobal, bool isReadOnly, Guid createUserGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("INSERT INTO [Plat_cfg_SystemConfigItem]\r\n                                               (ConfigItemGuid,\r\n                                                ApplicationGuid,\r\n                                                ConfigItemName,\r\n                                                ConfigItemDesc,\r\n                                                ConfigItemType,\r\n                                                ConfigItemStatus,\r\n                                                IsEncrypt,\r\n                                                IsGlobal,\r\n                                                IsReadOnly,\r\n                                                CreateDatetime,\r\n                                                CreateUser,\r\n                                                MyMark)\r\n                                         VALUES\r\n                                               (newid(),\r\n                                                '{0}',\r\n                                                '{1}',\r\n                                                '{2}',\r\n                                                '{3}',\r\n                                                '{4}',\r\n                                                '{5}',\r\n                                                '{6}',\r\n                                                '{7}',\r\n                                                getdate(),\r\n                                                '{8}',\r\n                                                0)", new object[] { applicationGuid, configItemName, configItemDesc, configItemType, configItemStatus, isEncrypt, isGlobal, isReadOnly, createUserGuid });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool InsertConfigOption(Guid configItemGuid, string optionName, string optionDesc, string optionDataType, string optionStatus, bool isSelected, int optionSequence, Guid createUserGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("INSERT INTO [Plat_cfg_SystemConfigOption]\r\n                                               (ConfigOptionGuid,\r\n                                                ConfigItemGuid,\r\n                                                OptionName,\r\n                                                OptionDesc,\r\n                                                OptionDataType,\r\n                                                OptionStatus,\r\n                                                IsSelected,\r\n                                                OptionSequence,\r\n                                                CreateDatetime,\r\n                                                CreateUser,\r\n                                                MyMark)\r\n                                         VALUES\r\n                                               (newid(),\r\n                                                '{0}',\r\n                                                '{1}',\r\n                                                '{2}',\r\n                                                '{3}',\r\n                                                '{4}',\r\n                                                '{5}',\r\n                                                '{6}',\r\n                                                getdate(),\r\n                                                '{7}',\r\n                                                0)", new object[] { configItemGuid, optionName, optionDesc, optionDataType, optionStatus, isSelected, optionSequence, createUserGuid });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool InsertConfigValue(Guid configItemGuid, Guid configOptionGuid, Guid environmentGuid, string valueName, string configValue, string valueDesc, bool isSelected, int configValueSequence)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("INSERT INTO [dbo].[Plat_cfg_SystemConfigValue]\r\n                                               ([ConfigValueGuid]\r\n                                               ,[ConfigItemGuid]\r\n                                               ,[ConfigOptionGuid]\r\n                                               ,[EnvironmentGuid]\r\n                                               ,[ValueName]\r\n                                               ,[ConfigValue]\r\n                                               ,[ValueDesc]\r\n                                               ,[IsSelected]\r\n                                               ,[ResetDatetime]\r\n                                               ,[ConfigValueSequence]\r\n                                               ,[IsActive]\r\n                                               ,[MyMark])\r\n                                         VALUES\r\n                                               (newid(),\r\n                                               '{0}',\r\n                                               '{1}',\r\n                                               '{2}',\r\n                                               '{3}',\r\n                                               '{4}',\r\n                                               '{5}',\r\n                                               '{6}',\r\n                                               getdate(),\r\n                                               '{7}',\r\n                                               1,\r\n                                               0)", new object[] { configItemGuid, configOptionGuid, environmentGuid, valueName, configValue, valueDesc, isSelected, configValueSequence });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool InsertConfigVersion(Guid applicationGuid, Guid environmentGuid, string versionDesc, Guid createUserGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("declare @ConfigVersionGuid uniqueidentifier\r\n                                                set @ConfigVersionGuid = newid()\r\n\r\n                                                INSERT INTO [dbo].[Plat_cfg_SystemConfigValueVersion]\r\n                                                   ([ConfigVersionGuid]\r\n                                                   ,[ApplicationGuid]\r\n                                                   ,[EnvironmentGuid]\r\n                                                   ,[VersionDesc]\r\n                                                   ,[VersionDatetime]\r\n                                                   ,[VersionCreator]\r\n                                                   ,[MyMark])\r\n                                             VALUES\r\n                                                   (@ConfigVersionGuid\r\n                                                   ,'{0}'\r\n                                                   ,'{1}'\r\n                                                   ,'{2}'\r\n                                                   ,getdate()\r\n                                                   ,'{3}'\r\n                                                   ,0)\r\n\r\n                                        INSERT INTO [dbo].[Plat_cfg_SystemConfigValueVersionDetail]\r\n                                                   ([ConfigVersionDetailGuid]\r\n                                                   ,[ConfigVersionGuid]\r\n                                                   ,[ConfigItemGuid]\r\n                                                   ,[ConfigOptionGuid]\r\n                                                   ,[EnvironmentGuid]\r\n                                                   ,[ValueName]\r\n                                                   ,[ConfigValue]\r\n                                                   ,[ValueDesc]\r\n                                                   ,[IsSelected]\r\n                                                   ,[ResetDatetime]\r\n                                                   ,[ConfigValueSequence]\r\n                                                   ,[IsActive]\r\n                                                   ,[MyMark])\r\n                                        select newid()\r\n\t\t                                           ,@ConfigVersionGuid\r\n                                                   ,v.ConfigItemGuid\r\n                                                   ,v.ConfigOptionGuid\r\n                                                   ,v.EnvironmentGuid\r\n                                                   ,v.ValueName\r\n                                                   ,v.ConfigValue\r\n                                                   ,v.ValueDesc\r\n                                                   ,v.IsSelected\r\n                                                   ,v.ResetDatetime\r\n                                                   ,v.ConfigValueSequence\r\n                                                   ,v.IsActive\r\n                                                   ,v.MyMark\r\n                                        from dbo.Plat_cfg_SystemConfigValue v\r\n                                        inner join dbo.Plat_cfg_SystemConfigItem i on v.ConfigItemGuid = i.ConfigItemGuid\r\n                                        where v.EnvironmentGuid = '{1}'\r\n                                        and i.ApplicationGuid = '{0}'", new object[] { applicationGuid, environmentGuid, versionDesc, createUserGuid });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool InsertEnvironment(string environmentName, string environmentDesc, Guid createUserGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                DataTable byEnvironmentName = this.GetByEnvironmentName(environmentName);
                if ((byEnvironmentName != null) && (byEnvironmentName.Rows.Count > 0))
                {
                    throw new Exception("Current Environment exists in database!");
                }
                string str2 = string.Format("INSERT INTO [Plat_cfg_Environment]\r\n                                               ([EnvironmentGuid]\r\n                                               ,[EnvironmentName]\r\n                                               ,[EnvironmentDesc]\r\n                                               ,[CreateDatetime]\r\n                                               ,[CreateUser]\r\n                                               ,[IsActive]\r\n                                               ,[MyMark])\r\n                                         VALUES\r\n                                               (newid()\r\n                                               ,'{0}'\r\n                                               ,'{1}'\r\n                                               ,getdate()\r\n                                               ,'{2}'\r\n                                               ,1\r\n                                               ,0)", environmentName, environmentDesc, createUserGuid);
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        //[WebMethod(EnableSession = true)]
        //public bool IsUserExisted(string username)
        //{
        //    try
        //    {
        //        if ((username == null) || (username == string.Empty))
        //        {
        //            return false;
        //        }
        //        if (CABUsers.GetUser(username) == null)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        [WebMethod]
        public void RestartCache()
        {
            if (HttpRuntime.Cache[this._systemCacheKey] != null)
            {
                HttpRuntime.Cache.Remove(this._systemCacheKey);
                this.GetAllSystemConfig();
            }
            if (HttpRuntime.Cache[this._resourceCacheKey] != null)
            {
                HttpRuntime.Cache.Remove(this._resourceCacheKey);
                this.GetAllResourceCulture();
            }
        }

        [WebMethod]
        public void RestartConfigCache()
        {
            if (HttpRuntime.Cache[this._systemCacheKey] != null)
            {
                HttpRuntime.Cache.Remove(this._systemCacheKey);
                this.GetAllSystemConfig();
            }
        }

        [WebMethod]
        public void RestartResourceCache()
        {
            if (HttpRuntime.Cache[this._resourceCacheKey] != null)
            {
                HttpRuntime.Cache.Remove(this._resourceCacheKey);
                this.GetAllResourceCulture();
            }
        }

        [WebMethod]
        public bool RestoreConfigVersion(Guid configVersionGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("declare @ApplicationGuid uniqueidentifier\r\n                                                declare @EnvironmentGuid uniqueidentifier\r\n\r\n                                                select @ApplicationGuid = ApplicationGuid,@EnvironmentGuid = EnvironmentGuid\r\n                                                from dbo.Plat_cfg_SystemConfigValueVersion where ConfigVersionGuid = '{0}'\r\n\r\n                                                delete from dbo.Plat_cfg_SystemConfigValue\r\n                                                from dbo.Plat_cfg_SystemConfigValue v\r\n                                                inner join dbo.Plat_cfg_SystemConfigItem i on v.ConfigItemGuid = i.ConfigItemGuid\r\n                                                where i.ApplicationGuid = @ApplicationGuid\r\n                                                and v.EnvironmentGuid = @EnvironmentGuid\r\n\r\n                                                INSERT INTO [dbo].[Plat_cfg_SystemConfigValue]\r\n                                                           ([ConfigValueGuid]\r\n                                                           ,[ConfigItemGuid]\r\n                                                           ,[ConfigOptionGuid]\r\n                                                           ,[EnvironmentGuid]\r\n                                                           ,[ValueName]\r\n                                                           ,[ConfigValue]\r\n                                                           ,[ValueDesc]\r\n                                                           ,[IsSelected]\r\n                                                           ,[ResetDatetime]\r\n                                                           ,[ConfigValueSequence]\r\n                                                           ,[IsActive]\r\n                                                           ,[MyMark])\r\n                                                select newid(),\r\n\t                                                ConfigItemGuid,\r\n\t                                                ConfigOptionGuid,\r\n\t                                                EnvironmentGuid,\r\n\t                                                ValueName,\r\n\t                                                ConfigValue,\r\n\t                                                ValueDesc,\r\n\t                                                IsSelected,\r\n                                                    ResetDatetime,\r\n\t                                                ConfigValueSequence,\r\n\t                                                IsActive,\r\n                                                    0\r\n                                                from dbo.Plat_cfg_SystemConfigValueVersionDetail\r\n                                                where ConfigVersionGuid = '{0}'", configVersionGuid);
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool SaveResourceCultureByKey(string applicationName, string environmentName, string cultureCode, string resourceHost, string resourceKey, string resourceValue, string resourceLocation, string resourceSize)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("declare @ApplicationGuid uniqueidentifier,\r\n\t                                                     @EnvironmentGuid uniqueidentifier\r\n\t    \r\n                                                select @ApplicationGuid = ApplicationGuid from Plat_cfg_Application where ApplicationName = '{0}'\r\n                                                select @EnvironmentGuid = EnvironmentGuid from Plat_cfg_Environment where EnvironmentName = '{1}'\r\n\r\n                                                delete from Plat_cfg_CultureResource\r\n                                                from Plat_cfg_CultureResource c\r\n                                                inner join Plat_cfg_Application a on c.ApplicationGuid = a.ApplicationGuid\r\n                                                inner join Plat_cfg_Environment e on c.EnvironmentGuid = e.EnvironmentGuid\r\n                                                where a.ApplicationName = '{0}'\r\n                                                and e.EnvironmentName = '{1}'\r\n                                                and c.CultureCode = '{2}'\r\n                                                and c.ResourceHost = '{3}'\r\n                                                and c.ResourceKey = '{4}'\r\n\r\n                                                INSERT INTO [Plat_cfg_CultureResource]\r\n                                                       ([ResourceGuid]\r\n                                                       ,[ApplicationGuid]\r\n                                                       ,[EnvironmentGuid]\r\n                                                       ,[ResourceHost]\r\n                                                       ,[ResourceKey]\r\n                                                       ,[CultureCode]\r\n                                                       ,[ResourceValue]\r\n                                                       ,[ResourceLocation]\r\n                                                       ,[ResourceSize]\r\n                                                       ,[IsActive]\r\n                                                       ,[MyMark])\r\n                                                 VALUES\r\n                                                       (newid()\r\n                                                       ,@ApplicationGuid\r\n                                                       ,@EnvironmentGuid\r\n                                                       ,'{3}'\r\n                                                       ,'{4}'\r\n                                                       ,'{2}'\r\n                                                       ,'{5}'\r\n                                                       ,'{6}'\r\n                                                       ,'{7}'\r\n                                                       ,1\r\n                                                       ,0)", new object[] { applicationName, environmentName, cultureCode, resourceHost, resourceKey, resourceValue, resourceLocation, resourceSize });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public DataTable SearchConfigItems(Guid applicationGuid, string configItemName, string configItemDesc, string configItemType, string configItemStatus, bool? isEncrypt, bool? isGlobal, bool? isReadOnly)
        {
            DataTable table = new DataTable();
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = " select * from dbo.Plat_cfg_SystemConfigItem where (1 = 1) ";
                if (applicationGuid != Guid.Empty)
                {
                    str2 = str2 + string.Format(" and ApplicationGuid = '{0}'", applicationGuid);
                }
                if (!string.IsNullOrEmpty(configItemName))
                {
                    str2 = str2 + string.Format(" and ConfigItemName like '%{0}%'", configItemName);
                }
                if (!string.IsNullOrEmpty(configItemDesc))
                {
                    str2 = str2 + string.Format(" and ConfigItemDesc like '%{0}%'", configItemDesc);
                }
                if (configItemType != "-1")
                {
                    str2 = str2 + string.Format(" and ConfigItemType = '{0}'", configItemType);
                }
                if (configItemStatus != "-1")
                {
                    str2 = str2 + string.Format(" and ConfigItemStatus = '{0}'", configItemStatus);
                }
                if (isEncrypt.HasValue)
                {
                    str2 = str2 + string.Format(" and IsEncrypt = '{0}'", isEncrypt.Value);
                }
                if (isGlobal.HasValue)
                {
                    str2 = str2 + string.Format(" and IsGlobal = '{0}'", isGlobal.Value);
                }
                if (isReadOnly.HasValue)
                {
                    str2 = str2 + string.Format(" and IsReadOnly = '{0}'", isReadOnly.Value);
                }
                str2 = str2 + " order by ConfigItemName asc ";
                SqlCommand selectCommand = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                DataSet dataSet = new DataSet();
                new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
                connection.Close();
                table = dataSet.Tables["tb1"];
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return table;
        }

        private void SetUserConfigByItem(Guid userGuid, Guid configItemGuid, Guid environmentGuid, string[] configNames, string[] configValues)
        {
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("delete from dbo.Plat_cfg_UserConfig \r\n                                            where UserGuid = '{0}'\r\n                                            and SystemConfigItemGuid = '{1}'\r\n                                            and EnvironmentGuid = '{2}'", userGuid, configItemGuid, environmentGuid);
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                string str3 = string.Empty;
                for (int i = 0; i < configNames.Length; i++)
                {
                    str3 = string.Format("INSERT INTO [Plat_cfg_UserConfig]\r\n                                               ([UserConfigGuid]\r\n                                               ,[UserGuid]\r\n                                               ,[SystemConfigItemGuid]\r\n                                               ,[EnvironmentGuid]\r\n                                               ,[UserConfigName]\r\n                                               ,[UserConfigValue]\r\n                                               ,[ResetDatetime]\r\n                                               ,[IsActive]\r\n                                               ,[MyMark])\r\n                                         VALUES\r\n                                               (newid()\r\n                                               ,'{0}'\r\n                                               ,'{1}'\r\n                                               ,'{2}'\r\n                                               ,'{3}'\r\n                                               ,'{4}'\r\n                                               ,getdate()\r\n                                               ,1\r\n                                               ,0)", new object[] { userGuid, configItemGuid, environmentGuid, configNames[i], configValues[i] });
                    command.CommandText = str3;
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
        }

        [WebMethod]
        public bool SetUserConfigByItem(Guid userGuid, string applicationName, string environment, string configItemName, string[] configItemNames, string[] configItemValues)
        {
            DataTable table = this.GetSystemConfigByItem(applicationName, environment, configItemName);
            if ((table == null) || (table.Rows.Count <= 0))
            {
                throw new Exception("设置出错：您当前要设置的配置项无效，请联系管理员！");
            }
            if ((configItemValues == null) || (configItemValues.Length <= 0))
            {
                throw new Exception("设置出错：您当前要设置的配置项值不能为空，请联系管理员！");
            }
            if (configItemNames.Length != configItemValues.Length)
            {
                throw new Exception("设置出错：配置项名称数组与配置项值数组长度不一致，请联系管理员！");
            }
            this.SetUserConfigByItem(userGuid, new Guid(table.Rows[0]["ConfigItemGuid"].ToString()), new Guid(table.Rows[0]["EnvironmentGuid"].ToString()), configItemNames, configItemValues);
            return true;
        }

        [WebMethod]
        public bool SynchronizeConfig(Guid appGuid, Guid sourceEnvGuid, Guid targetEnvGuid)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("--clear target config value\r\n                                            delete from dbo.Plat_cfg_SystemConfigValue \r\n                                            from dbo.Plat_cfg_SystemConfigValue a\r\n                                            inner join dbo.Plat_cfg_SystemConfigItem b on a.ConfigItemGuid = b.ConfigItemGuid\r\n                                            where b.ApplicationGuid = '{0}' \r\n                                            and a.EnvironmentGuid = '{2}'\r\n\r\n                                            --insert new config values from source env\r\n                                            insert into dbo.Plat_cfg_SystemConfigValue\r\n                                            select newid(),a.ConfigItemGuid,a.ConfigOptionGuid,'{2}',\r\n                                            a.ValueName,a.ConfigValue,a.ValueDesc,a.IsSelected,getdate(),a.ConfigValueSequence,1,0\r\n                                            from dbo.Plat_cfg_SystemConfigValue a\r\n                                            inner join dbo.Plat_cfg_SystemConfigItem b on a.ConfigItemGuid = b.ConfigItemGuid\r\n                                            where b.ApplicationGuid = '{0}' \r\n                                            and a.EnvironmentGuid = '{1}'", appGuid, sourceEnvGuid, targetEnvGuid);
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool UpdateApplication(Guid applicationGuid, string applicationName, string applicationDesc, string architecture, string applicationGroup)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("update dbo.Plat_cfg_Application \r\n                                                set ApplicationName = '{1}',\r\n                                                    ApplicationDesc = '{2}',\r\n                                                    Architecture = '{3}',\r\n                                                    ApplicationGroup = '{4}'\r\n                                                where ApplicationGuid = '{0}'", new object[] { applicationGuid, applicationName, applicationDesc, architecture, applicationGroup });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool UpdateConfigItem(Guid configItemGuid, Guid applicationGuid, string configItemName, string configItemDesc, string configItemType, string configItemStatus, bool isEncrypt, bool isGlobal, bool isReadOnly)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("update dbo.Plat_cfg_SystemConfigItem \r\n                                                set ApplicationGuid = '{1}',\r\n                                                    ConfigItemName = '{2}',\r\n                                                    ConfigItemDesc = '{3}',\r\n                                                    ConfigItemType = '{4}',\r\n                                                    ConfigItemStatus = '{5}',\r\n                                                    IsEncrypt = '{6}',\r\n                                                    IsGlobal = '{7}',\r\n                                                    IsReadOnly = '{8}'\r\n                                                where ConfigItemGuid = '{0}'", new object[] { configItemGuid, applicationGuid, configItemName, configItemDesc, configItemType, configItemStatus, isEncrypt, isGlobal, isReadOnly });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool UpdateConfigOption(Guid configOptionGuid, string optionName, string optionDesc, string optionDataType, string optionStatus, bool isSelected, int optionSequenc)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("update dbo.Plat_cfg_SystemConfigOption \r\n                                                set OptionName = '{1}',\r\n                                                    OptionDesc = '{2}',\r\n                                                    OptionDataType = '{3}',\r\n                                                    OptionStatus = '{4}',\r\n                                                    IsSelected = '{5}',\r\n                                                    OptionSequence = '{6}'\r\n                                                where ConfigOptionGuid = '{0}'", new object[] { configOptionGuid, optionName, optionDesc, optionDataType, optionStatus, isSelected, optionSequenc });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool UpdateConfigValue(Guid configValueGuid, string configValue, string valueDesc, bool isSelected)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("update dbo.Plat_cfg_SystemConfigValue\r\n                                                set ConfigValue = '{1}',\r\n\t                                                ValueDesc = '{2}',\r\n\t                                                IsSelected = '{3}',\r\n                                                    ResetDatetime = getdate()\r\n                                                where ConfigValueGuid = '{0}'", new object[] { configValueGuid, configValue, valueDesc, isSelected });
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }

        [WebMethod]
        public bool UpdateEnvironment(Guid environmentGuid, string environmentName, string environmentDesc)
        {
            bool flag;
            SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
            try
            {
                string str2 = string.Format("update dbo.Plat_cfg_Environment \r\n                                                set EnvironmentName = '{1}',\r\n                                                    EnvironmentDesc = '{2}' \r\n                                                where EnvironmentGuid = '{0}'", environmentGuid, environmentName, environmentDesc);
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = str2
                };
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                flag = true;
            }
            catch (Exception exception)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                throw new Exception(exception.Message);
            }
            return flag;
        }
    }
}

