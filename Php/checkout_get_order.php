// Uses Guzzle HTTP to handle requests. Curl can also be used.

<?php

class TestClass {
    private $svea_basic_settings = [
        "merchant_id" => "",
        "secret_key" => ""
    ];

    public function get_request_headers($requests_body = "", $extra_headers = []) {
        $timestamp = date("Y-m-d H:i:s");
        $token = $this->get_auth_token($timestamp, $requests_body);

        $headers = [
            "Authorization" => "Svea " . $token,
            "Timestamp" => $timestamp
        ];

        if (!empty($extra_headers)) {
            $headers = array_merge($headers, $extra_headers);
        }

        return $headers;
    }

    public function get_auth_token($timestamp, $request_body = "") {
        $digest = $request_body . $this->svea_basic_settings["secret_key"] . $timestamp;
        $h = hash("sha512", $digest);
        $auth = $this->svea_basic_settings["merchant_id"] . ":" . $h;
        $token = base64_encode($auth);

        return $token;
    }
}

function requests($method, $url, $headers, $data) {
    $client = new \GuzzleHttp\Client();

    $options = [
        "headers" => $headers,
        "query" => $data
    ];

    $response = $client->request($method, $url, $options);
    return $response->getBody()->getContents();
}

$testInstance = new TestClass();

$url = "https://checkoutapistage.svea.com/api/orders/1";

$my_sco_headers = $testInstance->get_request_headers([], ["Connection" => "keep-alive"]);
// print_r($my_sco_headers);

// Send request
$payload = [];
$response = requests("GET", $url, $my_sco_headers, $payload);
echo $response;
http_response_code($response->getStatusCode());