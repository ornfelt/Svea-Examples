const fs = require('fs');

async function main() {
    console.log("Running Create request for Webpay (Javascript)");
    const url = "https://webpaywsstage.svea.com/sveawebpay.asmx";
    const action = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu";
    const headers = {
        "Content-Type": "application/soap+xml; charset=utf-8",
        "SOAPAction": action
    };
    const randomOrderId = Array.from({ length: 8 }, () => Math.floor(Math.random() * 10)).join('');

    const soapTemplate = `
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
    `;

    soapEnvelope = soapTemplate.replace("my_order_id", randomOrderId);
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: headers,
            body: soapEnvelope
        });

        const responseText = await response.text();
        //console.log("Response Code:", response.status);
        //console.log("Response:");
        //console.log(responseText);

        if (response.status === 200 && responseText.toLowerCase().includes("accepted>true")) {
            console.log("Success!");

            const sveaOrderIdMatch = responseText.match(/<SveaOrderId>(\d+)<\/SveaOrderId>/i);
            if (sveaOrderIdMatch && sveaOrderIdMatch[1]) {
                const sveaOrderId = sveaOrderIdMatch[1];
                //console.log("SveaOrderId extracted:", sveaOrderId);

                const filePath = './created_order_id.txt';
                fs.writeFileSync(filePath, sveaOrderId, 'utf8');
                //console.log(`SveaOrderId saved to ${filePath}`);
            } else {
                console.log("Failed to extract SveaOrderId.");
            }
        } else {
            console.log("Failed...");
        }
    } catch (e) {
        console.error(e);
    }
}

main();

