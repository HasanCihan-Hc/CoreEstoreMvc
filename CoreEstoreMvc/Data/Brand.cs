using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Data
{
    public class Brand : SortableBaseEntity
    {
        [Display(Name = "Marka Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [MaxLength(50, ErrorMessage = "{0} alanı en fazla {1} karakter uzunluğunda olmalıdır!")]
        public string Name { get; set; }

        [Display(Name = "Logo Görseli")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Logo { get; set; }

        [NotMapped]
        public IFormFile LogoFile { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Brand>(entity =>
            {
                entity
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsUnicode();

                entity
                .Property(p => p.Logo)
                .IsRequired()
                .IsUnicode(false);

                entity
                .HasIndex(p => p.Name)
                .IsUnique();

                entity
                .HasMany(p => p.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);


            });
        }
    }
}
