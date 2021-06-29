using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Data
{
    public class CategoryProduct : AppEntity
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Category Category { get; set; }

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<CategoryProduct>(entity =>
            {
                entity
                 .HasKey(p => new { p.CategoryId, p.ProductId });

            });
        }
    }
}
