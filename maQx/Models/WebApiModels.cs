using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using maQx.Models;
using maQx.Utilities;

namespace maQx.WebApiModels
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T1">The type of the 1.</typeparam>
    /// <typeparam name="T2">The type of the 2.</typeparam>
    public interface IJsonBase<T1, T2>
    {
        /// <summary>
        /// To the specified value.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        T2 To(T1 Value);
    }

    /// <summary>
    ///
    /// </summary>
    public class JsonBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonBase"/> class.
        /// </summary>
        public JsonBase() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonBase"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JsonBase(IDateTimeStamp input)
        {
            this.Modified = input.Modified;
            this.Created = input.Created;

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonBase"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JsonBase(IAppBaseStamp input)
        {
            this.Key = input.Key;
            this.Modified = input.Modified;
            this.Created = input.Created;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        /// <value>
        /// The modified.
        /// </value>
        public string Modified { get; set; }
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public string Created { get; set; }
    }

    public class JDivisionBase : JsonBase
    {
        public JDivision Division { get; set; }

        public JDivisionBase() { }
        public JDivisionBase(DivisionBase input)
            : base(input)
        {
            Division = new JDivision(input.Division);
        }
    }

    public class JEntityDivisionBase : JDivisionBase
    {
        public string Code { get; set; }
        public string Description { get; set; }

        public JEntityDivisionBase() { }

        public JEntityDivisionBase(EntityDivisionBase input)
            : base(input)
        {
            Code = input.Code;
            Description = input.Description;
        }
    }

    public class JsonDivisionEntityBase : JsonBase
    {

    }

    public class JToolDieBase : JEntityDivisionBase
    {
        public int MaxCount { get; set; }
        public int Tolerance { get; set; }

        public JToolDieBase() { }
        public JToolDieBase(ToolDieBase input)
            : base(input)
        {
            MaxCount = input.MaxCount;
            Tolerance = input.Tolerance;
        }
    }

    public class JProductProcessBase : JsonBase
    {
        public JProductProcess ProductProcess { get; set; }
        public JProductProcessBase() { }
        public JProductProcessBase(IProductProcessBase input)
            : base(input)
        {
            ProductProcess = new JProductProcess(input.ProductProcess);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class JOrganization : JsonBase, IJsonBase<Organization, JOrganization>
    {
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
        /// Gets or sets the domain.
        /// </summary>
        /// <value>
        /// The domain.
        /// </value>
        public string Domain { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JOrganization"/> class.
        /// </summary>
        public JOrganization() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JOrganization"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JOrganization(Organization input)
            : base(input)
        {
            Code = input.Code;
            Name = input.Name;
            Domain = input.Domain;
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JOrganization To(Organization input)
        {
            return new JOrganization(input);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class JMenus : IJsonBase<Menus, JMenus>
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string ID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JMenus"/> class.
        /// </summary>
        public JMenus() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JMenus"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JMenus(Menus input)
        {
            ID = input.ID;
            Name = input.Name;
            Order = input.Order;
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JMenus To(Menus input)
        {
            return new JMenus(input);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class JInvite : JsonBase, IJsonBase<Invite, JInvite>
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }
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
        public JOrganization Organization { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JInvite"/> class.
        /// </summary>
        public JInvite() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JInvite"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JInvite(Invite input)
            : base(input)
        {
            Username = input.Username;
            Email = input.Email;
            Organization = new JOrganization(input.Organization);
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JInvite To(Invite input)
        {
            return new JInvite(input);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class JPlant : JsonBase, IJsonBase<Plant, JPlant>
    {
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
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }
        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>
        /// The organization.
        /// </value>
        public JOrganization Organization { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JPlant"/> class.
        /// </summary>
        public JPlant() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JPlant"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JPlant(Plant input)
            : base(input)
        {
            Code = input.Code;
            Name = input.Name;
            Location = input.Location;
            Organization = new JOrganization(input.Organization);
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JPlant To(Plant input)
        {
            return new JPlant(input);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class JDivision : JsonBase, IJsonBase<Division, JDivision>
    {
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
        /// Gets or sets the plant.
        /// </summary>
        /// <value>
        /// The plant.
        /// </value>
        public JPlant Plant { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JDivision"/> class.
        /// </summary>
        public JDivision() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JDivision"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JDivision(Division input)
            : base(input)
        {
            Code = input.Code;
            Name = input.Name;
            Plant = new JPlant(input.Plant);
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JDivision To(Division input)
        {
            return new JDivision(input);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class JDepartment : JsonBase, IJsonBase<Department, JDepartment>
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        public JDivision Division { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JDepartment"/> class.
        /// </summary>
        public JDepartment() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JDepartment"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JDepartment(Department input)
            : base(input)
        {
            Name = input.Name;
            Division = new JDivision(input.Division);
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JDepartment To(Department input)
        {
            return new JDepartment(input);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class JDepartmentMenu : JsonBase, IJsonBase<DepartmentMenu, JDepartmentMenu>
    {
        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public JMenus Menu { get; set; }
        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        public JDepartment Department { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JDepartmentMenu"/> class.
        /// </summary>
        public JDepartmentMenu() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JDepartmentMenu"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JDepartmentMenu(DepartmentMenu input)
            : base(input)
        {
            Menu = new JMenus(input.Menu);
            Department = new JDepartment(input.Department);
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JDepartmentMenu To(DepartmentMenu input)
        {
            return new JDepartmentMenu(input);
        }
    }


    /// <summary>
    ///
    /// </summary>
    public class JApplicationUser : IJsonBase<ApplicationUser, JApplicationUser>
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string Firstname { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JApplicationUser"/> class.
        /// </summary>
        public JApplicationUser() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JApplicationUser"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JApplicationUser(ApplicationUser input)
        {
            Firstname = input.Firstname;
            Id = input.Id;
            Code = input.Code;
            UserName = input.UserName;
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JApplicationUser To(ApplicationUser input)
        {
            return new JApplicationUser(input);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class JDepartmentUser : JsonBase, IJsonBase<DepartmentUser, JDepartmentUser>
    {
        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>
        /// The department.
        /// </value>
        public JDepartment Department { get; set; }
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public JApplicationUser User { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JDepartmentUser"/> class.
        /// </summary>
        public JDepartmentUser() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JDepartmentUser"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JDepartmentUser(DepartmentUser input)
            : base(input)
        {
            Department = new JDepartment(input.Department);
            User = new JApplicationUser(input.User);
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JDepartmentUser To(DepartmentUser input)
        {
            return new JDepartmentUser(input);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class JAccessLevel : JsonBase, IJsonBase<AccessLevel, JAccessLevel>
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public JApplicationUser User { get; set; }
        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>
        /// The division.
        /// </value>
        public JDivision Division { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JAccessLevel"/> class.
        /// </summary>
        public JAccessLevel() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JAccessLevel"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JAccessLevel(AccessLevel input)
            : base(input)
        {
            User = new JApplicationUser(input.User);
            Division = new JDivision(input.Division);
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JAccessLevel To(AccessLevel input)
        {
            return new JAccessLevel(input);
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class JMenuAccess : JsonBase, IJsonBase<MenuAccess, JMenuAccess>
    {
        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public JApplicationUser User { get; set; }
        /// <summary>
        /// Gets or sets the department menu.
        /// </summary>
        /// <value>
        /// The department menu.
        /// </value>
        public JDepartmentMenu DepartmentMenu { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JMenuAccess"/> class.
        /// </summary>
        public JMenuAccess() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="JMenuAccess"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JMenuAccess(MenuAccess input)
            : base(input)
        {
            User = new JApplicationUser(input.User);
            DepartmentMenu = new JDepartmentMenu(input.DepartmentMenu);
        }

        /// <summary>
        /// To the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public JMenuAccess To(MenuAccess input)
        {
            return new JMenuAccess(input);
        }
    }

    public class JRawMaterial : JEntityDivisionBase, IJsonBase<RawMaterial, JRawMaterial>
    {
        public Units Unit { get; set; }
        public Measurements Measurement { get; set; }

        public string UnitName
        {
            get
            {
                return Unit.DisplayName();
            }
        }

        public string MeasurementName
        {
            get
            {
                return Measurement.DisplayName();
            }
        }

        public JRawMaterial()
        {

        }

        public JRawMaterial(RawMaterial input)
            : base(input)
        {
            Unit = input.Unit;
            Measurement = input.Measurement;
        }

        public JRawMaterial To(RawMaterial input)
        {
            return new JRawMaterial(input);
        }
    }

    public class JProduct : JEntityDivisionBase, IJsonBase<Product, JProduct>
    {
        public JProduct() { }
        public JProduct(Product input)
            : base(input)
        {

        }

        public JProduct To(Product input)
        {
            return new JProduct(input);
        }
    }

    public class JProcess : JEntityDivisionBase, IJsonBase<Process, JProcess>
    {
        public bool ValidateRawMaterial { get; set; }
        public string ValidateRawmaterialMessage
        {
            get
            {
                return ValidateRawMaterial ? "Required" : "Not Required";
            }
        }

        public JProcess() { }
        public JProcess(Process input)
            : base(input)
        {
            ValidateRawMaterial = input.ValidateRawMaterial;
        }

        public JProcess To(Process input)
        {
            return new JProcess(input);
        }
    }

    public class JTool : JToolDieBase, IJsonBase<Tool, JTool>
    {
        public JTool() { }
        public JTool(Tool input)
            : base(input)
        {

        }

        public JTool To(Tool input)
        {
            return new JTool(input);
        }
    }

    public class JDie : JToolDieBase, IJsonBase<Die, JDie>
    {
        public int MaxSink { get; set; }

        public JDie() { }
        public JDie(Die input)
            : base(input)
        {
            MaxSink = input.MaxSink;
        }

        public JDie To(Die input)
        {
            return new JDie(input);
        }
    }

    public class JScrap : JEntityDivisionBase, IJsonBase<Scrap, JScrap>
    {
        public JScrap() { }
        public JScrap(Scrap input)
            : base(input)
        {

        }

        public JScrap To(Scrap input)
        {
            return new JScrap(input);
        }
    }

    public class JRework : JEntityDivisionBase, IJsonBase<Rework, JRework>
    {
        public JRework() { }
        public JRework(Rework input)
            : base(input)
        {

        }

        public JRework To(Rework input)
        {
            return new JRework(input);
        }
    }

    public class JProductRawMaterial : JsonBase, IJsonBase<ProductRawMaterial, JProductRawMaterial>
    {
        public double Quantity { get; set; }
        public double InputQuantity { get; set; }
        public Units SelectedUnit { get; set; }
        public string SelectedUnitString { get { return SelectedUnit.ToString(); } }
        public JRawMaterial RawMaterial { get; set; }
        public JProduct Product { get; set; }

        public JProductRawMaterial() { }
        public JProductRawMaterial(ProductRawMaterial input)
            : base(input)
        {
            Quantity = input.Quantity;
            RawMaterial = new JRawMaterial(input.RawMaterial);
            Product = new JProduct(input.Product);
            InputQuantity = input.InputQuantity;
            SelectedUnit = input.SelectedUnit;
        }

        public JProductRawMaterial To(ProductRawMaterial input)
        {
            return new JProductRawMaterial(input);
        }
    }

    public class JProductProcess : JsonBase, IJsonBase<ProductProcess, JProductProcess>
    {
        public int Order { get; set; }
        public JProduct Product { get; set; }
        public JProcess Process { get; set; }

        public JProductProcess() { }
        public JProductProcess(ProductProcess input)
            : base(input)
        {
            Order = input.Order;
            Process = new JProcess(input.Process);
            Product = new JProduct(input.Product);
        }

        public JProductProcess To(ProductProcess input)
        {
            return new JProductProcess(input);
        }
    }

    public class JProductProcessTool : JProductProcessBase, IJsonBase<ProductProcessTool, JProductProcessTool>
    {
        public JTool Tool { get; set; }

        public JProductProcessTool() { }
        public JProductProcessTool(ProductProcessTool input)
            : base(input)
        {
            Tool = new JTool(input.Tool);
        }

        public JProductProcessTool To(ProductProcessTool input)
        {
            return new JProductProcessTool(input);
        }
    }

    public class JProductProcessDie : JProductProcessBase, IJsonBase<ProductProcessDie, JProductProcessDie>
    {
        public JDie Die { get; set; }

        public JProductProcessDie() { }
        public JProductProcessDie(ProductProcessDie input)
            : base(input)
        {
            Die = new JDie(input.Die);
        }

        public JProductProcessDie To(ProductProcessDie input)
        {
            return new JProductProcessDie(input);
        }
    }

    public class JProductProcessScrap : JProductProcessBase, IJsonBase<ProductProcessScrap, JProductProcessScrap>
    {
        public JScrap Scrap { get; set; }

        public JProductProcessScrap() { }
        public JProductProcessScrap(ProductProcessScrap input)
            : base(input)
        {
            Scrap = new JScrap(input.Scrap);
        }

        public JProductProcessScrap To(ProductProcessScrap input)
        {
            return new JProductProcessScrap(input);
        }
    }

    public class JProductProcessRework : JProductProcessBase, IJsonBase<ProductProcessRework, JProductProcessRework>
    {
        public JRework Rework { get; set; }

        public JProductProcessRework() { }
        public JProductProcessRework(ProductProcessRework input)
            : base(input)
        {
            Rework = new JRework(input.Rework);
        }

        public JProductProcessRework To(ProductProcessRework input)
        {
            return new JProductProcessRework(input);
        }
    }

    public class JMachine : JEntityDivisionBase, IJsonBase<Machine, JMachine>
    {
        public double MinLoad { get; set; }
        public double MaxLoad { get; set; }
        public MachineTypes MachineType { get; set; }
        public string MachineTypeString
        {
            get
            {
                return MachineType.ToString();
            }
        }

        public JMachine() { }
        public JMachine(Machine input)
            : base(input)
        {
            MinLoad = input.MinLoad;
            MaxLoad = input.MaxLoad;
            MachineType = input.MachineType;
        }

        public JMachine To(Machine input)
        {
            return new JMachine(input);
        }
    }

    public class JShfit : JDivisionBase, IJsonBase<Shift, JShfit>
    {
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsDayLight { get; set; }
        public DateTime CurrentDay { get; set; }
        public DateTime StartTimeOfTheDay { get; set; }
        public DateTime EndTimeOfTheDay { get; set; }
        public double TotalHours { get; set; }

        public string TotalHoursString
        {
            get
            {
                return TotalHours + " " + ("Hr".Pluralize(TotalHours));
            }
        }

        public string StartTimeString
        {
            get
            {
                return StartTime.ToString("hh:mm tt");
            }
        }

        public string EndTimeString
        {
            get
            {
                return EndTime.ToString("hh:mm tt");
            }
        }

        public JShfit() { }
        public JShfit(Shift input)
            : base(input)
        {
            Description = input.Description;
            StartTime = input.StartTime;
            EndTime = input.EndTime;
            IsDayLight = input.IsDayLight;
            TotalHours = input.TotalHours;
            CurrentDay = input.CurrentDay;
            StartTimeOfTheDay = input.StartTimeOfTheDay;
            EndTimeOfTheDay = input.EndTimeOfTheDay;
        }

        public JShfit To(Shift input)
        {
            return new JShfit(input);
        }
    }
}