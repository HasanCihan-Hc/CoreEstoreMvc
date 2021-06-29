using PaymentBase;
using System;

namespace GarantiPaymentModule
{
    public class Module : Payment
    {
        public override string ProviderName => "GARANTI";

        public override PaymentResult Pay(decimal Amount, string CardNumber, string Name, string ExpireDate, string CVC, int? Installment = 1)
        {
            return new PaymentResult { Succeded = true, Error = null };
        }

        public override PaymentResult Pay(PaymentParameters parameters)
        {
            return new PaymentResult { Succeded = true, Error = null };
        }
    }
}