package main

import (
	"crypto/sha512"
	"encoding/base64"
	"fmt"
	"net/http"
	"time"
    "io"
    "strings"
)

const (
	orderID     = "CHECKOUT_ORDER_TO_FETCH"
	merchantID  = "CHECKOUT_MERCHANT_ID"
	secretWord  = "CHECKOUT_SECRET_KEY"
	//baseURL     = "https://paymentadminapistage.svea.com/api/v1/orders/"
    baseURL     = "https://checkoutapistage.svea.com/api/orders/"
	contentType = "application/json"
)

type TestClass struct{}

func (tc *TestClass) getRequestHeaders(requestBody string, extraHeaders map[string]string) map[string]string {
	timestamp := time.Now().UTC().Format("2006-01-02 15:04:05")
	token := tc.getAuthToken(timestamp, requestBody)
	headers := map[string]string{
		"Authorization": fmt.Sprintf("Svea %s", token),
		"Timestamp":     timestamp,
	}

	if extraHeaders != nil {
		for key, value := range extraHeaders {
			headers[key] = value
		}
	}

	return headers
}

func (tc *TestClass) getAuthToken(timestamp, requestBody string) string {
	digest := requestBody + secretWord + timestamp
	h := sha512.New()
	h.Write([]byte(digest))
	hashed := h.Sum(nil)
	auth := fmt.Sprintf("%s:%x", merchantID, hashed)
	token := base64.StdEncoding.EncodeToString([]byte(auth))
	return token
}

func main() {
    fmt.Println("Running GET request for Checkout (Go)")
	testInstance := &TestClass{}
	myHeaders := testInstance.getRequestHeaders("", nil)
	//fmt.Println(myHeaders)

	url := baseURL + orderID
	// payload := map[string]interface{}{}

	//fmt.Println("Fetching order:", orderID)
	client := &http.Client{}
	req, err := http.NewRequest("GET", url, nil)
	if err != nil {
		fmt.Println("Error creating request:", err)
		return
	}

	for key, value := range myHeaders {
		req.Header.Set(key, value)
	}

	resp, err := client.Do(req)
	if err != nil {
		fmt.Println("Error making request:", err)
		return
	}
	defer resp.Body.Close()

	//fmt.Println("Response Status:", resp.Status)

    if resp.StatusCode == http.StatusOK {
        bodyBytes, err := io.ReadAll(resp.Body)
        if err != nil {
            fmt.Println("Error")
        }
        bodyString := string(bodyBytes)
        //fmt.Println(bodyString)
        if resp.StatusCode == 200 && strings.Contains(strings.ToLower(bodyString), "iframe") {
            fmt.Println("Success!")
        } else {
            fmt.Println("Failed...")
        }
    }

	if resp.StatusCode != http.StatusOK {
		fmt.Println("Error: Request returned non-OK status code")
		return
	}
}
