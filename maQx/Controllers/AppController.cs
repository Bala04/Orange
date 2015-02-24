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
    /// <summary>
    ///
    /// </summary>
    [OutputCache(NoStore = true, Location = OutputCacheLocation.None, Duration = 0)]
    public class AppController : Controller
    {
        /// <summary>
        /// The _user manager
        /// </summary>
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
        /// <summary>
        /// The _sign in manager
        /// </summary>
        private ApplicationSignInManager _signInManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }
        /// <summary>
        /// The database
        /// </summary>
        private AppContext db = new AppContext();

        /// <summary>
        /// The admin username
        /// </summary>
        private const string AdminUsername = "Administrator";
        /// <summary>
        /// Gets the admin password.
        /// </summary>
        /// <value>
        /// The admin password.
        /// </value>
        private string AdminPassword { get { return AppSettings.GetValue("DefaultAdministratorPassword"); } }
        /// <summary>
        /// Gets the name of the _ session.
        /// </summary>
        /// <value>
        /// The name of the _ session.
        /// </value>
        private string _SessionName { get { return AppSettings.GetValue("_SessionName"); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppController"/> class.
        /// </summary>
        public AppController()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AppController"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="signInManager">The sign in manager.</param>
        public AppController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            // Remove session cookie always while a new session starts.
            HttpContext.RemoveSessionCookie(_SessionName);
            // If the user IsAuthenticated return Index view, other wise ask them to login to the application.
            return Request.IsAuthenticated ? View() : View("Login");
        }

        /// <summary>
        /// Logins the specified return URL.
        /// </summary>
        /// <param name="ReturnUrl">The return URL.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the current the user.
        /// </summary>
        /// <returns></returns>
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
                        Name = identity.FindFirstValue("Firstname"),
                        Department = identity.FindFirstValue("Department.Name")
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
                    // Otherwise return UserUnauhorizedError to client.
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

        /// <summary>
        /// Map able the users.
        /// </summary>
        /// <returns></returns>
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
                        var List = await db.Administrators.Include(x => x.User).Include(x => x.Organization).Where(x => x.Organization.Key == Organization && x.Role == Roles.AppUser).ToListAsync();

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

        /// <summary>
        /// Logins the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Bind(Include = "UserName, Password, RememberMe, _ReturnUrl")]LoginViewModel model)
        {
            try
            {
                // The submitted data should be valid.
                if (ModelState.IsValid)
                {
                    // Allow default user to access the application for creating Administrator account.
                    // Check for provided values are DefaultUsername & DefaultPassword to allow them access the application until the Administrator account is created.
                    if (model.UserName.ToLower() == AdminUsername.ToLower() && model.Password == AdminPassword.ToLower())
                    {
                        // If 'true' check whether a Administrator account is exists in the database or not.
                        var User = await db.Users.Where(x => x.UserName.ToLower() == model.UserName.ToLower()).FirstOrDefaultAsync();

                        // If the account doesn't exists allow them to continue the user creation process. Otherwise skip the user creation process.
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

                    // If the account is not default account then authenticate the user
                    // BUG: if (await AuthenticateUser(model.UserName, model.Password, model.RememberMe))
                    // FIX: User name should be in lowercase. 12/12/2014.
                    if (await AuthenticateUser(model.UserName.ToLower(), model.Password, model.RememberMe))
                    {
                        // If authenticated redirect to Index or ReturnUrl
                        return RedirectToLocal(HttpUtility.UrlDecode(model._ReturnUrl));
                    }
                }

                TempData.SetError("Invalid user name or password.", SetInfo);
            }
            catch (AccessDeniedException Ex)
            {
                ViewBag.ErrorTitle = "Access Denied.";
                ViewBag.ErrorMessage = Ex.Message;
            }
            catch
            {
                ViewBag.ErrorTitle = "Oops! Something went wrong.";
                ViewBag.ErrorMessage = "Please contact your System Administrator for further assistance.";
            }
            return View(model);
        }

        /// <summary>
        /// Logouts this instance.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "App");
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Initializes the specified get email.
        /// </summary>
        /// <param name="GetEmail">The get email.</param>
        /// <returns></returns>
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
                    else throw "Step Initialization failed in sequence. Contact your vendor for more information.".asException();

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

        /// <summary>
        /// Confirms the specified set email.
        /// </summary>
        /// <param name="SetEmail">The set email.</param>
        /// <returns></returns>
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
                        else throw "Specified confirmation code is invalid or expired.".asException();
                    }
                    else throw "Step confirmation failed in sequence. Contact your vendor for more information.".asException();
                }
                else throw "Specified confirmation code is invalid or expired.".asException();
            }
            catch (Exception ex)
            {
                TempData.SetError(ex.Message);
            }

            return RedirectToAction("Init");
        }

        /// <summary>
        /// Registers the specified admin.
        /// </summary>
        /// <param name="Admin">The admin.</param>
        /// <returns></returns>
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
                            throw "An error occurred while creating user. Contact your vendor for more information.".asException();
                    }
                    else
                        throw "Step finalization failed in sequence. Contact your vendor for more information.".asException();
                }

            }
            catch (Exception ex)
            {
                TempData.SetError(ex.Message, SetInfo);
            }

            return View("Init", new InitViewModel() { AdminModel = Admin });
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
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

        /// <summary>
        /// Gets the authentication manager.
        /// </summary>
        /// <value>
        /// The authentication manager.
        /// </value>
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        /// <summary>
        /// Signs the in asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        /// <returns></returns>
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await SignInManager.CreateUserIdentityAsync(user));
        }
        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="Username">The username.</param>
        /// <param name="Password">The password.</param>
        /// <param name="RememberMe">if set to <c>true</c> [remember me].</param>
        /// <returns></returns>
        /// <exception cref="AccessDeniedException">Your account doesn't have any scope to access this application. Please contact your System Administrator for further assistance.</exception>
        private async Task<bool> AuthenticateUser(string Username, string Password, bool RememberMe)
        {
            // Authenticate non-admin users
            // Default Count
            var Count = -1;

            // Allows user to specify only the user name without the domain if only one organization exists in the application scope.
            // Check whether the user have entered the fully qualified user name ie) username@organization.com. If 'true' skip the control.
            // BUG: if (Username.IndexOf('@') == -1)
            // FIX: The user should not be a administrator. 11/12/2014
            if (Username != AdminUsername.ToLower() && Username.IndexOf('@') == -1)
            {
                // if the user name doesn't have '@', retrieve the organization collection to find the number of entities.
                var List = await db.Organizations.ToListAsync();
                Count = List.Count();

                // if the collection is empty return to false, because user can't exists without an organization.
                if (Count == 0) return false;
                // if the collection has only one organization append the domain name with the specified user name.
                else if (Count == 1) Username = Username + "@" + List.First().Domain;
                // else nothing to do here.
            }

            // Check whether the specified domain is exists and the organization is active
            // BUG: var Organization = await db.Organizations.Where(x => x.ActiveFlag && x.Domain == Username.Split('@')[1]).FirstOrDefaultAsync(); ArrayIndex not supported in Entity Framework
            // FIX: Create Domain as string and pass to the query. 18/02/2015
            // BUG: var Domain = Username.Split('@')[1]; Administrator accounts are not belongs to specific organization
            // FIX: Skip authentication checks for administrator account. 20/02/2015
            // Check whether the account is the administrator. If yes skip authentication.
            if (Username != AdminUsername.ToLower())
            {
                var Domain = Username.Split('@')[1];
                var Organization = await db.Organizations.Where(x => x.ActiveFlag && x.Domain == Domain).FirstOrDefaultAsync();

                // if not found return false to prevent login.
                if (Organization == null)
                    return false;
            }

            // Check for whether the user is exists
            var User = await UserManager.FindAsync(Username, Password);

            // If 'true' SignIn user to the application, otherwise return 'false' to login action.
            if (User != null)
            {
                // AppUsers should belongs to a division and should have access to at least one Menu
                if (await UserManager.IsInRoleAsync(User.Id, Roles.AppUser))
                {
                    var AccessMenus = await db.MenuAccess.Include(x => x.DepartmentMenu.Department.Division.Plant).ToListAsync();

                    if (AccessMenus.Count() < 1)
                    {
                        throw new AccessDeniedException("Your account doesn't have any scope to access this application. Please contact your System Administrator for further assistance.");
                    }
                    else if (false) // Check dashboard access
                    {

                    }
                    else
                    {
                        var AccessMenu = AccessMenus.First();
                        string Entity = (AccessMenu.DepartmentMenu.Department.ActiveFlag ? AccessMenu.DepartmentMenu.Department.Division.ActiveFlag ? AccessMenu.DepartmentMenu.Department.Division.Plant.ActiveFlag ? String.Empty : "plant" : "division" : "department");

                        if (Entity != String.Empty)
                        {
                            throw new AccessDeniedException("Your " + Entity + " has been disabled by your System Administrator. Please contact your System Administrator for further assistance.");
                        }
                    }
                }

                await SignInAsync(User, RememberMe);
                return true;
            }

            return false;
        }
        /// <summary>
        /// Processes the step.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Registers the user.
        /// </summary>
        /// <param name="User">The user.</param>
        /// <param name="AdminBase">The admin base.</param>
        /// <returns></returns>
        private async Task<bool> RegisterUser(ApplicationUser User, AdminRegistrationBase AdminBase)
        {
            if (AdminBase.Role != Roles.AppAdmin)
            {
                var Invite = await db.Invites.Include("Organization").SingleOrDefaultAsync(x => x.Password == AdminBase.ConfirmationCode);

                if (Invite == null)
                {
                    throw "Unable to retrieve the information of the specified invite. Please try again.".asException();
                }

                if (Invite.Organization == null)
                {
                    throw "Unable to retrieve the information of the specified organization. Please try again.".asException();
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


        /// <summary>
        /// Adds the role to user.
        /// </summary>
        /// <param name="User">The user.</param>
        /// <param name="Role">The role.</param>
        /// <returns></returns>
        public async Task<bool> AddRoleToUser(ApplicationUser User, string Role)
        {
            // Check whether the user is in specified Role
            // If the User is already in the Role return true. Otherwise add the User to Role and return whether task is succeeded or not.
            if (!(await UserManager.IsInRoleAsync(User.Id, Role)))
            {
                return (await UserManager.AddToRoleAsync(User.Id, Role)).Succeeded;
            }

            return true;
        }

        /// <summary>
        /// Removes the role from user.
        /// </summary>
        /// <param name="User">The user.</param>
        /// <param name="Role">The role.</param>
        /// <returns></returns>
        public async Task<bool> RemoveRoleFromUser(ApplicationUser User, string Role)
        {
            // Check whether the User is in specified Role
            // If the User is already in the Role remove the User from the Role  and return whether task is succeeded or not. Otherwise return true.
            if (await UserManager.IsInRoleAsync(User.Id, Role))
            {
                return (await UserManager.RemoveFromRoleAsync(User.Id, Role)).Succeeded;
            }

            return true;
        }

        /// <summary>
        /// Redirects to local.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!String.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "App");
        }
        /// <summary>
        /// Adds the errors.
        /// </summary>
        /// <param name="result">The result.</param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        /// <summary>
        /// Sets the information.
        /// </summary>
        private void SetInfo()
        {
            ViewBag.Info = TempData["Info"] as ClientInfo;
        }

        #endregion
    }
}