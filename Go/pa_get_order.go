package main

import (
	"encoding/base64"
	"fmt"
	"crypto/sha512"
	"time"
	"net/http"
	"bytes"
	"io/ioutil"
)

var (
	orderID      = "1"
	merchantID   = ""
	secretWord   = ""
	merchantAuth string
)

type TestStruct struct{}

func main() {
	testInstance := TestStruct{}
	myHeaders := testInstance.getRequestHeaders(map[string]string{"Content-Type": "application/json", "User-Agent": "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:43.0) Gecko/20100101 Firefox/43.0"})
	fmt.Println(myHeaders)

	url := "https://paymentadminapistage.svea.com/api/v1/orders/" + orderID
	payload := []byte{}

	fmt.Println("Fetching order:", orderID)
	response, err := http.Get(url)
	if err != nil {
		fmt.Println("Error fetching order:", err)
		return
	}
	defer response.Body.Close()

	body, err := ioutil.ReadAll(response.Body)
	if err != nil {
		fmt.Println("Error reading response body:", err)
		return
	}
	fmt.Println(string(body))

	if response.StatusCode != http.StatusOK {
		fmt.Println("Request error:", response.Status)
		return
	}

	// Handle the response data as needed
}

func (t TestStruct) getRequestHeaders(extraHeaders map[string]string) map[string]string {
	timestamp := time.Now().Format("2006-01-02 15:04:05")
	token := t.getAuthToken(timestamp, "")
	headers := map[string]string{
		"Authorization": "Svea " + token,
		"Timestamp":     timestamp,
	}

	for key, value := range extraHeaders {
		headers[key] = value
	}

	return headers
}

func (t TestStruct) getAuthToken(timestamp, requestBody string) string {
	digest := requestBody + secretWord + timestamp
	h := sha512.New()
	h.Write([]byte(digest))
	sha := h.Sum(nil)
	merchantAuth = merchantID + ":" + fmt.Sprintf("%x", sha)
	token := base64.StdEncoding.EncodeToString([]byte(merchantAuth))
	return token
}