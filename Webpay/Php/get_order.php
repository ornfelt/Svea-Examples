<?php

echo "Running GET request for Webpay (Php)\n";

$url = "https://webpayadminservicestage.svea.com/AdminService.svc/secure";
$action = "http://tempuri.org/IAdminService/GetOrders";

$soapEnvelope = <<<EOT
<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:tem="http://tempuri.org/" xmlns:dat="http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service">
    <soap:Header xmlns:wsa="http://www.w3.org/2005/08/addressing">
        <wsa:Action>http://tempuri.org/IAdminService/GetOrders</wsa:Action>
        <wsa:To>https://webpayadminservicestage.svea.com/AdminService.svc/secure</wsa:To>
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
                        <dat:SveaOrderId>WEBPAY_ORDER_TO_FETCH_VALUE</dat:SveaOrderId>
                    </dat:GetOrderInformation>
                </dat:OrdersToRetrieve>
            </tem:request>
        </tem:GetOrders>
    </soap:Body>
</soap:Envelope>
EOT;

$orderIdFile = './created_order_id.txt';
if (!file_exists($orderIdFile)) {
    echo "Error: Order ID file not found: $orderIdFile\n";
    exit(1);
}

$sveaOrderId = trim(file_get_contents($orderIdFile));
if (empty($sveaOrderId)) {
    echo "Error: Order ID file is empty.\n";
    exit(1);
}

//echo "Using SveaOrderId: $sveaOrderId\n";
$soapEnvelope = str_replace('WEBPAY_ORDER_TO_FETCH_VALUE', $sveaOrderId, $soapEnvelope);

$ch = curl_init($url);
curl_setopt($ch, CURLOPT_HTTPHEADER, array(
    "Content-Type: application/soap+xml;charset=UTF-8",
    "SOAPAction: $action"
));
curl_setopt($ch, CURLOPT_POST, true);
curl_setopt($ch, CURLOPT_POSTFIELDS, $soapEnvelope);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

$response = curl_exec($ch);
$responseCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);

if ($response === false) {
    echo "Error: " . curl_error($ch);
} else {
    //echo "Response Code : " . $responseCode . "\n";
    //echo "Response: " . $response . "\n";
    if ($responseCode == 200)
        echo "Success!\n";
    else
        echo "Failed...\n";
}

curl_close($ch);
?>
