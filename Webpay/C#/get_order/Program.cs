using System.Net;

class Test
{
    static void Main()
    {
        Console.WriteLine("Running GET request for Webpay (C#)");
        try
        {
            string url = "https://webpayadminservicestage.svea.com/AdminService.svc/secure";
            string action = "http://tempuri.org/IAdminService/GetOrders";

            string soapEnvelope = @"
                <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:tem=""http://tempuri.org/"" xmlns:dat=""http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service"">
                    <soap:Header xmlns:wsa=""http://www.w3.org/2005/08/addressing"">
                        <wsa:Action>http://tempuri.org/IAdminService/GetOrders</wsa:Action>
                        <wsa:To>https://webpayadminservicestage.svea.com/AdminService.svc/secure</wsa:To>
                    </soap:Header>
                    <soap:Body>
                        <tem:GetOrders>
                            <tem:request>
                                <dat:Authentication>
                                    <dat:Password>WEBPAY_PASSWORD</dat:Password>
                                    <dat:Username>WEBPAY_PASSWORD</dat:Username>
                                </dat:Authentication>
                                <dat:OrdersToRetrieve>
                                    <dat:GetOrderInformation>
                                        <dat:ClientId>WEBPAY_CLIENT_ID</dat:ClientId>
                                        <dat:OrderType>Invoice</dat:OrderType>
                                        <dat:SveaOrderId>WEBPAY_ORDER_TO_FETCH</dat:SveaOrderId>
                                    </dat:GetOrderInformation>
                                </dat:OrdersToRetrieve>
                            </tem:request>
                        </tem:GetOrders>
                    </soap:Body>
                </soap:Envelope>";

            var filePath = Path.Combine("..", "created_order_id.txt");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: Order ID file not found at {filePath}");
                return;
            }

            string sveaOrderId = File.ReadAllText(filePath).Trim();
            if (string.IsNullOrEmpty(sveaOrderId))
            {
                Console.WriteLine("Error: Order ID is empty.");
                return;
            }

            //Console.WriteLine($"Using SveaOrderId: {sveaOrderId}");
            soapEnvelope = soapEnvelope.Replace("WEBPAY_ORDER_TO_FETCH", sveaOrderId); 

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/soap+xml;charset=UTF-8";
            request.Headers.Add("SOAPAction", action);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(soapEnvelope);
            }

            var response = (HttpWebResponse)request.GetResponse();
            //Console.WriteLine("Response Code : " + (int)response.StatusCode);

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                string responseContent = streamReader.ReadToEnd();
                //Console.WriteLine("Response:");
                //Console.WriteLine(responseContent);
            }

            if ((int)response.StatusCode == 200)
            {
                Console.WriteLine("Success!");
            }
            else
            {
                Console.WriteLine("Failed...");
            }

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }
}
