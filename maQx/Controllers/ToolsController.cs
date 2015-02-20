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
    [Authorize(Roles = Roles.AppUser)]
    [AccessAuthorize]
    public class ToolsController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Tools
        public ActionResult Index()
        {
            return View();
        }

        // GET: Tools/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = await db.Tools.FindAsync(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        // GET: Tools/Create
        [Roles(Roles.Create, Roles.CreateEdit)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tools/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Create, Roles.CreateEdit)]
        public async Task<ActionResult> Create([Bind(Include = "MaxCount,Tolerance,Code,Description")] ToolViewModel tool)
        {
            if (ModelState.IsValid)
            {
                var Division = await db.Divisions.FindAsync(User.GetDivision());

                if (Division == null)
                {
                    return HttpNotFound();
                }

                db.Tools.Add(new Tool
                {
                    MaxCount = tool.MaxCount,
                    Tolerance = tool.Tolerance,
                    Code = tool.Code,
                    Description = tool.Description,
                    Division = Division
                });
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tool);
        }

        // GET: Tools/Edit/5
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = await db.Tools.FindAsync(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(new ToolEditViewModel(tool));
        }

        // POST: Tools/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Edit, Roles.CreateEdit, Roles.EditDelete)]
        public async Task<ActionResult> Edit([Bind(Include = "Key,MaxCount,Tolerance,Code,Description")] ToolEditViewModel tool)
        {
            if (ModelState.IsValid)
            {
                var Tool = await db.Tools.Include(x => x.Division).Where(x => x.Key == tool.Key).FirstOrDefaultAsync();

                Tool.Code = tool.Code;
                Tool.Description = tool.Description;
                Tool.MaxCount = tool.MaxCount;
                Tool.Tolerance = tool.Tolerance;

                db.Entry(Tool).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tool);
        }

        // GET: Tools/Delete/5
        [Roles(Roles.Delete, Roles.EditDelete)]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = await db.Tools.FindAsync(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        // POST: Tools/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Roles(Roles.Delete, Roles.EditDelete)]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Tool tool = await db.Tools.FindAsync(id);
            db.Tools.Remove(tool);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
