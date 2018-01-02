/*******************************************************************************
 * Copyright © 2016 NFine.Framework 版权所有
 * Author: NFine
 * Description: NFine快速开发平台
 * Website：http://www.nfine.cn
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZGEDrySaltery.Code;
using ZGEDrySaltery.Model;
using ZGEDrySaltery.Web;

namespace NFine.Web.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public virtual ActionResult Index()
        {
            var test = string.Format("{0:E2}", 1);
            return View();
        }
        [HttpGet]
        public ActionResult GetAuthCode()
        {
            return File(new VerifyCode().GetVerifyCode(), @"image/Gif");
        }
        [HttpGet]
        public ActionResult OutLogin()
        {
            //new LogApp().WriteDbLog(new LogEntity
            //{
            //    F_ModuleName = "系统登录",
            //    F_Type = DbLogType.Exit.ToString(),
            //    F_Account = OperatorProvider.Provider.GetCurrent().UserCode,
            //    F_NickName = OperatorProvider.Provider.GetCurrent().UserName,
            //    F_Result = true,
            //    F_Description = "安全退出系统",
            //});
            Session.Abandon();
            Session.Clear();
            OperatorProvider.Provider.RemoveCurrent();
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult CheckLogin(string username, string password, string code)
        {
            try
            {
                if (Session["nfine_session_verifycode"].IsEmpty() || Md5.md5(code.ToLower(), 16) != Session["nfine_session_verifycode"].ToString())
                {
                    throw new Exception("验证码错误，请重新输入");
                }
                S_USER user = ZGEDrySaltery.BLL.SUserBLL.GetInstance().CheckLogin(username, password);
                if (user != null && user.USER_ID != 0)
                {
                    OperatorProvider.Provider.AddCurrent(user);
                    return Content(new AjaxResult { state = ResultType.success.ToString(), message = "登录成功。" }.ToJson());
                }
                else
                {
                    return Content(new AjaxResult { state = ResultType.error.ToString(), message = "该用户不存在。" }.ToJson());
                }
            }
            catch (Exception ex)
            {
                return Content(new AjaxResult { state = ResultType.error.ToString(), message = "登录失败，" + ex.Message }.ToJson());
            }
        }
    }
}
