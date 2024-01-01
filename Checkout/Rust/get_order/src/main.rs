use reqwest;
use chrono::Utc;
use sha2::{Sha512, Digest};
use base64::encode;
use std::collections::HashMap;
use reqwest::header::{HeaderMap, HeaderName, HeaderValue};

const ORDER_ID: &str = "8906830";
const MERCHANT_ID: &str = "124842";
const SECRET_WORD: &str = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW";
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

    let url = format!("{}{}", BASE_URL, ORDER_ID);

    //println!("Fetching order: {}", ORDER_ID);
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
