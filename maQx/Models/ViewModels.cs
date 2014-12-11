using maQx.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace maQx.Models
{
    public class BaseLoginViewModel
    {
        [Required]
        [Display(Name = "Username")]      
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]      
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class LoginViewModel : BaseLoginViewModel
    {
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string _ReturnUrl { get; set; }
    }   

    public class AdminRegisterViewModel
    {
        [Required]
        [RegularExpression(RegExPatterns.AlphaNumericWithOutSpace, ErrorMessage = "Please enter a valid employee code.")]
        [Display(Name = "User Code")]
        [MaxLength(100)]
        public string Code { get; set; }

        [Required]
        [RegularExpression(RegExPatterns.Name, ErrorMessage = "Please enter a valid name.")]
        [Display(Name = "Name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Contact Number")]
        [RegularExpression(RegExPatterns.PhoneNumber, ErrorMessage = "Please enter a valid contact number.")]
        [StringLength(10, ErrorMessage = "Please enter a valid contact number.")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Password")]
        [MinLength(6, ErrorMessage = "Password should have minimum 6 characters."), MaxLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string StepCode { get; set; }
    }

    public class RegisterViewModel : AdminRegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        [RegularExpression(RegExPatterns.Username, ErrorMessage = "Please enter a valid username.")]
        [MaxLength(100)]  
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        [RegularExpression(RegExPatterns.Email, ErrorMessage = "Please enter a valid email address.")]
        [MinLength(5, ErrorMessage = "Please enter a valid email address."), MaxLength(100)]        
        public string Email { get; set; }       
    }

    public class OrganizationViewModel 
    {       
        [Required]
        [MaxLength(50)]
        [Display(Name = "Organization Code")]
        [RegularExpression(RegExPatterns.AlphaNumeric, ErrorMessage = "Please enter a valid organization code.")]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Organization Name")]
        [RegularExpression(RegExPatterns.AlphaNumeric, ErrorMessage = "Please enter a valid organization name.")]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Domain Address")]
        [RegularExpression(RegExPatterns.Domain, ErrorMessage = "Please enter a valid domain name.")]
        public string Domain { get; set; }
    }

    public class OrganizationEditViewModel : OrganizationViewModel
    {
        [Required]
        public string Key { get; set; }
    }

    public class InviteViewModel
    {
        [Required]
        [Display(Name = "Username")]
        [RegularExpression(RegExPatterns.Username, ErrorMessage = "Please enter a valid username.")]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        [RegularExpression(RegExPatterns.Email, ErrorMessage = "Please enter a valid email address.")]
        [MinLength(5, ErrorMessage = "Please enter a valid email address."), MaxLength(100)]
        public string Email { get; set; }   

        [Required]
        public string Organization { get; set; }

        public System.Web.Mvc.SelectList Organizations { get; set; }
    }
    
    
    public enum ClientInfoType
    {
        Error,
        Success,
        Warning,
        Info
    }

    public class ClientInfo
    {
        public DateTime Time { get; set; }
        public ClientInfoType Type { get; set; }
        public string Message { get; set; }          
    }    

    public class GetEmailConfirmViewModel
    {
        public GetEmailConfirmViewModel()
        {
            ResendActivity = false;
        }

        [Required]
        public string StepCode { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(RegExPatterns.Email, ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        [MinLength(5, ErrorMessage = "Please enter a valid email address."), MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public bool ResendActivity { get; set; }
    }

    public class SetEmailConfirmViewModel : GetEmailConfirmViewModel
    {
        [Required(ErrorMessage = "Please enter a vaild confirmation code.")]
        public string ConfirmationCode { get; set; }
    }

    public class InitViewModel
    {
        public GetEmailConfirmViewModel GetEmail { get; set; }
        public SetEmailConfirmViewModel SetEmail { get; set; }
        public AdminRegisterViewModel AdminModel { get; set; }
    }

}