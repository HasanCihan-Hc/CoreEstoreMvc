using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Data
{
    public class ShoppingCartItem : AppEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual User User { get; set; }
        public virtual Product Product { get; set; }

        [NotMapped]
        public decimal Amount => Product.DiscountedPrice * Quantity;



        public override void Build(ModelBuilder builder)
        {

        }
    }
}