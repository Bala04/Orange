// Copyright (c) IP Rings Ltd. All rights reserved.
// Version 2.0.1. Author: Prasanth <@prashanth702> 

using Hangfire;
using maQx.Models;
using maQx.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace maQx.Controllers
{
    [OutputCache(NoStore = true, Location = OutputCacheLocation.None, Duration = 0)]
    public class AppController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        private ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        private AppContext db = new AppContext();

        private const string AdminUsername = "Administrator";
        private string AdminPassword { get { return AppSettings.GetValue("DefaultAdministratorPassword"); } }
        private string _SessionName { get { return AppSettings.GetValue("_SessionName"); } }

        public AppController()
        {
        }
        public AppController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        [HttpGet]
        public ActionResult Index()
        {
            // Remove session cookie always while a new session starts.
            HttpContext.RemoveSessionCookie(_SessionName);
            // If the user IsAuthenticated return Index view, other wise ask them to login to the application.
            return Request.IsAuthenticated ? View() : View("Login");
        }

        [HttpGet]
        public ActionResult Login(string ReturnUrl = "")
        {
            // Remove session cookie always while a new session starts.
            HttpContext.RemoveSessionCookie(_SessionName);

            // If the user IsAuthenticated return to Index action, skip login only if the ReturnUrl IsNullOrWhiteSpace.
            if (Request.IsAuthenticated)
            {
                if (String.IsNullOrWhiteSpace(ReturnUrl))
                {
                    return RedirectToAction("Index", "App");
                }
            }

            return View(new LoginViewModel()
            {
                // Assign ReturnUrl to Model
                _ReturnUrl = HttpUtility.UrlEncode(ReturnUrl)
            });
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<JsonResult> CurrentUser()
        {
            try
            {
                // Send the Current user as a Json object to the client if the user is authenticated
                if (Request.IsAuthenticated)
                {
                    var identity = (ClaimsIdentity)User.Identity;
                    return Json(new JsonCurrentUserViewModel()
                    {
                        Name = identity.FindFirst("Firstname").Value,
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // While sending current to the client user check whether user in user Init Action by checking the session cookie.
                    // For more security check the Auth code against the database to verify the received Auth is valid.
                    string Auth = HttpContext.GetSecuredSessionCookie(_SessionName);

                    // If 'true' return temp user configuration to the client.
                    if (!String.IsNullOrWhiteSpace(Auth))
                    {
                        return await new JsonCurrentUserViewModel()
                        {
                            Name = "New User",
                            Role = "TempSession",
                        }.toJson();
                    }
                    // Otherwisw return UserUnauhorizedError to client.
                    else
                    {
                        return await JsonErrorViewModel.GetUserUnauhorizedError().toJson();
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(JsonExceptionViewModel.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<JsonResult> MappableUsers()
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    // User should belongs to the Role SysAdmin to access MappableUsers
                    if (User.IsInRole(Roles.SysAdmin))
                    {   
                        // Returns users with the Role with AppUser
                        // BUG: var list = db.Administrators.Where(new Func<ApplicationUser, bool>(x => { return UserManager.IsInRole(x.Id, Roles.AppUser) })).ToList();
                        // FIX: AppUser should belongs to the Organization of the SysAdmin 31/1/2015
                        var Organization = User.GetOrganization();
                        var List = await db.Administrators.Include(x => x.User).Include(x => x.Organization).Where(x => x.Organization.Key == Organization && x.Role == Roles.SysAdmin).ToListAsync();

                        return await new JsonListViewModel<JApplicationUser>
                        {
                            List = List.Select(x => new JApplicationUser(x.User)).ToList()
                        }.toJson();
                    }
                }
                // If user is not authenticated return UserUnauhorizedError to client.
                return await JsonErrorViewModel.GetUserUnauhorizedError().toJson();
            }
            catch (Exception ex)
            {
                return Json(JsonExceptionViewModel.Get(ex), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Bind(Include = "UserName, Password, RememberMe, _ReturnUrl")]LoginViewModel model)
        {
            // The submitted data should be valid.
            if (ModelState.IsValid)
            {
                // Allow default user to access the application for creating Administrator account.
                // Check for provided values are DefaultUsername & DefaultPassword to allow them access the appilication until the Administrator account is created.
                if (model.UserName.ToLower() == AdminUsername.ToLower() && model.Password == AdminPassword.ToLower())
                {
                    // If 'true' check whether a Administrator account is exists in the database or not.
                    var User = await db.Users.Where(x => x.UserName.ToLower() == model.UserName.ToLower()).FirstOrDefaultAsync();

                    // If the account dosen't exists allow them to continue the user creation process. Otherwise skip the user creation process.
                    if (User == null)
                    {
                        // Find the current step of the process
                        var InitStep = await db.InitStep.Where(x => x.Mode == model.UserName).FirstOrDefaultAsync();

                        // If no step where found Call ProcessStep to continue the process
                        if (InitStep == null)
                        {
                            await ProcessStep(model);
                            return RedirectToAction("Init");
                        }
                        // Otherwise check whether the steps are completed or not.
                        else if (InitStep.Auth <= 4)
                        {
                            // If not secure the Code and set as cookie and let them proceed.
                            HttpContext.SetSecuredSessionCookie(_SessionName, InitStep.Code);
                            return RedirectToAction("Init");
                        }
                    }
                }

                // If the account is not default account then authenicate the user
                // BUG: if (await AuthenticateUser(model.UserName, model.Password, model.RememberMe))
                // FIX: Username should be in lowercase. 12/12/2014.
                if (await AuthenticateUser(model.UserName.ToLower(), model.Password, model.RememberMe))
                {
                    // If authenticated redirect to Index or ReturnUrl
                    return RedirectToLocal(HttpUtility.UrlDecode(model._ReturnUrl));
                }
            }

            TempData.SetError("Invalid username or password.", SetInfo);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "App");
        }

        [HttpGet]
        public async Task<ActionResult> Init()
        {
            SetInfo();

            string Auth = HttpContext.GetSecuredSessionCookie(_SessionName);

            if (!String.IsNullOrWhiteSpace(Auth))
            {
                var Step = await db.InitStep.FindAsync(Auth);

                if (Step != null)
                {
                    if (Step.Auth <= 2)
                    {
                        var AdminBase = await db.AdminBase.FindAsync(Auth);

                        if (AdminBase != null)
                        {
                            return View(new InitViewModel()
                            {
                                SetEmail = new SetEmailConfirmViewModel()
                                {
                                    ConfirmationCode = String.Empty,
                                    ResendActivity = AdminBase.ResendActivity,
                                    Email = AdminBase.Email,
                                    StepCode = Auth,
                                }
                            });
                        }
                        else return View(new InitViewModel() { GetEmail = new GetEmailConfirmViewModel() { StepCode = Auth } });
                    }
                    else if (Step.Auth == 3)
                    {
                        return View(new InitViewModel() { AdminModel = new AdminRegisterViewModel() { StepCode = Auth, } });
                    }
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Init([Bind(Include = "StepCode, ResendActivity, Email")] GetEmailConfirmViewModel GetEmail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var Step = await db.InitStep.FindAsync(GetEmail.StepCode);

                    if (Step != null)
                    {
                        var ConfirmationCode = Extensions.GetUniqueKey(7, Numeric: true);

                        if (GetEmail.ResendActivity)
                        {
                            var AdminBase = await db.AdminBase.FindAsync(GetEmail.StepCode);
                            AdminBase.Email = GetEmail.Email;
                            AdminBase.ConfirmationCode = ConfirmationCode;

                            db.Entry(AdminBase).State = EntityState.Modified;

                            TempData.SetSuccess("New confirmation code has been sent.");
                        }
                        else
                        {
                            Step.Auth++;

                            var AdminBase = new AdminRegistrationBase()
                            {
                                Email = GetEmail.Email,
                                ConfirmationCode = ConfirmationCode,
                                ResendActivity = true,
                                Step = Step,
                                Role = Roles.AppAdmin
                            };

                            db.Entry(AdminBase.Step).State = EntityState.Modified;
                            db.AdminBase.Add(AdminBase);
                        }

                        await db.SaveChangesAsync();
                        BackgroundJob.Enqueue(() => PostalMail.SendAdminConfrimationCode(GetEmail.Email, ConfirmationCode));

                        return RedirectToAction("Init");
                    }
                    else throw "Step Initilization failed in sequence. Contact your vendor for more information.".asException();

                }
                else throw "Submitted form is not valid. Please try again.".asException();
            }
            catch (Exception ex)
            {
                TempData.SetError(ex.Message);
            }

            SetInfo();
            return View(new InitViewModel()
            {
                GetEmail = GetEmail,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Confirm([Bind(Include = "ConfirmationCode, StepCode")] SetEmailConfirmViewModel SetEmail)
        {
            try
            {
                var Step = await db.InitStep.FindAsync(SetEmail.StepCode);

                if (Step == null)
                {
                    return HttpNotFound();
                }

                if (ModelState.IsValidField("ConfirmationCode"))
                {
                    var AdminBase = await db.AdminBase.FindAsync(SetEmail.StepCode);

                    if (AdminBase != null && Step != null)
                    {
                        if (AdminBase.ConfirmationCode == SetEmail.ConfirmationCode)
                        {
                            Step.Auth++;
                            db.Entry(AdminBase.Step).State = EntityState.Modified;

                            await db.SaveChangesAsync();
                        }
                        else throw "Specified confirmation code is invaild or expired.".asException();
                    }
                    else throw "Step confirmation failed in sequence. Contact your vendor for more information.".asException();
                }
                else throw "Specified confirmation code is invaild or expired.".asException();
            }
            catch (Exception ex)
            {
                TempData.SetError(ex.Message);
            }

            return RedirectToAction("Init");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register([Bind(Include = "Code,Name,Phone,Password,ConfirmPassword,StepCode")] AdminRegisterViewModel Admin)
        {
            try
            {
                var Step = await db.InitStep.FindAsync(Admin.StepCode);
                if (Step == null)
                {
                    return HttpNotFound();
                }

                if (ModelState.IsValid)
                {
                    var AdminBase = await db.AdminBase.FindAsync(Admin.StepCode);
                    if (AdminBase != null && Step != null)
                    {
                        var User = new ApplicationUser()
                        {
                            UserName = Step.Mode,
                            Email = AdminBase.Email,
                            Firstname = Admin.Name,
                            Code = Admin.Code,
                            PhoneNumber = Admin.Phone
                        };

                        var Result = await UserManager.CreateAsync(User, Admin.Password);

                        if (Result.Succeeded)
                        {
                            try
                            {
                                Result = await UserManager.AddToRoleAsync(User.Id, AdminBase.Role);

                                Result = AdminBase.Role.In(Roles.AppAdmin, Roles.SysAdmin) ? await UserManager.AddToRoleAsync(User.Id, Roles.Inviter) : Result;

                                using (var Transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                                {
                                    try
                                    {
                                        db.Entry(User).State = EntityState.Unchanged;

                                        if (Result.Succeeded && await RegisterUser(User, AdminBase))
                                        {
                                            Step.Auth++;
                                            db.Entry(AdminBase.Step).State = EntityState.Modified;

                                            await db.SaveChangesAsync();

                                            Transaction.Commit();

                                            await SignInAsync(User, isPersistent: false);
                                            HttpContext.RemoveSessionCookie(_SessionName);

                                            return RedirectToAction("Index", "App");
                                        }
                                        else
                                            throw "Unable to add specified rolls to the user. Contact your vendor for more information.".asException();
                                    }
                                    catch (Exception)
                                    {
                                        Transaction.Rollback();
                                        throw;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                UserManager.Delete(User);
                                throw;
                            }
                        }
                        else
                            throw "An error occured while creating user. Contact your vendor for more information.".asException();
                    }
                    else
                        throw "Step finilization failed in sequence. Contact your vendor for more information.".asException();
                }

            }
            catch (Exception ex)
            {
                TempData.SetError(ex.Message, SetInfo);
            }

            return View("Init", new InitViewModel() { AdminModel = Admin });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                db.Dispose();
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Locals

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await SignInManager.CreateUserIdentityAsync(user));
        }
        private async Task<bool> AuthenticateUser(string Username, string Password, bool RememberMe)
        {
            // Authenticate non-admin users
            // Default Count 
            var Count = -1;

            // Allows user to specify only the username without the domain if only one organization exists in the application scope.
            // Check whether the user have entered the fully qualified username ie) username@organization.com. If 'true' skip the control.
            // BUG: if (Username.IndexOf('@') == -1)
            // FIX: The user should not be a administrator. 11/12/2014
            if (Username != AdminUsername.ToLower() && Username.IndexOf('@') == -1)
            {
                // if the username dosen't have '@', retrive the organization collection to find the number of enitites.                
                var List = await db.Organizations.ToListAsync();
                Count = List.Count();

                // if the collection is empty return to false, because user can't exists without an organization.
                if (Count == 0) return false;
                // if the collection has only one organization append the domain name with the specified username.
                else if (Count == 1) Username = Username + "@" + List.First().Domain;
                // else nothing to do here.
            }

            // Check for whether the user is exists
            var user = await UserManager.FindAsync(Username, Password);

            // If 'true' SignIn user to the application, otherwise return 'false' to login action.
            if (user != null)
            {
                await SignInAsync(user, RememberMe);
                return true;
            }

            return false;
        }
        private async Task<bool> ProcessStep(LoginViewModel model)
        {
            var InitStep = new IntilizationStep()
            {
                Mode = model.UserName,
                Auth = 1,
                Code = Extensions.GetUniqueKey(7) + "-" + Extensions.GetUniqueKey(24)
            };

            db.InitStep.Add(InitStep);
            await db.SaveChangesAsync();

            HttpContext.SetSecuredSessionCookie(_SessionName, InitStep.Code);

            return true;
        }
        private async Task<bool> RegisterUser(ApplicationUser User, AdminRegistrationBase AdminBase)
        {
            if (AdminBase.Role != Roles.AppAdmin)
            {
                var Invite = await db.Invites.Include("Organization").SingleOrDefaultAsync(x => x.Password == AdminBase.ConfirmationCode);

                if (Invite == null)
                {
                    throw "Unable to retive the information of the specified invite. Please try again.".asException();
                }

                if (Invite.Organization == null)
                {
                    throw "Unable to retive the information of the specified organization. Please try again.".asException();
                }

                db.Administrators.Add(new Administrator
                {
                    User = User,
                    Organization = Invite.Organization,
                    Role = AdminBase.Role
                });

                db.Invites.Remove(Invite);
            }

            return true;
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!String.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "App");
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private void SetInfo()
        {
            ViewBag.Info = TempData["Info"] as ClientInfo;
        }

        #endregion
    }
}