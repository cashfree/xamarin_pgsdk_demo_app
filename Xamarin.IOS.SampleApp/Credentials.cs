using System;
namespace ios_sample_app
{
    public static class Credentials
    {
        public static String TEST = "TEST";
        public static String PROD = "PROD";

        public static String GetAppId()
        {
            return GetEnv().Equals(TEST) ? "-test-appId-here-" : "-prod-appId-here-";
        }

        public static String GetEnv()
        {
            // return PROD;
            return TEST;
        }

        public static String GetSecret()
        {
            return GetEnv().Equals(TEST) ? "-test-secret-here-" : "-prod-secret-here-";
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
