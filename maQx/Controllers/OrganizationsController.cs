using maQx.Models;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace maQx.Controllers
{
    [Authorize(Roles = Roles.AppAdmin)]
    public class OrganizationsController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Organizations
        public ActionResult Index()
        {
            // return View(await db.Organizations.Where(x => x.ActiveFlag).ToListAsync());
            return View();
        }

        // GET: Organizations/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = await db.Organizations.FindAsync(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // GET: Organizations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Organizations/Create
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Code, Name, Domain")] OrganizationViewModel organization)
        {
            if (ModelState.IsValid)
            {
                db.Organizations.Add(new Organization()
                {
                    Name = organization.Name,
                    Code = organization.Code,
                    Domain = organization.Domain
                });
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(organization);
        }

        // GET: Organizations/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Organization organization = await db.Organizations.FindAsync(id);
            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(new OrganizationEditViewModel()
            {
                Key = organization.Key,
                Name = organization.Name,
                Code = organization.Code,
                Domain = organization.Domain
            });
        }

        // POST: Organizations/Edit/5
        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Key, Code, Name, Domain")] OrganizationEditViewModel organization)
        {
            if (ModelState.IsValid)
            {
                Organization org = await db.Organizations.FindAsync(organization.Key);
                org.Name = organization.Name;
                org.Code = organization.Code;
                org.Domain = organization.Domain;

                db.Entry(org).State = EntityState.Modified;

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(organization);
        }

        // GET: Organizations/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Organization organization = await db.Organizations.FindAsync(id);

            if (organization == null)
            {
                return HttpNotFound();
            }
            return View(organization);
        }

        // POST: Organizations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Organization organization = await db.Organizations.FindAsync(id);
            db.Organizations.Remove(organization);
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
