package main

import (
	"bytes"
	"fmt"
	"io/ioutil"
	"net/http"
    "strings"
)

func main() {
    fmt.Println("Running GET request for Webpay (Go)")
	url := "https://webpayadminservicestage.svea.com/AdminService.svc/secure"
	soapAction := "http://tempuri.org/IAdminService/GetOrders"

	soapEnvelope := `<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:tem="http://tempuri.org/" xmlns:dat="http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service">
		<soap:Header xmlns:wsa="http://www.w3.org/2005/08/addressing">
			<wsa:Action>` + soapAction + `</wsa:Action>
			<wsa:To>` + url + `</wsa:To>
		</soap:Header>
		<soap:Body>
			<tem:GetOrders>
				<tem:request>
					<dat:Authentication>
						<dat:Password>WEBPAY_PASSWORD</dat:Password>
						<dat:Username>WEBPAY_PASSWORD</dat:Username>
					</dat:Authentication>
					<dat:OrdersToRetrieve>
						<dat:GetOrderInformation>
							<dat:ClientId>WEBPAY_CLIENT_ID</dat:ClientId>
							<dat:OrderType>Invoice</dat:OrderType>
							<dat:SveaOrderId>WEBPAY_ORDER_TO_FETCH</dat:SveaOrderId>
						</dat:GetOrderInformation>
					</dat:OrdersToRetrieve>
				</tem:request>
			</tem:GetOrders>
		</soap:Body>
	</soap:Envelope>`

	client := &http.Client{}
	req, err := http.NewRequest("POST", url, bytes.NewBufferString(soapEnvelope))
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}

	req.Header.Add("Content-Type", "application/soap+xml;charset=UTF-8")
	req.Header.Add("SOAPAction", soapAction)

	resp, err := client.Do(req)
	if err != nil {
		fmt.Println("Error sending request:", err)
		return
	}
	defer resp.Body.Close()

	body, err := ioutil.ReadAll(resp.Body)
	if err != nil {
		fmt.Println("Error reading response body:", err)
		return
	}
	//fmt.Printf("Response Code: %d\n", resp.StatusCode)
	//fmt.Println("Response:", string(body))

    if resp.StatusCode == 200 && strings.Contains(strings.ToLower(string(body)), "accepted>true") {
        fmt.Println("Success!")
    } else {
        fmt.Println("Failed...")
    }
}

