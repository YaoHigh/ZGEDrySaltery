using System;
using System.Web.Security;
using ZGEDrySaltery.Code;
using ZGEDrySaltery.Model;

namespace ZGEDrySaltery.BLL
{
    public class SUserBLL
    {
        /// <summary>
        /// 写日志
        /// </summary>
        private Log log;

        #region 单例模式
        private static SUserBLL instance;

        public static SUserBLL GetInstance()
        {
            if (instance == null)
            {
                lock (typeof(SUserBLL))
                {
                    if (instance == null)
                    {
                        instance = new SUserBLL();
                    }
                }
            }
            return instance;
        }
        private SUserBLL()
        {
            log = LogFactory.GetLogger(this.GetType().ToString());
        }

        #endregion

        #region 检测登录用户信息
        /// <summary>
        /// 检测登录用户信息
        /// </summary>
        public S_USER CheckLogin(string account, string password)
        {
            try
            {
                string pwd = FormsAuthentication.HashPasswordForStoringInConfigFile(password, "MD5");
                S_USER suser = DAL.SUserDAL.GetInstance().CheckLogin(account, pwd);
                return suser;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return null;
        }
        #endregion
    }
}
