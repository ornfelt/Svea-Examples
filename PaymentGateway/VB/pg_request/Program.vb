Imports System
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports System.Threading.Tasks
Imports System.Xml.Linq

Module Program
    Dim httpClient As New HttpClient()
    Dim random As New Random()

    Function GetSha512Hash(input As String) As String
        Using sha512Hash As SHA512 = SHA512.Create()
            Dim inputBytes As Byte() = Encoding.UTF8.GetBytes(input)
            Dim bytes As Byte() = sha512Hash.ComputeHash(inputBytes)
            Return BitConverter.ToString(bytes).Replace("-", "").ToLower()
        End Using
    End Function

    Async Function MakeGetQueryTransactionIdRequestAsync() As Task
        Try
            Dim transactionId As Integer = PG_ORDER_TO_FETCH
            Dim messageXML As String = $"<?xml version=""1.0"" encoding=""UTF-8""?><query><transactionid>{transactionId}</transactionid></query>"
            Dim encodedMessage As String = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageXML))
            Dim secret As String = "PG_SECRET_KEY"
            Dim mac As String = GetSha512Hash(encodedMessage + secret)

            Dim url As String = "https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid"
            Dim content As New FormUrlEncodedContent(New Dictionary(Of String, String) From {
                {"merchantid", "PG_MERCHANT_ID"},
                {"message", encodedMessage},
                {"mac", mac}
            })

            Dim response As HttpResponseMessage = Await httpClient.PostAsync(url, content)
            Dim responseString As String = Await response.Content.ReadAsStringAsync()

            If response.StatusCode = Net.HttpStatusCode.OK Then
                Console.WriteLine("Success!")
            Else
                Console.WriteLine("Failed...")
            End If

            Dim xmlDoc As XDocument = XDocument.Parse(responseString)
            Dim messageElement As XElement = xmlDoc.Root.Element("message")
            If messageElement IsNot Nothing Then
                Dim encodedMessagePart As String = messageElement.Value
                Dim decodedBytes As Byte() = Convert.FromBase64String(encodedMessagePart)
                Dim decodedString As String = Encoding.UTF8.GetString(decodedBytes)
                'Console.WriteLine("Decoded message:")
                'Console.WriteLine(decodedString)
            Else
                Console.WriteLine("Message element not found in response")
            End If
        Catch e As Exception
            Console.WriteLine(e.Message)
        End Try
    End Function

    Async Function MakePostRequestAsync() As Task
        Try
            Dim randomRefNo As String = random.Next(1000000, 10000000).ToString()
            Dim messageXML As String = $"<?xml version=""1.0"" encoding=""UTF-8""?>" &
                                      "<payment>" &
                                      "<paymentmethod>SVEACARDPAY</paymentmethod>" &
                                      "<currency>SEK</currency>" &
                                      "<amount>500</amount>" &
                                      "<vat>100</vat>" &
                                      $"<customerrefno>{randomRefNo}</customerrefno>" &
                                      "<returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl>" &
                                      "<lang>en</lang>" &
                                      "</payment>"

            Dim encodedMessage As String = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageXML))
            Dim secret As String = "PG_SECRET_KEY"
            Dim mac As String = GetSha512Hash(encodedMessage + secret)

            Dim url As String = "https://webpaypaymentgatewaystage.svea.com/webpay/payment"
            Dim content As New FormUrlEncodedContent(New Dictionary(Of String, String) From {
                {"merchantid", "PG_MERCHANT_ID"},
                {"message", encodedMessage},
                {"mac", mac}
            })

            Dim response As HttpResponseMessage = Await httpClient.PostAsync(url, content)
            Dim responseString As String = Await response.Content.ReadAsStringAsync()

            'Console.WriteLine("response: " + responseString)

            If responseString.ToLower().Contains("enter your card details") OrElse responseString.ToLower().Contains("select card type") Then
                Console.WriteLine("Success!")
            Else
                Console.WriteLine("Failed...")
            End If
        Catch e As Exception
            Console.WriteLine(e.Message)
        End Try
    End Function

    Sub Main()
        Console.WriteLine("Running Create request for PaymentGateway (VB.NET)")
        MakePostRequestAsync().Wait()
        Console.WriteLine("Running GET request for PaymentGateway (VB.NET)")
        MakeGetQueryTransactionIdRequestAsync().Wait()
    End Sub
End Module
