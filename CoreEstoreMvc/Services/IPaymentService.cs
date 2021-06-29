using PaymentBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CoreEstoreMvc.Services
{
    public interface IPaymentSevice
    {
        IList<IPayment> PaymentMethods { get; }

        Task<PaymentResult> Pay(PaymentParameters parameters);
    }
}