using Microsoft.AspNetCore.Hosting;
using PaymentBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CoreEstoreMvc.Services
{
    public class PaymentService : IPaymentSevice
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;

        public PaymentService(IConfiguration configuration,
            
            IWebHostEnvironment webHostEnvironment
            )
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;

            var files = Directory
            .GetFiles(Path.Combine(webHostEnvironment.WebRootPath, "lib", "paymentmodules"), "*.dll");

            files
            .ToList()
            .ForEach(p =>
            {
                var type = Assembly
                .LoadFile(p)
                .GetTypes()
                .SingleOrDefault(p => !p.IsAbstract && p.GetInterfaces().Any(q => q.Name == nameof(IPayment)));

                PaymentMethods.Add((IPayment)Activator.CreateInstance(type));

            });
        }

        public IList<IPayment> PaymentMethods { get; set; } = new List<IPayment>();

        public async Task<PaymentResult> Pay(PaymentParameters parameters)
        {
            var result = default(PaymentResult);
            var cardBankName = (await BinCheck.Request(parameters.CardNumber)).Bank.Name;
            var method = PaymentMethods.SingleOrDefault(p => p.ProviderName == cardBankName);
            if (method == null)
                method = PaymentMethods.Single(p => p.ProviderName == configuration.GetValue<string>("Application:DefaultPaymentProvider"));
            result = method.Pay(parameters);

            return result;
        }
    }
}