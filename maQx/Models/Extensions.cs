
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
using Microsoft.AspNet.Identity;
using UnitsNet;

namespace maQx.Utilities
{
    /// <summary>
    ///
    /// </summary>
    public static class TableTools
    {
        /// <summary>
        /// Gets the specified t.
        /// </summary>
        /// <param name="T">The t.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        private static bool Get(Type T, string Name, bool Condition)
        {
            if (Condition)
            {
                try
                {
                    var a = T.GetMethod(Name);
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

            return false;
        }

        /// <summary>
        /// Gets the tools.
        /// </summary>
        /// <param name="T">The t.</param>
        /// <returns></returns>
        public static Tuple<bool, bool, bool, bool> GetTools(Type T, List<string> UserRoles)
        {
            return new Tuple<bool, bool, bool, bool>(Get(T, "Details", true), Get(T, "Edit", UserRoles.ContainsAny(Roles.Edit, Roles.EditDelete)), Get(T, "Delete", UserRoles.ContainsAny(Roles.Delete, Roles.EditDelete)), UserRoles.ContainsAny(Roles.Create, Roles.CreateEdit));
        }

        public static Tuple<bool, bool, bool, bool> GetTools(Type T)
        {
            return new Tuple<bool, bool, bool, bool>(Get(T, "Details", true), Get(T, "Edit", true), Get(T, "Delete", true), true);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class EntityManupulateHelper
    {
        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public string Entity { get; set; }
        /// <summary>
        /// Gets or sets the add.
        /// </summary>
        /// <value>
        /// The add.
        /// </value>
        public string[] Add { get; set; }
        /// <summary>
        /// Gets or sets the remove.
        /// </summary>
        /// <value>
        /// The remove.
        /// </value>
        public string[] Remove { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class AccessDeniedException : Exception
    {
        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public new string Message { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeniedException" /> class.
        /// </summary>
        /// <param name="Message">The message.</param>
        public AccessDeniedException(string Message)
            : base(Message)
        {
            this.Message = Message;
        }
    }

    /// <summary>
    ///
    /// </summary>
    public static class RegExPatterns
    {
        /// <summary>
        /// The email
        /// </summary>
        public const string Email = "^([a-zA-Z0-9_.+-]{2,})+\\@(([a-zA-Z0-9-])+\\.)+([a-zA-Z0-9.]{2,})+$";
        /// <summary>
        /// The domain
        /// </summary>
        public const string Domain = "^(([a-zA-Z0-9-])+\\.)+([a-zA-Z0-9.]{2,})+$";
        /// <summary>
        /// The alpha numeric
        /// </summary>
        public const string AlphaNumeric = "^[a-zA-Z0-9 ]+$";
        /// <summary>
        /// The special alpha numeric
        /// </summary>
        public const string SpecialAlphaNumeric = "^[a-zA-Z0-9-_ ]+$";
        /// <summary>
        /// The alpha
        /// </summary>
        public const string Alpha = "^[a-zA-Z ]+$";
        /// <summary>
        /// The alpha with out space
        /// </summary>
        public const string AlphaWithOutSpace = "^[a-zA-Z]+$";
        /// <summary>
        /// The alpha numeric with out space
        /// </summary>
        public const string AlphaNumericWithOutSpace = "^[a-zA-Z0-9]+$";
        /// <summary>
        /// The username
        /// </summary>
        public const string Username = "^[a-zA-Z0-9._]+$";
        /// <summary>
        /// The name
        /// </summary>
        public const string Name = "^(([a-zA-Z ])\\2?(?!\\2))+$";
        /// <summary>
        /// The numeric
        /// </summary>
        public const string Numeric = "^[0-9]+$";
        /// <summary>
        /// The decimal
        /// </summary>
        public const string Decimal = @"^\d+(.\d+){0,1}$";
        /// <summary>
        /// The phone number
        /// </summary>
        public const string PhoneNumber = "^[0-9]{10}$";
    }

    /// <summary>
    ///
    /// </summary>
    public static class AppSettings
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            return new System.Configuration.AppSettingsReader().GetValue(key, typeof(String)).ToString();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets the aes crypto service provider.
        /// </summary>
        /// <param name="Salt">The salt.</param>
        /// <returns></returns>
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
        /// Gets the unique key.
        /// </summary>
        /// <param name="Length">The length.</param>
        /// <param name="Numeric">if set to <c>true</c> [numeric].</param>
        /// <returns></returns>
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
        /// <summary>
        /// Encrypts the specified session key.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="SessionKey">The session key.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Decrypts the specified session key.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="SessionKey">The session key.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Lefts the specified length.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string Left(this String input, int length)
        {
            return (input.Length < length) ? input : input.Substring(0, length);
        }
        /// <summary>
        /// Ins the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">source</exception>
        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");
            return list.Contains(source);
        }

        public static bool ContainsAll<T>(this IEnumerable<T> source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");

            foreach (var item in list)
            {
                if (!source.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ContainsAny<T>(this IEnumerable<T> source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");

            foreach (var item in list)
            {
                if (source.Contains(item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Withes the specified arguments.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        public static string With(this string s, params object[] args)
        {
            return string.Format(s, args);
        }
        /// <summary>
        /// Betweens the specified lower.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual">The actual.</param>
        /// <param name="lower">The lower.</param>
        /// <param name="upper">The upper.</param>
        /// <returns></returns>
        public static bool Between<T>(this T actual, T lower, T upper) where T : IComparable<T>
        {
            return actual.CompareTo(lower) >= 0 && actual.CompareTo(upper) < 0;
        }
        /// <summary>
        /// Withes the specified act.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="act">The act.</param>
        public static void With<T>(this T obj, Action<T> act) { act(obj); }
        /// <summary>
        /// As the specified item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static T As<T>(this object item) where T : class
        {
            return (T)(object)item;
        }
        /// <summary>
        /// Determines whether [is].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static bool Is<T>(this object item) where T : class
        {
            return item is T;
        }
        /// <summary>
        /// To the dictionary.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
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
        /// <summary>
        /// Ages the specified date of birth.
        /// </summary>
        /// <param name="dateOfBirth">The date of birth.</param>
        /// <returns></returns>
        public static int Age(this DateTime dateOfBirth)
        {
            if (DateTime.Today.Month < dateOfBirth.Month || DateTime.Today.Month == dateOfBirth.Month && DateTime.Today.Day < dateOfBirth.Day)
                return DateTime.Today.Year - dateOfBirth.Year - 1;
            else
                return DateTime.Today.Year - dateOfBirth.Year;
        }
        /// <summary>
        /// To the int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int toInt(this object value)
        {
            return Convert.ToInt32(value);
        }
        /// <summary>
        /// To the double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double toDouble(this object value)
        {
            return Convert.ToDouble(value).Normalize();
        }
        /// <summary>
        /// To the date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static DateTime toDateTime(this object value)
        {
            return Convert.ToDateTime(value);
        }
        /// <summary>
        /// For each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <param name="exp">The exp.</param>
        public static void ForEach<T>(this IEnumerable<T> input, Action<T> exp)
        {
            foreach (var item in input)
            {
                exp(item);
            }
        }
        /// <summary>
        /// For each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <param name="exp">The exp.</param>
        public static void ForEach<T>(this ICollection<T> input, Action<T> exp)
        {
            foreach (var item in input)
            {
                exp(item);
            }
        }
        /// <summary>
        /// For each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <param name="exp">The exp.</param>
        public static void ForEach<T>(this IEnumerable<T> input, Action<T, int> exp)
        {
            int i = 0;
            foreach (var item in input)
            {
                exp(item, i++);
            }
        }
        /// <summary>
        /// For each.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input">The input.</param>
        /// <param name="exp">The exp.</param>
        public static void ForEach<T>(this ICollection<T> input, Action<T, int> exp)
        {
            int i = 0;
            foreach (var item in input)
            {
                exp(item, i++);
            }
        }
        /// <summary>
        /// As the exception.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static Exception asException(this string value)
        {
            return new Exception(value);
        }
        /// <summary>
        /// Capitalizes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string Capitalize(this String input)
        {
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
        }
        /// <summary>
        /// To the date string.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public static string toDateString(this DateTime dt)
        {
            return dt.Day + " " + dt.ToString("MMM") + ", " + dt.Year;
        }
        /// <summary>
        /// As the SQL date.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public static string asSQLDate(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }
        /// <summary>
        /// To the date small string.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns></returns>
        public static string toDateSmallString(this DateTime dt)
        {
            return dt.Day.ToString("00") + " " + dt.ToString("MMM");
        }
        /// <summary>
        /// Hashes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
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
        /// <summary>
        /// As the size.
        /// </summary>
        /// <param name="byteCount">The byte count.</param>
        /// <returns></returns>
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
        /// <summary>
        /// To the json.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string toJson(this object value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }
        /// <summary>
        /// To the object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T toObject<T>(this string value)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }
        /// <summary>
        /// Normalizes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double Normalize(this double value)
        {
            return Math.Round(value, 2);
        }
        /// <summary>
        /// As the color.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static System.Drawing.Color asColor(this string value)
        {
            return System.Drawing.ColorTranslator.FromHtml(value);
        }
        /// <summary>
        /// Time the specified method.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="method">The method.</param>
        public static void Times(this int value, Action<int> method)
        {
            for (int i = 0; i < value; i++)
            {
                method(i);
            }
        }
        /// <summary>
        /// Time the specified method.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="method">The method.</param>
        public static void Times(this double value, Action<double> method)
        {
            for (int i = 0; i < value; i++)
            {
                method(i);
            }
        }
        /// <summary>
        /// Gets the week of year.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        public static int GetWeekOfYear(this DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        /// <summary>
        /// To the json.
        /// </summary>
        /// <param name="JsonViewModel">The json view model.</param>
        /// <returns></returns>
        public static async Task<JsonResult> toJson(this JsonViewModel JsonViewModel)
        {
            return await new JsonHelper().Json(JsonViewModel);
        }
        /// <summary>
        /// To the json un asynchronous.
        /// </summary>
        /// <param name="JsonViewModel">The json view model.</param>
        /// <returns></returns>
        private static JsonResult toJsonUnAsync(this JsonViewModel JsonViewModel)
        {
            return new JsonHelper().Json(JsonViewModel).Result;
        }
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object value, string propertyName)
        {
            return value.GetType().GetProperties().Single(x => x.Name == propertyName).GetValue(value, null);
        }
        /// <summary>
        /// Defaults the list value.
        /// </summary>
        /// <param name="DisplayName">The display name.</param>
        /// <param name="Selected">if set to <c>true</c> [selected].</param>
        /// <param name="DefaultValue">The default value.</param>
        /// <returns></returns>
        public static SelectListItem DefaultListValue(string DisplayName, bool Selected, string DefaultValue)
        {
            return new SelectListItem() { Selected = Selected, Text = DisplayName, Value = DefaultValue };
        }
        /// <summary>
        /// To the select list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List">The list.</param>
        /// <param name="ValueField">The value field.</param>
        /// <param name="DisplayName">The display name.</param>
        /// <param name="KeyField">The key field.</param>
        /// <param name="SelectedField">The selected field.</param>
        /// <param name="DefaultValue">The default value.</param>
        /// <returns></returns>
        public static SelectList ToSelectList<T>(this IEnumerable<T> List, string ValueField, string DisplayName = "-Select-", string KeyField = "Key", object SelectedField = null, string DefaultValue = "-1")
        {
            var SelectList = new List<SelectListItem>();
            var defaultValue = DefaultListValue(DisplayName, SelectedField == null, DefaultValue);
            SelectList.Add(defaultValue);

            List.ForEach(x =>
            {
                SelectList.Add(new SelectListItem() { Text = (string)x.GetPropertyValue(ValueField), Value = (string)x.GetPropertyValue(KeyField) });
            });

            return new SelectList(SelectList, "Value", "Text", (SelectedField == null ? "-1" : SelectedField), new List<string>() { "-1" });
        }
        /// <summary>
        /// Information's the specified message.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <param name="Type">The type.</param>
        /// <returns></returns>
        public static maQx.Models.ClientInfo Info(string Message, maQx.Models.ClientInfoType Type = maQx.Models.ClientInfoType.Success)
        {
            return new ClientInfo()
            {
                Time = DateTime.Now,
                Message = Message,
                Type = Type,
            };
        }
        /// <summary>
        /// Sets the error.
        /// </summary>
        /// <param name="TempData">The temporary data.</param>
        /// <param name="Message">The message.</param>
        /// <param name="Callback">The callback.</param>
        public static void SetError(this TempDataDictionary TempData, string Message, Action Callback = null)
        {
            TempData["Info"] = Info(Message, ClientInfoType.Error);

            if (Callback != null)
            {
                Callback();
            }
        }
        /// <summary>
        /// Sets the success.
        /// </summary>
        /// <param name="TempData">The temporary data.</param>
        /// <param name="Message">The message.</param>
        public static void SetSuccess(this TempDataDictionary TempData, string Message)
        {
            TempData["Info"] = Info(Message, ClientInfoType.Success);
        }
        /// <summary>
        /// Gets the session key.
        /// </summary>
        /// <param name="HttpContext">The HTTP context.</param>
        /// <returns></returns>
        public static string GetSessionKey(this HttpContextBase HttpContext)
        {
            return (HttpContext.Request.UserHostAddress + "" + HttpContext.Request.UserAgent).Hash();
        }
        /// <summary>
        /// Gets the session cookie.
        /// </summary>
        /// <param name="HttpContext">The HTTP context.</param>
        /// <param name="Name">The name.</param>
        /// <returns></returns>
        public static string GetSessionCookie(this HttpContextBase HttpContext, string Name)
        {
            return HttpContext.Request.Cookies[Name] != null ? HttpContext.Request.Cookies.Get(Name).Value : null;
        }
        /// <summary>
        /// Removes the session cookie.
        /// </summary>
        /// <param name="HttpContext">The HTTP context.</param>
        /// <param name="Name">The name.</param>
        public static void RemoveSessionCookie(this HttpContextBase HttpContext, string Name)
        {
            if (HttpContext.Response.Cookies[Name] != null)
                HttpContext.Response.Cookies[Name].Expires = DateTime.Now.AddDays(-1);
        }
        /// <summary>
        /// Gets the secured session cookie.
        /// </summary>
        /// <param name="HttpContext">The HTTP context.</param>
        /// <param name="Name">The name.</param>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public static string GetSecuredSessionCookie(this HttpContextBase HttpContext, string Name, string Key = null)
        {
            var Value = HttpContext.GetSessionCookie(Name);
            return String.IsNullOrWhiteSpace(Value) ? null : Value.Decrypt(String.IsNullOrWhiteSpace(Key) ? HttpContext.GetSessionKey() : Key);
        }
        /// <summary>
        /// Sets the session cookie.
        /// </summary>
        /// <param name="HttpContext">The HTTP context.</param>
        /// <param name="Name">The name.</param>
        /// <param name="Value">The value.</param>
        /// <param name="HttpOnly">if set to <c>true</c> [HTTP only].</param>
        public static void SetSessionCookie(this HttpContextBase HttpContext, string Name, string Value, bool HttpOnly = true)
        {
            HttpCookie Cookie = new HttpCookie(Name, Value);
            HttpContext.Response.Cookies.Remove(Name);
            Cookie.HttpOnly = HttpOnly;
            HttpContext.Response.SetCookie(Cookie);
        }
        /// <summary>
        /// Sets the secured session cookie.
        /// </summary>
        /// <param name="HttpContext">The HTTP context.</param>
        /// <param name="Name">The name.</param>
        /// <param name="Value">The value.</param>
        /// <param name="Key">The key.</param>
        public static void SetSecuredSessionCookie(this HttpContextBase HttpContext, string Name, string Value, string Key = null)
        {
            HttpContext.SetSessionCookie(Name, Value.Encrypt(String.IsNullOrWhiteSpace(Key) ? HttpContext.GetSessionKey() : Key));
        }
        /// <summary>
        /// Gets the roles.
        /// </summary>
        /// <param name="User">The user.</param>
        /// <returns></returns>
        public static List<string> GetRoles(this IPrincipal User)
        {
            var Roles = (((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)).ToList();

            return Roles;
        }

        /// <summary>
        /// Gets the organization.
        /// </summary>
        /// <param name="User">The user.</param>
        /// <returns></returns>
        public static string GetOrganization(this IPrincipal User)
        {
            return ((ClaimsIdentity)User.Identity).FindFirst("Organization.Key").Value;
        }

        public static string GetDivision(this IPrincipal User)
        {
            return ((ClaimsIdentity)User.Identity).FindFirst("Division.Key").Value;
        }

        public static string GetDepartment(this IPrincipal User)
        {
            return ((ClaimsIdentity)User.Identity).FindFirst("Department.Key").Value;
        }

        public static bool HasMenuAccess(this IPrincipal User, string Menu)
        {
            using (var db = new AppContext())
            {
                var Id = User.Identity.GetUserId();
                var Menus = db.MenuAccess.Include(x => x.DepartmentMenu.Menu).Where(x => x.User.Id == Id).Select(x => x.DepartmentMenu.Menu.ID).ToList();

                return Menus.Contains(Menu);
            }
        }

        public static async Task<bool> HasMenuAccessAsync(this IPrincipal User, string Menu)
        {
            using (var db = new AppContext())
            {
                var Id = User.Identity.GetUserId();
                var Menus = await db.MenuAccess.Include(x => x.DepartmentMenu.Menu).Where(x => x.User.Id == Id).Select(x => x.DepartmentMenu.Menu.ID).ToListAsync();

                return Menus.Contains(Menu);
            }
        }

        /// <summary>
        /// Gets the DataAnnotation DisplayName attribute for a given enum (for displaying enums values nicely to users)
        /// </summary>
        /// <param name="Value">Enum value to get display for</param>
        /// <returns>Pretty version of enum (if there is one)</returns>
        /// <remarks>
        public static string DisplayName(this Enum Value)
        {
            var EnumType = Value.GetType();
            var EnumValue = Enum.GetName(EnumType, Value);
            var Member = EnumType.GetMember(EnumValue)[0];

            var Attrs = Member.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false);

            if (Attrs.Any())
            {
                var DisplayAttribute = ((System.ComponentModel.DataAnnotations.DisplayAttribute)Attrs[0]);

                return DisplayAttribute.ResourceType != null ? DisplayAttribute.GetName() : DisplayAttribute.Name;
            }

            return Value.ToString();
        }

        /// <summary>
        /// Acts similar of .Include() LINQ method, but allows to include several object properties at once.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query.</param>
        /// <param name="paths">The paths.</param>
        /// <returns></returns>
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
        /// <returns>
        /// List of constant values
        /// </returns>
        public static List<T> GetConstantValues<T>(this Type type)
        {
            var fields = type.GetFields(BindingFlags.Public
                | BindingFlags.Static
                | BindingFlags.FlattenHierarchy);

            return (fields.Where(fieldInfo => fieldInfo.IsLiteral
                && !fieldInfo.IsInitOnly
                && fieldInfo.FieldType == typeof(T)).Select(fi => (T)fi.GetRawConstantValue())).ToList();
        }
    }

    /// <summary>
    ///
    /// </summary>
    public static class PostalMail
    {
        /// <summary>
        /// Gets the email service.
        /// </summary>
        /// <value>
        /// The email service.
        /// </value>
        private static EmailService EmailService
        {
            get
            {
                var engines = new ViewEngineCollection();
                engines.Add(new FileSystemRazorViewEngine(Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"))));

                return new EmailService(engines);
            }
        }
        /// <summary>
        /// Sends the admin confirmation code.
        /// </summary>
        /// <param name="MailTo">The mail to.</param>
        /// <param name="ConfirmationCode">The confirmation code.</param>
        public static void SendAdminConfrimationCode(string MailTo, string ConfirmationCode)
        {
            EmailService.SendAsync(new AdminConfirmationEmail
            {
                From = "ms@iprings.com",
                To = MailTo,
                Name = "Administrator",
                Subject = "maQx Management - Confirmation Email",
                ConfirmationCode = ConfirmationCode
            });
        }
        /// <summary>
        /// Sends the user invite.
        /// </summary>
        /// <param name="MailTo">The mail to.</param>
        /// <param name="Name">The name.</param>
        /// <param name="Position">The position.</param>
        /// <param name="OrganizationName">Name of the organization.</param>
        /// <param name="ActivationLink">The activation link.</param>
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

    /// <summary>
    ///
    /// </summary>
    public class ViewHelper
    {
        /// <summary>
        /// Lists the specified controller.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="Controller">The controller.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static async Task<JsonResult> List<T1, T2>(string Controller, Func<List<T2>, T1> operation, List<T2> data, IPrincipal User, string Role)
            where T1 : maQx.Models.JsonViewModel
            where T2 : class
        {
            if (operation != null)
            {
                return await operation(data).toJson();
            }

            return await new JsonListViewModel<T2>(data, String.IsNullOrWhiteSpace(Controller) ? null : (Role == Roles.AppUser ? TableTools.GetTools(Type.GetType("maQx.Controllers." + Controller), User.GetRoles()) : TableTools.GetTools(Type.GetType("maQx.Controllers." + Controller)))).toJson();
        }

        /// <summary>
        /// Lists the specified request.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="Request">The request.</param>
        /// <param name="Response">The response.</param>
        /// <param name="Controller">The controller.</param>
        /// <param name="Role">The role.</param>
        /// <param name="User">The user.</param>
        /// <param name="value">The value.</param>
        /// <param name="exp">The exp.</param>
        /// <param name="Includes">The includes.</param>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        public static async Task<JsonResult> List<T1, T2>(HttpRequestBase Request, HttpResponseBase Response, string Controller, string Role, IPrincipal User, DbSet<T1> value, Expression<Func<T1, bool>> exp, Expression<Func<T1, object>>[] Includes, Func<List<T1>, T2> operation = null)
            where T1 : class
            where T2 : maQx.Models.JsonViewModel
        {
            Exception Exception = null;

            try
            {
                if (typeof(T1) == typeof(Menus) || User.IsInRole(Role))
                {
                    var data = await value.IncludeMultiple(Includes).Where(exp).ToListAsync();

                    return await List<T2, T1>(Controller, operation, data, User, Role);
                }
                else
                {
                    return await JsonErrorViewModel.GetUserUnauhorizedError().toJson();
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            return await JsonExceptionViewModel.Get(Exception).toJson();
        }

        /// <summary>
        /// Formats the specified request.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <param name="Request">The request.</param>
        /// <param name="Response">The response.</param>
        /// <param name="Controller">The controller.</param>
        /// <param name="Role">The role.</param>
        /// <param name="User">The user.</param>
        /// <param name="value">The value.</param>
        /// <param name="exp">The exp.</param>
        /// <param name="Includes">The includes.</param>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        public static async Task<JsonResult> Format<T1, T2, T3>(HttpRequestBase Request, HttpResponseBase Response, string Controller, string Role, IPrincipal User, DbSet<T1> value, Expression<Func<T1, bool>> exp, Expression<Func<T1, object>>[] Includes, Func<List<T3>, T2> operation = null)
            where T1 : class
            where T2 : maQx.Models.JsonViewModel
            where T3 : class, IJsonBase<T1, T3>
        {
            Exception Exception = null;

            try
            {
                if (typeof(T1) == typeof(Menus) || User.IsInRole(Role))
                {
                    if (typeof(T1) != typeof(MenuAccess) && (Role == Roles.AppUser && !await User.HasMenuAccessAsync(Request.RequestContext.RouteData.Values["action"].ToString())))
                    {
                        return await JsonErrorViewModel.GetUserUnauhorizedError().toJson();
                    }

                    var format = Activator.CreateInstance<T3>();
                    var data = await value.IncludeMultiple(Includes).Where(exp).ToListAsync();
                    var d = data.Select(x =>
                    {
                        return format.To(x);
                    }).ToList();

                    return await List<T2, T3>(Controller, operation, d, User, Role);
                }
                else
                {
                    return await JsonErrorViewModel.GetUserUnauhorizedError().toJson();
                }
            }
            catch (Exception ex)
            {
                Exception = ex;
            }

            return await JsonExceptionViewModel.Get(Exception).toJson();
        }
    }

    /// <summary>
    ///
    /// </summary>
    internal class JsonHelper : Controller
    {
        /// <summary>
        /// Json the specified json view model.
        /// </summary>
        /// <param name="JsonViewModel">The json view model.</param>
        /// <returns></returns>
        internal async Task<JsonResult> Json(JsonViewModel JsonViewModel)
        {
            return await Task.Factory.StartNew(() => Json(JsonViewModel, JsonRequestBehavior.AllowGet));
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class AjaxOnlyAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
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

    /// <summary>
    ///
    /// </summary>
    public class AuthorizeGetViewAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
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

    public static class UnitOfMeasure
    {
        public static Mass GetBaseUnit(Units unitFrom, double Value)
        {
            switch (unitFrom)
            {
                case Units.Kgs: return Mass.FromKilograms(Value);
                case Units.Tons: return Mass.FromTonnes(Value);
                case Units.Grams: return Mass.FromGrams(Value);
                default: throw new Exception("Unable to convert specified Unit of Measures");
            }
        }

        public static double Convert(double Value, Units unitFrom, Units unitTo)
        {
            if (unitFrom == unitTo) return Value;
            if (unitFrom == Units.Nos || unitTo == Units.Nos) return Value;

            var BaseUnit = GetBaseUnit(unitFrom, Value);
            switch (unitTo)
            {
                case Units.Grams: return BaseUnit.Grams;
                case Units.Tons: return BaseUnit.Tonnes;
                case Units.Kgs: return BaseUnit.Kilograms;
                default: throw new Exception("Unable to convert specified Unit of Measures");
            }
        }
    }
}