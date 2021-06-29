using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CoreEstoreMvc.Models
{
    public class NewPasswordViewModel
    {
        [Display(Name = "Yeni Parola")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Password { get; set; }

        [Display(Name = "Yeni Parola Tekrarı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [Compare("Password", ErrorMessage = "{0} ve {1} alanları aynı olmalıdır!")]
        public string PasswordVerify { get; set; }
        public string Token { get; set; }
        public string Id{ get; set; }
    }
}
