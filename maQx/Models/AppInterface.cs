using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maQx.Models
{
    public interface IAppBase
    {
        
    }

    public interface IModelBase : IAppBase
    {
        string Key { get; set; }
        string Modified { get; }
        string Created { get; }
    }

    public interface IOrganization : IModelBase
    {
        string Code { get; set; }
        string Name { get; set; }
        string Domain { get; set; }
    }

    public interface IMenu : IAppBase
    {
        string ID { get; set; }
        string Name { get; set; }
        int Order { get; set; }
    }

    public interface IInvite : IModelBase
    {
        string Username { get; set; }
        string Email { get; set; }
        IOrganization Organization { get; }
    }

    public interface IPlant : IModelBase
    {
        string Code { get; set; }
        string Name { get; set; }
        string Location { get; set; }
        IOrganization Organization { get; }
    }

    public interface IDivision : IModelBase
    {
        string Code { get; set; }
        string Name { get; set; }
        IPlant Plant { get; }
    }
}