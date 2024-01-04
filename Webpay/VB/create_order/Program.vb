Imports System
Imports System.IO
Imports System.Net
Imports System.Text

Module Program
    Sub Main()
        Console.WriteLine("Running Create request for Webpay (VB.NET)")
        Try
            Dim url As String = "https://webpaywsstage.svea.com/sveawebpay.asmx"
            Dim action As String = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu"
            Dim randomOrderId As String = GenerateRandomOrderId()

            Dim soapTemplate As String = "<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" " & _
                                        "xmlns:web=""https://webservices.sveaekonomi.se/webpay"" " & _
                                        "xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">" & _
                                        "<soap:Header/>" & _
                                        "<soap:Body>" & _
                                        "<web:CreateOrderEu>" & _
                                        "<web:request>" & _
                                        "<web:Auth>" & _
                                        "<web:ClientNumber>WEBPAY_CLIENT_ID</web:ClientNumber>" & _
                                        "<web:Username>WEBPAY_PASSWORD</web:Username>" & _
                                        "<web:Password>WEBPAY_PASSWORD</web:Password>" & _
                                        "</web:Auth>" & _
                                        "<web:CreateOrderInformation>" & _
                                        "<web:ClientOrderNumber>my_order_id</web:ClientOrderNumber>" & _
                                        "<web:OrderRows>" & _
                                        "<web:OrderRow>" & _
                                        "<web:ArticleNumber>123</web:ArticleNumber>" & _
                                        "<web:Description>Some Product 1</web:Description>" & _
                                        "<web:PricePerUnit>293.6</web:PricePerUnit>" & _
                                        "<web:PriceIncludingVat>false</web:PriceIncludingVat>" & _
                                        "<web:NumberOfUnits>2</web:NumberOfUnits>" & _
                                        "<web:Unit>st</web:Unit>" & _
                                        "<web:VatPercent>25</web:VatPercent>" & _
                                        "<web:DiscountPercent>0</web:DiscountPercent>" & _
                                        "<web:DiscountAmount>0</web:DiscountAmount>" & _
                                        "<web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>" & _
                                        "</web:OrderRow>" & _
                                        "<web:OrderRow>" & _
                                        "<web:ArticleNumber>456</web:ArticleNumber>" & _
                                        "<web:Description>Some Product 2</web:Description>" & _
                                        "<web:PricePerUnit>39.2</web:PricePerUnit>" & _
                                        "<web:PriceIncludingVat>false</web:PriceIncludingVat>" & _
                                        "<web:NumberOfUnits>1</web:NumberOfUnits>" & _
                                        "<web:Unit>st</web:Unit>" & _
                                        "<web:VatPercent>25</web:VatPercent>" & _
                                        "<web:DiscountPercent>0</web:DiscountPercent>" & _
                                        "<web:DiscountAmount>0</web:DiscountAmount>" & _
                                        "<web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>" & _
                                        "</web:OrderRow>" & _
                                        "</web:OrderRows>" & _
                                        "<web:CustomerIdentity>" & _
                                        "<web:NationalIdNumber>4605092222</web:NationalIdNumber>" & _
                                        "<web:Email>firstname.lastname@svea.com</web:Email>" & _
                                        "<web:PhoneNumber>080000000</web:PhoneNumber>" & _
                                        "<web:FullName>Tester Testsson</web:FullName>" & _
                                        "<web:Street>Gatan 99</web:Street>" & _
                                        "<web:ZipCode>12345</web:ZipCode>" & _
                                        "<web:Locality>16733</web:Locality>" & _
                                        "<web:CountryCode>SE</web:CountryCode>" & _
                                        "<web:CustomerType>Individual</web:CustomerType>" & _
                                        "</web:CustomerIdentity>" & _
                                        "<web:OrderDate>2023-12-18T11:07:23</web:OrderDate>" & _
                                        "<web:OrderType>Invoice</web:OrderType>" & _
                                        "</web:CreateOrderInformation>" & _
                                        "</web:request>" & _
                                        "</web:CreateOrderEu>" & _
                                        "</soap:Body>" & _
                                        "</soap:Envelope>"

            Dim soapEnvelope = soapTemplate.Replace("my_order_id", randomOrderId)
            Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/soap+xml;charset=UTF-8"

            If Not String.IsNullOrEmpty(action) Then
                request.Headers.Add("SOAPAction", action)
            End If

            Using streamWriter As New StreamWriter(request.GetRequestStream())
                streamWriter.Write(soapEnvelope)
            End Using

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseContent As String = streamReader.ReadToEnd()

                    'Console.WriteLine("Response:")
                    'Console.WriteLine(responseContent)

                    If response.StatusCode = HttpStatusCode.OK AndAlso responseContent.ToLower().Contains("accepted>true") Then
                        Console.WriteLine("Success!")
                    Else
                        Console.WriteLine("Failed...")
                    End If
                End Using
            End Using
        Catch e As Exception
            Console.WriteLine(e.Message)
        End Try
        Console.WriteLine("----------------------------------------------------------")
    End Sub

    Private Function GenerateRandomOrderId() As String
        Dim random As New Random()
        Dim orderId As New StringBuilder()
        For i As Integer = 0 To 7
            orderId.Append(random.Next(0, 10)) ' Append a random digit
        Next
        Return orderId.ToString()
    End Function

End Module
