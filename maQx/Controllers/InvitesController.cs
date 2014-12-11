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
    [Authorize(Roles = Roles.Inviter)]
    public class InvitesController : Controller
    {
        private AppContext db = new AppContext();

        // GET: Invites
        public ActionResult Index()
        {
            SetInfo();           
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Resend(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var Invite = await db.Invites.Include("Organization").SingleAsync(x => x.Key == id);

            if (Invite == null)
            {
                return HttpNotFound();
            }

            Hangfire.BackgroundJob.Enqueue(() => PostalMail.SendUserInvite(Invite.Email, Invite.Username, "Organization Administrator", Invite.Organization.Name, Request.Url.GetLeftPart(UriPartial.Authority) + "/Invites/Verify/" + Invite.Password));

            TempData.SetSuccess("Invite has been successfully resent to " + Invite.Email);

            SetInfo();

            return View("Details", Invite);
        }


        // GET: Invites/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invite invite = await db.Invites.FindAsync(id);
            if (invite == null)
            {
                return HttpNotFound();
            }
            return View(invite);
        }

        // GET: Invites/Create
        public async Task<ActionResult> Create()
        {
            var Organization = await db.Organizations.ToListAsync();

            return View(new InviteViewModel()
            {
                Organizations = Organization.ToSelectList("Name", "- Organizations -")
            });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Verify(string id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var Invite = await db.Invites.SingleOrDefaultAsync(x => x.Password == id);
                if (Invite == null)
                {
                    return HttpNotFound();
                }

                var InitStep = await db.InitStep.FindAsync(id);

                if (InitStep == null)
                {
                    InitStep = new IntilizationStep
                    {
                        Auth = 3,
                        Code = id,
                        Mode = Invite.Username,                       
                    };

                    var AdminBase = new AdminRegistrationBase()
                    {
                        Email = Invite.Email,
                        ConfirmationCode = id,
                        ResendActivity = true,
                        StepCode = InitStep.Code,
                        Step = InitStep,
                        Role = Invite.Role
                    };

                    using (var Transaction = db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                    {
                        try
                        {
                            db.InitStep.Add(InitStep);
                            db.AdminBase.Add(AdminBase);

                            await db.SaveChangesAsync();

                            Transaction.Commit();
                        }
                        catch (Exception)
                        {
                            Transaction.Rollback();
                            throw;
                        }                        
                    }
                }

                HttpContext.SetSecuredSessionCookie(AppSettings.GetValue("_SessionName"), InitStep.Code);
                return RedirectToAction("Init", "App");

            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: Invites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Username,Email,Organization")] InviteViewModel invite)
        {
            var Organizations = await db.Organizations.ToListAsync();

            if (ModelState.IsValid)
            {
                var Organization = Organizations.Where(x => x.Key == invite.Organization).FirstOrDefault();
                invite.Organizations = Organizations.ToSelectList("Name", "- Organizations -");

                if (Organization != null)
                {
                    var Name = invite.Username + "@" + Organization.Domain;

                    if (await CheckUsername(Name))
                    {
                        var Auth = Extensions.GetUniqueKey(7) + "-" + Extensions.GetUniqueKey(24);

                        db.Invites.Add(new Invite()
                        {
                            Email = invite.Email,
                            Password = Auth,
                            Username = Name,
                            Organization = Organization,
                            Role = User.IsInRole(Roles.AppAdmin) ? Roles.SysAdmin : Roles.AppUser
                        });

                        await db.SaveChangesAsync();

                        Hangfire.BackgroundJob.Enqueue(() => PostalMail.SendUserInvite(invite.Email, invite.Username, "Organization Administrator", Organization.Name, Request.Url.GetLeftPart(UriPartial.Authority) + "/Invites/Verify/" + Auth));

                        TempData.SetSuccess("Invite has been successfully sent to " + invite.Email);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Username", "Specified username is already taken.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Organization", "Selected organization is not found.");
                }
            }

            invite.Organizations = Organizations.ToSelectList("Name", "- Organizations -", selectedField: invite.Organization);

            SetInfo();
            return View(invite);
        }

        // GET: Invites/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Invite invite = await db.Invites.FindAsync(id);
            if (invite == null)
            {
                return HttpNotFound();
            }
            return View(invite);
        }

        // POST: Invites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Invite invite = await db.Invites.FindAsync(id);
            db.Invites.Remove(invite);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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

        private async Task<bool> CheckUsername(string Name)
        {
            return await db.Invites.SingleOrDefaultAsync(x => x.Username == Name) == null && await db.Users.SingleOrDefaultAsync(x => x.UserName == Name) == null;
        }

        #endregion
    }
}
