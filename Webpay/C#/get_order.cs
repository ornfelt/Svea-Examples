using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class Test
{
    public static async Task Main(string[] args)
    {
        try
        {
            string url = "https://webpayadminservicestage.svea.com/AdminService.svc/secure";
            string action = "http://tempuri.org/IAdminService/GetOrders";

            string soapEnvelope = @"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:tem=""http://tempuri.org/"" xmlns:dat=""http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service"">
    <soap:Header xmlns:wsa=""http://www.w3.org/2005/08/addressing"">
        <wsa:Action>http://tempuri.org/IAdminService/GetOrders</wsa:Action>
        <wsa:To>https://webpayadminservicestage.svea.com/AdminService.svc/secure</wsa:To>
    </soap:Header>
    <soap:Body>
        <tem:GetOrders>
            <tem:request>
                <dat:Authentication>
                    <dat:Username>sverigetest</dat:Username>
                    <dat:Password>sverigetest</dat:Password>
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

            using (var client = new HttpClient())
            {
                var content = new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml;charset=UTF-8");
                content.Headers.Add("SOAPAction", action);

                var response = await client.PostAsync(url, content);
                var responseCode = (int)response.StatusCode;
                Console.WriteLine("Response Code : " + responseCode);

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response:");
                Console.WriteLine(responseContent);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
