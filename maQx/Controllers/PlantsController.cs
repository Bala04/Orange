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
    public class PlantsController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Plants
        public ActionResult Index()
        {
            return View();
        }

        // GET: Plants/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plant plant = await db.Plants.FindAsync(id);
            if (plant == null)
            {
                return HttpNotFound();
            }
            return View(plant);
        }

        // GET: Plants/Create
        public ActionResult Create()
        {
            return View(new PlantViewModel() { });
        }

        // POST: Plants/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Code,Name,Location")] PlantViewModel plant)
        {
            if (ModelState.IsValid)
            {
                var Organization = await db.Organizations.FindAsync(User.GetOrganization());

                if (Organization != null)
                {
                    db.Plants.Add(new Plant
                    {
                        Code = plant.Code,
                        Name = plant.Name,
                        Location = plant.Location,
                        Organization = Organization
                    });
                }
                else
                {
                    ModelState.AddModelError("Organization", "Unable to retrieve your organization information. Please try again.");
                }

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(plant);
        }

        // GET: Plants/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plant plant = await db.Plants.FindAsync(id);
            if (plant == null)
            {
                return HttpNotFound();
            }
            return View(new PlantEditViewModel
            {
                Key = plant.Key,
                Code = plant.Code,
                Location = plant.Location,
                Name = plant.Name
            });
        }

        // POST: Plants/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Key,Code,Name,Location")] PlantEditViewModel plant)
        {
            var pln = await db.Plants.Include("Organization").Where(x => x.Key == plant.Key).SingleOrDefaultAsync();

            if (pln == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                pln.Name = plant.Name;
                pln.Code = plant.Code;
                pln.Location = plant.Location;
                db.Entry(pln).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(plant);
        }

        // GET: Plants/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Plant plant = await db.Plants.FindAsync(id);
            if (plant == null)
            {
                return HttpNotFound();
            }
            return View(plant);
        }

        // POST: Plants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Plant plant = await db.Plants.FindAsync(id);
            db.Plants.Remove(plant);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
