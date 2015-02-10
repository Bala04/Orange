using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;

namespace maQx.Models
{
    public class Shared
    {
        public static async Task<IEnumerable<Division>> GetSelectableDivisions(string Organization)
        {
            using (AppContext db = new AppContext())
            {
                var Divisions = await db.Divisions.Include(x => x.Plant).Where(x => x.ActiveFlag).OrderBy(x => x.Name).ToListAsync();
                var a = Divisions.Select(x => x.Name).ToList();

                return Divisions.Select(x =>
                 {
                     if (a.Where(y => y == x.Name).Count() > 1)
                     {
                         x.Name += " - " + x.Plant.Name;
                     }
                     return x;
                 });
            }
        }

        public static async Task<IEnumerable<ApplicationUser>> GetSelectableUsers(string Organization, string Role)
        {
            using (AppContext db = new AppContext())
            {               
                var List = await db.Administrators.Include(x => x.User).Include(x => x.Organization).Where(x => x.Organization.Key == Organization && x.Role == Role).Select(x => x.User).ToListAsync();

                return List.Select(x =>
                {
                    x.Firstname = x.Code + " - " + x.Firstname;
                    return x;
                });
            }
        }
    }
}