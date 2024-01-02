<?php

class SveaAuth {
    public static function sendRequest() {
        echo "Running Create request for Checkout (Php)\n";

        $url = "https://checkoutapistage.svea.com/api/orders";
        $randomOrderId = substr(str_shuffle(str_repeat('0123456789', 15)), 0, 15);

        // Attempt to read the JSON payload from file
        $bodyStr = '';
        try {
            if (!file_exists('create_order_payload.json')) {
                throw new Exception("File not found.");
            }

            $bodyStr = file_get_contents('create_order_payload.json');
            $bodyStr = str_replace("my_order_id", $randomOrderId, $bodyStr);
            $body = json_decode($bodyStr, true);
        } catch (Exception $e) {
            echo $e->getMessage() . "\n";
            return;
        }

        $headers = self::setHttpRequestHeaders("", json_encode($body));
        $response = self::postRequest($url, $body, $headers);

        if ($response['status_code'] == 200 || $response['status_code'] == 201) {
            echo "Success!\n";
        } else {
            echo "Failed...\n";
        }
        echo "----------------------------------------------------------\n";
    }

    public static function setHttpRequestHeaders($operation, $requestMessage) {
        $timestamp = gmdate("Y-m-d H:i:s");
        $token = self::createAuthenticationToken($requestMessage, $timestamp);

        return [
            "Authorization: Svea " . $token,
            "Timestamp: " . $timestamp,
            "Content-Type: application/json; charset=utf-8"
        ];
    }

    public static function createAuthenticationToken($requestMessage, $timestamp) {
        $merchantId = "124842";
        $secretKey = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW";
        
        $hashString = hash('sha512', $requestMessage . $secretKey . $timestamp);
        $authToken = base64_encode($merchantId . ":" . $hashString);

        return $authToken;
    }

    private static function postRequest($url, $body, $headers) {
        $ch = curl_init($url);
        curl_setopt($ch, CURLOPT_POST, true);
        curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($body));
        curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

        $response = curl_exec($ch);
        $statusCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        curl_close($ch);

        //echo "response: " . $response . "\n";
        //echo "response status: " . $statusCode . "\n"; // Same as above

        return ['status_code' => $statusCode, 'body' => $response];
    }
}

SveaAuth::sendRequest();

?>
