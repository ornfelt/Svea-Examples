/**
  Uses Openssl and cpprestsdk (tried on Arch linux)
  Run with: g++ -std=c++17 -lcpprest -lcrypto -lssl get_order.cpp -o get_order && ./get_order
*/

#include <cpprest/http_client.h>
#include <cpprest/json.h>
#include <iostream>
#include <string>

#include "SveaAuth.h"

int main() {
    std::cout << "Running GET request for Checkout (C++)" << std::endl;
    // Define merchant ID and secret word
    std::string merchant_id = "124842";
    std::string secret_word = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW";
    std::string order_id = "8906830";

    // Create an instance of SveaAuth and set merchant ID and secret word
    SveaAuth sveaAuthInstance;
    sveaAuthInstance.setMerchantId(merchant_id);
    sveaAuthInstance.setSecretWord(secret_word);

    // Prepare the HTTP request with the headers
    web::http::http_request my_headers = sveaAuthInstance.get_request_headers();

    // Set up the HTTP client
    web::http::client::http_client_config config;
    web::http::client::http_client client("https://checkoutapistage.svea.com/api/orders/" + order_id, config);
    //web::http::client::http_client client("https://paymentadminapistage.svea.com/api/v1/orders/" + order_id, config);

    // Set up the HTTP GET request
    web::http::http_request request(web::http::methods::GET);
    for (const auto& header : my_headers.headers()) {
        request.headers().add(header.first, header.second);
    }

    // Send the request and wait for the response
    web::http::http_response response = client.request(request).get();
    //std::cout << "Response status: " << response.status_code() << std::endl;

    if (response.status_code() == web::http::status_codes::OK) {
        web::json::value json_resp = response.extract_json().get();
        //std::cout << json_resp.serialize() << std::endl;
        std::cout << "Success!" << std::endl;
    } else {
        std::cout << "Failed..." << std::endl;
    }

    return 0;
}
