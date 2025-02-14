package main

import (
    "bytes"
    "fmt"
    "io/ioutil"
    "math/rand"
    "net/http"
    "strings"
    "time"
    "os"
    "regexp"
)

func main() {
    fmt.Println("Running Create request for Webpay (Go)")
    url := "https://webpaywsstage.svea.com/sveawebpay.asmx"
    action := "https://webservices.sveaekonomi.se/webpay/CreateOrderEu"
    randomOrderID := generateRandomString(8)

    soapEnvelope := `
    <soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:web="https://webservices.sveaekonomi.se/webpay" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
            <soap:Header/>
            <soap:Body>
                <web:CreateOrderEu>
                    <web:request>
                        <web:Auth>
                            <web:ClientNumber>WEBPAY_CLIENT_ID</web:ClientNumber>
                            <web:Username>WEBPAY_PASSWORD</web:Username>
                            <web:Password>WEBPAY_PASSWORD</web:Password>
                        </web:Auth>
                        <web:CreateOrderInformation>
                            <web:ClientOrderNumber>my_order_id</web:ClientOrderNumber>
                            <web:OrderRows>
                                <web:OrderRow>
                                    <web:ArticleNumber>123</web:ArticleNumber>
                                    <web:Description>Some Product 1</web:Description>
                                    <web:PricePerUnit>293.6</web:PricePerUnit>
                                    <web:PriceIncludingVat>false</web:PriceIncludingVat>
                                    <web:NumberOfUnits>2</web:NumberOfUnits>
                                    <web:Unit>st</web:Unit>
                                    <web:VatPercent>25</web:VatPercent>
                                    <web:DiscountPercent>0</web:DiscountPercent>
                                    <web:DiscountAmount>0</web:DiscountAmount>
                                    <web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>
                                </web:OrderRow>
                                <web:OrderRow>
                                    <web:ArticleNumber>456</web:ArticleNumber>
                                    <web:Description>Some Product 2</web:Description>
                                    <web:PricePerUnit>39.2</web:PricePerUnit>
                                    <web:PriceIncludingVat>false</web:PriceIncludingVat>
                                    <web:NumberOfUnits>1</web:NumberOfUnits>
                                    <web:Unit>st</web:Unit>
                                    <web:VatPercent>25</web:VatPercent>
                                    <web:DiscountPercent>0</web:DiscountPercent>
                                    <web:DiscountAmount>0</web:DiscountAmount>
                                    <web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>
                                </web:OrderRow>
                            </web:OrderRows>
                            <web:CustomerIdentity>
                                <web:NationalIdNumber>4605092222</web:NationalIdNumber>
                                <web:Email>firstname.lastname@svea.com</web:Email>
                                <web:PhoneNumber>080000000</web:PhoneNumber>
                                <web:FullName>Tester Testsson</web:FullName>
                                <web:Street>Gatan 99</web:Street>
                                <web:ZipCode>12345</web:ZipCode>
                                <web:Locality>16733</web:Locality>
                                <web:CountryCode>SE</web:CountryCode>
                                <web:CustomerType>Individual</web:CustomerType>
                            </web:CustomerIdentity>
                            <web:OrderDate>2023-12-18T11:07:23</web:OrderDate>
                            <web:OrderType>Invoice</web:OrderType>
                        </web:CreateOrderInformation>
                    </web:request>
                </web:CreateOrderEu>
            </soap:Body>
    </soap:Envelope>`

    soapEnvelope = strings.ReplaceAll(soapEnvelope, "my_order_id", randomOrderID)

    req, err := http.NewRequest("POST", url, bytes.NewBufferString(soapEnvelope))
    if err != nil {
        fmt.Println("Error creating request: ", err)
        return
    }

    req.Header.Set("Content-Type", "application/soap+xml;charset=UTF-8")
    if action != "" {
        req.Header.Set("SOAPAction", action)
    }

    client := &http.Client{}
    resp, err := client.Do(req)
    if err != nil {
        fmt.Println("Error sending request: ", err)
        return
    }
    defer resp.Body.Close()

    //fmt.Println("Response Code: ", resp.StatusCode)

    body, err := ioutil.ReadAll(resp.Body)
    if err != nil {
        //fmt.Println("Error reading response: ", err)
        return
    }
    //fmt.Println("Response:")
    //fmt.Println(string(body))

    if resp.StatusCode == 200 && strings.Contains(strings.ToLower(string(body)), "accepted>true") {
        fmt.Println("Success!")

        orderIdRegex := regexp.MustCompile(`<SveaOrderId>(\d+)</SveaOrderId>`)
        match := orderIdRegex.FindStringSubmatch(string(body))

        if match != nil {
            sveaOrderId := match[1]
            //fmt.Println("SveaOrderId extracted:", sveaOrderId)

            orderIdFilePath := "./created_order_id.txt"
            err := os.WriteFile(orderIdFilePath, []byte(sveaOrderId), 0644)
            if err != nil {
                fmt.Println("Error saving order ID to file:", err)
            } else {
                //fmt.Println("Order ID saved to:", orderIdFilePath)
            }
        } else {
            fmt.Println("Failed to extract SveaOrderId.")
        }
    } else {
        fmt.Println("Failed...")
    }
}

func generateRandomString(length int) string {
    const charset = "0123456789"
    seededRand := rand.New(rand.NewSource(time.Now().UnixNano()))
    b := make([]byte, length)
    for i := range b {
        b[i] = charset[seededRand.Intn(len(charset))]
    }
    return string(b)
}
