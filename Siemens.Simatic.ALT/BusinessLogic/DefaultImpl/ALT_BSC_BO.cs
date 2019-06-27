
using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Simatic.Platform.Core;
using Siemens.Simatic.Platform.Common;
using Siemens.Simatic.Platform.Common.ExceptionHandling;
using Siemens.Simatic.Platform.Data;

using Siemens.Simatic.ALT.Common;
using Siemens.Simatic.ALT.DataAccess;
using System.Data;
using System.Collections;
using Siemens.Simatic.Util.Utilities;
//using Siemens.MES.Public;
using System.Reflection;
using Siemens.Simatic.Util.Utilities.DAO;
using Siemens.Simatic.Platform.Data.DataAccess;


namespace Siemens.Simatic.ALT.BusinessLogic
{
    [DefaultImplementationAttreibute(typeof(DefaultImpl.ALT_BSC_BO))]
    public partial interface IALT_BSC_BO
    {
        DataTable GetDataTableBySql(string sql);

        void ExecuteNonQueryBySql(string sql);

        DataTable GetDataTableByStoredProcedure(string storedProcedureName);

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="qp">对象List</param>
        /// <param name="tablename">插入的表名</param>
        /// <returns></returns>
        string ExcuteSqlTranction(object objList, string tablename);

        /// <summary>
        /// 把DataTable插入数据库
        /// </summary>
        /// <param name="dt">DataTable数据</param>
        /// <param name="tablename">插入的表名</param>
        /// <returns></returns>
        string ExcuteDataTableToDB(DataTable dt, string tablename);


        //string ExecuteProcedureWithParamList(string StoredProcedureName, List<ProcModel> paramList);

        //string ExecuteProcedureWithParam(string StoredProcedureName, Dictionary<string, object> paramDic);

        string InitServer();

        //bool ExecuteNotifyAll();

        /// <summary>
        /// 发送预警
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool ExecuteNotify(PM_ALT_MESSAGE msg);

        /// <summary>
        /// 发送预警
        /// Aux接口
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool ExecuteNotifyAux(PM_ALT_MESSAGE msg);

    }
}


namespace Siemens.Simatic.ALT.BusinessLogic.DefaultImpl
{
    public partial class ALT_BSC_BO : IALT_BSC_BO
    {
        private IALT_BSC_DAO _ALT_BSC_DAO;

        public ALT_BSC_BO()
        {
            _ALT_BSC_DAO = ObjectContainer.BuildUp<IALT_BSC_DAO>();
        }

        public DataTable GetDataTableBySql(string sql)
        {
            return _ALT_BSC_DAO.GetDataTableBySql(sql);
        }

        public void ExecuteNonQueryBySql(string sql)
        {
            _ALT_BSC_DAO.ExecuteNonQueryBySql(sql);
        }

        public DataTable GetDataTableByStoredProcedure(string storedProcedureName)
        {
            return _ALT_BSC_DAO.GetDataTableByStoredProcedure(storedProcedureName);
        }

        /// </summary>
        /// 把对象集合批量插入数据库
        /// 王浩田-2017年12月16日14:22:02
        /// </summary>
        /// <param name="qp">对象List</param>
        /// <param name="tablename">插入的表名</param>
        /// <returns>插入结果</returns>
        public string ExcuteSqlTranction(object objList, string tablename)
        {
            //这里有两种方法，一种，将大批量插入写入D层，解耦，同时使用实例化的DB连接串，不向外暴露，但是，一旦DAO层被重新生成，则更改起来较为麻烦
            //另一种，使用连接字符串，并进行加密，加密秘钥暂定"connKey"，此方法方便写入APP.config，更换数据库方便
            Database db = DatabaseFactory.CreateDatabase(DatabaseEnum.SITBusinessDB);
            string conn = db.ConnectionString;

            DataTable dt = new DataTable();
            try
            {
                IList temp = (IList)objList;
                //将泛型转化成DataTable
                dt = ToDataTableTow(temp);

                //使用秘钥进行加密,秘钥可以分发给专门的人进行记录，在更改数据时留出输入秘钥的接口
                conn = DESEncrypt.Encrypt(conn, "connKey");                
                //输入秘钥进行解密
                string finalConn = DESEncrypt.Decrypt(conn, "connKey");

                UtilDAO.InsertEntities(finalConn, tablename, dt);
                return "OK";
            }
            catch (Exception ex)
            {
                return "NG,批量插入失败:" + ex;
                //throw ex;
            }
        }

        /// <summary>
        /// 把DataTable插入数据库
        /// </summary>
        /// <param name="dt">DataTable数据</param>
        /// <param name="tablename">插入的表名</param>
        /// <returns></returns>
        public string ExcuteDataTableToDB(DataTable dt, string tablename)
        {
            //这里有两种方法，一种，将大批量插入写入D层，解耦，同时使用实例化的DB连接串，不向外暴露，但是，一旦DAO层被重新生成，则更改起来较为麻烦
            //另一种，使用连接字符串，并进行加密，加密秘钥暂定"connKey"，此方法方便写入APP.config，更换数据库方便
            Database db = DatabaseFactory.CreateDatabase(DatabaseEnum.SITBusinessDB);
            string conn = db.ConnectionString;
            
            try
            {
                //使用秘钥进行加密,秘钥可以分发给专门的人进行记录，在更改数据时留出输入秘钥的接口
                conn = DESEncrypt.Encrypt(conn, "connKey");
                //输入秘钥进行解密
                string finalConn = DESEncrypt.Decrypt(conn, "connKey");

                UtilDAO.InsertEntities(finalConn, tablename, dt);
                return "OK";
            }
            catch (Exception ex)
            {
                return "NG,插入失败:" + ex;
                //throw ex;
            }
        }

        /// <summary>
        /// 将泛型转化成DataTable
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static DataTable ToDataTableTow(IList list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();

                foreach (PropertyInfo pi in propertys)
                {
                    result.Columns.Add(pi.Name);//pi.PropertyType
                }
                foreach (object t in list)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        object obj = pi.GetValue(t, null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
   

    }
}