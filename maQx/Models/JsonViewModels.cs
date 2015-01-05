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

    public interface IJsonBase<T1, T2>
    {
        T2 To(T1 Value);
    }

    public class JsonBase
    {
        public string Key { get; set; }
        public string Modified { get; set; }
    }

    public class JOrganization : JsonBase, IJsonBase<Organization, JOrganization>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }

        public JOrganization To(Organization input)
        {
            return new JOrganization
            {
                Key = input.Key,
                Code = input.Code,
                Name = input.Name,
                Domain = input.Domain,
                Modified = input.Modified
            };
        }
    }

    public class JMenus : IJsonBase<Menus, JMenus>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        public JMenus To(Menus input)
        {
            return new JMenus
            {
                ID = input.ID,
                Name = input.Name,
                Order = input.Order
            };
        }
    }

    public class JPlant : JsonBase, IJsonBase<Plant, JPlant>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public JOrganization Organization { get; set; }

        public JPlant To(Plant input)
        {
            return new JPlant
            {
                Key = input.Key,
                Code = input.Code,
                Name = input.Name,
                Location = input.Location,
                Modified = input.Modified,
                Organization = new JOrganization().To(input.Organization)
            };
        }
    }

    public class JInvite : JsonBase, IJsonBase<Invite, JInvite>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public JOrganization Organization { get; set; }

        public JInvite To(Invite input)
        {
            return new JInvite
            {
                Key = input.Key,
                Username = input.Username,
                Email = input.Email,
                Modified = input.Modified,
                Organization = new JOrganization().To(input.Organization)
            };
        }
    }
}