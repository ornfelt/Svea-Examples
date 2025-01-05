package main

import (
    "crypto/sha512"
    "encoding/base64"
    "fmt"
    "math/rand"
    "time"
    "io/ioutil"
    "net/http"
    "net/url"
    "strings"
)

func main() {
    rand.Seed(time.Now().UnixNano())
    fmt.Println("Running Create request for PaymentGateway (Go)")
    makePostRequest()
    fmt.Println("Running GET request for PaymentGateway (Go)")
    makeGetQueryTransactionIdRequest()
    fmt.Println("----------------------------------------------------------");
}

func makeGetQueryTransactionIdRequest() {
    transactionId := PG_ORDER_TO_FETCH
    messageXML := fmt.Sprintf(`<?xml version="1.0" encoding="UTF-8"?><query><transactionid>%d</transactionid></query>`, transactionId)
    encodedMessage := base64.StdEncoding.EncodeToString([]byte(messageXML))
    secret := "PG_SECRET_KEY"
    mac := getSha512Hash(encodedMessage + secret)

    //fmt.Println("mac:", mac)
    //fmt.Println("encodedMessage:", encodedMessage)

    data := url.Values{}
    data.Set("merchantid", "PG_MERCHANT_ID")
    data.Set("message", encodedMessage)
    data.Set("mac", mac)

    client := &http.Client{}
    req, err := http.NewRequest("POST", "https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid", strings.NewReader(data.Encode()))
    if err != nil {
        fmt.Println(err)
        return
    }

    req.Header.Add("Content-Type", "application/x-www-form-urlencoded")
    resp, err := client.Do(req)
    if err != nil {
        fmt.Println(err)
        return
    }
    defer resp.Body.Close()

    //body, err := ioutil.ReadAll(resp.Body)
    if err != nil {
        fmt.Println(err)
        return
    }
    //fmt.Println("Response status code:", resp.StatusCode)
    //fmt.Println("Response message:", string(body))
    if resp.StatusCode == 200 {
        fmt.Println("Success!")
    } else {
        fmt.Println("Failed...")
    }

    // Additional code to parse the XML response can be added here if needed.
}

func makePostRequest() {
    randomRefNo := rand.Intn(9000000) + 1000000
    messageXML := fmt.Sprintf(`<?xml version="1.0" encoding="UTF-8"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>%d</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>`, randomRefNo)
    encodedMessage := base64.StdEncoding.EncodeToString([]byte(messageXML))
    secret := "PG_SECRET_KEY"
    mac := getSha512Hash(encodedMessage + secret)

    //fmt.Println("mac:", mac)
    //fmt.Println("encodedMessage:", encodedMessage)

    data := url.Values{}
    data.Set("merchantid", "PG_MERCHANT_ID")
    data.Set("message", encodedMessage)
    data.Set("mac", mac)

    client := &http.Client{}
    req, err := http.NewRequest("POST", "https://webpaypaymentgatewaystage.svea.com/webpay/payment", strings.NewReader(data.Encode()))
    if err != nil {
        fmt.Println(err)
        return
    }

    req.Header.Add("Content-Type", "application/x-www-form-urlencoded")
    resp, err := client.Do(req)
    if err != nil {
        fmt.Println(err)
        return
    }
    defer resp.Body.Close()
    //fmt.Println("Response status code:", resp.StatusCode)

    body, err := ioutil.ReadAll(resp.Body)
    if err != nil {
        fmt.Println(err)
        return
    }

    responseContent := string(body)
    //fmt.Println(responseContent)
    // Since we receive an iframe and the status tends to return 400 - verify the HTML content instead
    if strings.Contains(strings.ToLower(responseContent), "enter your card details") || strings.Contains(strings.ToLower(responseContent), "select card type") {
        fmt.Println("Success!")
    } else {
        fmt.Println("Failed...")
    }
}

func getSha512Hash(input string) string {
    h := sha512.New()
    h.Write([]byte(input))
    return fmt.Sprintf("%x", h.Sum(nil))
}
