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
    public class DivisionsController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Divisions
        public ActionResult Index()
        {
            return View();
        }

        // GET: Divisions/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Division division = await db.Divisions.Include(x => x.Plant).Where(x => x.Key == id).SingleOrDefaultAsync();
            if (division == null)
            {
                return HttpNotFound();
            }
            return View(division);
        }

        // GET: Divisions/Create
        public async Task<ActionResult> Create()
        {
            var Organization = User.GetOrganization();

            if (String.IsNullOrWhiteSpace(Organization))
            {
                return HttpNotFound();
            }

            var Plant = await db.Plants.Where(x => x.ActiveFlag).ToListAsync();

            return View(new DivisionViewModel
            {
                Plants = Plant.ToSelectList("Name", "- Plant -")
            });
        }

        // POST: Divisions/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Code,Name,Plant")] DivisionViewModel division)
        {
            var Plants = await db.Plants.Where(x => x.ActiveFlag).ToListAsync();

            if (ModelState.IsValid)
            {
                var Plant = Plants.Where(x => x.Key == division.Plant).FirstOrDefault();

                if (Plant != null)
                {
                    db.Divisions.Add(new Division
                    {
                        Code = division.Code,
                        Name = division.Name,
                        Plant = Plant
                    });
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Plant", "Selected plant is not found.");
                }
            }
            division.Plants = Plants.ToSelectList("Name", " - Plant -", SelectedField: division.Plant);
            return View(division);
        }

        // GET: Divisions/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Division division = await db.Divisions.Include(x => x.Plant).Where(x => x.Key == id).SingleOrDefaultAsync();
            if (division == null)
            {
                return HttpNotFound();
            }

            var Plants = await db.Plants.Where(x => x.ActiveFlag).ToListAsync();
            return View(new DivisionEditViewModel(division, Plants.ToSelectList("Name", SelectedField: division.Plant.Key)));
        }

        // POST: Divisions/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Key,Code,Name,Plant")] DivisionEditViewModel division)
        {
            var div = await db.Divisions.Include(x => x.Plant).Where(x => x.Key == division.Key).SingleOrDefaultAsync();

            if (div == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                if (div.Plant.Key != division.Plant)
                {
                    div.Plant = await db.Plants.FindAsync(division.Plant);
                }

                if (div.Plant != null)
                {
                    div.Code = division.Code;
                    div.Name = division.Name;
                    db.Entry(div).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Plant", "Selected plant is not found.");
                }
            }

            var Plants = await db.Plants.ToListAsync();
            return View(new DivisionEditViewModel(div, Plants.ToSelectList("Name", " - Plant -", SelectedField: division.Plant)));
        }

        // GET: Divisions/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Division division = await db.Divisions.FindAsync(id);
            if (division == null)
            {
                return HttpNotFound();
            }
            return View(division);
        }

        // POST: Divisions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Division division = await db.Divisions.FindAsync(id);
            db.Divisions.Remove(division);
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
