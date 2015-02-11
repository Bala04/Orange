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

    #region Enums
    public enum Units { Nos = 1, Kgs = 2, Grams = 3, Tons = 4 };
    public enum Measurements { Pieces = 1, Meters = 2, Others = 3 };
    public enum MachineTypes { Machine = 1, Furnace = 2 };
    public enum DowntimeCategories { ManagementLoss = 1, Planned = 2, UnPlanned = 3 };
    #endregion

    #region BaseClasses
    public class DateTimeStamp
    {
        [Required, DataType(DataType.Date), ScaffoldColumn(false)]
        public DateTime CreatedAt { get; set; }
        [Required, DataType(DataType.Date), ScaffoldColumn(false)]
        public DateTime UpdatedAt { get; set; }
        [Timestamp, ScaffoldColumn(false)]
        public Byte[] TimeStamp { get; set; }
        [Required, ScaffoldColumn(false)]
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
                //BUG: return UpdatedAt.toDateString();
                //FIX: Created should return CreatedAt 31/01/2015
                return CreatedAt.toDateString();
            }
        }
    }

    public class AppBaseStamp : DateTimeStamp
    {
        [Key, Column(Order = 1), Required, MaxLength(40), ScaffoldColumn(false)]
        public string Key { get; set; }
        [Required, ScaffoldColumn(false)]
        public string UserCreated { get; set; }
        [Required, ScaffoldColumn(false)]
        public string UserModified { get; set; }
    }

    public class DivisionBase : AppBaseStamp
    {
        [Required]
        public virtual Division Division { get; set; }
    }   

    public class EntityDivisionBase : DivisionBase
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Description { get; set; }
    }   

    #endregion

    #region Default
    public class Menus
    {
        [Key, Required]
        public string ID { get; set; }
        [Required, Index("IX_Name", IsUnique = true), MaxLength(50), MinLength(2)]
        public string Name { get; set; }
        [Required]
        public string Access { get; set; }
        [Required]
        public int Order { get; set; }
        [Required]
        public bool IsMappable { get; set; }
    }

    public class ContentType
    {
        [Key, Required]
        public string ID { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Extension { get; set; }
    }
    #endregion

    #region RegistrationClasses

    public class IntilizationStep : DateTimeStamp
    {
        [Key, Required, MinLength(16), MaxLength(32)]
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

        [Required, Key, ForeignKey("Step")]
        public string StepCode { get; set; }
        public virtual IntilizationStep Step { get; set; }
        [Required, MinLength(5), MaxLength(100)]
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
        [Required, Index("IX_OrganizationCode", 1, IsUnique = true), MaxLength(50), MinLength(2)]
        public string Code { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required, Index("IX_Domain", 1, IsUnique = true), MinLength(5), MaxLength(50)]
        public string Domain { get; set; }
    }

    public class Invite : AppBaseStamp
    {
        [Index("IX_Username", IsUnique = true), Required, MaxLength(100)]
        public string Username { get; set; }
        [Required, MinLength(6)]
        public string Password { get; set; }
        [Required, MinLength(5), MaxLength(100)]
        public string Email { get; set; }
        [Required]
        public virtual Organization Organization { get; set; }
        [Required]
        public string Role { get; set; }
    }

    public class Administrator : DateTimeStamp
    {
        [Key, Index("IX_OrganizationAdministrator", 1, IsUnique = true), ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        public virtual ApplicationUser User { get; set; }
        [Index("IX_OrganizationAdministrator", 2, IsUnique = true), ForeignKey("Organization")]
        public string OrganizationKey { get; set; }
        [Required]
        public virtual Organization Organization { get; set; }
        [Required]
        public string Role { get; set; }
    }

    public class Plant : AppBaseStamp
    {
        [Required, MaxLength(50)]
        public string Code { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required, MaxLength(50)]
        public string Location { get; set; }
        [Required]
        public virtual Organization Organization { get; set; }

    }

    public class Division : AppBaseStamp
    {
        [Required, MaxLength(50)]
        public string Code { get; set; }
        [Required, MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public virtual Plant Plant { get; set; }
    }

    public class Department : AppBaseStamp
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Access { get; set; }

        [Required]
        public virtual Division Division { get; set; }
    }

    public class DepartmentMenu : AppBaseStamp
    {
        [Index("IX_DivisionMenu", 1, IsUnique = true), ForeignKey("Department")]
        public string DepartmentKey { get; set; }

        [Required]
        public virtual Department Department { get; set; }

        [Index("IX_DivisionMenu", 2, IsUnique = true), ForeignKey("Menu")]
        public string MenuID { get; set; }

        [Required]
        public virtual Menus Menu { get; set; }
    }

    public class DepartmentUser : AppBaseStamp
    {
        [Required]
        public virtual Department Department { get; set; }
        [Index("IX_DivisionUser", IsUnique = true), ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        public virtual ApplicationUser User { get; set; }
    }

    public class AccessLevel : DivisionBase
    {
        [Index("IX_AccessLevel", 1, IsUnique = true), ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        public virtual ApplicationUser User { get; set; }

        [Index("IX_AccessLevel", 2, IsUnique = true), ForeignKey("Division")]
        public string DivisionKey { get; set; }
    }

    public class RawMaterial : EntityDivisionBase
    {
        [Required, Range(1, 4)]
        public Units Unit { get; set; }
        [Required, Range(1, 3)]
        public Measurements Measurement { get; set; }
    }

    public class Product : EntityDivisionBase
    {

    }

    public class ProductRawMaterial : AppBaseStamp
    {
        [Index("IX_ProductRawMaterial", 1, IsUnique = true), ForeignKey("RawMaterial")]
        public string RawMaterialKey { get; set; }
        [Required]
        public virtual RawMaterial RawMaterial { get; set; }
        [Index("IX_ProductRawMaterial", 2, IsUnique = true), ForeignKey("Product")]
        public string ProductKey { get; set; }
        [Required]
        public virtual Product Product { get; set; }
        [Required]
        public double Quantity { get; set; }
    }

    public class Process : EntityDivisionBase
    {
        [Required]
        public bool ValidateRawMaterial { get; set; }
    }

    public class ProductProcess : AppBaseStamp
    {
        [Index("IX_ProductProcess", 1, IsUnique = true), ForeignKey("Product")]
        public string ProductKey { get; set; }
        [Required]
        public virtual Product Product { get; set; }
        [Index("IX_ProductProcess", 2, IsUnique = true), ForeignKey("Process")]
        public string ProcessKey { get; set; }
        [Required]
        public virtual Process Process { get; set; }
        [Required, Range(1, int.MaxValue)]
        [Index("IX_ProductProcess", 3, IsUnique = true)]
        public int Order { get; set; }
    }

    public class ToolDieBase : EntityDivisionBase
    {
        [Required, Range(1, int.MaxValue)]
        public int MaxCount { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Tolerance { get; set; }
    }

    public class ToolDieImportBase : AppBaseStamp
    {
        [Required]
        public int Count { get; set; }
        [Required]
        public string HeatCode { get; set; }
        [Required]
        public DateTime Date { get; set; }
    }

    public class Tool : ToolDieBase
    {

    }

    public class ProductProcessTool : AppBaseStamp
    {
        [Index("IX_ProductProcessTool", 1, IsUnique = true), ForeignKey("ProductProcess")]
        public string ProductProcessKey { get; set; }
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        [Index("IX_ProductProcessTool", 2, IsUnique = true), ForeignKey("Tool")]
        public string ToolKey { get; set; }
        [Required]
        public virtual Tool Tool { get; set; }
    }

    public class ToolImport : ToolDieImportBase
    {
        [Required]
        public virtual Tool Tool { get; set; }
    }

    public class Die : ToolDieBase
    {
        [Required, Range(1, int.MaxValue)]
        public int MaxSink { get; set; }
    }

    public class ProductProcessDie : AppBaseStamp
    {
        [Index("IX_ProductProcessDie", 1, IsUnique = true), ForeignKey("ProductProcess")]
        public string ProductProcessKey { get; set; }
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        [Index("IX_ProductProcessDie", 2, IsUnique = true), ForeignKey("Die")]
        public string DieKey { get; set; }
        [Required]
        public virtual Die Die { get; set; }
    }

    public class DieImport : ToolDieImportBase
    {
        [Required]
        public int SinkCount { get; set; }
        [Required]
        public virtual Die Die { get; set; }
    }

    public class Scrap : EntityDivisionBase
    {

    }

    public class ProductProcessScrap : AppBaseStamp
    {
        [Index("IX_ProductProcessScrap", 1, IsUnique = true), ForeignKey("ProductProcess")]
        public string ProductProcessKey { get; set; }
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        [Index("IX_ProductProcessScrap", 2, IsUnique = true), ForeignKey("Scrap")]
        public string ScrapKey { get; set; }
        [Required]
        public virtual Scrap Scrap { get; set; }
    }

    public class Rework : EntityDivisionBase
    {

    }

    public class ProductProcessRework : AppBaseStamp
    {
        [Index("IX_ProductProcessRework", 1, IsUnique = true), ForeignKey("ProductProcess")]
        public string ProductProcessKey { get; set; }
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        [Index("IX_ProductProcessRework", 2, IsUnique = true), ForeignKey("Rework")]
        public string ReworkKey { get; set; }
        [Required]
        public virtual Rework Rework { get; set; }
    }

    public class Attachment : DivisionBase
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public double Size { get; set; }
        [Required]
        public Menus Menu { get; set; }
        [Required]
        public virtual ContentType Type { get; set; }
    }

    public class WorkInstruction : DivisionBase
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public Process Process { get; set; }
        [Required]
        public virtual Attachment Attachment { get; set; }
    }

    public class Machine : EntityDivisionBase
    {
        [Required]
        public MachineTypes MachineType { get; set; }
        public double MinLoad { get; set; }
        public double MaxLoad { get; set; }
    }

    public class MachineProcess : AppBaseStamp
    {
        [Index("IX_MachineProcess", 1, IsUnique = true), ForeignKey("Machine")]
        public string MachineKey { get; set; }
        [Required]
        public virtual Machine Machine { get; set; }
        [Index("IX_MachineProcess", 2, IsUnique = true), ForeignKey("Process")]
        public string ProcessKey { get; set; }
        [Required]
        public virtual Process Process { get; set; }
    }

    public class Cycletime : AppBaseStamp
    {
        [Required]
        public int Seconds { get; set; }
        [Required]
        public double Count { get; set; }
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        [Required]
        public virtual MachineProcess MachineProcess { get; set; }
    }

    public class SafetyInstruction : DivisionBase
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public virtual Attachment Attachment { get; set; }
        public virtual ICollection<Machine> Machines { get; set; }
    }

    public class SafetyQuestion : DivisionBase
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public virtual SafetyQuestionOption Answer { get; set; }
        public virtual ICollection<SafetyQuestionOption> Options { get; set; }
    }

    public class SafetyQuestionOption : AppBaseStamp
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public virtual SafetyQuestion Question { get; set; }
    }

    public class InspectionChecklist : DivisionBase
    {
        [Required]
        public string Description { get; set; }
    }

    public class MachineInspectionChecklist : AppBaseStamp
    {
        [Required]
        public virtual InspectionChecklist InspectionChecklist { get; set; }
        [Required]
        public virtual Machine Machine { get; set; }
        [Required]
        public int Period { get; set; }
    }

    public class DoneOnDueOn : AppBaseStamp
    {
        [Required]
        public virtual MachineInspectionChecklist MachineInspectionChecklist { get; set; }
        [Required]
        public DateTime DoneOn { get; set; }
    }

    public class Downtime : DivisionBase
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public DowntimeCategories DowntimeCategory { get; set; }
    }

    public class Operator : DivisionBase
    {
        [Required]
        public string Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
    }

    public class MachineOperator : AppBaseStamp
    {
        [Index("IX_MachineOperator", 1, IsUnique = true), ForeignKey("Machine")]
        public string MachineKey { get; set; }
        [Required]
        public virtual Machine Machine { get; set; }
        [Index("IX_MachineOperator", 2, IsUnique = true), ForeignKey("Operator")]
        public string OperatorKey { get; set; }
        [Required]
        public virtual Operator Operator { get; set; }
    }

    public class Skill : DivisionBase
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Color { get; set; }
    }

    public class MachineSkill : AppBaseStamp
    {
        [Index("IX_MachineSkill", 1, IsUnique = true), ForeignKey("Machine")]
        public string MachineKey { get; set; }
        [Required]
        public virtual Machine Machine { get; set; }
        [Index("IX_MachineSkill", 2, IsUnique = true), ForeignKey("Skill")]
        public string SkillKey { get; set; }
        [Required]
        public virtual Skill Skill { get; set; }
    }

    public class SkillMatrix : AppBaseStamp
    {
        [Index("IX_SkillMatrix", 1, IsUnique = true), ForeignKey("MachineSkill")]
        public string MachineSkillKey { get; set; }
        [Required]
        public virtual MachineSkill MachineSkill { get; set; }
        [Index("IX_SkillMatrix", 2, IsUnique = true), ForeignKey("Operator")]
        public string OperatorKey { get; set; }
        [Required]
        public virtual Operator Operator { get; set; }
    }

    public class Shift : DivisionBase
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public bool IsDayLight
        {
            get
            {
                return EndTime.TimeOfDay > StartTime.TimeOfDay;
            }
        }
        public DateTime CurrentDay
        {
            get
            {
                return IsDayLight ? (DateTime.Today.Date) : (DateTime.Now.TimeOfDay >= DateTime.Today.TimeOfDay ? DateTime.Now.AddDays(-1).Date : DateTime.Today.Date);
            }
        }
        public DateTime StartTimeOfTheDay
        {
            get
            {
                return IsDayLight ? (DateTime.Now.Date + StartTime.TimeOfDay) : ((DateTime.Now.TimeOfDay <= EndTime.TimeOfDay) ? (DateTime.Now.AddDays(-1).Date + StartTime.TimeOfDay) : (DateTime.Now.Date + StartTime.TimeOfDay));
            }
        }
        public DateTime EndTimeOfTheDay
        {
            get
            {
                return IsDayLight ? (DateTime.Now.Date + EndTime.TimeOfDay) : ((DateTime.Now.TimeOfDay <= EndTime.TimeOfDay) ? (DateTime.Now.Date + EndTime.TimeOfDay) : (DateTime.Now.AddDays(1).Date + EndTime.TimeOfDay));
            }
        }
    }

    public class ShiftPlan : AppBaseStamp
    {
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
        [Required]
        public virtual MachineProcess MachineProcess { get; set; }
        [Required]
        public virtual Shift Shift { get; set; }
        [Required]
        public DateTime ShiftDate { get; set; }
        [Required]
        public int PlanQuanity { get; set; }
        [Required]
        public int CurrentQuanity { get; set; }
        public int ProductionCount { get; set; }
        public bool IsRunning { get; set; }
        public bool IsPartial { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsMaterialConfrimed { get; set; }
        public DateTime PlanStart { get; set; }
        public DateTime PlanEnd { get; set; }
        public TimeSpan TotalProductionTime
        {
            get
            {
                return PlanEnd - PlanStart;
            }
        }
    }

    public class Break : DivisionBase
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public virtual Shift Shift { get; set; }
    }

    public class Production : DateTimeStamp
    {
        [Key]
        public string Key { get; set; }
        [Required]
        public virtual Operator Operator { get; set; }
        [Required]
        public virtual Shift Shift { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public DateTime EntryDate { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public DateTime ApplicationTime { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Produced { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int ManualCount { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public int Month { get; set; }
        [Required]
        public int Day { get; set; }
        [Required]
        public int Week { get; set; }
        [Required]
        public int Scrap { get; set; }
        [Required]
        public int Rework { get; set; }
        [Required]
        public int Rejected { get; set; }
        [Required]
        public int Accepted { get; set; }
    }

    public class Summary : Production
    {
        [Index("IX_Production", 1, IsUnique = true), ForeignKey("ShiftPlan")]
        public string ShiftPlanKey { get; set; }
        [Required]
        public virtual ShiftPlan ShiftPlan { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Runtime { get; set; }
        [Required]
        public int Downtime { get; set; }
        [Required]
        public int IdleTime { get; set; }
        [Required]
        public int Expired { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int PlanCount { get; set; }
        [Required]
        public double ProductPerCycle { get; set; }
        [Required]
        public int Overtime { get; set; }
        [Required]
        public int BreakSeconds { get; set; }
        [Required]
        public int AvailableSeconds { get; set; }
        [Required]
        public int Cycletime { get; set; }
        [Required]
        public int ShiftSeconds { get; set; }
        [Required]
        public string Reason { get; set; }
    }

    public class Stage : Production
    {
        [Required]
        public virtual ProductProcess ProductProcess { get; set; }
    }

    public class Activity : DateTimeStamp
    {
        [Key]
        public string Key { get; set; }
        public string Name { get; set; }
        public string Action { get; set; }
        public string ActionType { get; set; }
        public string Helper { get; set; }
        public string HelperType { get; set; }
        public string Owner { get; set; }
        public string OwnerType { get; set; }
        public string Extend { get; set; }
        public string ExtendType { get; set; }
        public string Actor { get; set; }
        public string ActorType { get; set; }
        public string Result { get; set; }
        public string ResultType { get; set; }
        public string State { get; set; }
        public string Tracker { get; set; }
        public string Value { get; set; }
        public string Instance { get; set; }
        public string Type { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}