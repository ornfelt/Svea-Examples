open System
open System.Net.Http
open System.Security.Cryptography
open System.Text
open System.Threading.Tasks
open System.Xml.Linq

type PGRequestTester() =
    let httpClient = new HttpClient()

    member this.MakePostRequestAsync() =
        async {
            let messageXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>125123123</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>"
            let encodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageXML))
            let secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d"
            let mac = this.GetSha512Hash(encodedMessage + secret)

            printfn "mac: %s" mac
            printfn "encodedMessage: %s" encodedMessage

            try
                let url = "https://webpaypaymentgatewaystage.svea.com/webpay/payment"
                let content = new FormUrlEncodedContent([|
                    KeyValuePair<string, string>("merchantid", "1200")
                    KeyValuePair<string, string>("message", encodedMessage)
                    KeyValuePair<string, string>("mac", mac)
                |])

                let! response = httpClient.PostAsync(url, content) |> Async.AwaitTask
                let! responseString = response.Content.ReadAsStringAsync() |> Async.AwaitTask
                printfn "%s" responseString
            with
            | e -> printfn "%s" e.Message
        }

    member this.MakeGetQueryTransactionIdRequestAsync() =
        async {
            // Implement similar to MakePostRequestAsync, adapting the XML and URL as needed
        }

    member private this.GetSha512Hash(input: string) =
        using (SHA512.Create()) (fun sha512Hash ->
            let bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(input))
            bytes |> Array.map (fun b -> b.ToString("x2")) |> String.concat ""
        )

[<EntryPoint>]
let main argv =
    let tester = PGRequestTester()
    Async.StartAsTask (tester.MakePostRequestAsync()) |> ignore
    Async.StartAsTask (tester.MakeGetQueryTransactionIdRequestAsync()) |> ignore
    0 // Return an integer exit code
