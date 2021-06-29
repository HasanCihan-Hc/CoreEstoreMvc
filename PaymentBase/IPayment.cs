using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentBase
{
    public class PaymentResult
    {
        public bool Succeded { get; set; }
        public string Error { get; set; }
    }
    public class PaymentParameters
    {
        [Display(Name = "Kart Numaranız")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [RegularExpression(@"[0-9]{16}", ErrorMessage = "Lütfen geçerli bir kart numarası yazınız!")]
        public string CardNumber { get; set; }

        [Display(Name = "Kart üzerindeki Ad-Soyad")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string CardHolderName { get; set; }

        [Display(Name = "Son Kullanma T.(Gün)")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public int ExpireMonth { get; set; }

        [Display(Name = "Son Kullanma T.(Ay)")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public int ExpireYear { get; set; }

        [Display(Name = "CVV")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [RegularExpression(@"[0-9]{3}", ErrorMessage = "Lütfen geçerli bir kart numarası yazınız!")]
        public string SecurityCode { get; set; }

    }
    public interface IPayment
    {
        string ProviderName { get; }
        PaymentResult Pay(decimal Amount, string CardNumber, string Name, string ExpireDate, string CVC, int? Installment = 1);
        PaymentResult Pay(PaymentParameters parameters);
    }

    public abstract class Payment : IPayment
    {
        public virtual string ProviderName { get; }
        public virtual bool IsAmex => false;
        public abstract PaymentResult Pay(decimal Amount, string CardNumber, string Name, string ExpireDate, string CVC, int? Installment = 1);
        public abstract PaymentResult Pay(PaymentParameters parameters);
    }
}
