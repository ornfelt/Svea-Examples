use reqwest;
use sha2::{Sha512, Digest};
use base64::{encode, decode};
use std::collections::HashMap;
use std::error::Error;
use std::str;

#[tokio::main]
async fn main() -> Result<(), Box<dyn Error>> {
    make_post_request().await?;
    make_get_query_transaction_id_request().await?;
    Ok(())
}

async fn make_post_request() -> Result<(), Box<dyn Error>> {
    let message_xml = r#"<?xml version="1.0" encoding="UTF-8"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>125123123</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>"#;
    let encoded_message = encode(message_xml);
    let secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
    let mac = get_sha512_hash(&(encoded_message.clone() + secret));

    println!("mac: {}", mac);
    println!("encodedMessage: {}", encoded_message);

    let client = reqwest::Client::new();
    let url = "https://webpaypaymentgatewaystage.svea.com/webpay/payment";
    let mut form = HashMap::new();
    form.insert("merchantid", "1200");
    form.insert("message", &encoded_message);
    form.insert("mac", &mac);

    let response = client.post(url)
        .form(&form)
        .send()
        .await?;
    let status = response.status();

    let response_text = response.text().await?;
    println!("Response status code: {}", status);

    println!("{}", response_text);
    Ok(())
}

async fn make_get_query_transaction_id_request() -> Result<(), Box<dyn Error>> {
    let message_xml = r#"<?xml version="1.0" encoding="UTF-8"?><query><transactionid>900497</transactionid></query>"#;
    let encoded_message = encode(message_xml);
    let secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
    let mac = get_sha512_hash(&(encoded_message.clone() + secret));

    println!("mac: {}", mac);
    println!("encodedMessage: {}", encoded_message);

    let client = reqwest::Client::new();
    let url = "https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid";
    let mut form = HashMap::new();
    form.insert("merchantid", "1200");
    form.insert("message", &encoded_message);
    form.insert("mac", &mac);

    let response = client.post(url)
        .form(&form)
        .send()
        .await?;

    let status = response.status();
    let response_text = response.text().await?;

    println!("Response status code: {}", status);
    println!("Response message: {}", response_text);
    // Processing the XML response would require an XML parsing crate like `quick-xml` or `xml-rs`.

    Ok(())
}

fn get_sha512_hash(input: &str) -> String {
    let mut hasher = Sha512::new();
    hasher.update(input.as_bytes());
    let result = hasher.finalize();
    result.iter().map(|byte| format!("{:02x}", byte)).collect::<String>()
}
