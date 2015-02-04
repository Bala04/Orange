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
    public class AccessLevelController : Controller
    {
        private AppContext db = new AppContext();

        // GET: AccessLevel
        public async Task<ActionResult> Index()
        {
            var Organization = User.GetOrganization();
            var List = await db.Administrators.Include(x => x.User).Include(x => x.Organization).Where(x => x.Organization.Key == Organization && x.Role == Roles.SysAdmin).ToListAsync();

            return View(new AccessLevelViewModel
            {
                Users = List.Select(x => new JApplicationUser(x.User)).ToSelectList("Firstname", KeyField: "Id")
            });
        }

        #region Locals

        private void SetInfo()
        {
            ViewBag.Info = TempData["Info"] as ClientInfo;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        #endregion
    }
}