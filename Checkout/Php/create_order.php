<?php

class SveaAuth {
    public static function sendRequest() {
        echo "Running Create request for Checkout (Php)\n";

        $url = "https://checkoutapistage.svea.com/api/orders";
        $randomOrderId = substr(str_shuffle(str_repeat('0123456789', 15)), 0, 15);

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

            if (preg_match('/"OrderId":\s*(\d+)/', $response['body'], $matches)) {
                $orderId = $matches[1];

                $filePath = './created_order_id.txt';
                try {
                    file_put_contents($filePath, $orderId);
                    //echo "OrderId saved to {$filePath}\n";
                } catch (Exception $e) {
                    echo "Failed to save OrderId: " . $e->getMessage() . "\n";
                }
            } else {
                echo "OrderId not found in the response.\n";
            }
        } else {
            echo "Failed...\n";
        }
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
        $merchantId = "CHECKOUT_MERCHANT_ID";
        $secretKey = "CHECKOUT_SECRET_KEY";

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
