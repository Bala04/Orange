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
        }

        public JsonBase(AppBaseStamp input)
        {
            this.Key = input.Key;
            this.Modified = input.Modified;
        }

        public string Key { get; set; }
        public string Modified { get; set; }
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
        public string Location { get; set; }
        public JOrganization Organization { get; set; }

        public JPlant() { }
        public JPlant(Plant input)
            : base(input)
        {
            Code = input.Code;
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
}