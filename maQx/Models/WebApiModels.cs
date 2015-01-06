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
        public string Key { get; set; }
        public string Modified { get; set; }
    }

    public class JOrganization : JsonBase, IJsonBase<Organization, JOrganization>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Domain { get; set; }

        public JOrganization To(Organization input)
        {
            return new JOrganization
            {
                Key = input.Key,
                Code = input.Code,
                Name = input.Name,
                Domain = input.Domain,
                Modified = input.Modified
            };
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

    public class JPlant : JsonBase, IJsonBase<Plant, JPlant>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public JOrganization Organization { get; set; }

        public JPlant To(Plant input)
        {
            return new JPlant
            {
                Key = input.Key,
                Code = input.Code,
                Name = input.Name,
                Location = input.Location,
                Modified = input.Modified,
                Organization = new JOrganization().To(input.Organization)
            };
        }
    }

    public class JInvite : JsonBase, IJsonBase<Invite, JInvite>
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public JOrganization Organization { get; set; }

        public JInvite To(Invite input)
        {
            return new JInvite
            {
                Key = input.Key,
                Username = input.Username,
                Email = input.Email,
                Modified = input.Modified,
                Organization = new JOrganization().To(input.Organization)
            };
        }
    }

public class Hello{}
}