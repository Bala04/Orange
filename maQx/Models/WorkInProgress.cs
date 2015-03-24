using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using maQx.Models;

namespace maQx.Models
{
    public static class WorkInProgress
    {
        public static void Calculate(ProductProcess ProductProcess, int Produced,int Rejected,WebApplication5Context db )
        {            
                Wip w = db.Wips.FirstOrDefault<Wip>(q => q.ProductProcess.ID == ProductProcess.ID);
                if (w == null)
                {
                    w = new Wip();
                    
                    w.ProductProcess = ProductProcess;
                    w.Input = Produced;
                    w.Accepted = Produced - Rejected;
                    w.Rejected = Rejected;
                    w.UpdatedAt = DateTime.Now;
                    db.Wips.Add(w);
                    UpdateNextProcess(ProductProcess,w.Accepted,db);
                }
                else
                {
                    db.Entry(w).State = EntityState.Modified;
                    w.ProductProcess = ProductProcess;
                    if (Produced <= w.Input)
                    {
                        w.Input = w.Input -Produced;
                        w.Accepted = Produced - Rejected;
                        w.Rejected = Rejected;
                        w.UpdatedAt = DateTime.Now;
                        UpdateNextProcess(ProductProcess, w.Accepted,db);
                    }
                }
            
            
        }
        static void UpdateNextProcess(ProductProcess ProductProcess, int Accepted ,WebApplication5Context db)
        {
            ProductProcess NextProductProcess = db.ProductProcesses.FirstOrDefault(q => q.Product.Code == ProductProcess.Product.Code && q.Order == ProductProcess.Order + 1);

            if (NextProductProcess != null)
            {
                //gets the next productprocess 
                Wip nextw = db.Wips.FirstOrDefault<Wip>(e => e.ProductProcess.ID == NextProductProcess.ID);
                if (nextw == null)
                {
                    nextw = new Wip();
                    nextw.ProductProcess = NextProductProcess;
                    nextw.Input = Accepted;
                    nextw.UpdatedAt = DateTime.Now;
                    db.Wips.Add(nextw);
                }
                else
                {
                    db.Entry(nextw).State = EntityState.Modified;
                    nextw.UpdatedAt = DateTime.Now;
                    //adds the previous accepter quantity
                    nextw.Input = nextw.Input + Accepted;
                }
            }
        }
    }
}