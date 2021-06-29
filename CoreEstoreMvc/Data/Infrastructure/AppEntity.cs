using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Data
{
    public  abstract class AppEntity : IAppEntity
    {
        public abstract void Build(ModelBuilder builder);
        
    }
}
