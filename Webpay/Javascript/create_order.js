async function main() {
    const url = "https://webpaywsstage.svea.com/sveawebpay.asmx";
    const action = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu";
    const headers = {
        "Content-Type": "application/soap+xml; charset=utf-8",
        "SOAPAction": action
    };

    const soapEnvelope = `
    <soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:web="https://webservices.sveaekonomi.se/webpay" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
        <soap:Header/>
        <soap:Body>
            <web:CreateOrderEu>
                <web:request>
                        <web:Auth>
                            <web:ClientNumber>79021</web:ClientNumber>
                            <web:Username>sverigetest</web:Username>
                            <web:Password>sverigetest</web:Password>
                        </web:Auth>
                        <web:CreateOrderInformation>
                            <web:ClientOrderNumber>MyTestingOrder123</web:ClientOrderNumber>
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

    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: headers,
            body: soapEnvelope
        });

        const responseText = await response.text();
        console.log("Response Code:", response.status);
        console.log("Response:");
        console.log(responseText);
    } catch (e) {
        console.error(e);
    }
}

main();
