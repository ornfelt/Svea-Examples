using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace TestRequests
{
    internal class PgRequestTester
    {
        private HttpClient _httpClient;

        public PgRequestTester()
        {
            _httpClient = new HttpClient();
        }
		
		public static async Task Main(string[] args)
		{
			await MakePostRequestAsync();
			await MakeGetQueryTransactionIdRequestAsync();
		}

        public async Task MakePostRequestAsync()
        {
            string messageXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>125123123</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>";

            string encodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageXML));
            string secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
            string mac = GetSha512Hash(encodedMessage + secret);

            Console.WriteLine("mac: " + mac);
            Console.WriteLine("encodedMessage: " + encodedMessage);

            try
            {
                var client = new HttpClient();
                var url = "https://webpaypaymentgatewaystage.svea.com/webpay/payment";

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("merchantid", "1200"),
                new KeyValuePair<string, string>("message", encodedMessage),
                new KeyValuePair<string, string>("mac", mac)
            });

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task MakeGetQueryTransactionIdRequestAsync()
        {
            //string messageXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><query><customerrefno>temping1238u96896</customerrefno></query>";
            string messageXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><query><transactionid>900497</transactionid></query>";

            string encodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageXML));
            string secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
            string mac = GetSha512Hash(encodedMessage + secret);

            Console.WriteLine("mac: " + mac);
            Console.WriteLine("encodedMessage: " + encodedMessage);

            try
            {
                var client = new HttpClient();
                //var url = "https://webpaypaymentgatewaytest.svea.com/webpay/rest/querycustomerrefno";
                var url = "https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid";

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("merchantid", "1200"),
                new KeyValuePair<string, string>("message", encodedMessage),
                new KeyValuePair<string, string>("mac", mac)
            });

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                string responseString = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Response status code: " + response.StatusCode);
                Console.WriteLine("Response message: " + responseString);

                // Parsing the XML to find the 'message' element
                XDocument xmlDoc = XDocument.Parse(responseString);
                XElement messageElement = xmlDoc.Root.Element("message");
                if (messageElement != null)
                {
                    string encodedMessagePart = messageElement.Value;

                    // Decoding the Base64 message part
                    byte[] decodedBytes = Convert.FromBase64String(encodedMessagePart);
                    string decodedString = Encoding.UTF8.GetString(decodedBytes);
                    Console.WriteLine("Decoded message:");
                    Console.WriteLine(decodedString);
                }
                else
                {
                    Console.WriteLine("Message element not found in response");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private string GetSha512Hash(string input)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                byte[] bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
