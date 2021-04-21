using System;
using System.Net.Http;
using System.Text;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Com.Cashfree.PG;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Android.Content;
using AndroidX.ConstraintLayout.Helper.Widget;
using AndroidX.ConstraintLayout.Widget;
using Android.Graphics;
using Android.Util;
using Android.Content.Res;

namespace bindingsampleapp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, CFPaymentService.IUPIAppsCallback
    {
        String OrderId;
        Flow flow;
        ConstraintLayout parentLayout;
        ProgressBar progressBar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            AndroidX.AppCompat.Widget.Toolbar toolbar = FindViewById<AndroidX.AppCompat.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Button btn = FindViewById<Button>(Resource.Id.button1);
            btn.Click += BtnOnWebClick;
            Button btn2 = FindViewById<Button>(Resource.Id.button2);
            btn2.Click += BtnOnUPIClick;
            flow = FindViewById<Flow>(Resource.Id.flow1);
            parentLayout = FindViewById<ConstraintLayout>(Resource.Id.upi_layout);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            getUpiApps();
        }

        private void getUpiApps()
        {
            progressBar.Visibility = ViewStates.Visible;
            CFPaymentService.CFPaymentServiceInstance.GetUpiClients(this, this);
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

        private void BtnOnWebClick(object sender, EventArgs eventArgs)
        {
            TextView textView = FindViewById<TextView>(Resource.Id.textView1);
            textView.SetText("Result Will Show Here", TextView.BufferType.Normal);

            StartWebPayment();
        }

        private void BtnOnUPIClick(object sender, EventArgs eventArgs)
        {
            TextView textView = FindViewById<TextView>(Resource.Id.textView1);
            textView.SetText("Result Will Show Here", TextView.BufferType.Normal);

            StartUPIPayment(null);
        }

        private async void StartWebPayment()
        {
            var Token = await GetOrderToken();
            var Params = GetCheckoutParams();
            CFPaymentService.CFPaymentServiceInstance.DoPayment(this, Params, Token, Credentials.GetEnv(), "#2c3e50", "#ffffff");
        }

        private async void StartUPIPayment(String app)
        {
            var Token = await GetOrderToken();
            var Params = GetCheckoutParams();
            if (app != null)
            {
                Params.Add("appName", app);
            }
            CFPaymentService.CFPaymentServiceInstance.UpiPayment(this, Params, Token, Credentials.GetEnv());
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
                    textView.SetText(dataString, TextView.BufferType.Normal);
                }
            }

        }

        public void OnUPIAppsFetched(IList<IDictionary<string, string>> list)
        {
            if (list.Count == 0)
            {
                progressBar.Visibility = ViewStates.Gone;
                FindViewById<TextView>(Resource.Id.no_upi_apps).Visibility = ViewStates.Visible;
                return;
            }
            if (flow != null && parentLayout != null)
            {
                int i = 0;
                List<LinearLayout> imageList = new List<LinearLayout>();
                List<int> refIds = new List<int>();
                foreach (IDictionary<string, string> dict in list)
                {
                    refIds.Add(i);
                    imageList.Add(SetImage(dict["displayName"], dict["id"], dict["icon"], i));
                    i++;
                }
                RunOnUiThread(() =>
                {
                    progressBar.Visibility = ViewStates.Gone;
                    foreach (LinearLayout view in imageList)
                    {
                        ConstraintLayout.LayoutParams layoutParams = new ConstraintLayout.LayoutParams(ConstraintLayout.LayoutParams.WrapContent,
                ConstraintLayout.LayoutParams.WrapContent);
                        parentLayout.AddView(view, layoutParams);
                    }
                    flow.SetReferencedIds(refIds.ToArray());
                });
            }
        }


        private LinearLayout SetImage(string DisplayName, string Id, string Icon, int viewId)
        {
            LinearLayout ll = new LinearLayout(this);
            ll.Orientation = Android.Widget.Orientation.Vertical;
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            LinearLayout.LayoutParams textLayoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            int size = (int)Math.Round(TypedValue.ApplyDimension(
                ComplexUnitType.Dip, 56, this.Resources.DisplayMetrics));
            int textSize = (int)Math.Round(TypedValue.ApplyDimension(
                ComplexUnitType.Sp, 10, this.Resources.DisplayMetrics));
            layoutParams.Height = size;
            layoutParams.Width = size;
            textLayoutParams.Gravity = GravityFlags.CenterHorizontal;

            ImageView imageView = new ImageView(this);
            imageView.SetMaxHeight(56);
            imageView.SetMaxWidth(56);

            byte[] decodedString = Base64.Decode(Icon, Base64Flags.NoWrap);
            Bitmap decodedByte = BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length);

            imageView.SetImageBitmap(decodedByte);

            TextView textView = new TextView(this);
            textView.SetText(DisplayName, TextView.BufferType.Normal);

            ll.Id = viewId;
            ll.Click += (sender, ea) =>
            {
                StartUPIPayment(Id);
            };

            ll.AddView(imageView, layoutParams);
            ll.AddView(textView, textLayoutParams);

            return ll;
        }
    }
}
