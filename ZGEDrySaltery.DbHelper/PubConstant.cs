using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
namespace ZGEDrySaltery.DbHelper
{
    
    public class PubConstant
    {        
        /// <summary>
        /// 获取连接字符串
        /// </summary>
        public static string ConnectionString
        {           
            get 
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                string str = System.AppDomain.CurrentDomain.BaseDirectory;// @"D:\CampusApi\SmartCampusAPI\";
                string[] split = str.TrimEnd('\\').Split('\\');
                List<string> lstSplit = split.ToList();
                lstSplit.RemoveAt(lstSplit.Count - 1);
                //string joinSplit = string.Join("\\", lstSplit);
                string siteConfigFile = lstSplit + "\\Web.config";
                map.ExeConfigFilename = siteConfigFile;//@"D:/ConfigFile.config"; 
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);               
                string _connectionString = config.AppSettings.Settings["ConnectionString"].Value;     
                string ConStringEncrypt = config.AppSettings.Settings["ConStringEncrypt"].Value;
                if (ConStringEncrypt == "true")
                {
                    _connectionString = DESEncrypt.Decrypt(_connectionString);
                }
                return _connectionString; 
            }
        }

        /// <summary>
        /// 得到web.config里配置项的数据库连接字符串。
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static string GetConnectionString(string configName)
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            string str = System.AppDomain.CurrentDomain.BaseDirectory;// @"D:\CampusApi\SmartCampusAPI\";
            string[] split = str.TrimEnd('\\').Split('\\');
            List<string> lstSplit = split.ToList();
            lstSplit.RemoveAt(lstSplit.Count - 1);
            string joinSplit = string.Join("\\", lstSplit);
            string siteConfigFile = joinSplit + "\\Web.config";
            map.ExeConfigFilename = siteConfigFile;//@"D:/ConfigFile.config"; 
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            string _connectionString = config.AppSettings.Settings["ConnectionString"].Value;
            string ConStringEncrypt = config.AppSettings.Settings["ConStringEncrypt"].Value;
            if (ConStringEncrypt == "true")
            {
                _connectionString = DESEncrypt.Decrypt(_connectionString);
            }
            return _connectionString;           
        }
       
    }
}
