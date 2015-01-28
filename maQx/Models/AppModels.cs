// Copyright (c) IP Rings Ltd. All rights reserved.
// Version 2.0.1. Author: Prasanth <@prashanth702> 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using maQx.Utilities;

namespace maQx.Models
{
    /// <summary>
    ///  Base class for all models under maQx.Models namespace. DateTimeStamp creates and updates CreatedAt, UpdatedAt, Timestamp and ActiveFlag properties.
    /// </summary>
    /// 

    #region BaseClasses

    public class DateTimeStamp
    {
        [Required]
        [DataType(DataType.Date)]
        [ScaffoldColumn(false)]
        public DateTime CreatedAt { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [ScaffoldColumn(false)]
        public DateTime UpdatedAt { get; set; }

        [Timestamp]
        [ScaffoldColumn(false)]
        public Byte[] TimeStamp { get; set; }

        [Required]
        [ScaffoldColumn(false)]
        public bool ActiveFlag { get; set; }

        public string Modified
        {
            get
            {
                return UpdatedAt.toDateString();
            }
        }

        public string Created
        {
            get
            {
                return UpdatedAt.toDateString();
            }
        }
    }

    public class AppBaseStamp : DateTimeStamp
    {
        [Key]
        [Column(Order = 1)]
        [Required]
        [MaxLength(40)]
        [ScaffoldColumn(false)]
        public string Key { get; set; }

        [Required]
        [ScaffoldColumn(false)]
        public string UserCreated { get; set; }

        [Required]
        [ScaffoldColumn(false)]
        public string UserModified { get; set; }
    }

    #endregion

    public class Department
    {
        [Key]
        [Required]
        public string ID { get; set; }

        [Index("IX_Name", IsUnique = true)]
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public string Access { get; set; }       
    }

    public class Menus
    {
        [Key]
        [Required]
        public string ID { get; set; }

        [Index("IX_Name", IsUnique = true)]
        [Required]
        [MaxLength(50), MinLength(2)]
        public string Name { get; set; }

        [Required]
        public string Access { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public bool IsMappable { get; set; }        
    }

    #region RegistrationClasses

    public class IntilizationStep : DateTimeStamp
    {
        [Key]
        [Required]
        [MinLength(16), MaxLength(32)]
        public string Code { get; set; }

        [Required]
        public string Mode { get; set; }

        [Required]
        public int Auth { get; set; }
    }

    public class AdminRegistrationBase : DateTimeStamp
    {
        public AdminRegistrationBase()
        {
            ResendActivity = false;
            ConfirmationCode = String.Empty;
        }

        [Required]
        [Key, ForeignKey("Step")]
        public string StepCode { get; set; }

        public virtual IntilizationStep Step { get; set; }

        [Required]
        [MinLength(5), MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public bool ResendActivity { get; set; }

        [Required]
        public string ConfirmationCode { get; set; }

        [Required]
        public string Role { get; set; }
    }

    #endregion

    public class Organization : AppBaseStamp
    {
        [Index("IX_OrganizationCode", 1, IsUnique = true)]
        [Required]
        [MaxLength(50), MinLength(2)]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [Index("IX_Domain", 1, IsUnique = true)]
        [MinLength(5), MaxLength(50)]
        public string Domain { get; set; }

        public ICollection<Administrator> Administrators { get; set; }
        public ICollection<Plant> Plants { get; set; }
    }

    public class Invite : AppBaseStamp
    {
        [Index("IX_Username", IsUnique = true)]
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MinLength(5), MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public virtual Organization Organization { get; set; }

        [Required]
        public string Role { get; set; }       
    }

    public class Administrator : AppBaseStamp
    {
        [Required]
        [Index("IX_OrganizationAdministrator", 1, IsUnique = true)]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [Index("IX_OrganizationAdministrator", 2, IsUnique = true)]
        public virtual Organization Organization { get; set; }

        [Required]
        public string Role { get; set; }
    }

    public class Plant : AppBaseStamp
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Location { get; set; }

        [Required]       
        public virtual Organization Organization { get; set; }
        public ICollection<Division> Divisions { get; set; }
    }

    public class Division : AppBaseStamp
    {
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public virtual Plant Plant { get; set; }

        ICollection<Department> Departments { get; set; }        
    }

    public class DepartmentMenu : AppBaseStamp
    {
        [Required]       
        [Index("IX_DivisionMenu", 1, IsUnique = true)]
        public virtual Division Division { get; set; }

        [Required]       
        public virtual Department Department { get; set; }

        [Required]       
        [Index("IX_DivisionMenu", 2, IsUnique = true)]
        public virtual Menus Menu { get; set; }
    }

    public class DepartmentUser : AppBaseStamp
    {
        [Required]        
        public virtual Division Division { get; set; }

        [Required]
        public virtual Department Department { get; set; }

        [Required]
        [Index("IX_DivisionUser", IsUnique = true)]
        public virtual ApplicationUser User { get; set; }
    }

    public class AppUser : AppBaseStamp
    {
        [Required]
        [Index("IX_AppUser", IsUnique = true)]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public virtual Division Division { get; set; }

        [Required]
        public virtual Department Department { get; set; }
    }
}