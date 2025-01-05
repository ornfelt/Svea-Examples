/**
  Uses Openssl and cpprestsdk (tried on Arch linux)
  Run with: g++ -std=c++17 -lcpprest -lcrypto -lssl get_order.cpp -o get_order && ./get_order
*/

#include <cpprest/http_client.h>
#include <cpprest/json.h>
#include <iostream>
#include <fstream>
#include <string>

#include "SveaAuth.h"

int main() {
    std::cout << "Running GET request for Checkout (C++)" << std::endl;
    // Credentials
    std::string merchant_id = "CHECKOUT_MERCHANT_ID";
    std::string secret_word = "CHECKOUT_SECRET_KEY";
    std::string order_id = "";

    std::ifstream orderFile("./created_order_id.txt");
    if (orderFile.is_open()) {
        std::getline(orderFile, order_id);
        orderFile.close();
        //std::cout << "Using OrderId: " << order_id << std::endl;
    } else {
        std::cerr << "Failed to open ../created_order_id.txt. Ensure the file exists and is readable." << std::endl;
        return 1;
    }

    SveaAuth sveaAuthInstance;
    sveaAuthInstance.setMerchantId(merchant_id);
    sveaAuthInstance.setSecretWord(secret_word);

    web::http::http_request my_headers = sveaAuthInstance.get_request_headers();

    web::http::client::http_client_config config;

    // Could also use PA api if order is finalized
    web::http::client::http_client client("https://checkoutapistage.svea.com/api/orders/" + order_id, config);
    //web::http::client::http_client client("https://paymentadminapistage.svea.com/api/v1/orders/" + order_id, config);

    web::http::http_request request(web::http::methods::GET);
    for (const auto& header : my_headers.headers()) {
        request.headers().add(header.first, header.second);
    }

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
