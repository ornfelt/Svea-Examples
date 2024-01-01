use reqwest;
use chrono::Utc;
use sha2::{Sha512, Digest};
use base64::encode;
use std::collections::HashMap;
use tokio::runtime::Runtime;
use reqwest::header::{HeaderMap, HeaderName, HeaderValue};

const ORDER_ID: &str = "8906830";
const MERCHANT_ID: &str = "123";
const SECRET_WORD: &str = "xxx";
//const BASE_URL: &str = "https://paymentadminapistage.svea.com/api/v1/orders/";
const BASE_URL: &str = "https://checkoutapistage.svea.com/api/orders/";
const CONTENT_TYPE: &str = "application/json";

struct TestClass;

impl TestClass {
    fn get_request_headers(&self, request_body: &str, extra_headers: Option<HashMap<String, String>>) -> HashMap<String, String> {
        let timestamp = Utc::now().format("%Y-%m-%d %H:%M:%S").to_string();
        let token = self.get_auth_token(&timestamp, request_body);
        let mut headers = HashMap::new();
        headers.insert("Authorization".to_string(), format!("Svea {}", token));
        headers.insert("Timestamp".to_string(), timestamp);

        if let Some(eh) = extra_headers {
            for (key, value) in eh {
                headers.insert(key, value);
            }
        }

        headers
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

fn main() -> Result<(), Box<dyn std::error::Error>> {
    println!("Running GET request for Checkout (Rust)");
    let rt = Runtime::new()?;
    rt.block_on(async {
        let test_instance = TestClass;
        let my_headers = test_instance.get_request_headers("", None);

        let mut headers = HeaderMap::new();
        for (key, value) in my_headers {
            let header_name = HeaderName::from_bytes(key.as_bytes())?;
            let header_value = HeaderValue::from_str(&value)?;
            headers.insert(header_name, header_value);
        }

        let url = format!("{}{}", BASE_URL, ORDER_ID);

        println!("Fetching order: {}", ORDER_ID);
        let client = reqwest::Client::new();
        let res = client.get(&url)
            .headers(headers)
            .send()
            .await?;

        println!("Response Status: {}", res.status());

        if res.status().is_success() {
            let body = res.text().await?;
            //println!("{}", body);
            println!("Success!");
        } else {
            println!("Failed...");
        }

        Ok(())
    })
}
