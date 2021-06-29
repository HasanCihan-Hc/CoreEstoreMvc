using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaymentBase
{
    public static class BinCheck
    {

        private const string baseUrl = "https://lookup.binlist.net/";

        public async static Task<BinResponse> Request(string cardNumber)
        {
            if (cardNumber.Length < 6)
                throw new InvalidOperationException("Kart numarasının ilk altı hanesi girilmelidir");

            using (var client = new HttpClient())
            {
                var absoluteUrl = $"{baseUrl}{cardNumber}";
                var response = await client.GetAsync(absoluteUrl);
                var responseContent = await response.Content.ReadAsStringAsync();
                //deserialization
                return JsonConvert.DeserializeObject<BinResponse>(responseContent);
            }
        }
    }
}
