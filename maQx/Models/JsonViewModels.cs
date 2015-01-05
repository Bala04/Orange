using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace maQx.Models
{
    public class JsonViewModel
    {
        public JsonViewModel(string Type)
        {
            this.Type = Type;
        }

        public string Type { get; private set; }
    }

    public class JsonItemViewModel : JsonViewModel
    {
        public JsonItemViewModel(string Status)
            : base(Status)
        {

        }

        public bool IsViewable { get; set; }
        public bool IsEditable { get; set; }
        public bool IsDeleteable { get; set; }
    }

    public class JsonCurrentUserViewModel : JsonViewModel
    {
        public JsonCurrentUserViewModel()
            : base("SUCCESS")
        {

        }


        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public string ImgURL { get; set; }
    }

    public class JsonViewModel<T> : JsonViewModel
    {
        public JsonViewModel()
            : base("SUCCESS")
        {

        }

        public T Value { get; set; }
    }

    public class JsonMenuViewModel : JsonViewModel
    {
        public JsonMenuViewModel()
            : base("SUCCESS")
        {

        }

        public List<JMenus> Menus { get; set; }
    }

    public class JsonListViewModel<T> : JsonItemViewModel where T : class
    {
        public JsonListViewModel()
            : base("SUCCESS")
        {

        }

        public JsonListViewModel(List<T> List, Tuple<bool, bool, bool> Values)
            : base("SUCCESS")
        {
            this.List = List;

            if (Values != null)
            {
                IsViewable = Values.Item1;
                IsEditable = Values.Item2;
                IsDeleteable = Values.Item3;
            }
        }

        public List<T> List { get; set; }
    }

    public class JsonErrorViewModel : JsonViewModel
    {
        public JsonErrorViewModel()
            : base("ERROR")
        {

        }

        public JsonErrorViewModel(string Status)
            : base(Status)
        {

        }

        public string Status { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public string ReturnUrl { get; set; }

        public static JsonErrorViewModel GetUserUnauhorizedError()
        {
            return new JsonErrorViewModel()
            {
                Message = "Requested user is not a authorized user.",
                ReturnUrl = "/Login",
                Status = "CRITICAL"
            };
        }

        public static JsonErrorViewModel GetResourceNotFoundError()
        {
            return new JsonErrorViewModel()
            {
                Message = "Requested resource is not found.",
                Status = "CRITICAL"
            };
        }
    }

    public class JsonExceptionViewModel : JsonErrorViewModel
    {
        public JsonExceptionViewModel()
            : base("EXCEPTION")
        {

        }

        public Exception Exception { get; set; }
        public static JsonErrorViewModel Get(Exception Exception)
        {
            return new JsonExceptionViewModel()
            {
                Message = "An error occured while process your request.",
                Title = "Unhandled Exception",
                Exception = Exception,
                Status = "HANDLED"
            };
        }
    }   
}