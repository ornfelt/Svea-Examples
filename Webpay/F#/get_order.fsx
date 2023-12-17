open System
open System.Net.Http
open System.Text

[<EntryPoint>]
let main argv =
    async {
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
            </soap:Envelope>
            """

        use client = new HttpClient()
        let request = new HttpRequestMessage(HttpMethod.Post, url)
        request.Content <- new StringContent(soapEnvelope, Encoding.UTF8, "application/soap+xml")
        request.Headers.Add("SOAPAction", soapAction)

        let! response = client.SendAsync(request) |> Async.AwaitTask
        let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

        printfn "Response Code: %d" (int response.StatusCode)
        printfn "Response: %s" responseBody

        return 0
    } |> Async.RunSynchronously
