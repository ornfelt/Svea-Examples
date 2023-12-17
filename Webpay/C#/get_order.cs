using System.Net;

class Test
{
    static void Main()
    {
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
                                    <dat:Password>sverigetest</dat:Password>
                                    <dat:Username>sverigetest</dat:Username>
                                </dat:Authentication>
                                <dat:OrdersToRetrieve>
                                    <dat:GetOrderInformation>
                                        <dat:ClientId>79021</dat:ClientId>
                                        <dat:OrderType>Invoice</dat:OrderType>
                                        <dat:SveaOrderId>9731563</dat:SveaOrderId>
                                    </dat:GetOrderInformation>
                                </dat:OrdersToRetrieve>
                            </tem:request>
                        </tem:GetOrders>
                    </soap:Body>
                </soap:Envelope>";

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/soap+xml;charset=UTF-8";
            request.Headers.Add("SOAPAction", action);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(soapEnvelope);
            }

            var response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine("Response Code : " + (int)response.StatusCode);

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                string responseContent = streamReader.ReadToEnd();
                Console.WriteLine("Response:");
                Console.WriteLine(responseContent);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
