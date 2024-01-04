use rand::{Rng, thread_rng};
use reqwest;
use sha2::{Sha512, Digest};
use base64::{encode, decode};
use std::collections::HashMap;
use std::error::Error;
use std::str;
use std::fmt::Write;

#[tokio::main]
async fn main() -> Result<(), Box<dyn Error>> {
    println!("Running GET request for PaymentGateway (Rust)");
    make_get_query_transaction_id_request().await?;
    println!("Running Create request for PaymentGateway (Rust)");
    make_post_request().await?;
    println!("----------------------------------------------------------");
    Ok(())
}

async fn make_get_query_transaction_id_request() -> Result<(), Box<dyn Error>> {
    let transaction_id = PG_ORDER_TO_FETCH;
    let message_xml = format!(r#"<?xml version="1.0" encoding="UTF-8"?><query><transactionid>{}</transactionid></query>"#, transaction_id);
    let encoded_message = encode(message_xml);
    let secret = "PG_SECRET_KEY";
    let mac = get_sha512_hash(&(encoded_message.clone() + secret));

    //println!("mac: {}", mac);
    //println!("encodedMessage: {}", encoded_message);

    let client = reqwest::Client::new();
    let url = "https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid";
    let mut form = HashMap::new();
    form.insert("merchantid", "PG_MERCHANT_ID");
    form.insert("message", &encoded_message);
    form.insert("mac", &mac);

    let response = client.post(url)
        .form(&form)
        .send()
        .await?;

    let status = response.status();
    let response_text = response.text().await?;
    //println!("Response status code: {}", status);
    //println!("Response message: {}", response_text);

    if status == 200 {
        println!("Success!");
    } else {
        println!("Failed...");
    }

    // Processing the XML response would require an XML parsing crate like `quick-xml` or `xml-rs`.
    Ok(())
}

async fn make_post_request() -> Result<(), Box<dyn Error>> {
    let mut rng = thread_rng();
    let random_ref_no: i32 = rng.gen_range(1000000..10000000);
    let mut message_xml = String::new();
    write!(&mut message_xml, r#"<?xml version="1.0" encoding="UTF-8"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>{}</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>"#, random_ref_no).unwrap();
    let encoded_message = encode(message_xml);
    let secret = "PG_SECRET_KEY";
    let mac = get_sha512_hash(&(encoded_message.clone() + secret));

    //println!("mac: {}", mac);
    //println!("encodedMessage: {}", encoded_message);

    let client = reqwest::Client::new();
    let url = "https://webpaypaymentgatewaystage.svea.com/webpay/payment";
    let mut form = HashMap::new();
    form.insert("merchantid", "PG_MERCHANT_ID");
    form.insert("message", &encoded_message);
    form.insert("mac", &mac);

    let response = client.post(url)
        .form(&form)
        .send()
        .await?;
    let status = response.status();

    let response_text = response.text().await?;
    //println!("Response status code: {}", status);
    //println!("{}", response_text);
    // Since we receive an iframe and the status tends to return 400 - verify the HTML content instead
    if response_text.to_lowercase().contains("enter your card details") || response_text.to_lowercase().contains("select card type") {
            println!("Success!");
    } else {
        println!("Failed...");
    }

    Ok(())
}

fn get_sha512_hash(input: &str) -> String {
    let mut hasher = Sha512::new();
    hasher.update(input.as_bytes());
    let result = hasher.finalize();
    result.iter().map(|byte| format!("{:02x}", byte)).collect::<String>()
}
