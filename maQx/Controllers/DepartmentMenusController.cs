using maQx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using maQx.Utilities;

namespace maQx.Controllers
{
    public class DepartmentMenusController : Controller
    {
        private AppContext db = new AppContext();

        // GET: DepartmentMenus
        public async Task<ActionResult> Index()
        {
            try
            {
                var Divisions = await db.Divisions.Include(x => x.Plant).Where(x => x.ActiveFlag).OrderBy(x => x.Name).ToListAsync();
                var a = Divisions.Select(x => x.Name).ToList();

                Divisions = Divisions.Select(x =>
                   {
                       if (a.Where(y => y == x.Name).Count() > 1)
                       {
                           x.Name += " - " + x.Plant.Name;
                       }
                       return x;
                   }).ToList();

                return View(new DepartmentMenuViewModel
                {
                    Divisions = Divisions.ToSelectList("Name")
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