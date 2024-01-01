use reqwest;
use std::error::Error;

#[tokio::main]
async fn main() -> Result<(), Box<dyn Error>> {
    println!("Running Create request for Webpay (Rust)");
    let client = reqwest::Client::new();
    let url = "https://webpaywsstage.svea.com/sveawebpay.asmx";
    let action = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu";

    let soap_envelope = r#"
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
    "#;

    let mut headers = reqwest::header::HeaderMap::new();
    headers.insert(reqwest::header::CONTENT_TYPE, "application/soap+xml;charset=UTF-8".parse()?);
    if !action.is_empty() {
        headers.insert("SOAPAction", action.parse()?);
    }

    let res = client.post(url)
        .headers(headers)
        .body(soap_envelope.to_string())
        .send()
        .await?;

    let response_status = res.status();
    //println!("Response Code: {}", response_status);

    let response_body = res.text().await?;
    //println!("Response: {}", response_body);

    if response_status == 200 && response_body.to_lowercase().contains("accepted>true") {
        println!("Success!");
    } else {
        println!("Failed...");
    }
    println!("----------------------------------------------------------");

    Ok(())
}
