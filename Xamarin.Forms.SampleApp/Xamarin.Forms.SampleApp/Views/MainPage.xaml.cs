using System;
using Xamarin.Forms;
using SampleFormsApp.ViewModels;
using Com.Cashfree.PG;

namespace SampleFormsApp.Views
{
    public partial class MainPage : ContentPage, IPaymentResult
    {
        private MainViewModel mainViewModel = new MainViewModel();
        private string buttonText;
        public string ButtonText
        {
            get { return buttonText; }
            set
            {
                buttonText = value;
                OnPropertyChanged(nameof(ButtonText)); // Notify that there was a change on this property
            }
        }

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;

            ButtonText = "Result will Show here"; // It will be shown at your label
        }

        public void OnComplete(string result)
        {
            ButtonText = result;
        }

        async void Button_Clicked(Object sender, EventArgs e)
        {
            var Token = await mainViewModel.GetOrderToken();
            var Params = mainViewModel.GetCheckoutParams();
            //var background = (Color)App.Current.Resources["NavigationPrimary"];
            CFPaymentService.Instance.ShowOrderID(true);
            //CFPaymentService.Instance.SetTheme(background, true);
            CFPaymentService.Instance.DoPayment(Navigation, Params, Token, Credentials.GetEnv(), this);
        }
    }
}
