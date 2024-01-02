extern crate hex;

use rand::Rng;
use rand::distributions::Uniform;
use reqwest;
use serde_json::Value;
use std::fs;
use chrono::Utc;
use sha2::{Sha512, Digest};
use base64;

struct SveaAuth;

impl SveaAuth {
    async fn send_request() {
        println!("Running Create request for Checkout (Rust)");
        let client = reqwest::Client::new();
        let url = "https://checkoutapistage.svea.com/api/orders";
        let rng = rand::thread_rng();
        let range = Uniform::new_inclusive('0', '9');
        let random_order_id: String = rng
            .sample_iter(&range)
            .take(15)
            .collect();

        let body = match fs::read_to_string("create_order_payload.json") {
            Ok(file_content) => {
                let body_str = file_content.replace("my_order_id", &random_order_id);
                serde_json::from_str(&body_str).unwrap_or_else(|_| Value::Null)
            },
            Err(_) => {
                println!("Error: File not found or not a valid JSON.");
                return;
            }
        };

        let headers = SveaAuth::set_http_request_headers(&body.to_string()).await;
        let response = client.post(url)
            .json(&body)
            .headers(headers)
            .send()
            .await
            .unwrap();

        let status_code = response.status().as_u16();

        //println!("Status Code: {}", status_code);
        //match response.text().await {
        //    Ok(text) => println!("Response Body: {}", text),
        //    Err(e) => println!("Error reading response text: {}", e),
        //}

        // Check specifically for status codes 200 or 201
        if status_code == 200 || status_code == 201 {
            println!("Success!");
        } else {
            println!("Failed...");
        }

        println!("----------------------------------------------------------");
    }

    async fn set_http_request_headers(request_message: &str) -> reqwest::header::HeaderMap {
        let timestamp = Utc::now().format("%Y-%m-%d %H:%M:%S").to_string();
        let token = SveaAuth::create_authentication_token(request_message, &timestamp);

        let mut headers = reqwest::header::HeaderMap::new();
        headers.insert("Authorization", format!("Svea {}", token).parse().unwrap());
        headers.insert("Timestamp", timestamp.parse().unwrap());
        headers.insert("Content-Type", "application/json; charset=utf-8".parse().unwrap());
        headers
    }

    fn create_authentication_token(request_message: &str, timestamp: &str) -> String {
        let merchant_id = "124842";
        let secret_key = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW";
        let mut hasher = Sha512::new();
        hasher.update(request_message.as_bytes());
        hasher.update(secret_key.as_bytes());
        hasher.update(timestamp.as_bytes());
        let hash_string = hasher.finalize();
        let auth_token = base64::encode(format!("{}:{}", merchant_id, hex::encode(hash_string)));
        auth_token
    }
}

#[tokio::main]
async fn main() {
    SveaAuth::send_request().await;
}
