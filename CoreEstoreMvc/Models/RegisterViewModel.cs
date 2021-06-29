using CoreEstoreMvc.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Models
{
    public class RegisterViewModel 
    {
        [Display(Name ="E-Posta")]
        [Required(ErrorMessage ="{0} alanı boş bırakılamaz!")]
        [EmailAddress(ErrorMessage ="Lütfen geçerli bir e-posta adresi yazınız!")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [Display(Name = "Parola")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Password { get; set; }

        [Display(Name = "Parola Tekrarı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [Compare("Password",ErrorMessage ="{0} ve {1} alanları aynı olmalıdır!")]
        public string PasswordVerify { get; set; }

        [Display(Name = "Ad-Soyad")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }
        [Display(Name = "Doğum Tarihi")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Cinsiyet")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public Genders  Gender { get; set; }
    }
}
