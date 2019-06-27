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
    public class ConfigService : WebService
    {
        ILog log = LogManager.GetLogger(typeof(ConfigService));

        private static object _lockObject = new object();
        private string _resourceCacheKey = "SS_ResourceCultureCache";
        private string _systemCacheKey = "SS_SystemConfigCache";
        private string _userCacheKey = "SS_UserConfigCache";

        [WebMethod]
        public DataTable GetActiveApplications()
        {
            log.Info("GetActiveApplications");

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
            log.Info("GetActiveConfigItemsByApp");

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
            log.Info("GetActiveEnvironments");

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
            log.Info("GetActiveOptionsByItem");

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
            log.Info("GetActiveValuesByItem");

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
            log.Info("GetActiveValuesByOption");

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

        private DataTable GetAllResourceCulture()
        {
            log.Info("GetAllResourceCulture");

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
            log.Info("GetAllSystemConfig");

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
            log.Info("GetByApplicationGuid");

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
            log.Info("GetByApplicationName");


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
            log.Info("GetByConfigItemGuid");
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
            log.Info("GetByConfigItemName");
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
            log.Info("GetByConfigOptionGuid");
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
            log.Info("GetByConfigOptionName");
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
            log.Info("GetByEnvironmentGuid");
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
            log.Info("GetByEnvironmentName");
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
            log.Info("GetConfigDBConnString");
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
            log.Info("GetConfigValueByOptionAndEnvironment");
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
            log.Info("GetConfigVersions");
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
            log.Info("GetLocalConfigItemName");
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
            log.Info("GetLocalConfigItemNameArray");
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
            log.Info("GetResourceCultureByHost_Control");
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
            log.Info("GetResourceCultureByHost_Message");
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
            log.Info("GetResourceCultureByKey");
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
            log.Info("GetSelectedSystemConfig");
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

        /// <summary>
        /// 获取数据库连接--只用到这个
        /// </summary>
        /// <param name="applicationName"></param>
        /// <param name="environment"></param>
        /// <param name="configItemName"></param>
        /// <returns></returns>
        [WebMethod]
        public DataTable GetSelectedSystemConfigByItem(string applicationName, string environment, string configItemName)
        {
            log.Info("GetSelectedSystemConfigByItem");
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
            log.Info("GetSelectedSystemConfigForLocal");
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
            log.Info("GetSystemConfig");
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
            log.Info("GetSystemConfigByItem");
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
            log.Info("GetSystemConfigForLocal");
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
            log.Info("GetSystemConfigItemName");
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
            log.Info("GetSystemConfigItemNameArray");
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

        //private DataTable GetUserConfig(Guid userGuid, string applicationName, string environment)
        //{
        //    log.Info("GetUserConfig");
        //    DataTable table = new DataTable();
        //    string configDBConnString = this.GetConfigDBConnString();
        //    string str2 = string.Format("SELECT [UserConfigGuid]\r\n                                              ,[UserGuid]\r\n                                              ,[SystemConfigItemGuid]\r\n                                              ,[ResetDatetime]\r\n                                              ,[IsActiveUserConfig]\r\n                                              ,[EnvironmentGuid]\r\n                                              ,[UserConfigName]\r\n                                              ,[UserConfigValue]\r\n                                              ,[EnvironmentName]\r\n                                              ,[IsActiveEnvironment]\r\n                                              ,[ConfigItemName]\r\n                                              ,[ConfigItemDesc]\r\n                                              ,[ConfigItemType]\r\n                                              ,[ConfigItemStatus]\r\n                                              ,[IsEncrypt]\r\n                                              ,[IsGlobal]\r\n                                              ,[IsReadOnly]\r\n                                              ,[ApplicationName]\r\n                                              ,[IsActiveApplication]\r\n                                          FROM [View_Plat_cfg_UserConfig]\r\n                                          WHERE [IsActiveUserConfig] = 1\r\n                                          AND [IsActiveApplication] = 1\r\n                                          AND [IsActiveEnvironment] = 1\r\n                                          AND [ConfigItemStatus] = 1\r\n                                          AND [UserGuid] = '{0}'\r\n                                          AND [ApplicationName] = '{1}'\r\n                                          AND [EnvironmentName] = '{2}'\r\n                                          order by [ConfigItemName]", userGuid, applicationName, environment);
        //    SqlConnection connection = new SqlConnection(configDBConnString);
        //    try
        //    {
        //        SqlCommand selectCommand = new SqlCommand
        //        {
        //            Connection = connection,
        //            CommandType = CommandType.Text,
        //            CommandText = str2
        //        };
        //        connection.Open();
        //        DataSet dataSet = new DataSet();
        //        new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
        //        connection.Close();
        //        table = dataSet.Tables["tb1"];
        //    }
        //    catch (Exception exception)
        //    {
        //        if (connection.State != ConnectionState.Closed)
        //        {
        //            connection.Close();
        //        }
        //        throw new Exception(exception.Message);
        //    }
        //    return table;
        //}

        //[WebMethod]
        //public DataTable GetUserConfigByItem(Guid userGuid, string applicationName, string environment, string configItemName)
        //{
        //    log.Info("GetUserConfigByItem");
        //    return this.GetUserConfigByItem_Private(userGuid, applicationName, environment, configItemName);
        //}

        //private DataTable GetUserConfigByItem_Private(Guid userGuid, string applicationName, string environment, string configItemName)
        //{
        //    DataTable table = new DataTable();
        //    string configDBConnString = this.GetConfigDBConnString();
        //    string str2 = string.Format("SELECT [UserConfigGuid]\r\n                                              ,[UserGuid]\r\n                                              ,[SystemConfigItemGuid]\r\n                                              ,[ResetDatetime]\r\n                                              ,[IsActiveUserConfig]\r\n                                              ,[EnvironmentGuid]\r\n                                              ,[UserConfigName]\r\n                                              ,[UserConfigValue]\r\n                                              ,[EnvironmentName]\r\n                                              ,[IsActiveEnvironment]\r\n                                              ,[ConfigItemName]\r\n                                              ,[ConfigItemDesc]\r\n                                              ,[ConfigItemType]\r\n                                              ,[ConfigItemStatus]\r\n                                              ,[IsEncrypt]\r\n                                              ,[IsGlobal]\r\n                                              ,[IsReadOnly]\r\n                                              ,[ApplicationName]\r\n                                              ,[IsActiveApplication]\r\n                                          FROM [View_Plat_cfg_UserConfig]\r\n                                          WHERE [IsActiveUserConfig] = 1\r\n                                          AND [IsActiveApplication] = 1\r\n                                          AND [IsActiveEnvironment] = 1\r\n                                          AND [ConfigItemStatus] = 1\r\n                                          AND [UserGuid] = '{0}'\r\n                                          AND [ApplicationName] = '{1}'\r\n                                          AND [EnvironmentName] = '{2}'\r\n                                          AND [ConfigItemName] = '{3}'\r\n                                          order by [ConfigItemName]", new object[] { userGuid, applicationName, environment, configItemName });
        //    SqlConnection connection = new SqlConnection(configDBConnString);
        //    try
        //    {
        //        SqlCommand selectCommand = new SqlCommand
        //        {
        //            Connection = connection,
        //            CommandType = CommandType.Text,
        //            CommandText = str2
        //        };
        //        connection.Open();
        //        DataSet dataSet = new DataSet();
        //        new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
        //        connection.Close();
        //        table = dataSet.Tables["tb1"];
        //    }
        //    catch (Exception exception)
        //    {
        //        if (connection.State != ConnectionState.Closed)
        //        {
        //            connection.Close();
        //        }
        //        throw new Exception(exception.Message);
        //    }
        //    return table;
        //}

        //[WebMethod]
        //public DataTable GetUserConfigByUser(Guid userGuid, string applicationName, string environment)
        //{
        //    return this.GetUserConfig(userGuid, applicationName, environment);
        //}

        //[WebMethod]
        //public void RestartCache()
        //{
        //    if (HttpRuntime.Cache[this._systemCacheKey] != null)
        //    {
        //        HttpRuntime.Cache.Remove(this._systemCacheKey);
        //        this.GetAllSystemConfig();
        //    }
        //    if (HttpRuntime.Cache[this._resourceCacheKey] != null)
        //    {
        //        HttpRuntime.Cache.Remove(this._resourceCacheKey);
        //        this.GetAllResourceCulture();
        //    }
        //}

        //[WebMethod]
        //public void RestartConfigCache()
        //{
        //    if (HttpRuntime.Cache[this._systemCacheKey] != null)
        //    {
        //        HttpRuntime.Cache.Remove(this._systemCacheKey);
        //        this.GetAllSystemConfig();
        //    }
        //}

        //[WebMethod]
        //public void RestartResourceCache()
        //{
        //    if (HttpRuntime.Cache[this._resourceCacheKey] != null)
        //    {
        //        HttpRuntime.Cache.Remove(this._resourceCacheKey);
        //        this.GetAllResourceCulture();
        //    }
        //}

        //[WebMethod]
        //public bool RestoreConfigVersion(Guid configVersionGuid)
        //{
        //    bool flag;
        //    SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
        //    try
        //    {
        //        string str2 = string.Format("declare @ApplicationGuid uniqueidentifier\r\n                                                declare @EnvironmentGuid uniqueidentifier\r\n\r\n                                                select @ApplicationGuid = ApplicationGuid,@EnvironmentGuid = EnvironmentGuid\r\n                                                from dbo.Plat_cfg_SystemConfigValueVersion where ConfigVersionGuid = '{0}'\r\n\r\n                                                delete from dbo.Plat_cfg_SystemConfigValue\r\n                                                from dbo.Plat_cfg_SystemConfigValue v\r\n                                                inner join dbo.Plat_cfg_SystemConfigItem i on v.ConfigItemGuid = i.ConfigItemGuid\r\n                                                where i.ApplicationGuid = @ApplicationGuid\r\n                                                and v.EnvironmentGuid = @EnvironmentGuid\r\n\r\n                                                INSERT INTO [dbo].[Plat_cfg_SystemConfigValue]\r\n                                                           ([ConfigValueGuid]\r\n                                                           ,[ConfigItemGuid]\r\n                                                           ,[ConfigOptionGuid]\r\n                                                           ,[EnvironmentGuid]\r\n                                                           ,[ValueName]\r\n                                                           ,[ConfigValue]\r\n                                                           ,[ValueDesc]\r\n                                                           ,[IsSelected]\r\n                                                           ,[ResetDatetime]\r\n                                                           ,[ConfigValueSequence]\r\n                                                           ,[IsActive]\r\n                                                           ,[MyMark])\r\n                                                select newid(),\r\n\t                                                ConfigItemGuid,\r\n\t                                                ConfigOptionGuid,\r\n\t                                                EnvironmentGuid,\r\n\t                                                ValueName,\r\n\t                                                ConfigValue,\r\n\t                                                ValueDesc,\r\n\t                                                IsSelected,\r\n                                                    ResetDatetime,\r\n\t                                                ConfigValueSequence,\r\n\t                                                IsActive,\r\n                                                    0\r\n                                                from dbo.Plat_cfg_SystemConfigValueVersionDetail\r\n                                                where ConfigVersionGuid = '{0}'", configVersionGuid);
        //        SqlCommand command = new SqlCommand
        //        {
        //            Connection = connection,
        //            CommandType = CommandType.Text,
        //            CommandText = str2
        //        };
        //        connection.Open();
        //        command.ExecuteNonQuery();
        //        connection.Close();
        //        flag = true;
        //    }
        //    catch (Exception exception)
        //    {
        //        if (connection.State != ConnectionState.Closed)
        //        {
        //            connection.Close();
        //        }
        //        throw new Exception(exception.Message);
        //    }
        //    return flag;
        //}

        //[WebMethod]
        //public bool SaveResourceCultureByKey(string applicationName, string environmentName, string cultureCode, string resourceHost, string resourceKey, string resourceValue, string resourceLocation, string resourceSize)
        //{
        //    bool flag;
        //    SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
        //    try
        //    {
        //        string str2 = string.Format("declare @ApplicationGuid uniqueidentifier,\r\n\t                                                     @EnvironmentGuid uniqueidentifier\r\n\t    \r\n                                                select @ApplicationGuid = ApplicationGuid from Plat_cfg_Application where ApplicationName = '{0}'\r\n                                                select @EnvironmentGuid = EnvironmentGuid from Plat_cfg_Environment where EnvironmentName = '{1}'\r\n\r\n                                                delete from Plat_cfg_CultureResource\r\n                                                from Plat_cfg_CultureResource c\r\n                                                inner join Plat_cfg_Application a on c.ApplicationGuid = a.ApplicationGuid\r\n                                                inner join Plat_cfg_Environment e on c.EnvironmentGuid = e.EnvironmentGuid\r\n                                                where a.ApplicationName = '{0}'\r\n                                                and e.EnvironmentName = '{1}'\r\n                                                and c.CultureCode = '{2}'\r\n                                                and c.ResourceHost = '{3}'\r\n                                                and c.ResourceKey = '{4}'\r\n\r\n                                                INSERT INTO [Plat_cfg_CultureResource]\r\n                                                       ([ResourceGuid]\r\n                                                       ,[ApplicationGuid]\r\n                                                       ,[EnvironmentGuid]\r\n                                                       ,[ResourceHost]\r\n                                                       ,[ResourceKey]\r\n                                                       ,[CultureCode]\r\n                                                       ,[ResourceValue]\r\n                                                       ,[ResourceLocation]\r\n                                                       ,[ResourceSize]\r\n                                                       ,[IsActive]\r\n                                                       ,[MyMark])\r\n                                                 VALUES\r\n                                                       (newid()\r\n                                                       ,@ApplicationGuid\r\n                                                       ,@EnvironmentGuid\r\n                                                       ,'{3}'\r\n                                                       ,'{4}'\r\n                                                       ,'{2}'\r\n                                                       ,'{5}'\r\n                                                       ,'{6}'\r\n                                                       ,'{7}'\r\n                                                       ,1\r\n                                                       ,0)", new object[] { applicationName, environmentName, cultureCode, resourceHost, resourceKey, resourceValue, resourceLocation, resourceSize });
        //        SqlCommand command = new SqlCommand
        //        {
        //            Connection = connection,
        //            CommandType = CommandType.Text,
        //            CommandText = str2
        //        };
        //        connection.Open();
        //        command.ExecuteNonQuery();
        //        connection.Close();
        //        flag = true;
        //    }
        //    catch (Exception exception)
        //    {
        //        if (connection.State != ConnectionState.Closed)
        //        {
        //            connection.Close();
        //        }
        //        throw new Exception(exception.Message);
        //    }
        //    return flag;
        //}

        //[WebMethod]
        //public DataTable SearchConfigItems(Guid applicationGuid, string configItemName, string configItemDesc, string configItemType, string configItemStatus, bool? isEncrypt, bool? isGlobal, bool? isReadOnly)
        //{
        //    DataTable table = new DataTable();
        //    SqlConnection connection = new SqlConnection(this.GetConfigDBConnString());
        //    try
        //    {
        //        string str2 = " select * from dbo.Plat_cfg_SystemConfigItem where (1 = 1) ";
        //        if (applicationGuid != Guid.Empty)
        //        {
        //            str2 = str2 + string.Format(" and ApplicationGuid = '{0}'", applicationGuid);
        //        }
        //        if (!string.IsNullOrEmpty(configItemName))
        //        {
        //            str2 = str2 + string.Format(" and ConfigItemName like '%{0}%'", configItemName);
        //        }
        //        if (!string.IsNullOrEmpty(configItemDesc))
        //        {
        //            str2 = str2 + string.Format(" and ConfigItemDesc like '%{0}%'", configItemDesc);
        //        }
        //        if (configItemType != "-1")
        //        {
        //            str2 = str2 + string.Format(" and ConfigItemType = '{0}'", configItemType);
        //        }
        //        if (configItemStatus != "-1")
        //        {
        //            str2 = str2 + string.Format(" and ConfigItemStatus = '{0}'", configItemStatus);
        //        }
        //        if (isEncrypt.HasValue)
        //        {
        //            str2 = str2 + string.Format(" and IsEncrypt = '{0}'", isEncrypt.Value);
        //        }
        //        if (isGlobal.HasValue)
        //        {
        //            str2 = str2 + string.Format(" and IsGlobal = '{0}'", isGlobal.Value);
        //        }
        //        if (isReadOnly.HasValue)
        //        {
        //            str2 = str2 + string.Format(" and IsReadOnly = '{0}'", isReadOnly.Value);
        //        }
        //        str2 = str2 + " order by ConfigItemName asc ";
        //        SqlCommand selectCommand = new SqlCommand
        //        {
        //            Connection = connection,
        //            CommandType = CommandType.Text,
        //            CommandText = str2
        //        };
        //        connection.Open();
        //        DataSet dataSet = new DataSet();
        //        new SqlDataAdapter(selectCommand).Fill(dataSet, "tb1");
        //        connection.Close();
        //        table = dataSet.Tables["tb1"];
        //    }
        //    catch (Exception exception)
        //    {
        //        if (connection.State != ConnectionState.Closed)
        //        {
        //            connection.Close();
        //        }
        //        throw new Exception(exception.Message);
        //    }
        //    return table;
        //}

    }
}

