package main

import (
    "crypto/sha512"
    "encoding/base64"
    "fmt"
    "io/ioutil"
    "net/http"
    "net/url"
    "strings"
)

func main() {
    makePostRequest()
    makeGetQueryTransactionIdRequest()
}

func makePostRequest() {
    messageXML := `<?xml version="1.0" encoding="UTF-8"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>125123123</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>`
    encodedMessage := base64.StdEncoding.EncodeToString([]byte(messageXML))
    secret := "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d"
    mac := getSha512Hash(encodedMessage + secret)

    fmt.Println("mac:", mac)
    fmt.Println("encodedMessage:", encodedMessage)

    data := url.Values{}
    data.Set("merchantid", "1200")
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
    fmt.Println("Response status code:", resp.StatusCode)

    body, err := ioutil.ReadAll(resp.Body)
    if err != nil {
        fmt.Println(err)
        return
    }
    fmt.Println(string(body))
}

func makeGetQueryTransactionIdRequest() {
    messageXML := `<?xml version="1.0" encoding="UTF-8"?><query><transactionid>900497</transactionid></query>`
    encodedMessage := base64.StdEncoding.EncodeToString([]byte(messageXML))
    secret := "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d"
    mac := getSha512Hash(encodedMessage + secret)

    fmt.Println("mac:", mac)
    fmt.Println("encodedMessage:", encodedMessage)

    data := url.Values{}
    data.Set("merchantid", "1200")
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

    body, err := ioutil.ReadAll(resp.Body)
    if err != nil {
        fmt.Println(err)
        return
    }
    fmt.Println("Response status code:", resp.StatusCode)
    fmt.Println("Response message:", string(body))
    // Additional code to parse the XML response can be added here if needed.
}

func getSha512Hash(input string) string {
    h := sha512.New()
    h.Write([]byte(input))
    return fmt.Sprintf("%x", h.Sum(nil))
}
