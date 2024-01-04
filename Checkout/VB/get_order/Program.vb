Imports System
Imports System.Net.Http
Imports System.Security.Cryptography
Imports System.Text
Imports System.Globalization

Public Class TestClass
    Private Const OrderId As String = "CHECKOUT_ORDER_TO_FETCH"
    Private Const MerchantId As String = "CHECKOUT_MERCHANT_ID"
    Private Const SecretWord As String = "CHECKOUT_SECRET_KEY"

    Public Function GetRequestHeaders(Optional requestBody As String = "", Optional extraHeaders As Dictionary(Of String, String) = Nothing) As Dictionary(Of String, String)
        Dim timestamp As String = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
        Dim token As String = GetAuthToken(timestamp, requestBody)

        Dim headers As New Dictionary(Of String, String) From {
            {"Authorization", "Svea " & token},
            {"Timestamp", timestamp}
        }

        If extraHeaders IsNot Nothing Then
            For Each header In extraHeaders
                headers.Add(header.Key, header.Value)
            Next
        End If

        Return headers
    End Function

    Private Function GetAuthToken(timestamp As String, requestBody As String) As String
        Dim digest As String = requestBody & SecretWord & timestamp
        Using sha512 As SHA512 = SHA512.Create()
            Dim hashed As Byte() = sha512.ComputeHash(Encoding.UTF8.GetBytes(digest))
            Dim auth As String = MerchantId & ":" & BitConverter.ToString(hashed).Replace("-", "").ToLower()
            Return Convert.ToBase64String(Encoding.UTF8.GetBytes(auth))
        End Using
    End Function

    Public Async Function PerformGetRequest() As Task
        Console.WriteLine("Running GET request for Checkout (VB.NET)")

        Dim headers As Dictionary(Of String, String) = GetRequestHeaders()
        'Dim url As String = "https://paymentadminapistage.svea.com/api/v1/orders/" & OrderId
        Dim url As String = "https://checkoutapistage.svea.com/api/orders/" & OrderId

        Using client As New HttpClient()
            For Each header In headers
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value)
            Next

            Dim response As HttpResponseMessage = Await client.GetAsync(url)

            If response.IsSuccessStatusCode Then
                Console.WriteLine("Success!")
                Dim responseBody As String = Await response.Content.ReadAsStringAsync()
                'Console.WriteLine("response body: " & responseBody)
            Else
                Console.WriteLine("Failed...")
            End If
        End Using
    End Function
End Class

Module Program
    Sub Main()
        Dim testInstance As New TestClass()
        testInstance.PerformGetRequest().Wait()
    End Sub
End Module
