<?php

class PgRequest
{
    private $httpClient;

    public function __construct()
    {
        $this->httpClient = curl_init();
    }

    public function makePostRequestAsync()
    {
        $messageXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>125123123</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>";
        $encodedMessage = base64_encode($messageXML);
        $secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
        $mac = $this->getSha512Hash($encodedMessage . $secret);

        echo "mac: " . $mac . "\n";
        echo "encodedMessage: " . $encodedMessage . "\n";

        try {
            $url = "https://webpaypaymentgatewaystage.svea.com/webpay/payment";
            $content = array(
                "merchantid" => "1200",
                "message" => $encodedMessage,
                "mac" => $mac
            );

            $response = $this->makeHttpRequest($url, $content);
            echo "Response status code: " . $response['status_code'] . "\n";
            echo "Response message: " . $response['body'] . "\n";
        } catch (Exception $e) {
            echo $e->getMessage();
        }
    }

    public function makeGetQueryTransactionIdRequestAsync()
    {
        $messageXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><query><transactionid>900497</transactionid></query>";
        $encodedMessage = base64_encode($messageXML);
        $secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
        $mac = $this->getSha512Hash($encodedMessage . $secret);

        echo "mac: " . $mac . "\n";
        echo "encodedMessage: " . $encodedMessage . "\n";

        try {
            $url = "https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid";
            $content = array(
                "merchantid" => "1200",
                "message" => $encodedMessage,
                "mac" => $mac
            );

            $response = $this->makeHttpRequest($url, $content);
            echo "Response status code: " . $response['status_code'] . "\n";
            echo "Response message: " . $response['body'] . "\n";

            // Parsing the XML to find the 'message' element
            $xml = simplexml_load_string($response['body']);
            $messageElement = $xml->xpath('//message');
            if ($messageElement) {
                $encodedMessagePart = (string)$messageElement[0];

                // Decoding the Base64 message part
                $decodedBytes = base64_decode($encodedMessagePart);
                $decodedString = utf8_decode($decodedBytes);
                echo "Decoded message:\n";
                echo $decodedString . "\n";
            } else {
                echo "Message element not found in response\n";
            }
        } catch (Exception $e) {
            echo $e->getMessage();
        }
    }

    private function getSha512Hash($input)
    {
        return hash('sha512', $input);
    }

    private function makeHttpRequest($url, $postData)
    {
        curl_setopt_array($this->httpClient, array(
            CURLOPT_URL => $url,
            CURLOPT_POST => true,
            CURLOPT_POSTFIELDS => http_build_query($postData),
            CURLOPT_RETURNTRANSFER => true,
            CURLOPT_HEADER => true,
            CURLOPT_FOLLOWLOCATION => true, // Follow redirects
            CURLOPT_MAXREDIRS => 10 // Maximum number of redirects to follow
        ));

        $response = curl_exec($this->httpClient);
        if (curl_errno($this->httpClient)) {
            throw new Exception(curl_error($this->httpClient));
        }

        $headerSize = curl_getinfo($this->httpClient, CURLINFO_HEADER_SIZE);
        $header = substr($response, 0, $headerSize);
        $body = substr($response, $headerSize);
        $statusCode = curl_getinfo($this->httpClient, CURLINFO_HTTP_CODE);

        return array(
            'status_code' => $statusCode,
            'header' => $header,
            'body' => $body
        );
    }
}

// Usage
$tester = new PgRequest();
$tester->makePostRequestAsync();
$tester->makeGetQueryTransactionIdRequestAsync();

?>
