﻿open System
open System.Net.Http
open System.Security.Cryptography
open System.Text

let orderId = "CHECKOUT_ORDER_TO_FETCH"
let merchantId = "CHECKOUT_MERCHANT_ID"
let secretWord = "CHECKOUT_SECRET_KEY"
//let baseUrl = "https://paymentadminapistage.svea.com/api/v1/orders/"
let baseUrl = "https://checkoutapistage.svea.com/api/orders/"

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
        printfn "Running GET request for Checkout (F#)"
        let url = baseUrl + orderId
        let headers = getRequestHeaders("", None)
        use client = new HttpClient()
        let request = new HttpRequestMessage(HttpMethod.Get, url)
        headers |> Map.iter (fun key value -> request.Headers.Add(key, value))
        let! response = client.SendAsync(request) |> Async.AwaitTask
        let! responseBody = response.Content.ReadAsStringAsync() |> Async.AwaitTask
        let statusCode = response.StatusCode

        if response.IsSuccessStatusCode then
            //printfn "%s" responseBody
            printfn "Success!"
        else
            //printfn "Error: Request returned non-OK status code"
            printfn "Failed..."
    } |> Async.RunSynchronously
    0
