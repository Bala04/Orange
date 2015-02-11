using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maQx.Models
{
    public interface IJsonBase<T1, T2>
    {
        T2 To(T1 Value);
    }

    public class JsonBase
    {
        public JsonBase() { }
        public JsonBase(DateTimeStamp input)
        {
            this.Modified = input.Modified;
            this.Created = input.Created;

        }
        public JsonBase(AppBaseStamp input)
        {
            this.Key = input.Key;
            this.Modified = input.Modified;
            this.Created = input.Created;
        }

        public string Key { get; set; }
        public string Modified { get; set; }
        public string Created { get; set; }
    }

    public class JOrganization : JsonBase, IJsonBase<Organization, JOrganization>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }

        public JOrganization() { }
        public JOrganization(Organization input)
            : base(input)
        {
            Code = input.Code;
            Name = input.Name;
            Domain = input.Domain;
        }

        public JOrganization To(Organization input)
        {
            return new JOrganization(input);
        }
    }

    public class JMenus : IJsonBase<Menus, JMenus>
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }

        public JMenus() { }
        public JMenus(Menus input)
        {
            ID = input.ID;
            Name = input.Name;
            Order = input.Order;
        }

        public JMenus To(Menus input)
        {
            return new JMenus(input);
        }
    }

    public class JInvite : JsonBase, IJsonBase<Invite, JInvite>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public JOrganization Organization { get; set; }

        public JInvite() { }
        public JInvite(Invite input)
            : base(input)
        {
            Username = input.Username;
            Email = input.Email;
            Organization = new JOrganization(input.Organization);
        }

        public JInvite To(Invite input)
        {
            return new JInvite(input);
        }
    }

    public class JPlant : JsonBase, IJsonBase<Plant, JPlant>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public JOrganization Organization { get; set; }

        public JPlant() { }
        public JPlant(Plant input)
            : base(input)
        {
            Code = input.Code;
            Name = input.Name;
            Location = input.Location;
            Organization = new JOrganization(input.Organization);
        }

        public JPlant To(Plant input)
        {
            return new JPlant(input);
        }
    }

    public class JDivision : JsonBase, IJsonBase<Division, JDivision>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public JPlant Plant { get; set; }

        public JDivision() { }
        public JDivision(Division input)
            : base(input)
        {
            Code = input.Code;
            Name = input.Name;
            Plant = new JPlant(input.Plant);
        }

        public JDivision To(Division input)
        {
            return new JDivision(input);
        }
    }

    public class JDepartment : JsonBase, IJsonBase<Department, JDepartment>
    {
        public string Name { get; set; }
        public JDivision Division { get; set; }

        public JDepartment() { }
        public JDepartment(Department input)
            : base(input)
        {
            Name = input.Name;
            Division = new JDivision(input.Division);
        }

        public JDepartment To(Department input)
        {
            return new JDepartment(input);
        }
    }

    public class JDepartmentMenu : JsonBase, IJsonBase<DepartmentMenu, JDepartmentMenu>
    {       
        public JMenus Menu { get; set; }
        public JDepartment Department { get; set; }

        public JDepartmentMenu() { }
        public JDepartmentMenu(DepartmentMenu input)
            : base(input)
        {          
            Menu = new JMenus(input.Menu);
            Department = new JDepartment(input.Department);
        }

        public JDepartmentMenu To(DepartmentMenu input)
        {
            return new JDepartmentMenu(input);
        }
    }


    public class JApplicationUser : IJsonBase<ApplicationUser, JApplicationUser>
    {
        public string UserName { get; set; }
        public string Id { get; set; }
        public string Code { get; set; }
        public string Firstname { get; set; }

        public JApplicationUser() { }
        public JApplicationUser(ApplicationUser input)
        {
            Firstname = input.Firstname;
            Id = input.Id;
            Code = input.Code;
            UserName = input.UserName;
        }

        public JApplicationUser To(ApplicationUser input)
        {
            return new JApplicationUser(input);
        }
    }

    public class JDepartmentUser : JsonBase, IJsonBase<DepartmentUser, JDepartmentUser>
    {       
        public JDepartment Department { get; set; }
        public JApplicationUser User { get; set; }

        public JDepartmentUser() { }
        public JDepartmentUser(DepartmentUser input)
            : base(input)
        {          
            Department = new JDepartment(input.Department);
            User = new JApplicationUser(input.User);
        }

        public JDepartmentUser To(DepartmentUser input)
        {
            return new JDepartmentUser(input);
        }
    }

    public class JAccessLevel : JsonBase, IJsonBase<AccessLevel, JAccessLevel>
    {
        public JApplicationUser User { get; set; }
        public JDivision Division { get; set; }

        public JAccessLevel() { }
        public JAccessLevel(AccessLevel input)
            : base(input)
        {
            User = new JApplicationUser(input.User);
            Division = new JDivision(input.Division);
        }

        public JAccessLevel To(AccessLevel input)
        {
            return new JAccessLevel(input);
        }
    }
}