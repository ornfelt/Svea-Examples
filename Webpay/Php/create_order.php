<?php

echo "Running Create request for Webpay (Php)\n";

$url = "https://webpaywsstage.svea.com/sveawebpay.asmx";
$action = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu";
$randomOrderId = substr(str_shuffle(str_repeat('0123456789', 8)), 0, 8);

$soapTemplate = <<<XML
<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:web="https://webservices.sveaekonomi.se/webpay" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
            <soap:Header/>
            <soap:Body>
                <web:CreateOrderEu>
                    <web:request>
                        <web:Auth>
                            <web:ClientNumber>WEBPAY_CLIENT_ID</web:ClientNumber>
                            <web:Username>WEBPAY_PASSWORD</web:Username>
                            <web:Password>WEBPAY_PASSWORD</web:Password>
                        </web:Auth>
                        <web:CreateOrderInformation>
                            <web:ClientOrderNumber>my_order_id</web:ClientOrderNumber>
                            <web:OrderRows>
                                <web:OrderRow>
                                    <web:ArticleNumber>123</web:ArticleNumber>
                                    <web:Description>Some Product 1</web:Description>
                                    <web:PricePerUnit>293.6</web:PricePerUnit>
                                    <web:PriceIncludingVat>false</web:PriceIncludingVat>
                                    <web:NumberOfUnits>2</web:NumberOfUnits>
                                    <web:Unit>st</web:Unit>
                                    <web:VatPercent>25</web:VatPercent>
                                    <web:DiscountPercent>0</web:DiscountPercent>
                                    <web:DiscountAmount>0</web:DiscountAmount>
                                    <web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>
                                </web:OrderRow>
                                <web:OrderRow>
                                    <web:ArticleNumber>456</web:ArticleNumber>
                                    <web:Description>Some Product 2</web:Description>
                                    <web:PricePerUnit>39.2</web:PricePerUnit>
                                    <web:PriceIncludingVat>false</web:PriceIncludingVat>
                                    <web:NumberOfUnits>1</web:NumberOfUnits>
                                    <web:Unit>st</web:Unit>
                                    <web:VatPercent>25</web:VatPercent>
                                    <web:DiscountPercent>0</web:DiscountPercent>
                                    <web:DiscountAmount>0</web:DiscountAmount>
                                    <web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>
                                </web:OrderRow>
                            </web:OrderRows>
                            <web:CustomerIdentity>
                                <web:NationalIdNumber>4605092222</web:NationalIdNumber>
                                <web:Email>firstname.lastname@svea.com</web:Email>
                                <web:PhoneNumber>080000000</web:PhoneNumber>
                                <web:FullName>Tester Testsson</web:FullName>
                                <web:Street>Gatan 99</web:Street>
                                <web:ZipCode>12345</web:ZipCode>
                                <web:Locality>16733</web:Locality>
                                <web:CountryCode>SE</web:CountryCode>
                                <web:CustomerType>Individual</web:CustomerType>
                            </web:CustomerIdentity>
                            <web:OrderDate>2023-12-18T11:07:23</web:OrderDate>
                            <web:OrderType>Invoice</web:OrderType>
                        </web:CreateOrderInformation>
                    </web:request>
                </web:CreateOrderEu>
            </soap:Body>
</soap:Envelope>
XML;

$soapEnvelope = str_replace("my_order_id", $randomOrderId, $soapTemplate);

$headers = [
    "Content-Type: application/soap+xml;charset=UTF-8",
];

if (!empty($action)) {
    $headers[] = "SOAPAction: $action";
}

$ch = curl_init();
curl_setopt($ch, CURLOPT_URL, $url);
curl_setopt($ch, CURLOPT_POST, true);
curl_setopt($ch, CURLOPT_POSTFIELDS, $soapEnvelope);
curl_setopt($ch, CURLOPT_HTTPHEADER, $headers);
curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

$response = curl_exec($ch);

if (curl_errno($ch)) {
    echo "Error: " . curl_error($ch);
} else {
    $statusCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    //echo "Response Code: " . $statusCode . "\n";
    //echo "Response: \n" . $response;
    if ($statusCode == 200 && strpos(strtolower($response), "accepted>true") !== false) {
        echo "Success!\n";
        $matches = [];
        if (preg_match('/<SveaOrderId>(\d+)<\/SveaOrderId>/', $response, $matches)) {
            $sveaOrderId = $matches[1];
            //echo "SveaOrderId extracted: $sveaOrderId\n";

            $filePath = './created_order_id.txt';
            if (file_put_contents($filePath, $sveaOrderId) !== false) {
                //echo "SveaOrderId saved to $filePath\n";
            } else {
                echo "Failed to save SveaOrderId to $filePath\n";
            }
        } else {
            echo "Failed to extract SveaOrderId.\n";
        }
    } else {
        echo "Failed...\n";
    }
}

curl_close($ch);

