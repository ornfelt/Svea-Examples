Imports System
Imports System.Text
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Globalization
Imports System.Diagnostics
Imports System.IO
Imports System.Net.Http.Headers

Module Program
    Sub Main()
        SveaAuth.Main(New String() {}).Wait()
    End Sub
End Module

Public Class SveaAuth
    Private Shared httpRequest As HttpRequestMessage

    Public Shared Async Function Main(args As String()) As Task
        Console.WriteLine("Running Create request for Checkout (VB.NET)")
        Await SendRequest()
    End Function

    Private Shared Async Function SendRequest() As Task
        Dim client As New HttpClient()
        Dim randomOrderId As String = GenerateRandomOrderId()
        httpRequest = New HttpRequestMessage(HttpMethod.Post, "https://checkoutapistage.svea.com/api/orders")

        Dim filePath As String = "create_order_payload.json"
        Dim body As String = Await File.ReadAllTextAsync(filePath)
        body = body.Replace("my_order_id", randomOrderId)
        httpRequest.Content = New StringContent(body, Encoding.UTF8, "application/json")
        Await SetHttpRequestHeaders(client, body)

        Dim response As HttpResponseMessage = Await client.SendAsync(httpRequest)
        Dim responseBody As String = Await response.Content.ReadAsStringAsync().ConfigureAwait(False)

        'Console.WriteLine("response: " & response.ToString())
        'Console.WriteLine("response body: " & responseBody)

        If response.StatusCode = Net.HttpStatusCode.OK OrElse 
           response.StatusCode = Net.HttpStatusCode.Created Then
            Console.WriteLine("Success!")

            Dim orderIdRegex As New Text.RegularExpressions.Regex("""OrderId"":\s*(\d+)", Text.RegularExpressions.RegexOptions.IgnoreCase)
            Dim match As Text.RegularExpressions.Match = orderIdRegex.Match(responseBody)

            If match.Success Then
                Dim orderId As String = match.Groups(1).Value
                Try
                    Await File.WriteAllTextAsync("../created_order_id.txt", orderId)
                    'Console.WriteLine("OrderId saved to ../created_order_id.txt: " & orderId)
                    Catch ex As Exception
                    Console.WriteLine("Error saving OrderId to file: " & ex.Message)
                End Try
            Else
                Console.WriteLine("OrderId not found in the response.")
            End If
        Else
            Console.WriteLine("Failed...")
        End If
        Await Task.CompletedTask
    End Function

    Private Shared Function GenerateRandomOrderId() As String
        Dim random As New Random()
        Dim orderId As New StringBuilder()
        For i As Integer = 0 To 14
            orderId.Append(random.Next(0, 10)) ' Append a random digit
        Next
        Return orderId.ToString()
    End Function

    Protected Shared Async Function SetHttpRequestHeaders(client As HttpClient, requestMessage As String) As Task
        client.Timeout = New TimeSpan(0, 0, 30)
        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8")
        client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
        Dim timestamp As String = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
        Dim token As String = CreateAuthenticationToken(Await httpRequest.Content.ReadAsStringAsync().ConfigureAwait(False), timestamp)

        httpRequest.Headers.Add("Authorization", "Svea" & " " & token)
        httpRequest.Headers.Add("Timestamp", timestamp)
        Await Task.CompletedTask
    End Function

    Private Shared Function CreateAuthenticationToken(requestMessage As String, timestamp As String) As String
        Using sha512 As SHA512 = SHA512.Create()
            Dim merchantId As String = "CHECKOUT_MERCHANT_ID"
            Dim secretKey As String = "CHECKOUT_SECRET_KEY"

            Dim hashBytes As Byte() = sha512.ComputeHash(Encoding.UTF8.GetBytes(requestMessage & secretKey & timestamp))
            Dim hashString As String = BitConverter.ToString(hashBytes).Replace("-", "").ToLower()
            Return Convert.ToBase64String(Encoding.UTF8.GetBytes(merchantId & ":" & hashString))
        End Using
    End Function
End Class

