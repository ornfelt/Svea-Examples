open System
open System.IO
open System.Net
open System.Text

let generateRandomOrderId () =
    let random = Random()
    let orderId = StringBuilder()
    for i in 0 .. 7 do
        orderId.Append(random.Next(0, 10)) |> ignore
    orderId.ToString()

let randomOrderId = generateRandomOrderId()
let soapTemplate = 
    """
    <soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:web="https://webservices.sveaekonomi.se/webpay" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
        <soap:Header/>
        <soap:Body>
            <web:CreateOrderEu>
                <web:request>
                    <web:Auth>
                        <web:ClientNumber>WEBPAY_CLIENT_ID</web:ClientNumber>
                        <web:Username>WEBPAY_PASSWORD</web:Username>
                        <web:Password>WEBPAY_PASSWORD</web:Password>
                    </web:Auth>
                    <web:CreateOrderInformation>
                        <web:ClientOrderNumber>my_order_id</web:ClientOrderNumber>
                        <web:OrderRows>
                            <web:OrderRow>
                                <web:ArticleNumber>123</web:ArticleNumber>
                                <web:Description>Some Product 1</web:Description>
                                <web:PricePerUnit>293.6</web:PricePerUnit>
                                <web:PriceIncludingVat>false</web:PriceIncludingVat>
                                <web:NumberOfUnits>2</web:NumberOfUnits>
                                <web:Unit>st</web:Unit>
                                <web:VatPercent>25</web:VatPercent>
                                <web:DiscountPercent>0</web:DiscountPercent>
                                <web:DiscountAmount>0</web:DiscountAmount>
                                <web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>
                            </web:OrderRow>
                            <web:OrderRow>
                                <web:ArticleNumber>456</web:ArticleNumber>
                                <web:Description>Some Product 2</web:Description>
                                <web:PricePerUnit>39.2</web:PricePerUnit>
                                <web:PriceIncludingVat>false</web:PriceIncludingVat>
                                <web:NumberOfUnits>1</web:NumberOfUnits>
                                <web:Unit>st</web:Unit>
                                <web:VatPercent>25</web:VatPercent>
                                <web:DiscountPercent>0</web:DiscountPercent>
                                <web:DiscountAmount>0</web:DiscountAmount>
                                <web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>
                            </web:OrderRow>
                        </web:OrderRows>
                        <web:CustomerIdentity>
                            <web:NationalIdNumber>4605092222</web:NationalIdNumber>
                            <web:Email>firstname.lastname@svea.com</web:Email>
                            <web:PhoneNumber>080000000</web:PhoneNumber>
                            <web:FullName>Tester Testsson</web:FullName>
                            <web:Street>Gatan 99</web:Street>
                            <web:ZipCode>12345</web:ZipCode>
                            <web:Locality>16733</web:Locality>
                            <web:CountryCode>SE</web:CountryCode>
                            <web:CustomerType>Individual</web:CustomerType>
                        </web:CustomerIdentity>
                        <web:OrderDate>2023-12-18T11:07:23</web:OrderDate>
                        <web:OrderType>Invoice</web:OrderType>
                    </web:CreateOrderInformation>
                </web:request>
            </web:CreateOrderEu>
        </soap:Body>
    </soap:Envelope>
    """
let soapEnvelope = soapTemplate.Replace("my_order_id", randomOrderId)

let sendSoapRequest (url: string) (action: string) (soapXml: string) =
    async {
        try
            let request = WebRequest.Create(url) :?> HttpWebRequest
            request.Method <- "POST"
            request.ContentType <- "application/soap+xml;charset=UTF-8"
            if not (String.IsNullOrEmpty(action)) then
                request.Headers.Add("SOAPAction", action)

            use streamWriter = new StreamWriter(request.GetRequestStream())
            streamWriter.Write(soapXml)
            streamWriter.Flush()

            use response = request.GetResponse() :?> HttpWebResponse
            use streamReader = new StreamReader(response.GetResponseStream())

            let responseContent = streamReader.ReadToEnd()
            //Console.WriteLine("Response:")
            //Console.WriteLine(responseContent)
            
            if response.StatusCode = HttpStatusCode.OK && responseContent.ToLower().Contains("accepted>true") then
                Console.WriteLine("Success!")
            else
                Console.WriteLine("Failed...")
            Console.WriteLine("----------------------------------------------------------")
        with
        | e -> Console.WriteLine(e.Message)
    }

[<EntryPoint>]
let main argv =
    Console.WriteLine("Running Create request for Webpay (F#)")
    let url = "https://webpaywsstage.svea.com/sveawebpay.asmx"
    let action = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu"
    sendSoapRequest url action soapEnvelope |> Async.RunSynchronously
    0 // Return an integer exit code
