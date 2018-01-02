using System;
namespace ZGEDrySaltery.Model
{
	/// <summary>
	/// 用户表
	/// </summary>
	[Serializable]
	public partial class S_USER
	{
		public S_USER()
		{}
        #region Model
        private int _user_id;
        private int? _org_id;
        private string _user_name;
        private string _real_name;
        private string _sex;
        private string _id_card;
        private string _user_psd;
        private string _OPEN_ID;
        private string _tel;
        private string _email;
        private string _image_path;
        private DateTime? _last_online_time;
        private int _login_times;
        private string _enable_flag = "1";
        private string _create_user_id;
        private DateTime _create_time;
        private string _update_user_id;
        private DateTime? _update_time;
        private string _remark;
        private int? _tenant_id;
        /// <summary>
        /// 用户ID
        /// </summary>
        public int USER_ID
        {
            set { _user_id = value; }
            get { return _user_id; }
        }
        /// <summary>
        /// 组织机构ID
        /// </summary>
        public int? ORG_ID
        {
            set { _org_id = value; }
            get { return _org_id; }
        }
        /// <summary>
        /// 用户名
        /// </summary>
        public string USER_NAME
        {
            set { _user_name = value; }
            get { return _user_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string REAL_NAME
        {
            set { _real_name = value; }
            get { return _real_name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SEX
        {
            set { _sex = value; }
            get { return _sex; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ID_CARD
        {
            set { _id_card = value; }
            get { return _id_card; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string USER_PSD
        {
            set { _user_psd = value; }
            get { return _user_psd; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OPEN_ID
        {
            set { _OPEN_ID = value; }
            get { return _OPEN_ID; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TEL
        {
            set { _tel = value; }
            get { return _tel; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string EMAIL
        {
            set { _email = value; }
            get { return _email; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string IMAGE_PATH
        {
            set { _image_path = value; }
            get { return _image_path; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? LAST_ONLINE_TIME
        {
            set { _last_online_time = value; }
            get { return _last_online_time; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int LOGIN_TIMES
        {
            set { _login_times = value; }
            get { return _login_times; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ENABLE_FLAG
        {
            set { _enable_flag = value; }
            get { return _enable_flag; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CREATE_USER_ID
        {
            set { _create_user_id = value; }
            get { return _create_user_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CREATE_TIME
        {
            set { _create_time = value; }
            get { return _create_time; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string UPDATE_USER_ID
        {
            set { _update_user_id = value; }
            get { return _update_user_id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UPDATE_TIME
        {
            set { _update_time = value; }
            get { return _update_time; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string REMARK
        {
            set { _remark = value; }
            get { return _remark; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? TENANT_ID
        {
            set { _tenant_id = value; }
            get { return _tenant_id; }
        }
        #endregion Model 
	}
}

