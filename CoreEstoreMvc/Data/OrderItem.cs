using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Data
{
    public class OrderItem : AppEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public int Quantity { get; set; }


        [NotMapped]
        public decimal DiscountAmount { get => Price - Discount / 100m; }

        [NotMapped]
        public decimal DiscountedPrice { get => Price - DiscountAmount; }

        [NotMapped]
        public decimal Amount { get => Quantity * DiscountedPrice; }
        public virtual Product Product { get; set; }

        public virtual Order Order { get; set; }

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<OrderItem>(entity => 
            {
                entity
                .Property(p => p.Price)
                .HasColumnType("money");
                

            
            });
        }
    }
}
