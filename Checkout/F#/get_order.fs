open System
open System.Net.Http
open System.Security.Cryptography
open System.Text

let orderId = "8906830"
let merchantId = "124842"
let secretWord = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW"
let baseUrl = "https://paymentadminapistage.svea.com/api/v1/orders/"

let bytesToHex (bytes: byte[]) =
    bytes |> Array.map (fun b -> b.ToString("x2")) |> String.concat ""

let getAuthToken (timestamp: string, requestBody: string) =
    let digest = requestBody + secretWord + timestamp
    use md = SHA512.Create()
    let hashed = md.ComputeHash(Encoding.UTF8.GetBytes(digest))
    let auth = merchantId + ":" + bytesToHex(hashed)
    Convert.ToBase64String(Encoding.UTF8.GetBytes(auth))

let getRequestHeaders (requestBody: string, extraHeaders: Map<string, string> option) =
    let timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
    let token = getAuthToken(timestamp, requestBody)
    let headers = Map.ofList [("Authorization", "Svea " + token); ("Timestamp", timestamp)]

    match extraHeaders with
    | Some eh -> Map.fold (fun acc k v -> Map.add k v acc) headers eh
    | None -> headers

[<EntryPoint>]
let main argv =
    async {
        let url = baseUrl + orderId
        let headers = getRequestHeaders("", None)
        use client = new HttpClient()
        let request = new HttpRequestMessage(HttpMethod.Get, url)
        headers |> Map.iter (fun key value -> request.Headers.Add(key, value))
        let! response = client.SendAsync(request) |> Async.AwaitTask
        let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        if response.IsSuccessStatusCode then
            printfn "%s" responseBody
        else
            printfn "Error: Request returned non-OK status code"
    } |> Async.RunSynchronously
    0
