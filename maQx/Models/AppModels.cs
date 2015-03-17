// Copyright (c) IP Rings Ltd. All rights reserved.
// Version 2.0.1. Author: Prasanth <@prashanth702> 

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using maQx.Utilities;

/// <summary>
///
/// </summary>
namespace maQx.Models
{
    /// <summary>
    /// Base class for all models under maQx.Models namespace. DateTimeStamp creates and updates CreatedAt, UpdatedAt, Timestamp and ActiveFlag properties.
    /// </summary>

    #region Enums
    public enum Units { Nos = 1, Kgs = 2, Grams = 3, Tons = 4 };
    /// <summary>
    ///
    /// </summary>
    public enum Measurements { Pieces = 1, Meters = 2, Others = 3 };
    /// <summary>
    ///
    /// </summary>
    public enum MachineTypes { Machine = 1, Furnace = 2 };
    /// <summary>
    ///
    /// </summary>
    public enum DowntimeCategories { ManagementLoss = 1, Planned = 2, UnPlanned = 3 };
    #endregion

    #region BaseClasses
    /// <summary>
    ///
    /// </summary>
    public class DateTimeStamp
    {
        /// <summary>
        /// Gets or sets the created at.
        /// </summary>
        /// <value>
        /// The created at.
        /// </value>
        [Required, DataType(DataType.Date), ScaffoldColumn(false)]
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>
        /// The updated at.
        /// </value>
        [Required, DataType(DataType.Date), ScaffoldColumn(false)]
        public DateTime UpdatedAt { get; set; }
        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [Timestamp, ScaffoldColumn(false)]
        public Byte[] TimeStamp { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [active flag].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [active flag]; otherwise, <c>false</c>.
        /// </value>
        [Required, ScaffoldColumn(false)]
        public bool ActiveFlag { get; set; }
        /// <summary>
        /// Gets the modified.
        /// </summary>
        /// <value>
        /// The modified.
        /// </value>
        public string Modified
        {
            get
            {
                return UpdatedAt.toDateString();
            }
        }
        /// <summary>
        /// Gets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public string Created
        {
            get
            {
                //BUG: return UpdatedAt.toDateString();
                //FIX: Created should return CreatedAt 31/01/2015
                return CreatedAt.toDateString();
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class AppBaseStamp : DateTimeStamp
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [Key, Column(Order = 1), Required, MaxLength(40), ScaffoldColumn(false)]
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the user created.
        /// </summary>
        /// <value>
        /// The user created.
        /// </value>
        [Required, ScaffoldColumn(false)]
        public string UserCreated { get; set; }
        /// <summary>
        /// Gets or sets the user modified.
        /// </summary>
        /// <value>
        /// The user modified.
        /// </value>
        [Required, ScaffoldColumn(false)]
        public string UserModified { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class DivisionBase : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [Required]
        public virtual Division Division { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class EntityDivisionBase : DivisionBase
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Required]
        public string Description { get; set; }
    }

    #endregion

    #region Default
    /// <summary>
    ///
    /// </summary>
    public class Menus
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key, Required]
        public string ID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required, Index("IX_Name", IsUnique = true), MaxLength(50), MinLength(2)]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        [Required]
        public string Access { get; set; }
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        [Required]
        public int Order { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is mappable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is mappable; otherwise, <c>false</c>.
        /// </value>
        [Required]
        public bool IsMappable { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ContentType
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [Key, Required]
        public string ID { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Required]
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        [Required]
        public string Category { get; set; }
        /// <summary>
        /// Gets or sets the extension.
        /// </summary>
        /// <value>
        /// The extension.
        /// </value>
        [Required]
        public string Extension { get; set; }
    }
    #endregion

    #region RegistrationClasses

    /// <summary>
    /// 
    /// </summary>
    public class IntilizationStep : DateTimeStamp
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Key, Required, MinLength(16), MaxLength(32)]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>
        /// The mode.
        /// </value>
        [Required]
        public string Mode { get; set; }
        /// <summary>
        /// Gets or sets the authentication.
        /// </summary>
        /// <value>
        /// The authentication.
        /// </value>
        [Required]
        public int Auth { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AdminRegistrationBase : DateTimeStamp
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminRegistrationBase"/> class.
        /// </summary>
        public AdminRegistrationBase()
        {
            ResendActivity = false;
            ConfirmationCode = String.Empty;
        }

        /// <summary>
        /// Gets or sets the step code.
        /// </summary>
        /// <value>
        /// The step code.
        /// </value>
        [Required, Key, ForeignKey("Step")]
        public string StepCode { get; set; }
        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        /// <value>
        /// The step.
        /// </value>
        public virtual IntilizationStep Step { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required, MinLength(5), MaxLength(100)]
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [resend activity].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [resend activity]; otherwise, <c>false</c>.
        /// </value>
        [Required]
        public bool ResendActivity { get; set; }
        /// <summary>
        /// Gets or sets the confirmation code.
        /// </summary>
        /// <value>
        /// The confirmation code.
        /// </value>
        [Required]
        public string ConfirmationCode { get; set; }
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [Required]
        public string Role { get; set; }
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class Organization : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required, Index("IX_OrganizationCode", 1, IsUnique = true), MaxLength(50), MinLength(2)]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required, MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        [Required, Index("IX_Domain", 1, IsUnique = true), MinLength(5), MaxLength(50)]
        public string Domain { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Invite : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [Index("IX_Username", IsUnique = true), Required, MaxLength(100)]
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required, MinLength(6)]
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required, MinLength(5), MaxLength(100)]
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>
        /// The organization.
        /// </value>
        [Required]
        public virtual Organization Organization { get; set; }
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [Required]
        public string Role { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Administrator : DateTimeStamp
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Key, Index("IX_OrganizationAdministrator", 1, IsUnique = true), ForeignKey("User")]
        public string UserId { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [Required]
        public virtual ApplicationUser User { get; set; }
        /// <summary>
        /// Gets or sets the organization key.
        /// </summary>
        /// <value>
        /// The organization key.
        /// </value>
        [Index("IX_OrganizationAdministrator", 2, IsUnique = true), ForeignKey("Organization")]
        public string OrganizationKey { get; set; }
        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>
        /// The organization.
        /// </value>
        [Required]
        public virtual Organization Organization { get; set; }
        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        [Required]
        public string Role { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Plant : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required, MaxLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required, MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [Required, MaxLength(50)]
        public string Location { get; set; }
        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>
        /// The organization.
        /// </value>
        [Required]
        public virtual Organization Organization { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class Division : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required, MaxLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required, MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the plant.
        /// </summary>
        /// <value>
        /// The plant.
        /// </value>
        [Required]
        public virtual Plant Plant { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Department : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the access.
        /// </summary>
        /// <value>
        /// The access.
        /// </value>
        [Required]
        public string Access { get; set; }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        [Required]
        public virtual Division Division { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DepartmentMenu : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the department key.
        /// </summary>
        /// <value>
        /// The department key.
        /// </value>
        [Index("IX_DivisionMenu", 1, IsUnique = true), ForeignKey("Department")]
        public string DepartmentKey { get; set; }

        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        [Required]
        public virtual Department Department { get; set; }

        /// <summary>
        /// Gets or sets the menu identifier.
        /// </summary>
        /// <value>
        /// The menu identifier.
        /// </value>
        [Index("IX_DivisionMenu", 2, IsUnique = true), ForeignKey("Menu")]
        public string MenuID { get; set; }

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        [Required]
        public virtual Menus Menu { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DepartmentUser : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        [Required]
        public virtual Department Department { get; set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Index("IX_DivisionUser", IsUnique = true), ForeignKey("User")]
        public string UserId { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [Required]
        public virtual ApplicationUser User { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AccessLevel : DivisionBase
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Index("IX_AccessLevel", 1, IsUnique = true), ForeignKey("User")]
        public string UserId { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [Required]
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Gets or sets the division key.
        /// </summary>
        /// <value>
        /// The division key.
        /// </value>
        [Index("IX_AccessLevel", 2, IsUnique = true), ForeignKey("Division")]
        public string DivisionKey { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MenuAccess : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [Index("IX_MenuAccess", 1, IsUnique = true), ForeignKey("User")]
        public string UserId { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [Required]
        public virtual ApplicationUser User { get; set; }

        /// <summary>
        /// Gets or sets the department menu key.
        /// </summary>
        /// <value>
        /// The department menu key.
        /// </value>
        [Index("IX_MenuAccess", 2, IsUnique = true), ForeignKey("DepartmentMenu")]
        public string DepartmentMenuKey { get; set; }
        /// <summary>
        /// Gets or sets the department menu.
        /// </summary>
        /// <value>
        /// The department menu.
        /// </value>
        [Required]
        public DepartmentMenu DepartmentMenu { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RawMaterial : EntityDivisionBase
    {
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        [Required, Range(1, 4)]
        public Units Unit { get; set; }
        /// <summary>
        /// Gets or sets the measurement.
        /// </summary>
        /// <value>
        /// The measurement.
        /// </value>
        [Required, Range(1, 3)]
        public Measurements Measurement { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Product : EntityDivisionBase
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class ProductRawMaterial : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the raw material key.
        /// </summary>
        /// <value>
        /// The raw material key.
        /// </value>
        [Index("IX_ProductRawMaterial", 1, IsUnique = true), ForeignKey("RawMaterial")]
        public string RawMaterialKey { get; set; }
        /// <summary>
        /// Gets or sets the raw material.
        /// </summary>
        /// <value>
        /// The raw material.
        /// </value>
        [Required]
        public virtual RawMaterial RawMaterial { get; set; }

        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        /// <value>
        /// The product key.
        /// </value>
        [Index("IX_ProductRawMaterial", 2, IsUnique = true), ForeignKey("Product")]
        public string ProductKey { get; set; }
        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        [Required]
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>
        /// The quantity.
        /// </value>
        [Required]
        public double Quantity { get; set; }

        [Required]
        public double InputQuantity { get; set; }

        [Required]
        public Units SelectedUnit { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Process : EntityDivisionBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether [validate raw material].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [validate raw material]; otherwise, <c>false</c>.
        /// </value>
        [Required]
        public bool ValidateRawMaterial { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProductProcess : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        /// <value>
        /// The product key.
        /// </value>
        [Index("IX_ProductProcess", 1, IsUnique = true), ForeignKey("Product")]
        public string ProductKey { get; set; }
        /// <summary>
        /// Gets or sets the product.
        /// </summary>
        /// <value>
        /// The product.
        /// </value>
        [Required]
        public virtual Product Product { get; set; }
        /// <summary>
        /// Gets or sets the process key.
        /// </summary>
        /// <value>
        /// The process key.
        /// </value>
        [Index("IX_ProductProcess", 2, IsUnique = true), ForeignKey("Process")]
        public string ProcessKey { get; set; }
        /// <summary>
        /// Gets or sets the process.
        /// </summary>
        /// <value>
        /// The process.
        /// </value>
        [Required]
        public virtual Process Process { get; set; }
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        [Required]
        [Index("IX_ProductProcess", 3, IsUnique = true)]
        public int Order { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ToolDieBase : EntityDivisionBase
    {
        /// <summary>
        /// Gets or sets the maximum count.
        /// </summary>
        /// <value>
        /// The maximum count.
        /// </value>
        [Required, Range(1, int.MaxValue)]
        public int MaxCount { get; set; }
        /// <summary>
        /// Gets or sets the tolerance.
        /// </summary>
        /// <value>
        /// The tolerance.
        /// </value>
        [Required, Range(0, int.MaxValue)]
        public int Tolerance { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ToolDieImportBase : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [Required]
        public int Count { get; set; }
        /// <summary>
        /// Gets or sets the heat code.
        /// </summary>
        /// <value>
        /// The heat code.
        /// </value>
        [Required]
        public string HeatCode { get; set; }
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [Required]
        public DateTime Date { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Tool : ToolDieBase
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class ProductProcessTool : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the product process key.
        /// </summary>
        /// <value>
        /// The product process key.
        /// </value>
        [Index("IX_ProductProcessTool", 1, IsUnique = true), ForeignKey("ProductProcess")]
        public string ProductProcessKey { get; set; }
        /// <summary>
        /// Gets or sets the product process.
        /// </summary>
        /// <value>
        /// The product process.
        /// </value>
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        /// <summary>
        /// Gets or sets the tool key.
        /// </summary>
        /// <value>
        /// The tool key.
        /// </value>
        [Index("IX_ProductProcessTool", 2, IsUnique = true), ForeignKey("Tool")]
        public string ToolKey { get; set; }
        /// <summary>
        /// Gets or sets the tool.
        /// </summary>
        /// <value>
        /// The tool.
        /// </value>
        [Required]
        public virtual Tool Tool { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ToolImport : ToolDieImportBase
    {
        /// <summary>
        /// Gets or sets the tool.
        /// </summary>
        /// <value>
        /// The tool.
        /// </value>
        [Required]
        public virtual Tool Tool { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Die : ToolDieBase
    {
        /// <summary>
        /// Gets or sets the maximum sink.
        /// </summary>
        /// <value>
        /// The maximum sink.
        /// </value>
        [Required, Range(0, int.MaxValue)]
        public int MaxSink { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProductProcessDie : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the product process key.
        /// </summary>
        /// <value>
        /// The product process key.
        /// </value>
        [Index("IX_ProductProcessDie", 1, IsUnique = true), ForeignKey("ProductProcess")]
        public string ProductProcessKey { get; set; }
        /// <summary>
        /// Gets or sets the product process.
        /// </summary>
        /// <value>
        /// The product process.
        /// </value>
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        /// <summary>
        /// Gets or sets the die key.
        /// </summary>
        /// <value>
        /// The die key.
        /// </value>
        [Index("IX_ProductProcessDie", 2, IsUnique = true), ForeignKey("Die")]
        public string DieKey { get; set; }
        /// <summary>
        /// Gets or sets the die.
        /// </summary>
        /// <value>
        /// The die.
        /// </value>
        [Required]
        public virtual Die Die { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DieImport : ToolDieImportBase
    {
        /// <summary>
        /// Gets or sets the sink count.
        /// </summary>
        /// <value>
        /// The sink count.
        /// </value>
        [Required]
        public int SinkCount { get; set; }
        /// <summary>
        /// Gets or sets the die.
        /// </summary>
        /// <value>
        /// The die.
        /// </value>
        [Required]
        public virtual Die Die { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Scrap : EntityDivisionBase
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class ProductProcessScrap : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the product process key.
        /// </summary>
        /// <value>
        /// The product process key.
        /// </value>
        [Index("IX_ProductProcessScrap", 1, IsUnique = true), ForeignKey("ProductProcess")]
        public string ProductProcessKey { get; set; }
        /// <summary>
        /// Gets or sets the product process.
        /// </summary>
        /// <value>
        /// The product process.
        /// </value>
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        /// <summary>
        /// Gets or sets the scrap key.
        /// </summary>
        /// <value>
        /// The scrap key.
        /// </value>
        [Index("IX_ProductProcessScrap", 2, IsUnique = true), ForeignKey("Scrap")]
        public string ScrapKey { get; set; }
        /// <summary>
        /// Gets or sets the scrap.
        /// </summary>
        /// <value>
        /// The scrap.
        /// </value>
        [Required]
        public virtual Scrap Scrap { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Rework : EntityDivisionBase
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class ProductProcessRework : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the product process key.
        /// </summary>
        /// <value>
        /// The product process key.
        /// </value>
        [Index("IX_ProductProcessRework", 1, IsUnique = true), ForeignKey("ProductProcess")]
        public string ProductProcessKey { get; set; }
        /// <summary>
        /// Gets or sets the product process.
        /// </summary>
        /// <value>
        /// The product process.
        /// </value>
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        /// <summary>
        /// Gets or sets the rework key.
        /// </summary>
        /// <value>
        /// The rework key.
        /// </value>
        [Index("IX_ProductProcessRework", 2, IsUnique = true), ForeignKey("Rework")]
        public string ReworkKey { get; set; }
        /// <summary>
        /// Gets or sets the rework.
        /// </summary>
        /// <value>
        /// The rework.
        /// </value>
        [Required]
        public virtual Rework Rework { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Attachment : DivisionBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        [Required]
        public double Size { get; set; }
        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        [Required]
        public Menus Menu { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [Required]
        public virtual ContentType Type { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class WorkInstruction : DivisionBase
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the process.
        /// </summary>
        /// <value>
        /// The process.
        /// </value>
        [Required]
        public Process Process { get; set; }
        /// <summary>
        /// Gets or sets the attachment.
        /// </summary>
        /// <value>
        /// The attachment.
        /// </value>
        [Required]
        public virtual Attachment Attachment { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Machine : EntityDivisionBase
    {
        /// <summary>
        /// Gets or sets the type of the machine.
        /// </summary>
        /// <value>
        /// The type of the machine.
        /// </value>
        [Required]
        public MachineTypes MachineType { get; set; }
        /// <summary>
        /// Gets or sets the minimum load.
        /// </summary>
        /// <value>
        /// The minimum load.
        /// </value>
        public double MinLoad { get; set; }
        /// <summary>
        /// Gets or sets the maximum load.
        /// </summary>
        /// <value>
        /// The maximum load.
        /// </value>
        public double MaxLoad { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MachineProcess : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the machine key.
        /// </summary>
        /// <value>
        /// The machine key.
        /// </value>
        [Index("IX_MachineProcess", 1, IsUnique = true), ForeignKey("Machine")]
        public string MachineKey { get; set; }
        /// <summary>
        /// Gets or sets the machine.
        /// </summary>
        /// <value>
        /// The machine.
        /// </value>
        [Required]
        public virtual Machine Machine { get; set; }
        /// <summary>
        /// Gets or sets the process key.
        /// </summary>
        /// <value>
        /// The process key.
        /// </value>
        [Index("IX_MachineProcess", 2, IsUnique = true), ForeignKey("Process")]
        public string ProcessKey { get; set; }
        /// <summary>
        /// Gets or sets the process.
        /// </summary>
        /// <value>
        /// The process.
        /// </value>
        [Required]
        public virtual Process Process { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Cycletime : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the seconds.
        /// </summary>
        /// <value>
        /// The seconds.
        /// </value>
        [Required]
        public int Seconds { get; set; }
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        [Required]
        public double Count { get; set; }
        /// <summary>
        /// Gets or sets the product process.
        /// </summary>
        /// <value>
        /// The product process.
        /// </value>
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        /// <summary>
        /// Gets or sets the machine process.
        /// </summary>
        /// <value>
        /// The machine process.
        /// </value>
        [Required]
        public virtual MachineProcess MachineProcess { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SafetyInstruction : DivisionBase
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the attachment.
        /// </summary>
        /// <value>
        /// The attachment.
        /// </value>
        [Required]
        public virtual Attachment Attachment { get; set; }
        /// <summary>
        /// Gets or sets the machines.
        /// </summary>
        /// <value>
        /// The machines.
        /// </value>
        public virtual ICollection<Machine> Machines { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SafetyQuestion : DivisionBase
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the answer.
        /// </summary>
        /// <value>
        /// The answer.
        /// </value>
        [Required]
        public virtual SafetyQuestionOption Answer { get; set; }
        /// <summary>
        /// Gets or sets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public virtual ICollection<SafetyQuestionOption> Options { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SafetyQuestionOption : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        [Required]
        public virtual SafetyQuestion Question { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class InspectionChecklist : DivisionBase
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Required]
        public string Description { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MachineInspectionChecklist : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the inspection checklist.
        /// </summary>
        /// <value>
        /// The inspection checklist.
        /// </value>
        [Required]
        public virtual InspectionChecklist InspectionChecklist { get; set; }
        /// <summary>
        /// Gets or sets the machine.
        /// </summary>
        /// <value>
        /// The machine.
        /// </value>
        [Required]
        public virtual Machine Machine { get; set; }
        /// <summary>
        /// Gets or sets the period.
        /// </summary>
        /// <value>
        /// The period.
        /// </value>
        [Required]
        public int Period { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DoneOnDueOn : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the machine inspection checklist.
        /// </summary>
        /// <value>
        /// The machine inspection checklist.
        /// </value>
        [Required]
        public virtual MachineInspectionChecklist MachineInspectionChecklist { get; set; }
        /// <summary>
        /// Gets or sets the done on.
        /// </summary>
        /// <value>
        /// The done on.
        /// </value>
        [Required]
        public DateTime DoneOn { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Downtime : DivisionBase
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the downtime category.
        /// </summary>
        /// <value>
        /// The downtime category.
        /// </value>
        [Required]
        public DowntimeCategories DowntimeCategory { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Operator : DivisionBase
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [Required]
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        [Required]
        public DateTime DateOfBirth { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MachineOperator : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the machine key.
        /// </summary>
        /// <value>
        /// The machine key.
        /// </value>
        [Index("IX_MachineOperator", 1, IsUnique = true), ForeignKey("Machine")]
        public string MachineKey { get; set; }
        /// <summary>
        /// Gets or sets the machine.
        /// </summary>
        /// <value>
        /// The machine.
        /// </value>
        [Required]
        public virtual Machine Machine { get; set; }
        /// <summary>
        /// Gets or sets the operator key.
        /// </summary>
        /// <value>
        /// The operator key.
        /// </value>
        [Index("IX_MachineOperator", 2, IsUnique = true), ForeignKey("Operator")]
        public string OperatorKey { get; set; }
        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        [Required]
        public virtual Operator Operator { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Skill : DivisionBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        [Required]
        public string Color { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class MachineSkill : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the machine key.
        /// </summary>
        /// <value>
        /// The machine key.
        /// </value>
        [Index("IX_MachineSkill", 1, IsUnique = true), ForeignKey("Machine")]
        public string MachineKey { get; set; }
        /// <summary>
        /// Gets or sets the machine.
        /// </summary>
        /// <value>
        /// The machine.
        /// </value>
        [Required]
        public virtual Machine Machine { get; set; }
        /// <summary>
        /// Gets or sets the skill key.
        /// </summary>
        /// <value>
        /// The skill key.
        /// </value>
        [Index("IX_MachineSkill", 2, IsUnique = true), ForeignKey("Skill")]
        public string SkillKey { get; set; }
        /// <summary>
        /// Gets or sets the skill.
        /// </summary>
        /// <value>
        /// The skill.
        /// </value>
        [Required]
        public virtual Skill Skill { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SkillMatrix : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the machine skill key.
        /// </summary>
        /// <value>
        /// The machine skill key.
        /// </value>
        [Index("IX_SkillMatrix", 1, IsUnique = true), ForeignKey("MachineSkill")]
        public string MachineSkillKey { get; set; }
        /// <summary>
        /// Gets or sets the machine skill.
        /// </summary>
        /// <value>
        /// The machine skill.
        /// </value>
        [Required]
        public virtual MachineSkill MachineSkill { get; set; }
        /// <summary>
        /// Gets or sets the operator key.
        /// </summary>
        /// <value>
        /// The operator key.
        /// </value>
        [Index("IX_SkillMatrix", 2, IsUnique = true), ForeignKey("Operator")]
        public string OperatorKey { get; set; }
        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        [Required]
        public virtual Operator Operator { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Shift : DivisionBase
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        [Required]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        [Required]
        public DateTime EndTime { get; set; }
        /// <summary>
        /// Gets a value indicating whether this instance is day light.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is day light; otherwise, <c>false</c>.
        /// </value>
        public bool IsDayLight
        {
            get
            {
                return EndTime.TimeOfDay > StartTime.TimeOfDay;
            }
        }
        /// <summary>
        /// Gets the current day.
        /// </summary>
        /// <value>
        /// The current day.
        /// </value>
        public DateTime CurrentDay
        {
            get
            {
                return IsDayLight ? (DateTime.Today.Date) : (DateTime.Now.TimeOfDay >= DateTime.Today.TimeOfDay ? DateTime.Now.AddDays(-1).Date : DateTime.Today.Date);
            }
        }
        /// <summary>
        /// Gets the start time of the day.
        /// </summary>
        /// <value>
        /// The start time of the day.
        /// </value>
        public DateTime StartTimeOfTheDay
        {
            get
            {
                return IsDayLight ? (DateTime.Now.Date + StartTime.TimeOfDay) : ((DateTime.Now.TimeOfDay <= EndTime.TimeOfDay) ? (DateTime.Now.AddDays(-1).Date + StartTime.TimeOfDay) : (DateTime.Now.Date + StartTime.TimeOfDay));
            }
        }
        /// <summary>
        /// Gets the end time of the day.
        /// </summary>
        /// <value>
        /// The end time of the day.
        /// </value>
        public DateTime EndTimeOfTheDay
        {
            get
            {
                return IsDayLight ? (DateTime.Now.Date + EndTime.TimeOfDay) : ((DateTime.Now.TimeOfDay <= EndTime.TimeOfDay) ? (DateTime.Now.Date + EndTime.TimeOfDay) : (DateTime.Now.AddDays(1).Date + EndTime.TimeOfDay));
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ShiftPlan : AppBaseStamp
    {
        /// <summary>
        /// Gets or sets the product process.
        /// </summary>
        /// <value>
        /// The product process.
        /// </value>
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        /// <summary>
        /// Gets or sets the machine process.
        /// </summary>
        /// <value>
        /// The machine process.
        /// </value>
        [Required]
        public virtual MachineProcess MachineProcess { get; set; }
        /// <summary>
        /// Gets or sets the shift.
        /// </summary>
        /// <value>
        /// The shift.
        /// </value>
        [Required]
        public virtual Shift Shift { get; set; }
        /// <summary>
        /// Gets or sets the shift date.
        /// </summary>
        /// <value>
        /// The shift date.
        /// </value>
        [Required]
        public DateTime ShiftDate { get; set; }
        /// <summary>
        /// Gets or sets the plan quanity.
        /// </summary>
        /// <value>
        /// The plan quanity.
        /// </value>
        [Required]
        public int PlanQuanity { get; set; }
        /// <summary>
        /// Gets or sets the current quanity.
        /// </summary>
        /// <value>
        /// The current quanity.
        /// </value>
        [Required]
        public int CurrentQuanity { get; set; }
        /// <summary>
        /// Gets or sets the production count.
        /// </summary>
        /// <value>
        /// The production count.
        /// </value>
        public int ProductionCount { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is partial.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is partial; otherwise, <c>false</c>.
        /// </value>
        public bool IsPartial { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompleted { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is material confrimed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is material confrimed; otherwise, <c>false</c>.
        /// </value>
        public bool IsMaterialConfrimed { get; set; }
        /// <summary>
        /// Gets or sets the plan start.
        /// </summary>
        /// <value>
        /// The plan start.
        /// </value>
        public DateTime PlanStart { get; set; }
        /// <summary>
        /// Gets or sets the plan end.
        /// </summary>
        /// <value>
        /// The plan end.
        /// </value>
        public DateTime PlanEnd { get; set; }
        /// <summary>
        /// Gets the total production time.
        /// </summary>
        /// <value>
        /// The total production time.
        /// </value>
        public TimeSpan TotalProductionTime
        {
            get
            {
                return PlanEnd - PlanStart;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Break : DivisionBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        [Required]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        [Required]
        public DateTime EndTime { get; set; }
        /// <summary>
        /// Gets or sets the shift.
        /// </summary>
        /// <value>
        /// The shift.
        /// </value>
        [Required]
        public virtual Shift Shift { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Production : DateTimeStamp
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [Key]
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        [Required]
        public virtual Operator Operator { get; set; }
        /// <summary>
        /// Gets or sets the shift.
        /// </summary>
        /// <value>
        /// The shift.
        /// </value>
        [Required]
        public virtual Shift Shift { get; set; }
        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        [Required]
        public DateTime Date { get; set; }
        /// <summary>
        /// Gets or sets the entry date.
        /// </summary>
        /// <value>
        /// The entry date.
        /// </value>
        [Required]
        public DateTime EntryDate { get; set; }
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        [Required]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        [Required]
        public DateTime EndTime { get; set; }
        /// <summary>
        /// Gets or sets the application time.
        /// </summary>
        /// <value>
        /// The application time.
        /// </value>
        [Required]
        public DateTime ApplicationTime { get; set; }
        /// <summary>
        /// Gets or sets the produced.
        /// </summary>
        /// <value>
        /// The produced.
        /// </value>
        [Required, Range(0, int.MaxValue)]
        public int Produced { get; set; }
        /// <summary>
        /// Gets or sets the manual count.
        /// </summary>
        /// <value>
        /// The manual count.
        /// </value>
        [Required, Range(0, int.MaxValue)]
        public int ManualCount { get; set; }
        /// <summary>
        /// Gets or sets the year.
        /// </summary>
        /// <value>
        /// The year.
        /// </value>
        [Required]
        public int Year { get; set; }
        /// <summary>
        /// Gets or sets the month.
        /// </summary>
        /// <value>
        /// The month.
        /// </value>
        [Required]
        public int Month { get; set; }
        /// <summary>
        /// Gets or sets the day.
        /// </summary>
        /// <value>
        /// The day.
        /// </value>
        [Required]
        public int Day { get; set; }
        /// <summary>
        /// Gets or sets the week.
        /// </summary>
        /// <value>
        /// The week.
        /// </value>
        [Required]
        public int Week { get; set; }
        /// <summary>
        /// Gets or sets the scrap.
        /// </summary>
        /// <value>
        /// The scrap.
        /// </value>
        [Required]
        public int Scrap { get; set; }
        /// <summary>
        /// Gets or sets the rework.
        /// </summary>
        /// <value>
        /// The rework.
        /// </value>
        [Required]
        public int Rework { get; set; }
        /// <summary>
        /// Gets or sets the rejected.
        /// </summary>
        /// <value>
        /// The rejected.
        /// </value>
        [Required]
        public int Rejected { get; set; }
        /// <summary>
        /// Gets or sets the accepted.
        /// </summary>
        /// <value>
        /// The accepted.
        /// </value>
        [Required]
        public int Accepted { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Summary : Production
    {
        /// <summary>
        /// Gets or sets the shift plan key.
        /// </summary>
        /// <value>
        /// The shift plan key.
        /// </value>
        [Index("IX_Production", 1, IsUnique = true), ForeignKey("ShiftPlan")]
        public string ShiftPlanKey { get; set; }
        /// <summary>
        /// Gets or sets the shift plan.
        /// </summary>
        /// <value>
        /// The shift plan.
        /// </value>
        [Required]
        public virtual ShiftPlan ShiftPlan { get; set; }
        /// <summary>
        /// Gets or sets the runtime.
        /// </summary>
        /// <value>
        /// The runtime.
        /// </value>
        [Required, Range(1, int.MaxValue)]
        public int Runtime { get; set; }
        /// <summary>
        /// Gets or sets the downtime.
        /// </summary>
        /// <value>
        /// The downtime.
        /// </value>
        [Required]
        public int Downtime { get; set; }
        /// <summary>
        /// Gets or sets the idle time.
        /// </summary>
        /// <value>
        /// The idle time.
        /// </value>
        [Required]
        public int IdleTime { get; set; }
        /// <summary>
        /// Gets or sets the expired.
        /// </summary>
        /// <value>
        /// The expired.
        /// </value>
        [Required]
        public int Expired { get; set; }
        /// <summary>
        /// Gets or sets the plan count.
        /// </summary>
        /// <value>
        /// The plan count.
        /// </value>
        [Required, Range(1, int.MaxValue)]
        public int PlanCount { get; set; }
        /// <summary>
        /// Gets or sets the product per cycle.
        /// </summary>
        /// <value>
        /// The product per cycle.
        /// </value>
        [Required]
        public double ProductPerCycle { get; set; }
        /// <summary>
        /// Gets or sets the overtime.
        /// </summary>
        /// <value>
        /// The overtime.
        /// </value>
        [Required]
        public int Overtime { get; set; }
        /// <summary>
        /// Gets or sets the break seconds.
        /// </summary>
        /// <value>
        /// The break seconds.
        /// </value>
        [Required]
        public int BreakSeconds { get; set; }
        /// <summary>
        /// Gets or sets the available seconds.
        /// </summary>
        /// <value>
        /// The available seconds.
        /// </value>
        [Required]
        public int AvailableSeconds { get; set; }
        /// <summary>
        /// Gets or sets the cycletime.
        /// </summary>
        /// <value>
        /// The cycletime.
        /// </value>
        [Required]
        public int Cycletime { get; set; }
        /// <summary>
        /// Gets or sets the shift seconds.
        /// </summary>
        /// <value>
        /// The shift seconds.
        /// </value>
        [Required]
        public int ShiftSeconds { get; set; }
        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        [Required]
        public string Reason { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Stage : Production
    {
        /// <summary>
        /// Gets or sets the product process.
        /// </summary>
        /// <value>
        /// The product process.
        /// </value>
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Activity : DateTimeStamp
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        [Key]
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action { get; set; }
        /// <summary>
        /// Gets or sets the type of the action.
        /// </summary>
        /// <value>
        /// The type of the action.
        /// </value>
        public string ActionType { get; set; }
        /// <summary>
        /// Gets or sets the helper.
        /// </summary>
        /// <value>
        /// The helper.
        /// </value>
        public string Helper { get; set; }
        /// <summary>
        /// Gets or sets the type of the helper.
        /// </summary>
        /// <value>
        /// The type of the helper.
        /// </value>
        public string HelperType { get; set; }
        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>
        /// The owner.
        /// </value>
        public string Owner { get; set; }
        /// <summary>
        /// Gets or sets the type of the owner.
        /// </summary>
        /// <value>
        /// The type of the owner.
        /// </value>
        public string OwnerType { get; set; }
        /// <summary>
        /// Gets or sets the extend.
        /// </summary>
        /// <value>
        /// The extend.
        /// </value>
        public string Extend { get; set; }
        /// <summary>
        /// Gets or sets the type of the extend.
        /// </summary>
        /// <value>
        /// The type of the extend.
        /// </value>
        public string ExtendType { get; set; }
        /// <summary>
        /// Gets or sets the actor.
        /// </summary>
        /// <value>
        /// The actor.
        /// </value>
        public string Actor { get; set; }
        /// <summary>
        /// Gets or sets the type of the actor.
        /// </summary>
        /// <value>
        /// The type of the actor.
        /// </value>
        public string ActorType { get; set; }
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public string Result { get; set; }
        /// <summary>
        /// Gets or sets the type of the result.
        /// </summary>
        /// <value>
        /// The type of the result.
        /// </value>
        public string ResultType { get; set; }
        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public string State { get; set; }
        /// <summary>
        /// Gets or sets the tracker.
        /// </summary>
        /// <value>
        /// The tracker.
        /// </value>
        public string Tracker { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public string Instance { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public string StartTime { get; set; }
        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        public string EndTime { get; set; }
    }
}