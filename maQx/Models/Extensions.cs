
using maQx.Models;
using Postal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace maQx.Utilities
{
    public class HelperType<T>
    {
       public T Data { get; set; }
    }

    public static class TableTools
    {
        private static bool Get(Type T, string name)
        {
            try
            {
                var a = T.GetMethod(name);
                return a != null;
            }
            catch (AmbiguousMatchException)
            {
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Tuple<bool, bool, bool> GetTools(Type T)
        {
            return new Tuple<bool, bool, bool>(Get(T, "Details"), Get(T, "Edit"), Get(T, "Delete"));
        }
    }

    public static class RegExPatterns
    {
        public const string Email = "^([a-zA-Z0-9_.+-]{2,})+\\@(([a-zA-Z0-9-])+\\.)+([a-zA-Z0-9.]{2,})+$";
        public const string Domain = "^(([a-zA-Z0-9-])+\\.)+([a-zA-Z0-9.]{2,})+$";
        public const string AlphaNumeric = "^[a-zA-Z0-9 ]+$";
        public const string SpecialAlphaNumeric = "^[a-zA-Z0-9-_ ]+$";
        public const string Alpha = "^[a-zA-Z ]+$";
        public const string AlphaWithOutSpace = "^[a-zA-Z]+$";
        public const string AlphaNumericWithOutSpace = "^[a-zA-Z0-9]+$";
        public const string Username = "^[a-zA-Z0-9._]+$";
        public const string Name = "^(([a-zA-Z ])\\2?(?!\\2))+$";
        public const string Numeric = "^[0-9]+$";
        public const string Decimal = "^[0-9.]+$";
        public const string PhoneNumber = "^[0-9]{10}$";
    }

    public static class AppSettings
    {
        public static string GetValue(string key)
        {
            return new System.Configuration.AppSettingsReader().GetValue(key, typeof(String)).ToString();
        }
    }

    public static class Extensions
    {
        private static AesCryptoServiceProvider GetAesCryptoServiceProvider(string Salt)
        {
            AesCryptoServiceProvider AESProvider = new AesCryptoServiceProvider();
            AESProvider.KeySize = 256;
            AESProvider.Padding = PaddingMode.Zeros;

            System.Diagnostics.Debug.WriteLine(Salt);

            SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] Key = sha256.ComputeHash(UnicodeEncoding.Unicode.GetBytes(AppSettings.GetValue("CryptoText") + Salt));

            AESProvider.Key = Key;

            return AESProvider;
        }

        /// <summary>
        /// Returns a string of random generated characters with specified size
        /// </summary>
        /// <param name="Length">Absolute length of the string</typeparam>
        /// <param name="Numeric">Indicates that the string should contain only numerical values</typeparam>        
        /// <returns>string of random generated characters</returns>
        public static string GetUniqueKey(int Length = 32, bool Numeric = false)
        {
            char[] chars = new char[62];

            chars = (Numeric ? "1234567890" : "abcdefghijklmnopqrstuvwxyz1234567890").ToCharArray();

            byte[] data = new byte[Length];

            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);

            StringBuilder result = new StringBuilder(Length);

            foreach (byte b in data)
            {
                chars = chars.Reverse().ToArray();

                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }
        public static string Encrypt(this string Value, string SessionKey)
        {
            byte[] PlainBytes = UnicodeEncoding.Unicode.GetBytes(Value);
            var AESProvider = GetAesCryptoServiceProvider(SessionKey);
            AESProvider.GenerateIV();

            using (ICryptoTransform Encryptor = AESProvider.CreateEncryptor())
            {
                byte[] SecureBytes = Encryptor.TransformFinalBlock(PlainBytes, 0, PlainBytes.Length);
                SecureBytes = AESProvider.IV.Concat(SecureBytes).ToArray();
                AESProvider.Clear();
                return Convert.ToBase64String(SecureBytes);
            }
        }
        public static string Decrypt(this string Value, string SessionKey)
        {
            byte[] SecureBytes = Convert.FromBase64String(Value);
            var AESProvider = GetAesCryptoServiceProvider(SessionKey);
            AESProvider.IV = SecureBytes.Take(16).ToArray();

            using (ICryptoTransform Decryptor = AESProvider.CreateDecryptor())
            {
                byte[] PlainBytes = Decryptor.TransformFinalBlock(SecureBytes, 16, SecureBytes.Length - 16);
                AESProvider.Clear();
                return UnicodeEncoding.Unicode.GetString(PlainBytes);
            }
        }
        public static string Left(this String input, int length)
        {
            return (input.Length < length) ? input : input.Substring(0, length);
        }
        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");
            return list.Contains(source);
        }
        public static string With(this string s, params object[] args)
        {
            return string.Format(s, args);
        }
        public static bool Between<T>(this T actual, T lower, T upper) where T : IComparable<T>
        {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }
        public static void With<T>(this T obj, Action<T> act) { act(obj); }
        public static T As<T>(this object item) where T : class
        {
            return (T)(object)item;
        }
        public static bool Is<T>(this object item) where T : class
        {
            return item is T;
        }
        public static Dictionary<string, object> ToDictionary(this object o)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var propertyInfo in o.GetType().GetProperties())
            {
                if (propertyInfo.GetIndexParameters().Length == 0)
                {
                    dictionary.Add(propertyInfo.Name, propertyInfo.GetValue(o, null));
                }
            }

            return dictionary;
        }
        public static int Age(this DateTime dateOfBirth)
        {
            if (DateTime.Today.Month < dateOfBirth.Month || DateTime.Today.Month == dateOfBirth.Month && DateTime.Today.Day < dateOfBirth.Day)
                return DateTime.Today.Year - dateOfBirth.Year - 1;
            else
                return DateTime.Today.Year - dateOfBirth.Year;
        }
        public static int toInt(this object value)
        {
            return Convert.ToInt32(value);
        }
        public static double toDouble(this object value)
        {
            return Convert.ToDouble(value).Normalize();
        }
        public static DateTime toDateTime(this object value)
        {
            return Convert.ToDateTime(value);
        }
        public static void ForEach<T>(this IEnumerable<T> input, Action<T> exp)
        {
            foreach (var item in input)
            {
                exp(item);
            }
        }
        public static void ForEach<T>(this ICollection<T> input, Action<T> exp)
        {
            foreach (var item in input)
            {
                exp(item);
            }
        }
        public static void ForEach<T>(this IEnumerable<T> input, Action<T, int> exp)
        {
            int i = 0;
            foreach (var item in input)
            {
                exp(item, i++);
            }
        }
        public static void ForEach<T>(this ICollection<T> input, Action<T, int> exp)
        {
            int i = 0;
            foreach (var item in input)
            {
                exp(item, i++);
            }
        }
        public static Exception asException(this string value)
        {
            return new Exception(value);
        }
        public static string Capitalize(this String input)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }
        public static string toDateString(this DateTime dt)
        {
            return dt.Day + " " + dt.ToString("MMM") + ", " + dt.Year;
        }
        public static string asSQLDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string toDateSmallString(this DateTime dt)
        {
            return dt.Day.ToString("00") + " " + dt.ToString("MMM");
        }
        public static string Hash(this string text)
        {
            using (MD5 MD5 = new MD5CryptoServiceProvider())
            {
                MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
                byte[] Result = MD5.Hash;

                StringBuilder StrBuilder = new StringBuilder();
                for (int i = 0; i < Result.Length; i++)
                {
                    StrBuilder.Append(Result[i].ToString("x2"));
                }

                return StrBuilder.ToString();
            }
        }
        public static string asSize(this double byteCount)
        {
            try
            {
                string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
                if (byteCount == 0)
                    return "0" + suf[0];
                double bytes = Math.Abs(byteCount);
                int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                double num = Math.Round(bytes / Math.Pow(1024, place), 1);
                return (Math.Sign(byteCount) * num).ToString() + " " + suf[place];
            }
            catch
            {
                return byteCount + " B";
            }
        }
        public static string toJson(this object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
        public static T toObject<T>(this string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }
        public static double Normalize(this double value)
        {
            return Math.Round(value, 2);
        }
        public static System.Drawing.Color asColor(this string value)
        {
            return System.Drawing.ColorTranslator.FromHtml(value);
        }
        public static void Times(this int value, Action<int> method)
        {
            for (int i = 0; i < value; i++)
            {
                method(i);
            }
        }
        public static void Times(this double value, Action<double> method)
        {
            for (int i = 0; i < value; i++)
            {
                method(i);
            }
        }
        public static int GetWeekOfYear(this DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public static async Task<JsonResult> toJson(this JsonViewModel JsonViewModel)
        {
            return await new JsonHelper().Json(JsonViewModel);
        }
        public static JsonResult toJsonUnAsync(this JsonViewModel JsonViewModel)
        {
            return new JsonHelper().Json(JsonViewModel).Result;
        }
        public static object GetPropertyValue(this object value, string propertyName)
        {
            return value.GetType().GetProperties().Single(x => x.Name == propertyName).GetValue(value, null);
        }
        public static SelectListItem DefaultListValue(string DisplayName = "-Select-", bool Selected = true)
        {
            return new SelectListItem() { Disabled = true, Selected = Selected, Text = DisplayName, Value = "-1" };
        }
        public static SelectList ToSelectList<T>(this IEnumerable<T> list, string valueField, string DisplayName = "-Select-", string keyField = "Key", object selectedField = null)
        {
            var SelectList = new List<SelectListItem>();
            var defaultValue = DefaultListValue(DisplayName, selectedField == null);
            SelectList.Add(defaultValue);

            list.ForEach(x =>
            {
                SelectList.Add(new SelectListItem() { Text = (string)x.GetPropertyValue(valueField), Value = (string)x.GetPropertyValue(keyField) });
            });

            return new SelectList(SelectList, "Value", "Text", (selectedField == null ? "-1" : selectedField), new List<string>() { "-1" });
        }
        public static maQx.Models.ClientInfo Info(string Message, maQx.Models.ClientInfoType Type = maQx.Models.ClientInfoType.Success)
        {
            return new ClientInfo()
            {
                Time = DateTime.Now,
                Message = Message,
                Type = Type,
            };
        }
        public static void SetError(this TempDataDictionary TempData, string Message, Action Callback = null)
        {
            TempData["Info"] = Info(Message, ClientInfoType.Error);

            if (Callback != null)
            {
                Callback();
            }
        }
        public static void SetSuccess(this TempDataDictionary TempData, string Message)
        {
            TempData["Info"] = Info(Message, ClientInfoType.Success);
        }
        public static string GetSessionKey(this HttpContextBase HttpContext)
        {
            return (HttpContext.Request.UserHostAddress + "" + HttpContext.Request.UserAgent).Hash();
        }
        public static string GetSessionCookie(this HttpContextBase HttpContext, string Name)
        {
            return HttpContext.Request.Cookies[Name] != null ? HttpContext.Request.Cookies.Get(Name).Value : null;
        }
        public static void RemoveSessionCookie(this HttpContextBase HttpContext, string Name)
        {
            if (HttpContext.Response.Cookies[Name] != null)
                HttpContext.Response.Cookies[Name].Expires = DateTime.Now.AddDays(-1);
        }
        public static string GetSecuredSessionCookie(this HttpContextBase HttpContext, string Name, string Key = null)
        {
            var Value = HttpContext.GetSessionCookie(Name);
            return String.IsNullOrWhiteSpace(Value) ? null : Value.Decrypt(String.IsNullOrWhiteSpace(Key) ? HttpContext.GetSessionKey() : Key);
        }
        public static void SetSessionCookie(this HttpContextBase HttpContext, string Name, string Value, bool HttpOnly = true)
        {
            HttpCookie Cookie = new HttpCookie(Name, Value);
            HttpContext.Response.Cookies.Remove(Name);
            Cookie.HttpOnly = HttpOnly;
            HttpContext.Response.SetCookie(Cookie);
        }
        public static void SetSecuredSessionCookie(this HttpContextBase HttpContext, string Name, string Value, string Key = null)
        {
            HttpContext.SetSessionCookie(Name, Value.Encrypt(String.IsNullOrWhiteSpace(Key) ? HttpContext.GetSessionKey() : Key));
        }
        public static List<string> GetRoles(this IPrincipal User)
        {
            var Roles = (((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)).ToList();

            return Roles;
        }

        public static string GetOrganization(this IPrincipal User)
        {
            return ((ClaimsIdentity)User.Identity).FindFirst("Organization.Key").Value;
        }

        /// <summary>
        /// Acts similar of .Include() LINQ method, but allows to include several object properties at once.
        /// </summary>
        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, Expression<Func<T, object>>[] paths)
            where T : class
        {
            foreach (var path in paths)
                query = query.Include(path);

            return query;
        }

        /// <summary>
        /// Returns all the values of constants of the specified type
        /// </summary>
        /// <typeparam name="T">What type of constants to return</typeparam>
        /// <param name="type">Type to examine</param>
        /// <returns>List of constant values</returns>
        public static List<T> GetConstantValues<T>(this Type type)
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public
                | BindingFlags.Static
                | BindingFlags.FlattenHierarchy);

            return (fields.Where(fieldInfo => fieldInfo.IsLiteral
                && !fieldInfo.IsInitOnly
                && fieldInfo.FieldType == typeof(T)).Select(fi => (T)fi.GetRawConstantValue())).ToList();
        }
    }

    public static class PostalMail
    {
        private static EmailService EmailService
        {
            get
            {
                var engines = new ViewEngineCollection();
                engines.Add(new FileSystemRazorViewEngine(Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"))));

                return new EmailService(engines);
            }
        }
        public static void SendAdminConfrimationCode(string MailTo, string ConfirmationCode)
        {
            EmailService.SendAsync(new AdminConfirmationEmail
            {
                From = "ms@iprings.com",
                To = MailTo,
                Name = "Administrator",
                Subject = "maQx Management - Confrimation Email",
                ConfirmationCode = ConfirmationCode
            });
        }
        public static void SendUserInvite(string MailTo, string Name, string Position, string OrganizationName, string ActivationLink)
        {
            EmailService.SendAsync(new UserInvite
            {
                From = "ms@iprings.com",
                To = MailTo,
                Name = Name,
                Subject = "maQx Management - Application Invite",
                AcivationLink = ActivationLink,
                Position = Position,
                OrganizationName = OrganizationName
            });
        }
    }

    public class ViewHelper
    {
       

        private static async Task<JsonResult> List<T1, T2>(string Controller, Func<List<T2>, T1> operation, List<T2> data)
            where T1 : maQx.Models.JsonViewModel
            where T2 : class
        {
            if (operation != null)
            {
                return await operation(data).toJson();
            }

            return await new JsonListViewModel<T2>(data, String.IsNullOrWhiteSpace(Controller) ? null : TableTools.GetTools(Type.GetType("maQx.Controllers." + Controller))).toJson();
        }

        public static async Task<JsonResult> List<T1, T2>(HttpRequestBase Request, HttpResponseBase Response, string Controller, string Role, IPrincipal User, DbSet<T1> value, Expression<Func<T1, bool>> exp, Expression<Func<T1, object>>[] Includes, Func<List<T1>, T2> operation = null)
            where T1 : class
            where T2 : maQx.Models.JsonViewModel
        {
            try
            {
                if (typeof(T1) == typeof(Menus) || User.IsInRole(Role))
                {
                    var data = await value.IncludeMultiple(Includes).Where(exp).ToListAsync();

                    return await List<T2, T1>(Controller, operation, data);
                }
                else
                {
                    return await JsonErrorViewModel.GetUserUnauhorizedError().toJson();
                }
            }
            catch (Exception ex)
            {
                return JsonExceptionViewModel.Get(ex).toJsonUnAsync();
            }
        }

        public static async Task<JsonResult> Format<T1, T2, T3>(HttpRequestBase Request, HttpResponseBase Response, string Controller, string Role, IPrincipal User, DbSet<T1> value, Expression<Func<T1, bool>> exp, Expression<Func<T1, object>>[] Includes, Func<List<T3>, T2> operation = null)
            where T1 : class
            where T2 : maQx.Models.JsonViewModel
            where T3 : class, IJsonBase<T1, T3>
        {
            try
            {
                if (typeof(T1) == typeof(Menus) || User.IsInRole(Role))
                {
                    var format = Activator.CreateInstance<T3>();
                    var data = await value.IncludeMultiple(Includes).Where(exp).ToListAsync();
                    var d = data.Select(x =>
                    {
                        return format.To(x);
                    }).ToList();

                    return await List<T2, T3>(Controller, operation, d);
                }
                else
                {
                    return await JsonErrorViewModel.GetUserUnauhorizedError().toJson();
                }
            }
            catch (Exception ex)
            {
                return JsonExceptionViewModel.Get(ex).toJsonUnAsync();
            }
        }
    }

    internal class JsonHelper : Controller
    {
        internal async Task<JsonResult> Json(JsonViewModel JsonViewModel)
        {
            return await Task.Factory.StartNew(() => Json(JsonViewModel, JsonRequestBehavior.AllowGet));
        }
    }

    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.HttpContext.Response.StatusCode = 404;
                filterContext.Result = new HttpNotFoundResult();
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }

    public class AuthorizeGetViewAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.HttpContext.Response.StatusCode = 403;
                filterContext.Result = (JsonErrorViewModel.GetUserUnauhorizedError().toJson()).Result;
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }


}