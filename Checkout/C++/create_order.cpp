// Usage:
// g++ -std=c++17 -lcpprest -lcrypto -lssl create_order.cpp -o create_order && ./create_order

#include <cpprest/http_client.h>
#include <cpprest/json.h>
#include <cpprest/asyncrt_utils.h>
#include <openssl/sha.h>
#include <openssl/evp.h>
#include <openssl/bio.h>
#include <iostream>
#include <sstream>
#include <fstream>
#include <ctime>
#include <string>
#include <random>

#include "SveaAuth.h"

// Helper function to generate a random order_id
std::string generate_random_string(size_t length) {
    const std::string characters = "0123456789";
    std::random_device random_device;
    std::mt19937 generator(random_device());
    std::uniform_int_distribution<> distribution(0, characters.size() - 1);

    std::string random_string;
    for (size_t i = 0; i < length; ++i) {
        random_string += characters[distribution(generator)];
    }
    return random_string;
}

// Main function to send request
void send_request() {
    std::cout << "Running Create request for Checkout (C++)" << std::endl;

    // Generating a random order ID
    std::string random_order_id = generate_random_string(8);

    // Read JSON payload from file
    std::string body_str;
    try {
        std::ifstream file("create_order_payload.json");
        if (!file.is_open()) {
            throw std::runtime_error("Error: File not found.");
        }

        std::stringstream buffer;
        buffer << file.rdbuf();
        body_str = buffer.str();

        // Replace placeholder with the random order ID
        size_t pos = body_str.find("my_order_id");
        if (pos != std::string::npos) {
            body_str.replace(pos, std::string("my_order_id").length(), random_order_id);
        }
        //std::cout << "body: " << body_str << std::endl;
    } catch (const std::runtime_error& e) {
        std::cout << e.what() << std::endl;
        return;
    }

    // Create HTTP client
    web::http::client::http_client client(U("https://checkoutapistage.svea.com/api/orders"));

    SveaAuth sveaAuth;
    sveaAuth.setMerchantId("124842");
    sveaAuth.setSecretWord("1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW");

    // Create request with headers
    web::http::http_request request = sveaAuth.get_request_headers(body_str);
    request.set_method(web::http::methods::POST);
    request.set_body(body_str);

    // Send the request asynchronously
    client.request(request).then([](web::http::http_response response) {
        if (response.status_code() == 200 || response.status_code() == 201) {
            std::cout << "Success!" << std::endl;
        } else {
            std::cout << "Failed..." << std::endl;
        }

         //std::cout << "Response: " << response.to_string() << std::endl;
         //response.extract_string().then([](std::string body) {
         //    std::cout << "Response body: " << body << std::endl;
         //}).wait();
    }).wait();
    std::cout << "----------------------------------------------------------" << std::endl;
}

int main() {
    send_request();
    return 0;
}
