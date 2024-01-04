open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.Security.Cryptography
open System.Text
open System.Threading.Tasks
open System.Globalization
open System.IO

let generateRandomOrderId () =
    let random = Random()
    let orderId = StringBuilder()
    for i in 0 .. 14 do
        orderId.Append(random.Next(0, 10)) |> ignore
    orderId.ToString()

let createAuthenticationToken (requestMessage: string) (timestamp: string) =
    use sha512 = SHA512.Create()
    let merchantId = "CHECKOUT_MERCHANT_ID"
    let secretKey = "CHECKOUT_SECRET_KEY"
    let hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(requestMessage + secretKey + timestamp))
    let hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower()
    Convert.ToBase64String(Encoding.UTF8.GetBytes(merchantId + ":" + hashString))

let setHttpRequestHeaders (client: HttpClient) (request: HttpRequestMessage) (body: string) =
    let timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
    let token = createAuthenticationToken body timestamp
    client.Timeout <- TimeSpan(0, 0, 30)
    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8") |> ignore
    client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue("application/json")) |> ignore
    request.Headers.Add("Authorization", "Svea " + token) |> ignore
    request.Headers.Add("Timestamp", timestamp) |> ignore

let sendRequest () =
    async {
        printfn "Running Create request for Checkout (F#)"
        let client = new HttpClient()
        let randomOrderId = generateRandomOrderId()
        let request = new HttpRequestMessage(HttpMethod.Post, "https://checkoutapistage.svea.com/api/orders")
        let filePath = "create_order_payload.json"
        let body = File.ReadAllText(filePath)
        let bodyWithOrderId = body.Replace("my_order_id", randomOrderId)
        request.Content <- new StringContent(bodyWithOrderId, Encoding.UTF8, "application/json")
        
        // Modified to directly await the Task without Async.AwaitTask
        setHttpRequestHeaders client request bodyWithOrderId
        let! response = client.SendAsync(request) |> Async.AwaitTask
        let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

        //printfn "Response status code: %A" response.StatusCode
        //printfn "Response body: %s" responseBody

        if response.StatusCode = HttpStatusCode.OK || response.StatusCode = HttpStatusCode.Created then
            printfn "Success!"
        else
            printfn "Failed..."
        printfn "----------------------------------------------------------"
    }

[<EntryPoint>]
let main argv =
    sendRequest() |> Async.RunSynchronously
    0
