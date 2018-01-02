/*******************************************************************************
 * Copyright © 2016 NFine.Framework 版权所有
 * Author: NFine
 * Description: NFine快速开发平台
 * Website：http://www.nfine.cn
*********************************************************************************/
using ZGEDrySaltery.Code;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ZGEDrySaltery.Web;

namespace ZGEDrySaltery.Web.Controllers
{
    [HandlerLogin]
    public class ClientsDataController : Controller
    {
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetClientsDataJson()
        {
            var data = new
            {
                user = "",
            };
            return Content(data.ToJson());
        }
    }
}
