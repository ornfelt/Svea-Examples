using System.Text;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Testing
{
    class SveaAuth
    {
        private static HttpRequestMessage httpRequest;

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Running Create request for Checkout (C#)");
            await SendRequest();
            //Console.ReadLine();
        }

        private static async Task SendRequest()
        {
            var client = new HttpClient();
            var randomOrderId = GenerateRandomOrderId();
            httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://checkoutapistage.svea.com/api/orders");

            //String body = "{\"countryCode\":\"SE\",\"currency\":\"SEK\",\"locale\":\"sv-SE\",\"clientOrderNumber\":\"1234ABCDXX1\",\"merchantSettings\":{\"CheckoutValidationCallBackUri\":\"https://your.domain/validation-callback/{checkout.order.uri}\",\"PushUri\":\"https://your.domain/push-callback/{checkout.order.uri}\",\"TermsUri\":\"https://your.domain/terms/\",\"CheckoutUri\":\"https://your.domain/checkout-callback/\",\"ConfirmationUri\":\"https://your.domain/confirmation-callback/\",\"ActivePartPaymentCampaigns\":null,\"PromotedPartPaymentCampaign\":0},\"cart\":{\"Items\":[{\"ArticleNumber\":\"ABC80\",\"Name\":\"Computer\",\"Quantity\":300,\"UnitPrice\":500000,\"DiscountPercent\":1000,\"VatPercent\":2500,\"Unit\":null,\"TemporaryReference\":null,\"RowNumber\":1,\"MerchantData\":null},{\"ArticleNumber\":\"ABC81\",\"Name\":\"AnotherComputer\",\"Quantity\":200,\"UnitPrice\":400000,\"DiscountAmount\":10000,\"VatPercent\":2500,\"Unit\":null,\"TemporaryReference\":null,\"RowNumber\":2,\"MerchantData\":null}]},\"presetValues\":[{\"TypeName\":\"EmailAddress\",\"Value\":\"test.person@svea.com\",\"IsReadonly\":true}],\"identityFlags\":null,\"requireElectronicIdAuthentication\":false,\"partnerKey\":null,\"merchantData\":null}";
            string filePath = "create_order_payload.json";
            string body = await File.ReadAllTextAsync(filePath);
            body = body.Replace("my_order_id", randomOrderId);
            httpRequest.Content = new StringContent(body, Encoding.UTF8, "application/json");
            await SetHttpRequestHeaders(client, body);

            var response = await client.SendAsync(httpRequest);
            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            //Console.WriteLine("reponse: " + response);
            //Console.WriteLine("response body: " + responseBody);

            if (response.StatusCode == System.Net.HttpStatusCode.OK || 
                    response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                Console.WriteLine("Success!");

                var orderIdRegex = new Regex(@"""OrderId"":\s*(\d+)", RegexOptions.IgnoreCase);
                var match = orderIdRegex.Match(responseBody);

                if (match.Success)
                {
                    var orderId = match.Groups[1].Value;
                    //Console.WriteLine($"Extracted OrderId: {orderId}");

                    var outputFilePath = "../created_order_id.txt";
                    try
                    {
                        await File.WriteAllTextAsync(outputFilePath, orderId);
                        //Console.WriteLine($"OrderId saved to {outputFilePath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to save OrderId to file: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Failed to extract OrderId from the response.");
                }
            }
            else
            {
                Console.WriteLine("Failed...");
            }
            await Task.CompletedTask;
        }

        private static string GenerateRandomOrderId()
        {
            var random = new Random();
            var orderId = new StringBuilder();
            for (int i = 0; i < 15; i++)
            {
                orderId.Append(random.Next(0, 10));
            }
            return orderId.ToString();
        }

        protected static async Task SetHttpRequestHeaders(HttpClient client, string requestMessage)
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
                String MerchantId = "CHECKOUT_MERCHANT_ID";
                String SecretKey ="CHECKOUT_SECRET_KEY";

                var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(requestMessage + SecretKey + timestamp));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(MerchantId + ":" + hashString));
            }
        }
    }
}
