using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using maQx.Models;
using System.Threading.Tasks;
using maQx.Utilities;
using System.Data.Entity;

namespace maQx.Controllers
{
    [Authorize(Roles = Roles.SysAdmin)]
    public class AccessLevelController : Controller
    {
        // GET: AccessLevel
        public async Task<ActionResult> Index()
        {
            var Organization = User.GetOrganization();

            return View(new AccessLevelViewModel
            {
                Users = (await Shared.GetSelectableUsers(Organization)).ToSelectList("Firstname", KeyField: "Id")
            });
        }

        #region Locals

        private void SetInfo()
        {
            ViewBag.Info = TempData["Info"] as ClientInfo;
        }

        #endregion
    }
}