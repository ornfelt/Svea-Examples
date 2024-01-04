using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SveaAuthentication
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Running GET request for Checkout (C#)");
            var testInstance = new TestClass();
            var myHeaders = testInstance.GetRequestHeaders();
            string orderId = "CHECKOUT_ORDER_TO_FETCH";
            //string url = "https://paymentadminapistage.svea.com/api/v1/orders/" + orderId;
            string url = "https://checkoutapistage.svea.com/api/orders/" + orderId;

            //Console.WriteLine("Fetching order: " + orderId);
            var response = await testInstance.SendRequest(url, myHeaders);

            //Console.WriteLine("Response: " + response);
            //Console.WriteLine("Response body: " + await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Success!");
            }
            else
            {
                Console.WriteLine("Failed...");
            }
        }
    }

    class TestClass
    {
        private const string MerchantId = "CHECKOUT_MERCHANT_ID";
        private const string SecretKey = "CHECKOUT_SECRET_KEY";

        public HttpClientHeaders GetRequestHeaders(string requestBody = "")
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            var token = CreateAuthenticationToken(requestBody, timestamp);
            var headers = new HttpClientHeaders
            {
                {"Authorization", $"Svea {token}"},
                {"Timestamp", timestamp}
            };
            return headers;
        }

        public async Task<HttpResponseMessage> SendRequest(string url, HttpClientHeaders headers)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                return await client.SendAsync(request);
            }
        }

        private string CreateAuthenticationToken(string requestMessage, string timestamp)
        {
            using (var sha512 = SHA512.Create())
            {
                var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(requestMessage + SecretKey + timestamp));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(MerchantId + ":" + hashString));
            }
        }
    }

    class HttpClientHeaders : Dictionary<string, string> { }
}
