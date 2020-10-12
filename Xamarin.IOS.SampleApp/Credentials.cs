using System;
namespace ios_sample_app
{
    public static class Credentials
    {
        public static String TEST = "TEST";
        public static String PROD = "PROD";

        public static String GetAppId()
        {
            if (GetEnv().Equals(TEST))
            {
                throw new ArgumentNullException("-test-appId-here--");
                //return "-test-appId-here-";
            }
            else
            {
                throw new ArgumentNullException("-prod-appId-here-");
                //return "-prod-secret-here-";
            }
        }

        public static String GetEnv()
        {
            // return PROD;
            return TEST;
        }

        public static String GetSecret()
        {
            if (GetEnv().Equals(TEST))
            {
                throw new ArgumentNullException("-test-secret-here-");
                //return "-test-secret-here-";
            }
            else
            {
                throw new ArgumentNullException("-prod-secret-here-");
                //return "-prod-secret-here-";
            }
        }

        public static string GetOrderID()
        {
            return new Random().Next(100000000).ToString();
        }

        public static string PostURL()
        {
            return GetEnv().Equals(TEST) ? "https://test.cashfree.com/api/v2/cftoken/order" : "https://api.cashfree.com/api/v2/cftoken/order";
        }
    }
}
