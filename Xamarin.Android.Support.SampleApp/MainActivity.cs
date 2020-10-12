using System;
using System.Net.Http;
using System.Text;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using System.Threading.Tasks;
using Android.Widget;
using Com.Gocashfree.Cashfreesdk;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Android.Content;
using System.Collections.Generic;

namespace androidbindingapp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        String OrderId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Button btn = FindViewById<Button>(Resource.Id.button1);
            btn.Click += BtnOnClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void BtnOnClick(object sender, EventArgs eventArgs)
        {
            TextView textView = FindViewById<TextView>(Resource.Id.textView1);
            textView.SetText("", TextView.BufferType.Normal);

            StartPayment();
        }

        private async void StartPayment()
        {
            var Token = await GetOrderToken();
            var Params = GetCheckoutParams();
            DoPayment(Params, Token);
        }

        private Dictionary<String, String> GetCheckoutParams()
        {
            Dictionary<String, String> Pairs = new Dictionary<string, string>();

            Pairs.Add(CFPaymentService.ParamAppId, Credentials.GetAppId());
            Pairs.Add(CFPaymentService.ParamOrderId, OrderId);
            Pairs.Add(CFPaymentService.ParamOrderAmount, "1");
            Pairs.Add(CFPaymentService.ParamOrderCurrency, "INR");
            Pairs.Add(CFPaymentService.ParamOrderNote, "Cashfree Test");
            Pairs.Add(CFPaymentService.ParamCustomerName, "Cashfree");
            Pairs.Add(CFPaymentService.ParamCustomerPhone, "9094395340");
            Pairs.Add(CFPaymentService.ParamCustomerEmail, "arjun@cashfree.com");

            return Pairs;
        }

        private async Task<String> GetOrderToken()
        {
            OrderId = Credentials.GetOrderID();
            HttpClient Client = new HttpClient();
            Client.DefaultRequestHeaders.Add("x-client-id", Credentials.GetAppId());
            Client.DefaultRequestHeaders.Add("x-client-secret", Credentials.GetSecret());

            string request = JsonConvert.SerializeObject(GetOrderParams(), Formatting.Indented);
            StringContent httpContent = new StringContent(request, Encoding.UTF8, "application/json");
            HttpResponseMessage ResponseMessage = await Client.PostAsync(Credentials.PostURL(), httpContent);
            String response = await ResponseMessage.Content.ReadAsStringAsync();
            JObject json = JObject.Parse(response);
            String Token = json.GetValue("cftoken").ToString();

            Toast.MakeText(this, "Token is " + Token, ToastLength.Short).Show();

            return Token;
        }

        private void DoPayment(Dictionary<string, string> pairs, string token)
        {
            CFPaymentService.CFPaymentServiceInstance.DoPayment(this, pairs, token, Credentials.GetEnv());
        }

        private Dictionary<string, string> GetOrderParams()
        {
            Dictionary<string, string> Params = new Dictionary<string, string>
            {
                { CFPaymentService.ParamOrderId, OrderId },
                { CFPaymentService.ParamOrderAmount, "1" },
                { CFPaymentService.ParamOrderCurrency, "INR" }
            };
            return Params;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (data != null)
            {
                TextView textView = FindViewById<TextView>(Resource.Id.textView1);
                String dataString = data.GetStringExtra("txStatus");
                if (dataString != null)
                {
                    textView.SetText(dataString, Android.Widget.TextView.BufferType.Normal);
                }
            }

        }

    }
}
