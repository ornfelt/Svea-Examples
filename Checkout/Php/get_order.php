<?php

$orderId = "8906830";
$merchantId = "124842";
$secretWord = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW";

class TestClass {

    public function get_request_headers($requestBody = "", $extraHeaders = null) {
        date_default_timezone_set('UTC');
        $timestamp = date("Y-m-d H:i:s");
        $token = $this->get_auth_token($timestamp, $requestBody);
        $headers = array("Authorization: Svea $token", "Timestamp: $timestamp");
        if ($extraHeaders) {
            $headers = array_merge($headers, $extraHeaders);
        }
        return $headers;
    }

    private function get_auth_token($timestamp, $requestBody = "") {
        $payloadJson = json_encode($requestBody); // Convert payload to JSON
        $payloadJson = "";
        $digest = $payloadJson . $GLOBALS['secretWord'] . $timestamp;
        $h = hash("sha512", $digest);
        $auth = $GLOBALS['merchantId'] . ":" . $h;
        $token = base64_encode($auth);
        return $token;
    }

    public function createTimestamp()
    {
        return gmdate('Y-m-d H:i');
    }
    
    public function createAuthorizationToken()
    {
        $timestamp = $this->createTimestamp();
        $authToken = base64_encode($GLOBALS['merchantId'] . ':' .
            hash('sha512', "" . $GLOBALS['secretWord'] . $timestamp));
        return $authToken;
    }
}

echo "Running GET request for Checkout (PHP)\n";
$testInstance = new TestClass();
$myHeaders = $testInstance->get_request_headers(array('Content-Type' => 'application/json'));

// This also works
//$myHeaders = array();
//$myHeaders[] = 'Content-type: application/json';
//$myHeaders[] = 'Authorization: Svea ' . $testInstance->createAuthorizationToken();
//$myHeaders[] = 'Timestamp: ' . $testInstance->createTimestamp();
//$myHeaders[] = 'Expect:';

// $url = "https://paymentadminapistage.svea.com/api/v1/orders/" . $orderId;
$url = "https://checkoutapistage.svea.com/api/orders/" . $orderId;
$payload = array();

//echo "Fetching order: " . $orderId . "\n";
//echo "headers: " . $myHeaders[0] . "\n";

$ch = curl_init($url);
curl_setopt($ch, CURLOPT_HTTPHEADER, $myHeaders);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, 1);
curl_setopt($ch, CURLOPT_HEADER, 1);
curl_setopt($ch,CURLOPT_SSL_VERIFYPEER, false);
curl_setopt($ch, CURLOPT_CUSTOMREQUEST, 'GET');
//curl_setopt($ch, CURLOPT_VERBOSE, true);
curl_setopt($ch, CURLOPT_POSTFIELDS, "");
$response = curl_exec($ch);
curl_close($ch);

//echo "Response: " . $response . "\n";

$httpcode = curl_getinfo($ch, CURLINFO_HTTP_CODE);

if ($httpcode == 200) {
    echo "Success!\n";
} else {
    echo "Failed...\n";
}

?>
