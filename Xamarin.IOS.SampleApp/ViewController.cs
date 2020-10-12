using System;
using UIKit;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using Foundation;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Com.Cashfree.PG;

namespace ios_sample_app
{
    public partial class ViewController : UIViewController
    {
        String orderID;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            Button.AccessibilityIdentifier = "myButton";
            Button.SetTitle("Start Payment", UIControlState.Normal);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }

        partial void Button_TouchUpInside(UIButton sender)
        {
            TextView.Text = "";
            StartPayment();
        }

        // void Button_TouchUpInside(UIButton sender)
        //{
        //    StartPayment();
        //}

        private async void StartPayment()
        {
            var Token = await GetOrderToken();
            var Params = GetCheckoutParams(Token);
            DoPayment(Params);
        }

            
        private NSDictionary<NSString, NSObject> GetCheckoutParams(String Token)
        {
            var keys = new[]
            {
                new NSString("appId"),
                new NSString("tokenData"),
                new NSString("orderId"),
                new NSString("orderAmount"),
                new NSString("orderCurrency"),
                new NSString("orderNote"),
                new NSString("customerName"),
                new NSString("customerPhone"),
                new NSString("customerEmail")

                //new NSString("paymentOption"),
                //new NSString("card_number"),
                //new NSString("card_expiryMonth"),
                //new NSString("card_expiryYear"),
                //new NSString("card_holder"),
                //new NSString("card_cvv")
            };

            var objects = new[]
            {
                // don't have to be strings... can be any NSObject.
                new NSString(Credentials.GetAppId()),
                new NSString(Token),
                new NSString(orderID),
                new NSString("1"),
                new NSString("INR"),
                new NSString("Cashfree Test"),
                new NSString("Cashfree"),
                new NSString("9999999999"),
                new NSString("cashfree@cashfree.com")

                //card params here
                //new NSString("card"),
                //new NSString("4434260000000008"),
                //new NSString("05"),
                //new NSString("2021"),
                //new NSString("John Doe"),
                //new NSString("123")

            };
            return new NSDictionary<NSString, NSObject>(keys, objects);
        }

        private async Task<String> GetOrderToken()
        {
            HttpClient Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("x-client-id", Credentials.GetAppId());
            Client.DefaultRequestHeaders.Add("x-client-secret", Credentials.GetSecret());

            orderID = Credentials.GetOrderID();
            string request = JsonConvert.SerializeObject(GetOrderParams(orderID), Formatting.Indented);
            StringContent httpContent = new StringContent(request, Encoding.UTF8, "application/json");
            HttpResponseMessage ResponseMessage = await Client.PostAsync(Credentials.PostURL(), httpContent);
            String response = await ResponseMessage.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(response);
            return json.GetValue("cftoken").ToString();
        }

        private void DoPayment(NSDictionary<NSString, NSObject> dicionary)
        {
            var resultDelegate = new ResultDelegateImpl((arg) => showMsg(arg));
            new CFPaymentService().DoWebCheckoutPaymentWithParams(dicionary, Credentials.GetEnv(), resultDelegate);
        }

        private string showMsg(string message)
        {
            TextView.Text = message;
            return message;
        }

        private Dictionary<string, string> GetOrderParams(string orderId)
        {
            Dictionary<string, string> Params = new Dictionary<string, string>
            {
                { "orderId", orderId },
                { "orderAmount", "1" },
                { "orderCurrency", "INR" }
            };
            return Params;
        }
    }

    public class ResultDelegateImpl : ResultDelegate
    {
        Func<String, String> func;
        public ResultDelegateImpl(Func<String, String> function)
        {
            func = function;
        }

        private ResultDelegateImpl()
        {

        }

        public override void OnPaymentCompletionWithMsg(string msg)
        {
            func.Invoke(msg);
        }
    }
}