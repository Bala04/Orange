using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using maQx.Models;
using maQx.Utilities;

namespace maQx.Controllers
{
    [Authorize(Roles = Roles.SysAdmin)]
    public class DepartmentsController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Departments
        public async Task<ActionResult> Index()
        {
            try
            {
                return View(new DepartmentViewModel
                {
                    Divisions = (await Shared.GetSelectableDivisions(User.GetOrganization())).ToSelectList("Name", "All", DefaultValue: "0")
                });
            }
            catch (Exception ex)
            {
                TempData.SetError(ex.Message, SetInfo);
            }

            return View();
        }

        [HttpGet, Route("Context/Departments")]
        public async Task<JsonResult> ContextList(string Division)
        {
            return Json((await db.Departments.Include(x => x.Division.Plant.Organization).Where(x => x.Division.Key == Division).ToListAsync()).Select(x => new JDepartment(x)), JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("Context/Departments/{ID}")]
        public async Task<JsonResult> ContextList(string ID, string Division)
        {
            return Json(new JDepartment((await db.Departments.Include(x => x.Division.Plant.Organization).Where(x => x.Key == ID && x.Division.Key == Division).SingleOrDefaultAsync())), JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("Context/Departments")]
        public async Task<JsonResult> ContextCreate([Bind(Include = "Name,Division")] DepartmentViewModel Department)
        {
            try
            {
                var Division = await db.Divisions.Include(x => x.Plant.Organization).Where(x => x.Key == Department.Division).SingleOrDefaultAsync();

                if (Division == null)
                {
                    return await JsonErrorViewModel.GetDataNotFoundError(Response).toJson();
                }

                var Dep = new Department
                {
                    Name = Department.Name,
                    Division = Division,
                    Access = Roles.SysAdmin
                };

                db.Departments.Add(Dep);

                await db.SaveChangesAsync();

                return Json(new JDepartment(Dep), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonErrorViewModel { Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPut, Route("Context/Departments/{ID}")]
        public async Task<JsonResult> ContextUpdate([Bind(Include = "Key, Name")] DepartmentEditViewModel Department)
        {
            try
            {
                var Dep = await db.Departments.Include(x => x.Division.Plant.Organization).Where(x => x.Key == Department.Key).SingleOrDefaultAsync();
                Dep.Name = Department.Name;

                if (Dep == null)
                {
                    return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                }

                db.Entry(Dep).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return Json(new JDepartment(Dep), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonErrorViewModel { Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpDelete, Route("Context/Departments/{ID}")]
        public async Task<JsonResult> ContextDelete(string ID)
        {
            try
            {
                var Dep = await db.Departments.Include(x => x.Division.Plant.Organization).Where(x => x.Key == ID).SingleOrDefaultAsync();
                var Del = new JDepartment(Dep);

                if (Dep == null)
                {
                    return await JsonErrorViewModel.GetResourceNotFoundError(Response).toJson();
                }

                db.Departments.Remove(Dep);
                await db.SaveChangesAsync();

                return Json(Del, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JsonErrorViewModel { Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

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
    }
}
