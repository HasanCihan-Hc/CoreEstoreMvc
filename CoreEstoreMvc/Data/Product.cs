using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Data
{
    public class Product : BaseEntity   
    {
        [Display(Name = "Ürün Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }

        [Display (Name ="Ürün Kodu")]
        [Required(ErrorMessage ="{0} alanı boş bırakılamaz!")]
        [RegularExpression("^[0-9A-Z-]{6,16}$",ErrorMessage ="Lütfen geçerli bir indirim yazınız!")]
        public string Code { get; set; }

        [Display(Name ="Min. Adet ")]
        [Range(1,int.MaxValue, ErrorMessage ="{0} alanı en az {1}, en çok {2} olmalıdır!")]
        public int MinOrder { get; set; }

        public string Picture { get; set; }

        
        public decimal Price { get; set; }

        [Display(Name = "İndirim (%)")]
        [RegularExpression("^[0-9]{1,2}$", ErrorMessage = "Lütfen geçerli bir indirim yazınız!")]
        public int Discount { get; set; }

        public string Descriptions { get; set; }
        public int Reviews { get; set; }

        [Display(Name= "Marka")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public int BrandId { get; set; }

        [NotMapped]
        [Display(Name="Görsel")]
        public IFormFile PictureFile { get; set; }

        [NotMapped]
        [Display(Name="Foto Galeri Görselleri")]
        public IEnumerable<IFormFile> PictureFiles { get; set; }

        [NotMapped]
        [Display(Name = "Kategoriler")]
        [Required(ErrorMessage = "Lütfen en az bir kategori seçiniz")]
        public IEnumerable<int> SelectedCategories { get; set; }

        [NotMapped]
        [Display(Name = "Fiyat")]
        [RegularExpression(@"^[0-9]+(\,[0-9]{1,2})?$", ErrorMessage = "Lütfen geçerli bir fiyat yazınız!")]
        public string PriceText { get; set; }



        [NotMapped]
        public decimal DiscountAmount { get => Price * Discount / 100m; }

        [NotMapped]
        public decimal DiscountedPrice { get => Price - DiscountAmount; }

        public virtual ICollection<CategoryProduct> CategoryProducts { get; set; } = new HashSet<CategoryProduct>();
        public virtual ICollection<ProductPicture> ProductPictures { get; set; } = new HashSet<ProductPicture>();
        public virtual ICollection<ProductComment> ProductComments { get; set; } = new HashSet<ProductComment>();
        public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new HashSet<ShoppingCartItem>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
        public virtual Brand Brand { get; set; }

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Product>(entity => 
            {
                entity
                .Property(p => p.Name)
                .HasMaxLength(400)
                .IsRequired();

                entity
                .HasIndex(p => p.Name)
                .IsUnique(false);

                entity
                .Property(p => p.Picture)
                .IsUnicode(false);

                entity
                .Property(p => p.Price)
                .HasColumnType("money");
                

                
                entity
                .HasMany(p => p.CategoryProducts)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

                entity
                .HasMany(p => p.ProductPictures)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

                entity
                .HasMany(p => p.ProductComments)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

                entity
                .HasMany(p => p.ShoppingCartItems)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

                entity
                .HasMany(p => p.OrderItems)
                .WithOne(p => p.Product)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);


                //SoftDelete?



            });
        }
        //Specification özelliği eklenecek


    }
}
