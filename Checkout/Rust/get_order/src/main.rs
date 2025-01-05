use reqwest;
use chrono::Utc;
use sha2::{Sha512, Digest};
use base64::encode;
use std::collections::HashMap;
use reqwest::header::{HeaderMap, HeaderName, HeaderValue};
use std::fs;

const MERCHANT_ID: &str = "CHECKOUT_MERCHANT_ID";
const SECRET_WORD: &str = "CHECKOUT_SECRET_KEY";
//const BASE_URL: &str = "https://paymentadminapistage.svea.com/api/v1/orders/";
const BASE_URL: &str = "https://checkoutapistage.svea.com/api/orders/";
const CONTENT_TYPE: &str = "application/json";

struct TestClass;

impl TestClass {
    fn get_request_headers(&self, request_body: &str, extra_headers: Option<HashMap<String, String>>) -> Result<HeaderMap, Box<dyn std::error::Error>> {
        let timestamp = Utc::now().format("%Y-%m-%d %H:%M:%S").to_string();
        let token = self.get_auth_token(&timestamp, request_body);
        
        let mut headers = HeaderMap::new();
        headers.insert(HeaderName::from_static("authorization"), HeaderValue::from_str(&format!("Svea {}", token))?);
        headers.insert(HeaderName::from_static("timestamp"), HeaderValue::from_str(&timestamp)?);

        if let Some(extra) = extra_headers {
            for (key, value) in extra {
                headers.insert(HeaderName::from_bytes(key.as_bytes())?, HeaderValue::from_str(&value)?);
            }
        }

        Ok(headers)
    }

    fn get_auth_token(&self, timestamp: &str, request_body: &str) -> String {
        let digest = format!("{}{}{}", request_body, SECRET_WORD, timestamp);
        let mut hasher = Sha512::new();
        hasher.update(digest.as_bytes());
        let hashed = hasher.finalize();
        let auth = format!("{}:{:x}", MERCHANT_ID, hashed);
        encode(auth)
    }
}

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    println!("Running GET request for Checkout (Rust)");
    let test_instance = TestClass;
    let my_headers = test_instance.get_request_headers("", None)?;

    //println!("{:?}", my_headers);

    let order_id = match fs::read_to_string("../created_order_id.txt") {
        Ok(content) => content.trim().to_string(),
        Err(e) => {
            eprintln!("Failed to read OrderId from file: {}", e);
            return Ok(()); // Exit early if the file cannot be read
        }
    };

    //println!("Using OrderID: {}", order_id);
    let url = format!("{}{}", BASE_URL, order_id);

    //println!("Fetching order: {}", order_id);

    let client = reqwest::Client::new();
    let res = client.get(&url)
        .headers(my_headers)
        .send()
        .await?;

    //println!("Response Status: {}", res.status());

    if res.status().is_success() {
        let body = res.text().await?;
        //println!("{}", body);
        println!("Success!");
    } else {
        println!("Failed...");
    }

    Ok(())
}

