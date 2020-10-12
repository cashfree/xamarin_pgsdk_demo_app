using System;
using System.Net.Http;
using System.Text;
using Com.Cashfree.PG;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleFormsApp.ViewModels
{
    public class MainViewModel
    {
        private String orderId;
        public MainViewModel()
        {
        }

        public async Task<String> GetOrderToken()
        {
            orderId = Credentials.GetOrderID();
            HttpClient Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("x-client-id", Credentials.GetAppId());
            Client.DefaultRequestHeaders.Add("x-client-secret", Credentials.GetSecret());

            string request = JsonConvert.SerializeObject(GetOrderParams(), Formatting.Indented);
            StringContent httpContent = new StringContent(request, Encoding.UTF8, "application/json");
            HttpResponseMessage ResponseMessage = await Client.PostAsync(Credentials.PostURL(), httpContent);
            String response = await ResponseMessage.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(response);
            return json.GetValue("cftoken").ToString();
        }

        public Dictionary<String, String> GetCheckoutParams()
        {
            Dictionary<String, String> Pairs = new Dictionary<string, string>();

            Pairs.Add(CFPaymentService.ParamAppId, Credentials.GetAppId());
            Pairs.Add(CFPaymentService.ParamOrderId, orderId);
            Pairs.Add(CFPaymentService.ParamOrderAmount, "1");
            Pairs.Add(CFPaymentService.ParamOrderCurrency, "INR");
            Pairs.Add(CFPaymentService.ParamOrderNote, "Cashfree Test");
            Pairs.Add(CFPaymentService.ParamCustomerName, "Cashfree");
            Pairs.Add(CFPaymentService.ParamCustomerPhone, "9094395340");
            Pairs.Add(CFPaymentService.ParamCustomerEmail, "arjun@cashfree.com");

            return Pairs;
        }

        private Dictionary<string, string> GetOrderParams()
        {
            Dictionary<string, string> Params = new Dictionary<string, string>
            {
                { CFPaymentService.ParamOrderId, orderId },
                { CFPaymentService.ParamOrderAmount, "1" },
                { CFPaymentService.ParamOrderCurrency, "INR" }
            };
            return Params;
        }
    }
}
