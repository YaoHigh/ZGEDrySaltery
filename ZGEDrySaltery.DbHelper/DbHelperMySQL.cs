using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;
using System.Text;
namespace ZGEDrySaltery.DbHelper
{
    /// <summary>
    /// ���ݷ��ʳ��������
    /// Copyright (C) XHD
    /// </summary>
    public abstract class DbHelperMySQL
    {
        //���ݿ������ַ���(web.config������)�����Զ�̬����connectionString֧�ֶ����ݿ�.		
        public static string connectionString = PubConstant.ConnectionString;
        public DbHelperMySQL()
        {            
        }

        #region ���÷���
        /// <summary>
        /// �õ����ֵ
        /// </summary>
        /// <param name="FieldName"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }
        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public static bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }    
        /// <summary>
        /// �Ƿ���ڣ�����MySqlParameter��
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="cmdParms"></param>
        /// <returns></returns>
        public static bool Exists(string strSql, params MySqlParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region  ִ�м�SQL���

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="SQLString">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public static int ExecuteSql(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch
                    {
                        connection.Close();
                        throw ;
                    }
                }
            }
        }

        public static int ExecuteSqlByTime(string SQLString, int Times)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch
                    {
                        connection.Close();
                        throw ;
                    }
                }
            }
        }
      
        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ����� ÿ500���������һ������
        /// </summary>
        /// <param name="SQLStringList">����SQL���</param>		
        public static int ExecuteSqlTran(List<String> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                        //�������ϵ� ÿ500������һ��
                       if (n > 0 && (n % 500 == 0 || n == SQLStringList.Count - 1))
                        {
                            tx.Commit();
                            tx = conn.BeginTransaction();
                        }
                    }
                    //tx.Commit();
                    return count;
                }
                catch
                {
                    tx.Rollback();
                    return 0;
                }        
            }
        }

        /// <summary>
        /// MySql�������룬ʵ�����ݿ����� ÿ500���������һ������
        /// 20170706++
        /// insert into table (f1,f2) values(1,'sss'),(2,'bbbb'),(3,'cccc')
        /// </summary>
        /// <param name="sqlHeader">����ͷ�� insert into table (f1,f2)</param>
        /// <param name="sqlstringlist">����SQL (1,'sss')</param>
        /// <returns></returns>
	
        public static int ExecuteSqlTranGroup(string sqlHeader, List<string> sqlstringlist)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    StringBuilder sqlBuilder = new StringBuilder(sqlHeader + " VALUES ");
                    for (int n = 0; n < sqlstringlist.Count; n++)
                    {
                        string strsql = sqlstringlist[n];
                        if (strsql.Trim().Length > 1)
                        {
                            sqlBuilder.Append(strsql + ",");
                        }
                        //�������ϵ� ÿ500������һ��
                        if (n % 500 == 0 || n == sqlstringlist.Count - 1)
                        {
                            cmd.CommandText = sqlBuilder.ToString().Substring(0, sqlBuilder.Length - 1);
                            count += cmd.ExecuteNonQuery();
                            tx.Commit();
                            tx = conn.BeginTransaction();
                            sqlBuilder.Clear();//���
                            sqlBuilder.Append(sqlHeader + " VALUES ");//�ڼ���insert ͷ
                        }
                    }
                    //tx.Commit();
                    return count;
                }
                catch
                {
                    tx.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// ִ�д�һ���洢���̲����ĵ�SQL��䡣
        /// </summary>
        /// <param name="SQLString">SQL���</param>
        /// <param name="content">��������,����һ���ֶ��Ǹ�ʽ���ӵ����£���������ţ�����ͨ�������ʽ���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public static int ExecuteSql(string SQLString, string content)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(SQLString, connection);
                MySql.Data.MySqlClient.MySqlParameter myParameter = new MySql.Data.MySqlClient.MySqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch
                {
                    throw ;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// ִ�д�һ���洢���̲����ĵ�SQL��䡣
        /// </summary>
        /// <param name="SQLString">SQL���</param>
        /// <param name="content">��������,����һ���ֶ��Ǹ�ʽ���ӵ����£���������ţ�����ͨ�������ʽ���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public static object ExecuteSqlGet(string SQLString, string content)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(SQLString, connection);
                MySql.Data.MySqlClient.MySqlParameter myParameter = new MySql.Data.MySqlClient.MySqlParameter("@content", SqlDbType.NText);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    object obj = cmd.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch
                {
                    throw ;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// �����ݿ������ͼ���ʽ���ֶ�(������������Ƶ���һ��ʵ��)
        /// </summary>
        /// <param name="strSQL">SQL���</param>
        /// <param name="fs">ͼ���ֽ�,���ݿ���ֶ�����Ϊimage�����</param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand(strSQL, connection);
                MySql.Data.MySqlClient.MySqlParameter myParameter = new MySql.Data.MySqlClient.MySqlParameter("@fs", SqlDbType.Image);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch 
                {
                    throw ;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="SQLString">�����ѯ������</param>
        /// <returns>��ѯ�����object��</returns>
        public static object GetSingle(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch
                    {
                        connection.Close();
                        throw ;
                    }
                }
            }
        }
        public static object GetSingle(string SQLString, int Times)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch
                    {
                        connection.Close();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// ִ�в�ѯ��䣬����MySqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��MySqlDataReader����Close )
        /// </summary>
        /// <param name="strSQL">��ѯ���</param>
        /// <returns>MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(string strSQL)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(strSQL, connection);
            try
            {
                connection.Open();
                MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch
            {
                throw ;
            }   

        }
        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="SQLString">��ѯ���</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch 
                {
                    throw;
                }
                return ds;
            }
        }
        public static DataSet Query(string SQLString, int Times)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.SelectCommand.CommandTimeout = Times;
                    command.Fill(ds, "ds");
                }
                catch
                {
                    throw ;
                }
                return ds;
            }
        }
        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet �����Ӳ�ͬ���ݿ�
        /// </summary>
        /// <param name="SQLString">��ѯ���</param>
        /// <returns>DataSet</returns>
        public static DataSet Query_DataBase(string SQLString, string ConnectionString)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    MySqlDataAdapter command = new MySqlDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch
                {
                    throw;
                }
                return ds;
            }
        }


        #endregion

        #region ִ�д�������SQL���

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="SQLString">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public static int ExecuteSql(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch 
                    {
                        throw ;
                    }
                }
            }
        }
        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="SQLString">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public static int ExecuteSql_DataBase(string SQLString,string ConnectionString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="SQLStringList">SQL���Ĺ�ϣ��keyΪsql��䣬value�Ǹ�����MySqlParameter[]��</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        //ѭ��
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw ;
                    }
                }
            }
        }
        public static int ExecuteSqlTranReturnInt(System.Collections.Generic.List<CommandInfo> cmdList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int count = 0;
                        //ѭ��
                        foreach (CommandInfo myDE in cmdList)
                        {
                            string cmdText = myDE.CommandText;
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                            {
                                object obj = cmd.ExecuteNonQuery();
                                bool isHave = Convert.ToInt32(obj) > 0;
                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                continue;
                            }
                            int val =Convert.ToInt32(cmd.ExecuteScalar());
                            count += val;
                            if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                            {
                                trans.Rollback();
                                return 0;
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        return count;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw ;
                    }
                }
            }
        }
        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="SQLStringList">SQL���Ĺ�ϣ��keyΪsql��䣬value�Ǹ�����MySqlParameter[]��</param>
        public static int ExecuteSqlTran(System.Collections.Generic.List<CommandInfo> cmdList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    { int count = 0;
                        //ѭ��
                        foreach (CommandInfo myDE in cmdList)
                        {
                            string cmdText = myDE.CommandText;
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                           
                            if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                            {
                                if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                                {
                                    trans.Rollback();
                                    return 0;
                                }

                                object obj = cmd.ExecuteScalar();
                                bool isHave = false;
                                if (obj == null && obj == DBNull.Value)
                                {
                                    isHave = false;
                                }
                                isHave = Convert.ToInt32(obj) > 0;

                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                continue;
                            }
                            int val = cmd.ExecuteNonQuery();
                            count += val;
                            if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                            {
                                trans.Rollback();
                                return 0;
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        return count;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw ;
                    }
                }
            }
        }
        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="SQLStringList">SQL���Ĺ�ϣ��keyΪsql��䣬value�Ǹ�����MySqlParameter[]��</param>
        public static void ExecuteSqlTranWithIndentity(System.Collections.Generic.List<CommandInfo> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int indentity = 0;
                        //ѭ��
                        foreach (CommandInfo myDE in SQLStringList)
                        {
                            string cmdText = myDE.CommandText;
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="SQLStringList">SQL���Ĺ�ϣ��keyΪsql��䣬value�Ǹ�����MySqlParameter[]��</param>
        public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int indentity = 0;
                        //ѭ��
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="SQLString">�����ѯ������</param>
        /// <returns>��ѯ�����object��</returns>
        public static object GetSingle(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// ִ�в�ѯ��䣬����MySqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��MySqlDataReader����Close )
        /// </summary>
        /// <param name="strSQL">��ѯ���</param>
        /// <returns>MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(string SQLString, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch
            {
                throw ;
            }
            //			finally
            //			{
            //				cmd.Dispose();
            //				connection.Close();
            //			}	

        }
        /// <summary>
        /// ִ�в�ѯ��䣬����MySqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��MySqlDataReader����Close )
        /// </summary>
        /// <param name="strSQL">��ѯ���</param>
        /// <returns>MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader_DataBase(string SQLString,string ConnectionString, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch
            {
                throw;
            }
            //			finally
            //			{
            //				cmd.Dispose();
            //				connection.Close();
            //			}	

        }
        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="SQLString">��ѯ���</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch
                    {
                        throw ;
                    }
                    return ds;
                }
            }
        }
        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="SQLString">��ѯ���</param>
        /// <returns>DataSet</returns>
        public static DataSet Query_DataBase(string SQLString, string ConnectionString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch
                    {
                        throw;
                    }
                    return ds;
                }
            }
        }
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
        /// <summary>
        /// ִ�д洢���̣�����Ӱ�������		
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="rowsAffected">Ӱ�������</param>
        /// <returns></returns>
        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                int result;
                connection.Open();
                MySqlCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                return result;
            }
        }

        /// <summary>
        /// ִ�д洢���̣�����out����		
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="rowsAffected">Ӱ�������</param>
        /// <returns></returns>
        public static string RunProcedureReturnValue(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string result;
                connection.Open();
                MySqlCommand command = BuildStringCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = command.Parameters["ReturnValue"].Value.ToString();
                return result;
            }
        }

        /// <summary>
        /// ִ�д洢���� ���ض�����ݼ�
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="tableName">DataSet����еı���</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                MySqlDataAdapter sqlDA = new MySqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// ���� MySqlCommand ����ʵ��(��������һ������ֵ)	
        /// ��ע:mysql��ֻ�з�������ReturnValue,�洢���̲�����,ֻ��Output
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>OracleCommand ����ʵ��</returns>
        private static MySqlCommand BuildIntCommand(MySqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            MySqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new MySqlParameter("ReturnValue",
               MySqlDbType.Int32, 4, ParameterDirection.Output,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        /// <summary>
        /// ���� MySqlCommand ����ʵ��(��������һ��string)	
        /// ��ע:mysql��ֻ�з�������ReturnValue,�洢���̲�����,ֻ��Output
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>OracleCommand ����ʵ��</returns>
        private static MySqlCommand BuildStringCommand(MySqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            MySqlCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.Parameters.Add(new MySqlParameter("ReturnValue",
               MySqlDbType.VarChar, 512, ParameterDirection.Output,
                false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }
        /// <summary>
        /// ���� MySqlCommand ����(��������һ���������������һ������ֵ)
        /// </summary>
        /// <param name="connection">���ݿ�����</param>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>OracleCommand</returns>
        private static MySqlCommand BuildQueryCommand(MySqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            MySqlCommand command = new MySqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (MySqlParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// ��ȡ������ؼ�������List
        /// 20170413+++
        /// ������
        /// </summary>
        /// <param name="StoredName"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public List<DataTable> StroedGetTableList(string StoredName, List<Sqlparameters> Parameters)
        {
            MySqlDataAdapter mysqldata = new MySqlDataAdapter();
            MySqlCommand sqlCommand = new MySqlCommand();
            sqlCommand.CommandText = StoredName;//�洢��������
            sqlCommand.CommandType = CommandType.StoredProcedure;
            MySqlConnection conn = new MySqlConnection(connectionString);
            sqlCommand.Connection = conn;
            for (int i = 0; i < Parameters.Count; i++)
            {
                sqlCommand.Parameters.AddWithValue(Parameters[i].name, Parameters[i].pvalue);
            }
            conn.Open();
            List<DataTable> dts = new List<DataTable>();
            MySqlDataReader mysqlreser = sqlCommand.ExecuteReader();//mysqlreader�޹��캯��
            bool re = true;
            System.Threading.CancellationToken _cts;//����Cancel�õ�
            while (re)
            {
                DataTable dt = new DataTable();

                //public Task<int> FillAsync
                //mysqldata.FillAsync(dt, mysqlreser).Wait(_cts);//�ȴ��߳����
                dts.Add(dt);
                re = mysqlreser.NextResult();//ȡ��һ�������
                //  Trace.WriteLine(dt.Rows.Count);       

            }
            conn.Close();
            return dts;
        }

        public struct Sqlparameters
        {
            public string name;//�洢���̵������ַ�����
            public object pvalue;//�洢���̵��������
            public Sqlparameters(string names, object pvalues)
            {
                name = names;
                pvalue = pvalues;
            }
        }
        #endregion

        ///// <summary>
        ///// �û�_����Ȩ��_���ô洢���� 
        ///// </summary>
        ///// <param name="procedureName">�洢��������</param>
        ///// <param name="lststring">�洢�����������</param>
        ///// <returns></returns>
        //public static int ExecuteSPUserFunction(string procedureName, List<string> lststring)
        //{
        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
        //        {
        //            try
        //            {
        //                connection.Open();
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                cmd.Parameters.Add("?f_userid", MySqlDbType.Int32);
        //                cmd.Parameters["?f_userid"].Value = int.Parse(lststring[0]);
        //                cmd.Parameters.Add("?f_systemid", MySqlDbType.Int32);
        //                cmd.Parameters["?f_systemid"].Value = int.Parse(lststring[1]);
        //                cmd.Parameters.Add("?f_ids", MySqlDbType.VarChar, 1024);
        //                cmd.Parameters["?f_ids"].Value = lststring[2];
        //                cmd.Parameters.Add("?f_delimiter", MySqlDbType.VarChar, 5);
        //                cmd.Parameters["?f_delimiter"].Value = lststring[3];
        //                cmd.Parameters.Add("?f_show_type", MySqlDbType.Int32);
        //                cmd.Parameters["?f_show_type"].Value = int.Parse(lststring[4]);
        //                cmd.Parameters.Add("?f_menuids", MySqlDbType.VarChar, 1024);
        //                cmd.Parameters["?f_menuids"].Value = lststring[5];
        //                int rows = Convert.ToInt32(cmd.ExecuteScalar());
        //                //if (rows > 0)
        //                //{
        //                //    // ������ȷ
        //                //}

        //                return rows;
        //            }
        //            catch (MySql.Data.MySqlClient.MySqlException e)
        //            {
        //                connection.Close();
        //                throw e;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// ��ɫ_����Ȩ��_���ô洢���� 
        ///// </summary>
        ///// <param name="procedureName">�洢��������</param>
        ///// <param name="lststring">�洢�����������</param>
        ///// <returns></returns>
        //public static int ExecuteSPROLEFunction(string procedureName, List<string> lststring)
        //{
        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
        //        {
        //            try
        //            {
        //                connection.Open();
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                cmd.Parameters.Add("?roleid", MySqlDbType.Int32);
        //                cmd.Parameters["?roleid"].Value = int.Parse(lststring[0]);
        //                cmd.Parameters.Add("?systemid", MySqlDbType.Int32);
        //                cmd.Parameters["?systemid"].Value = int.Parse(lststring[1]);
        //                cmd.Parameters.Add("?ids", MySqlDbType.VarChar, 1024);
        //                cmd.Parameters["?ids"].Value = lststring[2];
        //                cmd.Parameters.Add("?delimiter", MySqlDbType.VarChar, 5);
        //                cmd.Parameters["?delimiter"].Value = lststring[3];
        //                cmd.Parameters.Add("?f_show_type", MySqlDbType.Int32);
        //                cmd.Parameters["?f_show_type"].Value = int.Parse(lststring[4]);
        //                cmd.Parameters.Add("?f_menuids", MySqlDbType.VarChar, 1024);
        //                cmd.Parameters["?f_menuids"].Value = lststring[5];
        //                int rows = Convert.ToInt32(cmd.ExecuteScalar());
        //                //if (rows > 0)
        //                //{
        //                //    // ������ȷ
        //                //}

        //                return rows;
        //            }
        //            catch (MySql.Data.MySqlClient.MySqlException e)
        //            {
        //                connection.Close();
        //                throw e;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// �û���_����Ȩ��_���ô洢���� 
        ///// </summary>
        ///// <param name="procedureName">�洢��������</param>
        ///// <param name="lststring">�洢�����������</param>
        ///// <returns></returns>
        //public static int ExecuteSPGroupFunction(string procedureName, List<string> lststring)
        //{
        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        using (MySqlCommand cmd = new MySqlCommand(procedureName, connection))
        //        {
        //            try
        //            {
        //                connection.Open();
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                cmd.Parameters.Add("?groupid", MySqlDbType.Int32);
        //                cmd.Parameters["?groupid"].Value = int.Parse(lststring[0]);
        //                cmd.Parameters.Add("?systemid", MySqlDbType.Int32);
        //                cmd.Parameters["?systemid"].Value = int.Parse(lststring[1]);
        //                cmd.Parameters.Add("?ids", MySqlDbType.VarChar, 1024);
        //                cmd.Parameters["?ids"].Value = lststring[2];
        //                cmd.Parameters.Add("?delimiter", MySqlDbType.VarChar, 5);
        //                cmd.Parameters["?delimiter"].Value = lststring[3];
        //                cmd.Parameters.Add("?f_show_type", MySqlDbType.Int32);
        //                cmd.Parameters["?f_show_type"].Value = int.Parse(lststring[4]);
        //                cmd.Parameters.Add("?f_menuids", MySqlDbType.VarChar, 1024);
        //                cmd.Parameters["?f_menuids"].Value = lststring[5];
        //                int rows = Convert.ToInt32(cmd.ExecuteScalar());
        //                //if (rows > 0)
        //                //{
        //                //    // ������ȷ
        //                //}

        //                return rows;
        //            }
        //            catch (MySql.Data.MySqlClient.MySqlException e)
        //            {
        //                connection.Close();
        //                throw e;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// �û���_������Ա_���ô洢���� 
        ///// </summary>
        ///// <param name="procedureName">�洢��������</param>
        ///// <param name="lststring">�洢�����������</param>
        ///// <returns></returns>
        //public static int ExecuteUserGroupFunction(List<string> lststring)
        //{
        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        using (MySqlCommand cmd = new MySqlCommand("sp_save_user_group", connection))
        //        {
        //            try
        //            {
        //                connection.Open();
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                cmd.Parameters.Add("?groupid", MySqlDbType.Int32);
        //                cmd.Parameters["?groupid"].Value = int.Parse(lststring[0]);
        //                cmd.Parameters.Add("?uids", MySqlDbType.VarChar, 1000);
        //                cmd.Parameters["?uids"].Value = lststring[1];
        //                cmd.Parameters.Add("?f_delimiter", MySqlDbType.VarChar, 5);
        //                cmd.Parameters["?f_delimiter"].Value = lststring[2];
        //                int rows = Convert.ToInt32(cmd.ExecuteScalar());
        //                //if (rows > 0)
        //                //{
        //                //    // ������ȷ
        //                //}

        //                return rows;
        //            }
        //            catch (MySql.Data.MySqlClient.MySqlException e)
        //            {
        //                connection.Close();
        //                throw e;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// ��ɫ_������Ա_���ô洢���� 
        ///// </summary>
        ///// <param name="procedureName">�洢��������</param>
        ///// <param name="lststring">�洢�����������</param>
        ///// <returns></returns>
        //public static int ExecuteUserRoleFunction(List<string> lststring)
        //{
        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        using (MySqlCommand cmd = new MySqlCommand("sp_save_user_role", connection))
        //        {
        //            try
        //            {
        //                connection.Open();
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                cmd.Parameters.Add("?roleid", MySqlDbType.Int32);
        //                cmd.Parameters["?roleid"].Value = int.Parse(lststring[0]);
        //                cmd.Parameters.Add("?uids", MySqlDbType.VarChar, 1000);
        //                cmd.Parameters["?uids"].Value = lststring[1];
        //                cmd.Parameters.Add("?f_delimiter", MySqlDbType.VarChar, 5);
        //                cmd.Parameters["?f_delimiter"].Value = lststring[2];
        //                int rows = Convert.ToInt32(cmd.ExecuteScalar());
        //                //if (rows > 0)
        //                //{
        //                //    // ������ȷ
        //                //}

        //                return rows;
        //            }
        //            catch (MySql.Data.MySqlClient.MySqlException e)
        //            {
        //                connection.Close();
        //                throw e;
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// �������_�������_��������_���ô洢���� 
        ///// </summary>
        ///// <param name="procedureName">�洢��������</param>
        ///// <param name="lststring">�洢�����������</param>
        ///// <returns></returns>
        //public static int ExecuteHealthRatFunction(List<string> lststring)
        //{
        //    using (MySqlConnection connection = new MySqlConnection(connectionString))
        //    {
        //        using (MySqlCommand cmd = new MySqlCommand("p_dhealth_rating", connection))
        //        {
        //            try
        //            {
        //                connection.Open();
        //                cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //                cmd.Parameters.Add("?d_lot_nbr", MySqlDbType.VarChar, 16);
        //                cmd.Parameters["?d_lot_nbr"].Value = lststring[0];
        //                cmd.Parameters.Add("?ids", MySqlDbType.VarChar, 100);
        //                cmd.Parameters["?ids"].Value = lststring[1];
        //                cmd.Parameters.Add("?d_user_id", MySqlDbType.VarChar, 10);
        //                cmd.Parameters["?d_user_id"].Value = lststring[2];
        //                cmd.Parameters.Add("?d_user_name", MySqlDbType.VarChar, 30);
        //                cmd.Parameters["?d_user_name"].Value = lststring[3];
        //                cmd.Parameters.Add("?tenant_id", MySqlDbType.Int32);
        //                cmd.Parameters["?tenant_id"].Value = lststring[4];
        //                int rows = Convert.ToInt32(cmd.ExecuteScalar());
        //                //if (rows > 0)
        //                //{
        //                //    // ������ȷ
        //                //}

        //                return rows;
        //            }
        //            catch (MySql.Data.MySqlClient.MySqlException e)
        //            {
        //                connection.Close();
        //                throw e;
        //            }
        //        }
        //    }
        //}

    }

}
