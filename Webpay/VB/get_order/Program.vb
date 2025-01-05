Imports System
Imports System.IO
Imports System.Net

Module Program
    Sub Main()
        Console.WriteLine("Running GET request for Webpay (VB.NET)")
        Try
            Dim url As String = "https://webpayadminservicestage.svea.com/AdminService.svc/secure"
            Dim action As String = "http://tempuri.org/IAdminService/GetOrders"

            Dim soapEnvelope As String = "<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" " & _
                                         "xmlns:tem=""http://tempuri.org/"" xmlns:dat=""http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service"">" & _
                                         "<soap:Header xmlns:wsa=""http://www.w3.org/2005/08/addressing"">" & _
                                         "<wsa:Action>http://tempuri.org/IAdminService/GetOrders</wsa:Action>" & _
                                         "<wsa:To>https://webpayadminservicestage.svea.com/AdminService.svc/secure</wsa:To>" & _
                                         "</soap:Header>" & _
                                         "<soap:Body>" & _
                                         "<tem:GetOrders>" & _
                                         "<tem:request>" & _
                                         "<dat:Authentication>" & _
                                         "<dat:Password>WEBPAY_PASSWORD</dat:Password>" & _
                                         "<dat:Username>WEBPAY_PASSWORD</dat:Username>" & _
                                         "</dat:Authentication>" & _
                                         "<dat:OrdersToRetrieve>" & _
                                         "<dat:GetOrderInformation>" & _
                                         "<dat:ClientId>WEBPAY_CLIENT_ID</dat:ClientId>" & _
                                         "<dat:OrderType>Invoice</dat:OrderType>" & _
                                         "<dat:SveaOrderId>WEBPAY_ORDER_TO_FETCH_VALUE</dat:SveaOrderId>" & _
                                         "</dat:GetOrderInformation>" & _
                                         "</dat:OrdersToRetrieve>" & _
                                         "</tem:request>" & _
                                         "</tem:GetOrders>" & _
                                         "</soap:Body>" & _
                                         "</soap:Envelope>"

            Dim sveaOrderId As String = "WEBPAY_ORDER_TO_FETCH_VALUE"
            Try
                Dim filePath As String = "../created_order_id.txt"

                If File.Exists(filePath) Then
                    sveaOrderId = File.ReadAllText(filePath).Trim()
                Else
                    Console.WriteLine("Order ID file not found. Using default placeholder.")
                End If
            Catch ex As Exception
                Console.WriteLine($"Failed to read SveaOrderId from file: {ex.Message}")
            End Try

            'Console.WriteLine("Using SveaOrderId: " + sveaOrderId)
            soapEnvelope = soapEnvelope.replace("WEBPAY_ORDER_TO_FETCH_VALUE", sveaOrderId)

            Dim request As HttpWebRequest = CType(WebRequest.Create(url), HttpWebRequest)
            request.Method = "POST"
            request.ContentType = "application/soap+xml;charset=UTF-8"
            request.Headers.Add("SOAPAction", action)

            Using streamWriter As New StreamWriter(request.GetRequestStream())
                streamWriter.Write(soapEnvelope)
            End Using

            Using response As HttpWebResponse = CType(request.GetResponse(), HttpWebResponse)
                Using streamReader As New StreamReader(response.GetResponseStream())
                    Dim responseContent As String = streamReader.ReadToEnd()

                    'Console.WriteLine("Response:")
                    'Console.WriteLine(responseContent)

                    If response.StatusCode = HttpStatusCode.OK Then
                        Console.WriteLine("Success!")
                    Else
                        Console.WriteLine("Failed...")
                    End If
                End Using
            End Using
        Catch e As Exception
            Console.WriteLine(e.Message)
        End Try
    End Sub
End Module

