open System
open System.Net.Http
open System.Security.Cryptography
open System.Text
open FSharp.Data

let orderId = "8906830"
let merchantId = "124842"
let secretWord = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW"
let baseUrl = "https://paymentadminapistage.svea.com/api/v1/orders/"

let bytesToHex (bytes: byte[]) = 
    BitConverter.ToString(bytes).Replace("-", "").ToLower()

let getAuthToken timestamp requestBody =
    let digest = Encoding.UTF8.GetBytes(requestBody + secretWord + timestamp)
    use sha512 = new SHA512Managed()
    let hashed = sha512.ComputeHash(digest)
    let auth = merchantId + ":" + bytesToHex(hashed)
    Convert.ToBase64String(Encoding.UTF8.GetBytes(auth))

let getRequestHeaders (requestBody: string) (extraHeaders: Map<string, string>) =
    let timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
    let token = getAuthToken timestamp requestBody
    let headers = dict [
        "Authorization", "Svea " + token
        "Timestamp", timestamp
    ]
    match extraHeaders with
    | null -> headers
    | _ -> extraHeaders |> Map.toSeq |> Seq.append headers |> dict

[<EntryPoint>]
let main argv =
    async {
        let url = baseUrl + orderId
        let myHeaders = getRequestHeaders "" Map.empty

        use client = new HttpClient()
        let request = new HttpRequestMessage(HttpMethod.Get, url)
        myHeaders |> Seq.iter (fun (key, value) -> request.Headers.TryAddWithoutValidation(key, value))

        let! response = client.SendAsync(request) |> Async.AwaitTask
        let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask

        if response.IsSuccessStatusCode then
            printfn "Response: %s" responseBody
        else
            printfn "Error: Request returned non-OK status code"

        return 0
    } |> Async.RunSynchronously
