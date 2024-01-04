package main

import (
    "bufio"
    "bytes"
    "crypto/sha512"
    "encoding/base64"
    //"encoding/json"
    "fmt"
    "io/ioutil"
    "math/rand"
    "net/http"
    "os"
    "strings"
    "time"
)

const (
    merchantID = "CHECKOUT_MERCHANT_ID"
    secretKey  = "CHECKOUT_SECRET_KEY"
    url        = "https://checkoutapistage.svea.com/api/orders"
)

func main() {
    sendRequest()
}

func sendRequest() {
    fmt.Println("Running Create request for Checkout (Go)")

    randomOrderID := generateRandomString(15)
    body, err := readAndPrepareBody("create_order_payload.json", randomOrderID)
    if err != nil {
        fmt.Println(err)
        return
    }

    headers, err := setHTTPRequestHeaders("", string(body))
    if err != nil {
        fmt.Println(err)
        return
    }

    client := &http.Client{Timeout: 30 * time.Second}
    req, err := http.NewRequest("POST", url, bytes.NewBuffer(body))
    if err != nil {
        fmt.Println("Error creating request:", err)
        return
    }
    for key, value := range headers {
        req.Header.Set(key, value)
    }

    resp, err := client.Do(req)
    if err != nil {
        fmt.Println("Error sending request:", err)
        return
    }
    defer resp.Body.Close()

    respBody, err := ioutil.ReadAll(resp.Body)
    if err != nil {
        fmt.Println("Error reading response body:", err)
        return
    }

    //fmt.Println("response:", resp.Status)
    //fmt.Println("response body:", string(respBody))

    if (resp.StatusCode == 200 || resp.StatusCode == 201) && strings.Contains(strings.ToLower(string(respBody)), "created") {
        fmt.Println("Success!")
    } else {
        fmt.Println("Failed...")
    }

    fmt.Println("----------------------------------------------------------")
}

func setHTTPRequestHeaders(operation, requestMessage string) (map[string]string, error) {
    timestamp := time.Now().UTC().Format("2006-01-02 15:04:05")
    token, err := createAuthenticationToken(requestMessage, timestamp)
    if err != nil {
        return nil, err
    }

    return map[string]string{
        "Authorization": "Svea " + token,
        "Timestamp":     timestamp,
        "Content-Type":  "application/json; charset=utf-8",
    }, nil
}

func createAuthenticationToken(requestMessage, timestamp string) (string, error) {
    hasher := sha512.New()
    _, err := hasher.Write([]byte(requestMessage + secretKey + timestamp))
    if err != nil {
        return "", err
    }
    hashString := hasher.Sum(nil)
    authToken := base64.StdEncoding.EncodeToString([]byte(merchantID + ":" + fmt.Sprintf("%x", hashString)))
    return authToken, nil
}

func readAndPrepareBody(filePath, randomOrderID string) ([]byte, error) {
    file, err := os.Open(filePath)
    if err != nil {
        return nil, fmt.Errorf("Error: File not found")
    }
    defer file.Close()

    scanner := bufio.NewScanner(file)
    var buffer bytes.Buffer
    for scanner.Scan() {
        line := scanner.Text()
        line = strings.ReplaceAll(line, "my_order_id", randomOrderID)
        buffer.WriteString(line + "\n")
    }

    if err := scanner.Err(); err != nil {
        return nil, fmt.Errorf("Error: File is not a valid JSON")
    }

    return buffer.Bytes(), nil
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
