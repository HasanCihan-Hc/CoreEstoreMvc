using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Data
{
    public enum OrderStates
    {
        New, Shipped, Cancelled
    }
    public class Order : BaseEntity
    {
        public OrderStates OrderState { get; set; } = OrderStates.New;
        public string ShippingNumber { get; set; }
        [NotMapped]
        public decimal GrandTotal => OrderItems.Sum(p => p.Amount);
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Order>(entity =>
            {
                entity
                .HasMany(p => p.OrderItems)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
               
            });
        }
    }
}
