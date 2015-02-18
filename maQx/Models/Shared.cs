using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity;

namespace maQx.Models
{
    /// <summary>
    ///
    /// </summary>
    public class Shared
    {
        /// <summary>
        /// Gets the selectable divisions.
        /// </summary>
        /// <param name="Organization">The organization.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the selectable users.
        /// </summary>
        /// <param name="Organization">The organization.</param>
        /// <returns></returns>
        public static async Task<IEnumerable<ApplicationUser>> GetSelectableUsers(string Organization)
        {
            using (AppContext db = new AppContext())
            {
                var List = await db.DepartmentUsers.Include(x => x.User).Where(x => x.ActiveFlag && x.Department.Division.Plant.Organization.Key == Organization).Select(x => x.User).OrderBy(x => x.Firstname).ToListAsync();

                return List.Select(x =>
                {
                    x.Firstname = x.Code + " - " + x.Firstname;
                    return x;
                });
            }
        }
    }
}