using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreEstoreMvc.Data
{
    public class Banner : SortableBaseEntity
    {
        [Display(Name ="Görsel")]
        public string Image { get; set; }

        [Display(Name = "Url")]
        public string Url { get; set; }
       
        [Display(Name = "İlk T.")]
        [DisplayFormat(DataFormatString = @"{0:d\/MM\/yyyy}")]
        public DateTime? DateStart { get; set; }

        [Display(Name = "Son T.")]
        [DisplayFormat(DataFormatString = @"{0:d\/MM\/yyyy}")]
        public DateTime? DateEnd { get; set; }

        [Display(Name = "Görsel")]
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public int? CategoryId { get; set; }

        public virtual Category Category  { get; set; }
        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Banner>(entity =>
            {
                entity
                .Property(p => p.Image)
                .IsRequired()
                .IsUnicode(false);


            });


        }

    }
}
