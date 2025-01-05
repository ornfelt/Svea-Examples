open System
open System.Net
open System.Net.Http
open System.Text
open System.IO

// [<EntryPoint>]
let main argv =
    async {
        printfn "Running GET request for Webpay (F#)"
        let url = "https://webpayadminservicestage.svea.com/AdminService.svc/secure"
        let soapAction = "http://tempuri.org/IAdminService/GetOrders"
        let soapEnvelope = """
            <soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:tem="http://tempuri.org/" xmlns:dat="http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service">
                <soap:Header xmlns:wsa="http://www.w3.org/2005/08/addressing">
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
                                    <dat:SveaOrderId>WEBPAY_ORDER_TO_FETCH_VALUE</dat:SveaOrderId>
                                </dat:GetOrderInformation>
                            </dat:OrdersToRetrieve>
                        </tem:request>
                    </tem:GetOrders>
                </soap:Body>
            </soap:Envelope>
            """

        let orderIdFilePath = "../created_order_id.txt"
        let sveaOrderId = 
            if File.Exists(orderIdFilePath) then
                File.ReadAllText(orderIdFilePath).Trim()
            else
                failwithf "Order ID file not found: %s" orderIdFilePath

        //Console.WriteLine("Using SveaOrderId: " + sveaOrderId)
        let updatedSoapEnvelope = soapEnvelope.Replace("WEBPAY_ORDER_TO_FETCH_VALUE", sveaOrderId)

        use client = new HttpClient()
        let request = new HttpRequestMessage(HttpMethod.Post, url)
        request.Content <- new StringContent(updatedSoapEnvelope, Encoding.UTF8, "application/soap+xml")
        request.Headers.Add("SOAPAction", soapAction)

        let! response = client.SendAsync(request) |> Async.AwaitTask
        let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

        //printfn "Response Code: %d" (int response.StatusCode)
        //printfn "Response: %s" responseBody

        if response.StatusCode = HttpStatusCode.OK then
            Console.WriteLine("Success!")
        else
            Console.WriteLine("Failed...")

        return 0
    } |> Async.RunSynchronously

main [||]

