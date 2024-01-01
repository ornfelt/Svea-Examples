open System
open System.Net.Http
open System.Security.Cryptography
open System.Text
open System.Threading.Tasks
open System.Xml.Linq
open System.Collections.Generic

let httpClient = HttpClient()
let random = Random()

let getSha512Hash (input: string) =
    use sha512Hash = SHA512.Create()
    let inputBytes: byte[] = Encoding.UTF8.GetBytes(input) // Added type annotation
    let bytes = sha512Hash.ComputeHash(inputBytes)

    bytes |> Array.map (fun b -> b.ToString("x2")) |> String.concat ""

let makeGetQueryTransactionIdRequestAsync () =
    async {
        try
            let transactionId = 900497
            let messageXML = sprintf "<?xml version=\"1.0\" encoding=\"UTF-8\"?><query><transactionid>%d</transactionid></query>" transactionId
            let encodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageXML))
            let secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d"
            let mac = getSha512Hash (encodedMessage + secret)

            let url = "https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid"
            let content = new FormUrlEncodedContent(Seq.toArray [ // Use Seq.toArray to create an array
                KeyValuePair("merchantid", "1200")
                KeyValuePair("message", encodedMessage)
                KeyValuePair("mac", mac)
            ])

            let! response = httpClient.PostAsync(url, content) |> Async.AwaitTask
            let! responseString = response.Content.ReadAsStringAsync() |> Async.AwaitTask

            if response.StatusCode = System.Net.HttpStatusCode.OK then
                Console.WriteLine("Success!")
            else
                Console.WriteLine("Failed...")

            // Parsing the XML to find the 'message' element
            let xmlDoc = XDocument.Parse(responseString)
            let messageElement = xmlDoc.Root.Element(XName.Get("message"))
            match messageElement with
            | null -> 
                Console.WriteLine("Message element not found in response")
                return ()
            | _ ->
                let encodedMessagePart = messageElement.Value
                let decodedBytes = Convert.FromBase64String(encodedMessagePart)
                let decodedString = Encoding.UTF8.GetString(decodedBytes)
                //Console.WriteLine("Decoded message:")
                //Console.WriteLine(decodedString)
                return ()
        with
        | e -> 
            Console.WriteLine(e.Message)
            return ()
    }


let makePostRequestAsync () =
    async {
        try
            let randomRefNo = random.Next(1000000, 10000000).ToString()
            let messageXML = sprintf "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\
                                      <payment>\
                                        <paymentmethod>SVEACARDPAY</paymentmethod>\
                                        <currency>SEK</currency>\
                                        <amount>500</amount>\
                                        <vat>100</vat>\
                                        <customerrefno>%s</customerrefno>\
                                        <returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl>\
                                        <lang>en</lang>\
                                      </payment>" randomRefNo

            //Console.WriteLine($"messageXML: {messageXML}")

            let encodedMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageXML))
            let secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d"
            let mac = getSha512Hash(encodedMessage + secret)
            //Console.WriteLine($"encodedMessage: {encodedMessage}")
            //Console.WriteLine($"mac: {mac}")

            let url = "https://webpaypaymentgatewaystage.svea.com/webpay/payment"
            let content = new FormUrlEncodedContent(Seq.toArray [ // Use Seq.toArray to create an array
                KeyValuePair("merchantid", "1200")
                KeyValuePair("message", encodedMessage)
                KeyValuePair("mac", mac)
            ])

            let! response = httpClient.PostAsync(url, content) |> Async.AwaitTask
            let! responseString = response.Content.ReadAsStringAsync() |> Async.AwaitTask

            //Console.WriteLine($"Response status code: {response.StatusCode}")
            //Console.WriteLine("Response content:")
            //Console.WriteLine(responseString)

            if responseString.ToLower().Contains("enter your card details") || responseString.ToLower().Contains("select card type") then
                Console.WriteLine("Success!")
            else
                Console.WriteLine("Failed...")
        with
        | e -> 
            Console.WriteLine(e.Message)
            return ()
    }

[<EntryPoint>]
let main argv =
    async {
        Console.WriteLine("Running GET request for PaymentGateway (F#)")
        do! makeGetQueryTransactionIdRequestAsync ()
        Console.WriteLine("Running Create request for PaymentGateway (F#)")
        do! makePostRequestAsync ()
        Console.WriteLine("----------------------------------------------------------")
    } |> Async.RunSynchronously
    0
