using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Data
{
    public class Rayon  :  SortableBaseEntity
    {
        [Display(Name ="Reyon Adı")]
        [Required(ErrorMessage ="{0} alanı boş bırakılamaz!")]
        [MaxLength(50,ErrorMessage ="{0} alanı en fazla {1} karakter uzunluğunda olmalıdır!")]
        public string Name   { get; set; }
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Rayon>(entity => 
            {
                entity
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired();

                entity
                .HasIndex(p => p.Name)
                .IsUnique();

                entity
                .HasMany(p => p.Categories)
                .WithOne(p => p.Rayon)
                .HasForeignKey(p => p.RayonId)
                .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
