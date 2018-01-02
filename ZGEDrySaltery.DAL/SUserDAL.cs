using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZGEDrySaltery.DbHelper;
using ZGEDrySaltery.Model;

namespace ZGEDrySaltery.DAL
{
    public class SUserDAL
    {
        #region 单例模式
        private static SUserDAL instance;

        public static SUserDAL GetInstance()
        {
            if (instance == null)
            {
                lock (typeof(SUserDAL))
                {
                    if (instance == null)
                    {
                        instance = new SUserDAL();
                    }
                }
            }
            return instance;
        }
        private SUserDAL()
        {
        }

        #endregion

        #region 检测登录用户信息
        /// <summary>
        /// 检测登录用户信息
        /// </summary>
        public S_USER CheckLogin(string account, string password)
        {
            string sqlValue = @"SELECT * FROM S_USER WHERE TEL=@ACCOUNT AND USER_PSD =@PASSWORD
                                UNION ALL SELECT * FROM S_USER WHERE REAL_NAME=@ACCOUNT AND USER_PSD =@PASSWORD
                                UNION ALL SELECT * FROM S_USER WHERE USER_NAME=@ACCOUNT AND USER_PSD =@PASSWORD ";
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.Add(new MySqlParameter("@ACCOUNT", account));
            parameters.Add(new MySqlParameter("@PASSWORD", password));

            S_USER model = new S_USER();
            try
            {
                using (IDataReader reader = DbHelperMySQL.ExecuteReader(sqlValue, parameters.ToArray()))
                {
                    while (reader.Read())
                    {
                        model = DataReaderToModel(reader);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return model;

        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public S_USER DataReaderToModel(IDataReader row)
        {
            S_USER model = new S_USER();
            if (row != null)
            {
                if (row["USER_ID"] != null && row["USER_ID"].ToString() != "")
                {
                    model.USER_ID = int.Parse(row["USER_ID"].ToString());
                }
                if (row["ORG_ID"] != null && row["ORG_ID"].ToString() != "")
                {
                    model.ORG_ID = int.Parse(row["ORG_ID"].ToString());
                }
                if (row["USER_NAME"] != null)
                {
                    model.USER_NAME = row["USER_NAME"].ToString();
                }
                if (row["REAL_NAME"] != null)
                {
                    model.REAL_NAME = row["REAL_NAME"].ToString();
                }
                if (row["SEX"] != null)
                {
                    model.SEX = row["SEX"].ToString();
                }
                if (row["ID_CARD"] != null)
                {
                    model.ID_CARD = row["ID_CARD"].ToString();
                }
                if (row["USER_PSD"] != null)
                {
                    model.USER_PSD = row["USER_PSD"].ToString();
                }
                if (row["OPEN_ID"] != null)
                {
                    model.OPEN_ID = row["OPEN_ID"].ToString();
                }
                if (row["TEL"] != null)
                {
                    model.TEL = row["TEL"].ToString();
                }
                if (row["EMAIL"] != null)
                {
                    model.EMAIL = row["EMAIL"].ToString();
                }
                if (row["IMAGE_PATH"] != null)
                {
                    model.IMAGE_PATH = row["IMAGE_PATH"].ToString();
                }
                if (row["LAST_ONLINE_TIME"] != null && row["LAST_ONLINE_TIME"].ToString() != "")
                {
                    model.LAST_ONLINE_TIME = DateTime.Parse(row["LAST_ONLINE_TIME"].ToString());
                }
                if (row["LOGIN_TIMES"] != null && row["LOGIN_TIMES"].ToString() != "")
                {
                    model.LOGIN_TIMES = int.Parse(row["LOGIN_TIMES"].ToString());
                }
                if (row["ENABLE_FLAG"] != null)
                {
                    model.ENABLE_FLAG = row["ENABLE_FLAG"].ToString();
                }
                if (row["CREATE_USER_ID"] != null)
                {
                    model.CREATE_USER_ID = row["CREATE_USER_ID"].ToString();
                }
                if (row["CREATE_TIME"] != null && row["CREATE_TIME"].ToString() != "")
                {
                    model.CREATE_TIME = DateTime.Parse(row["CREATE_TIME"].ToString());
                }
                if (row["UPDATE_USER_ID"] != null)
                {
                    model.UPDATE_USER_ID = row["UPDATE_USER_ID"].ToString();
                }
                if (row["UPDATE_TIME"] != null && row["UPDATE_TIME"].ToString() != "")
                {
                    model.UPDATE_TIME = DateTime.Parse(row["UPDATE_TIME"].ToString());
                }
                if (row["REMARK"] != null)
                {
                    model.REMARK = row["REMARK"].ToString();
                }
                if (row["TENANT_ID"] != null && row["TENANT_ID"].ToString() != "")
                {
                    model.TENANT_ID = int.Parse(row["TENANT_ID"].ToString());
                }
            }
            return model;
        }
        #endregion
    }
}
