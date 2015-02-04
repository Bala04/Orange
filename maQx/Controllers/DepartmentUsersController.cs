using maQx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using maQx.Utilities;

namespace maQx.Controllers
{
    public class DepartmentUsersController : Controller
    {
        private AppContext db = new AppContext();

        // GET: DepartmentUsers
        public async Task<ActionResult> Index()
        {
            try
            {
                return View(new DepartmentDivisionViewModel
                {
                    Divisions = (await Shared.GetSelectableDivisions(User.GetOrganization())).ToSelectList("Name")
                });
            }
            catch (Exception ex)
            {
                TempData.SetError(ex.Message, SetInfo);
            }

            return View();
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