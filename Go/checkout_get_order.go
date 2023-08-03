package main

import (
	"crypto/sha512"
	"encoding/base64"
	"fmt"
	"net/http"
	"time"
	"io/ioutil"
)

// Adjust SveaBasicSettings based on your credentials.
type TestStruct struct {
	SveaBasicSettings struct {
		MerchantID string
		SecretKey  string
	}
}

func main() {
	testInstance := TestStruct{}

	url := "https://checkoutapistage.svea.com/api/orders/1"

	mySCOHeaders := testInstance.getRequestHeaders(map[string]string{"Connection": "keep-alive"})
	// fmt.Println(mySCOHeaders)

	// Send request
	payload := []byte{}
	response, err := http.Get(url)
	if err != nil {
		fmt.Println("Error sending request:", err)
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
	digest := requestBody + t.SveaBasicSettings.SecretKey + timestamp
	h := sha512.New()
	h.Write([]byte(digest))
	sha := h.Sum(nil)
	merchantAuth := t.SveaBasicSettings.MerchantID + ":" + fmt.Sprintf("%x", sha)
	token := base64.StdEncoding.EncodeToString([]byte(merchantAuth))
	return token
}