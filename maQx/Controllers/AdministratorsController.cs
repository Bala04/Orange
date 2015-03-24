using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using maQx.Models;

namespace maQx.Controllers
{
    [Authorize(Roles = Roles.Inviter)]
    public class AdministratorsController : Controller
    {
        // GET: Administrators
        public ActionResult Index()
        {
            return View();
        }
    }
}