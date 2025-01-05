use reqwest::header::{CONTENT_TYPE, HeaderMap};
use std::error::Error;
use std::fs;
use std::io::{self, Read};

#[tokio::main]
async fn main() -> Result<(), Box<dyn Error>> {
    println!("Running GET request for Webpay (Rust)");
    let url = "https://webpayadminservicestage.svea.com/AdminService.svc/secure";
    let soap_action = "http://tempuri.org/IAdminService/GetOrders";
    let mut soap_envelope = r#"<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:tem="http://tempuri.org/" xmlns:dat="http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service">
        <soap:Header xmlns:wsa="http://www.w3.org/2005/08/addressing">
            <wsa:Action>"#.to_owned() + soap_action + r#"</wsa:Action>
            <wsa:To>"# + url + r#"</wsa:To>
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
    </soap:Envelope>"#;

    let svea_order_id = fs::read_to_string("../created_order_id.txt")
        .or_else(|_| fs::read_to_string("../../created_order_id.txt"))
        .map(|s| s.trim().to_string())
        .unwrap_or_else(|_| {
            eprintln!("Failed to read SveaOrderId from file. Using default placeholder.");
            "WEBPAY_ORDER_TO_FETCH_VALUE".to_string()
        });

    //println!("Using SveaOrderId: {}", svea_order_id);
    soap_envelope = soap_envelope.replace("WEBPAY_ORDER_TO_FETCH_VALUE", &svea_order_id);

    let client = reqwest::Client::new();
    let mut headers = HeaderMap::new();
    headers.insert(CONTENT_TYPE, "application/soap+xml;charset=UTF-8".parse()?);

    let response = client
        .post(url)
        .headers(headers)
        .body(soap_envelope)
        .send()
        .await?;

    //println!("Response Code: {}", response.status());
    //println!("Response: {}", response.text().await?);
    if response.status() == 200 {
        println!("Success!");
    } else {
        println!("Failed...");
    }

    Ok(())
}
