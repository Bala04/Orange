using maQx.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace maQx.Models
{
    /// <summary>
    ///
    /// </summary>
    public class BaseLoginViewModel
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [Required, Display(Name = "Username")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required, Display(Name = "Password"), DataType(DataType.Password)]
        public string Password { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class LoginViewModel : BaseLoginViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether [remember me].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [remember me]; otherwise, <c>false</c>.
        /// </value>
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        /// <summary>
        /// Gets or sets the _ return URL.
        /// </summary>
        /// <value>
        /// The _ return URL.
        /// </value>
        public string _ReturnUrl { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class AdminRegisterViewModel
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required, Display(Name = "User Code"), MaxLength(100), RegularExpression(RegExPatterns.AlphaNumericWithOutSpace, ErrorMessage = "Please enter a valid employee code.")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required, Display(Name = "Name"), MaxLength(100), RegularExpression(RegExPatterns.Name, ErrorMessage = "Please enter a valid name.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>
        /// The phone.
        /// </value>
        [Required, DataType(DataType.PhoneNumber), Display(Name = "Contact Number"), RegularExpression(RegExPatterns.PhoneNumber, ErrorMessage = "Please enter a valid contact number."), StringLength(10, ErrorMessage = "Please enter a valid contact number.")]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required, Display(Name = "Password"), MinLength(6, ErrorMessage = "Password should have minimum 6 characters."), MaxLength(100), DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the confirm password.
        /// </summary>
        /// <value>
        /// The confirm password.
        /// </value>
        [Required, DataType(DataType.Password), Display(Name = "Confirm Password"), Compare("Password")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Gets or sets the step code.
        /// </summary>
        /// <value>
        /// The step code.
        /// </value>
        [Required]
        public string StepCode { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class RegisterViewModel : AdminRegisterViewModel
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [Required, Display(Name = "Username"), RegularExpression(RegExPatterns.Username, ErrorMessage = "Please enter a valid user name."), MaxLength(100)]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required, DataType(DataType.EmailAddress), Display(Name = "Email Address"), RegularExpression(RegExPatterns.Email, ErrorMessage = "Please enter a valid email address."), MinLength(5, ErrorMessage = "Please enter a valid email address."), MaxLength(100)]
        public string Email { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class OrganizationViewModel
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required, MaxLength(50), Display(Name = "Organization Code"), RegularExpression(RegExPatterns.AlphaNumeric, ErrorMessage = "Please enter a valid organization code.")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required, MaxLength(50), Display(Name = "Organization Name"), RegularExpression(RegExPatterns.AlphaNumeric, ErrorMessage = "Please enter a valid organization name.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        [Required, MaxLength(100), Display(Name = "Domain Address"), RegularExpression(RegExPatterns.Domain, ErrorMessage = "Please enter a valid domain name.")]
        public string Domain { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class OrganizationEditViewModel : OrganizationViewModel
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [Required]
        public string Key { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class PlantEditViewModel : PlantViewModel
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [Required]
        public string Key { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class DivisionEditViewModel : DivisionViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionEditViewModel"/> class.
        /// </summary>
        public DivisionEditViewModel() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionEditViewModel"/> class.
        /// </summary>
        /// <param name="division">The division.</param>
        /// <param name="Plants">The plants.</param>
        public DivisionEditViewModel(Division division, System.Web.Mvc.SelectList Plants)
        {
            Key = division.Key;
            Code = division.Code;
            Name = division.Name;
            Plant = division.Plant.Key;
            this.Plants = Plants;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [Required]
        public string Key { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class OrganizationBaseViewModel
    {
        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>
        /// The organization.
        /// </value>
        [Required]
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the organizations.
        /// </summary>
        /// <value>
        /// The organizations.
        /// </value>
        public System.Web.Mvc.SelectList Organizations { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class DepartmentDivisionViewModel
    {
        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        public string Division { get; set; }

        /// <summary>
        /// Gets or sets the divisions.
        /// </summary>
        /// <value>
        /// The divisions.
        /// </value>
        public System.Web.Mvc.SelectList Divisions { get; set; }
        /// <summary>
        /// Gets or sets the j controller.
        /// </summary>
        /// <value>
        /// The j controller.
        /// </value>
        public string JController { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class PlantBaseViewModel
    {
        /// <summary>
        /// Gets or sets the plant.
        /// </summary>
        /// <value>
        /// The plant.
        /// </value>
        [Required]
        public string Plant { get; set; }

        /// <summary>
        /// Gets or sets the plants.
        /// </summary>
        /// <value>
        /// The plants.
        /// </value>
        public System.Web.Mvc.SelectList Plants { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class InviteViewModel : OrganizationBaseViewModel
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [Required, Display(Name = "Username"), RegularExpression(RegExPatterns.Username, ErrorMessage = "Please enter a valid user name."), MaxLength(100)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required, DataType(DataType.EmailAddress), Display(Name = "Email Address"), RegularExpression(RegExPatterns.Email, ErrorMessage = "Please enter a valid email address."), MinLength(5, ErrorMessage = "Please enter a valid email address."), MaxLength(100)]
        public string Email { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class PlantViewModel
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required, MaxLength(50), Display(Name = "Plant Code"), RegularExpression(RegExPatterns.AlphaNumeric, ErrorMessage = "Please enter a valid plant code.")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required, MaxLength(50), Display(Name = "Plant Name"), RegularExpression(RegExPatterns.SpecialAlphaNumeric, ErrorMessage = "Please enter a valid plant name.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [Required, Display(Name = "Plant Name"), MaxLength(50)]
        public string Location { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class DivisionViewModel : PlantBaseViewModel
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required, MaxLength(50), Display(Name = "Division Code"), RegularExpression(RegExPatterns.AlphaNumeric, ErrorMessage = "Please enter a valid division code.")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required, MaxLength(50), Display(Name = "Division Name"), RegularExpression(RegExPatterns.SpecialAlphaNumeric, ErrorMessage = "Please enter a valid division name.")]
        public string Name { get; set; }

        public DivisionViewModel() { }

        public DivisionViewModel(Division division, System.Web.Mvc.SelectList Plants)
        {
            Code = division.Code;
            Name = division.Name;
            Plant = division.Plant.Key;
            this.Plants = Plants;
        }

    }

    /// <summary>
    ///
    /// </summary>
    public enum ClientInfoType
    {
        /// <summary>
        /// The error
        /// </summary>
        Error,
        /// <summary>
        /// The success
        /// </summary>
        Success,
        /// <summary>
        /// The warning
        /// </summary>
        Warning,
        /// <summary>
        /// The information
        /// </summary>
        Info
    }

    /// <summary>
    ///
    /// </summary>
    public class ClientInfo
    {
        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public DateTime Time { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public ClientInfoType Type { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class GetEmailConfirmViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetEmailConfirmViewModel"/> class.
        /// </summary>
        public GetEmailConfirmViewModel()
        {
            ResendActivity = false;
        }

        /// <summary>
        /// Gets or sets the step code.
        /// </summary>
        /// <value>
        /// The step code.
        /// </value>
        [Required]
        public string StepCode { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required, DataType(DataType.EmailAddress), RegularExpression(RegExPatterns.Email, ErrorMessage = "Please enter a valid email address."), Display(Name = "Email Address"), MinLength(5, ErrorMessage = "Please enter a valid email address."), MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [resend activity].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [resend activity]; otherwise, <c>false</c>.
        /// </value>
        [Required]
        public bool ResendActivity { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class SetEmailConfirmViewModel : GetEmailConfirmViewModel
    {
        /// <summary>
        /// Gets or sets the confirmation code.
        /// </summary>
        /// <value>
        /// The confirmation code.
        /// </value>
        [Required(ErrorMessage = "Please enter a valid confirmation code.")]
        public string ConfirmationCode { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class InitViewModel
    {
        /// <summary>
        /// Gets or sets the get email.
        /// </summary>
        /// <value>
        /// The get email.
        /// </value>
        public GetEmailConfirmViewModel GetEmail { get; set; }
        /// <summary>
        /// Gets or sets the set email.
        /// </summary>
        /// <value>
        /// The set email.
        /// </value>
        public SetEmailConfirmViewModel SetEmail { get; set; }
        /// <summary>
        /// Gets or sets the admin model.
        /// </summary>
        /// <value>
        /// The admin model.
        /// </value>
        public AdminRegisterViewModel AdminModel { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>
        /// The organization.
        /// </value>
        public string Organization { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [show organization].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show organization]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowOrganization { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class DepartmentViewModel : DepartmentDivisionViewModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public string Name { get; set; }
    }

    public class ProductRawMaterialViewModel
    {
        public string Product { get; set; }
        public System.Web.Mvc.SelectList Products { get; set; }

        public string RawMaterial { get; set; }
        public System.Web.Mvc.SelectList RawMaterials { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class DepartmentEditViewModel : DepartmentViewModel
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [Required]
        public string Key { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class AccessLevelViewModel
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>
        /// The users.
        /// </value>
        public System.Web.Mvc.SelectList Users { get; set; }
    }

    public class RawMaterialViewModel
    {
        [Required, MaxLength(50), Display(Name = "Raw Material Code")]
        public string Code { get; set; }

        [Required, MaxLength(50), Display(Name = "Raw Material Description")]
        public string Description { get; set; }

        [Required, Range(1, 4, ErrorMessage = "Unit Of Measure field is required."), Display(Name = "Unit Of Measure")]
        public Units Unit { get; set; }

        [Required, Range(1, 3, ErrorMessage = "Measurement Type field is required."), Display(Name = "Measurement Type")]
        public Measurements Measurement { get; set; }

        public RawMaterialViewModel() { }
        public RawMaterialViewModel(RawMaterial input)
        {
            Code = input.Code;
            Description = input.Description;
            Unit = input.Unit;
            Measurement = input.Measurement;
        }
    }

    public class RawMaterialEditViewModel : RawMaterialViewModel
    {
        [Required]
        public string Key { get; set; }

        public RawMaterialEditViewModel() { }
        public RawMaterialEditViewModel(RawMaterial input)
            : base(input)
        {
            Key = input.Key;
        }
    }

    public class ProductViewModel
    {
        [Required, MaxLength(50), Display(Name = "Product Code")]
        public string Code { get; set; }

        [Required, MaxLength(50), Display(Name = "Product Description")]
        public string Description { get; set; }

        public ProductViewModel() { }
        public ProductViewModel(Product input)
        {
            Code = input.Code;
            Description = input.Description;
        }
    }

    public class ProductEditViewModel : ProductViewModel
    {
        [Required]
        public string Key { get; set; }

        public ProductEditViewModel() { }
        public ProductEditViewModel(Product input)
            : base(input)
        {
            Key = input.Key;
        }
    }

    public class ProcessViewModel
    {
        [Required, MaxLength(50), Display(Name = "Process Code")]
        public string Code { get; set; }

        [Required, MaxLength(50), Display(Name = "Process Description")]
        public string Description { get; set; }

        [Required, Display(Name = "Validate Raw Material")]
        public bool ValidateRawMaterial { get; set; }

        public ProcessViewModel() { }
        public ProcessViewModel(Process input)
        {
            Code = input.Code;
            Description = input.Description;
            ValidateRawMaterial = input.ValidateRawMaterial;
        }
    }

    public class ProcessEditViewModel : ProcessViewModel
    {
        [Required]
        public string Key { get; set; }

        public ProcessEditViewModel() { }
        public ProcessEditViewModel(Process input)
            : base(input)
        {
            Key = input.Key;
        }
    }

    public class ToolDieBaseViewModel
    {
        [Required, Display(Name = "Life Count"), RegularExpression(RegExPatterns.Numeric, ErrorMessage = "Please enter a valid life count."), Range(1, int.MaxValue, ErrorMessage = "Please enter a valid life count.")]
        public int MaxCount { get; set; }

        [Required, Display(Name = "Tolerance"), RegularExpression(RegExPatterns.Numeric, ErrorMessage = "Please enter a valid tolerance."), Range(0, int.MaxValue, ErrorMessage = "Please enter a valid tolerance.")]
        public int Tolerance { get; set; }

        public ToolDieBaseViewModel() { }
        public ToolDieBaseViewModel(ToolDieBase input)
        {
            MaxCount = input.MaxCount;
            Tolerance = input.Tolerance;
        }
    }

    public class ToolViewModel : ToolDieBaseViewModel
    {
        [Required, MaxLength(50), Display(Name = "Tool Code")]
        public string Code { get; set; }

        [Required, MaxLength(50), Display(Name = "Tool Description")]
        public string Description { get; set; }

        public ToolViewModel() { }
        public ToolViewModel(Tool input)
            : base(input)
        {
            Code = input.Code;
            Description = input.Description;
        }
    }

    public class ToolEditViewModel : ToolViewModel
    {
        [Required]
        public string Key { get; set; }

        public ToolEditViewModel() { }
        public ToolEditViewModel(Tool input)
            : base(input)
        {
            Key = input.Key;
        }
    }

    public class DieViewModel : ToolDieBaseViewModel
    {
        [Required, MaxLength(50), Display(Name = "Die Code")]
        public string Code { get; set; }

        [Required, MaxLength(50), Display(Name = "Die Description")]
        public string Description { get; set; }

        [Required, Display(Name = "Sink Count"), RegularExpression(RegExPatterns.Numeric, ErrorMessage = "Please enter a valid sink count."), Range(0, int.MaxValue, ErrorMessage = "Please enter a valid sink count.")]
        public int MaxSink { get; set; }

        public DieViewModel() { }
        public DieViewModel(Die input)
            : base(input)
        {
            Code = input.Code;
            Description = input.Description;
            MaxSink = input.MaxSink;
        }
    }

    public class DieEditViewModel : DieViewModel
    {
        [Required]
        public string Key { get; set; }

        public DieEditViewModel() { }
        public DieEditViewModel(Die input)
            : base(input)
        {
            Key = input.Key;
        }
    }

    public class ProductRawMaterialCreateViewModel
    {
        [Required]
        public string Product { get; set; }

        [Required, Display(Name = "Raw Material")]
        public string RawMaterial { get; set; }

        [Required, Range(typeof(decimal), "0.0000000001", "2147483647", ErrorMessage = "Enter a valid raw material norm value"), Display(Name = "Raw Material Norms"), RegularExpression(RegExPatterns.Decimal, ErrorMessage = "Enter a valid raw material norm value")]
        public string InputQuantity { get; set; }

        [Required, Range(1, 4, ErrorMessage = "Unit Of Measure field is required."), Display(Name = "Unit Of Measure")]
        public Units Unit { get; set; }

        public System.Web.Mvc.SelectList RawMaterials { get; set; }
        public System.Web.Mvc.SelectList Products { get; set; }
    }

    public class ProductRawMaterialEditViewModel : ProductRawMaterialCreateViewModel
    {
        [Required]
        public string Key { get; set; }

        public ProductRawMaterialEditViewModel() { }
        public ProductRawMaterialEditViewModel(ProductRawMaterial ProductRawMaterial)
        {
            Key = ProductRawMaterial.Key;
            ProductItem = ProductRawMaterial.Product;
            RawMaterialItem = ProductRawMaterial.RawMaterial;
            InputQuantity = ProductRawMaterial.InputQuantity.ToString();
            Unit = ProductRawMaterial.SelectedUnit;
            Product = ProductRawMaterial.Product.Key;
            RawMaterial = ProductRawMaterial.RawMaterial.Key;
        }

        public Product ProductItem { get; set; }
        public RawMaterial RawMaterialItem { get; set; }
    }

    public class ProductProcessViewModel
    {
        public string Product { get; set; }
        public System.Web.Mvc.SelectList Products { get; set; }
    }

    public class MachineViewModel
    {
        public MachineViewModel() { }
        public MachineViewModel(Machine input)
        {
            Code = input.Code;
            Description = input.Description;
            MachineType = input.MachineType;
            MinLoad = input.MinLoad;
            MaxLoad = input.MaxLoad;
        }

        [Required, MaxLength(50), Display(Name = "Machine Code")]
        public string Code { get; set; }

        [Required, MaxLength(50), Display(Name = "Machine Description")]
        public string Description { get; set; }

        [Required, Display(Name = "Min Load"), RegularExpression(RegExPatterns.Numeric, ErrorMessage = "Please enter a valid min load."), Range(0, int.MaxValue, ErrorMessage = "Please enter a valid min load.")]
        public double MinLoad { get; set; }

        [Required, Display(Name = "Max Load"), RegularExpression(RegExPatterns.Numeric, ErrorMessage = "Please enter a valid max load."), Range(0, int.MaxValue, ErrorMessage = "Please enter a valid max load.")]
        public double MaxLoad { get; set; }

        [Required, Range(1, 2, ErrorMessage = "Machine Type field is required."), Display(Name = "Machine Type")]
        public MachineTypes MachineType { get; set; }
    }

    public class MachineEditViewModel : MachineViewModel
    {
        [Required]
        public string Key { get; set; }

        public MachineEditViewModel() { }
        public MachineEditViewModel(Machine input)
            : base(input)
        {
            Key = input.Key;
        }
    }

    public class ShiftViewModel
    {
        [Required, Display(Name = "Shift Name")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Select a valid Start Time"), DataType(DataType.Time)]
        public double StartTime { get; set; }
        [Required(ErrorMessage = "Select a valid End Time"), DataType(DataType.Time)]
        public double EndTime { get; set; }

        public ShiftViewModel() { }
        public ShiftViewModel(Shift input)
        {
            Description = input.Description;
            StartTime = input.StartTime.ToUnixTime();
            EndTime = input.EndTime.ToUnixTime();
        }
    }

    public class ShiftEditViewModel : ShiftViewModel
    {
        [Required]
        public string Key { get; set; }

         public ShiftEditViewModel() { }
         public ShiftEditViewModel(Shift input)
            : base(input)
        {
            Key = input.Key;
        }
    }
}