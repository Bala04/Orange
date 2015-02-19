using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace maQx.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class JsonViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonViewModel"/> class.
        /// </summary>
        /// <param name="Type">The type.</param>
        public JsonViewModel(string Type)
        {
            this.Type = Type;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JsonItemViewModel : JsonViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonItemViewModel"/> class.
        /// </summary>
        /// <param name="Status">The status.</param>
        public JsonItemViewModel(string Status)
            : base(Status)
        {

        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is viewable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is viewable; otherwise, <c>false</c>.
        /// </value>
        public bool IsViewable { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is editable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is editable; otherwise, <c>false</c>.
        /// </value>
        public bool IsEditable { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is delete able.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is delete able; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleteable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow create].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow create]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowCreate { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JsonCurrentUserViewModel : JsonViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCurrentUserViewModel"/> class.
        /// </summary>
        public JsonCurrentUserViewModel()
            : base("SUCCESS")
        {

        }


        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the designation.
        /// </summary>
        /// <value>
        /// The designation.
        /// </value>
        public string Designation { get; set; }
        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        public string Department { get; set; }
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public string Role { get; set; }
        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public string ImgURL { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonViewModel<T> : JsonViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonViewModel{T}"/> class.
        /// </summary>
        public JsonViewModel()
            : base("SUCCESS")
        {

        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JsonMenuViewModel : JsonViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMenuViewModel"/> class.
        /// </summary>
        public JsonMenuViewModel()
            : base("SUCCESS")
        {

        }

        /// <summary>
        /// Gets or sets the menus.
        /// </summary>
        /// <value>
        /// The menus.
        /// </value>
        public List<JMenus> Menus { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JsonListViewModel<T> : JsonItemViewModel where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonListViewModel{T}"/> class.
        /// </summary>
        public JsonListViewModel()
            : base("SUCCESS")
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonListViewModel{T}"/> class.
        /// </summary>
        /// <param name="List">The list.</param>
        /// <param name="Values">The values.</param>
        public JsonListViewModel(List<T> List, Tuple<bool, bool, bool, bool> Values)
            : base("SUCCESS")
        {
            this.List = List;

            if (Values != null)
            {
                IsViewable = Values.Item1;
                IsEditable = Values.Item2;
                IsDeleteable = Values.Item3;
                AllowCreate = Values.Item4;
            }
        }

        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>
        /// The list.
        /// </value>
        public List<T> List { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JsonErrorViewModel : JsonViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonErrorViewModel"/> class.
        /// </summary>
        public JsonErrorViewModel()
            : base("ERROR")
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonErrorViewModel"/> class.
        /// </summary>
        /// <param name="Status">The status.</param>
        public JsonErrorViewModel(string Status)
            : base(Status)
        {

        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the return URL.
        /// </summary>
        /// <value>
        /// The return URL.
        /// </value>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets the user unauthorized error.
        /// </summary>
        /// <returns></returns>
        public static JsonErrorViewModel GetUserUnauhorizedError()
        {
            return new JsonErrorViewModel()
            {
                Message = "Requested user is not a authorized user.",
                ReturnUrl = "/Login",
                Status = "CRITICAL"
            };
        }

        /// <summary>
        /// Gets the resource not found error.
        /// </summary>
        /// <param name="Response">The response.</param>
        /// <returns></returns>
        public static JsonErrorViewModel GetResourceNotFoundError(HttpResponseBase Response = null)
        {
            if (Response != null)
                Response.StatusCode = 400;

            return new JsonErrorViewModel()
            {
                Message = "Requested resource is not found.",
                Status = "CRITICAL"
            };
        }

        /// <summary>
        /// Gets the data not found error.
        /// </summary>
        /// <param name="Response">The response.</param>
        /// <returns></returns>
        public static JsonErrorViewModel GetDataNotFoundError(HttpResponseBase Response = null)
        {
            if (Response != null)
                Response.StatusCode = 404;

            return new JsonErrorViewModel()
            {
                Message = "Requested resource is not found.",
                Status = "CRITICAL"
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JsonExceptionViewModel : JsonErrorViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonExceptionViewModel"/> class.
        /// </summary>
        public JsonExceptionViewModel()
            : base("EXCEPTION")
        {

        }
        /// <summary>
        /// Gets the specified input.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns></returns>
        public static JsonErrorViewModel Get(Exception Input)
        {
            while (Input.InnerException != null)
            {
                Input = Input.InnerException;
            }

            return Input != null ? new JsonExceptionViewModel()
            {
                Message = Input.Message,
                Title = "Runtime Exception",
                Status = "HANDLED"
            } : new JsonExceptionViewModel()
            {
                Message = "An error occurred while process your request.",
                Title = "Runtime Exception",
                Status = "HANDLED"
            };
        }
    }
}