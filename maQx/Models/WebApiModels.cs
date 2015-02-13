using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maQx.Models
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
        public JsonBase(DateTimeStamp input)
        {
            this.Modified = input.Modified;
            this.Created = input.Created;

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonBase"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public JsonBase(AppBaseStamp input)
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
}