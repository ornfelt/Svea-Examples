using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Globalization;
using System.Diagnostics;
using System.IO;

namespace Testing
{
    class SveaAuth
    {
        private static HttpRequestMessage httpRequest;

        public static void Main(string[] args)
        {
            ExecuteAsync_Request();
            Console.ReadLine();
        }

        private static async void ExecuteAsync_Request()
        {
            await TestAuth();
        }

        private static async Task TestAuth()
        {
            var client = new HttpClient();
            httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://checkoutapistage.svea.com/api/orders");

            String body = "{\"countryCode\":\"SE\",\"currency\":\"SEK\",\"locale\":\"sv-SE\",\"clientOrderNumber\":\"1234ABCDXX1\",\"merchantSettings\":{\"CheckoutValidationCallBackUri\":\"https://your.domain/validation-callback/{checkout.order.uri}\",\"PushUri\":\"https://your.domain/push-callback/{checkout.order.uri}\",\"TermsUri\":\"https://your.domain/terms/\",\"CheckoutUri\":\"https://your.domain/checkout-callback/\",\"ConfirmationUri\":\"https://your.domain/confirmation-callback/\",\"ActivePartPaymentCampaigns\":null,\"PromotedPartPaymentCampaign\":0},\"cart\":{\"Items\":[{\"ArticleNumber\":\"ABC80\",\"Name\":\"Computer\",\"Quantity\":300,\"UnitPrice\":500000,\"DiscountPercent\":1000,\"VatPercent\":2500,\"Unit\":null,\"TemporaryReference\":null,\"RowNumber\":1,\"MerchantData\":null},{\"ArticleNumber\":\"ABC81\",\"Name\":\"AnotherComputer\",\"Quantity\":200,\"UnitPrice\":400000,\"DiscountAmount\":10000,\"VatPercent\":2500,\"Unit\":null,\"TemporaryReference\":null,\"RowNumber\":2,\"MerchantData\":null}]},\"presetValues\":[{\"TypeName\":\"EmailAddress\",\"Value\":\"test.person@svea.com\",\"IsReadonly\":true}],\"identityFlags\":null,\"requireElectronicIdAuthentication\":false,\"partnerKey\":null,\"merchantData\":null}";
            httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
            await SetHttpRequestHeaders(client, "", String.Empty);

            var response = await client.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Console.WriteLine("reponse: " + response);
            Console.WriteLine("response body: " + responseBody);
            await Task.CompletedTask;
        }

        protected static async Task SetHttpRequestHeaders(HttpClient client, string operation, string requestMessage)
        {
            client.Timeout = new TimeSpan(0, 0, 30);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            //string token = CreateAuthenticationToken(requestMessage, timestamp);
            var token = CreateAuthenticationToken(httpRequest.Content != null ? await httpRequest.Content.ReadAsStringAsync().ConfigureAwait(false) : string.Empty, timestamp);

            httpRequest.Headers.Add("Authorization", "Svea" + " " + token);
            httpRequest.Headers.Add("Timestamp", timestamp);
            await Task.CompletedTask;
        }

        private static String CreateAuthenticationToken (string requestMessage, string timestamp)
        {
            using (var sha512 = SHA512.Create())
            {
                String MerchantId = "123";
                String SecretKey ="xxx";

                var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(requestMessage + SecretKey + timestamp));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(MerchantId + ":" + hashString));
            }
        }
    }
}
