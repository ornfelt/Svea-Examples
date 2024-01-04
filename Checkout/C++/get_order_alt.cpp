/**
  Uses Openssl and cpprestsdk (tried on Arch linux)
  g++ -std=c++17 -lcpprest -lcrypto -lssl get_order_alt.cpp -o get_order_alt && ./get_order_alt
*/

#include <cpprest/http_client.h>
#include <cpprest/json.h>
#include <cpprest/uri_builder.h>
#include <cpprest/http_headers.h>
#include <iomanip>
#include <sstream>
#include <chrono>
#include <openssl/sha.h>
#include <codecvt>

using namespace web;
using namespace web::http;
using namespace web::http::client;

class TestClass {
    public:
        TestClass() {}

        utility::string_t get_formatted_timestamp() {
            auto now = std::chrono::system_clock::now();
            std::time_t time_now = std::chrono::system_clock::to_time_t(now);
            struct tm* gmTime = std::gmtime(&time_now);
            std::stringstream stream;
            stream << std::put_time(gmTime, "%Y-%m-%d %H:%M:%S");
            return utility::conversions::to_string_t(stream.str());
        }

        web::http::http_headers get_request_headers(const std::string& requests_body = "", const web::http::http_headers& extra_headers = web::http::http_headers()) {
            utility::string_t timestamp = get_formatted_timestamp();
            utility::string_t token = get_auth_token(timestamp, requests_body);

            web::http::http_headers headers;
            std::cout << "Token: " << token << std::endl;
            std::cout << "Timestamp: " << timestamp << std::endl;
            headers[U("Timestamp")] = timestamp;
            headers[U("Authorization")] = U("Svea ") + token;
            //for (auto const& header : extra_headers) {
            //    headers[header.first] = header.second;
            //}

            return headers;
        }

    private:
        const utility::string_t merchant_id = U("CHECKOUT_MERCHANT_ID");
        const utility::string_t secret_word = U("CHECKOUT_SECRET_KEY");

        std::string sha512(const std::string& input) {
            unsigned char hash[SHA512_DIGEST_LENGTH];
            SHA512(reinterpret_cast<const unsigned char*>(input.c_str()), input.length(), hash);
            std::stringstream ss;
            for (int i = 0; i < SHA512_DIGEST_LENGTH; ++i) {
                ss << std::hex << std::setw(2) << std::setfill('0') << static_cast<int>(hash[i]);
            }
            return ss.str();
        }

        std::string base64Encode(const std::string& input) {
            BIO* bio = BIO_new(BIO_s_mem());
            BIO* b64 = BIO_new(BIO_f_base64());
            bio = BIO_push(b64, bio);

            BIO_set_flags(bio, BIO_FLAGS_BASE64_NO_NL); // Prevent line breaks
            BIO_write(bio, input.c_str(), static_cast<int>(input.length()));
            BIO_flush(bio);

            BUF_MEM* bufferPtr;
            BIO_get_mem_ptr(bio, &bufferPtr);
            std::string encodedStr(bufferPtr->data, bufferPtr->length);

            BIO_free_all(bio);

            return encodedStr;
        }

        std::string get_auth_token(const std::string& timestamp, const std::string& request_body = "") {
            std::string digest = request_body + secret_word + timestamp;
            std::string hashedDigest = sha512(digest);

            std::string auth = merchant_id + ":" + hashedDigest;
            std::string token = base64Encode(auth);

            return token;
        }
};

int main() {
    TestClass testInstance;
    web::http::http_headers my_headers = testInstance.get_request_headers();
    //web::http::http_headers my_headers = testInstance.get_request_headers({"Content-Type", "application/json"});

    utility::string_t order_id = U("CHECKOUT_ORDER_TO_FETCH");
    //utility::string_t url = U("https://checkoutapistage.svea.com/api/orders/") + order_id;
    utility::string_t url = U("https://paymentadminapistage.svea.com/api/v1/orders/") + order_id;
    web::http::http_request request(web::http::methods::GET);
    request.headers() = my_headers;

    web::http::client::http_client_config config;
    config.set_timeout(std::chrono::seconds(30));
    web::http::client::http_client client(url, config);

    client.request(request).then([](web::http::http_response response) {
            std::cout << "Status: " << response.status_code() << std::endl;
            if (response.status_code() == web::http::status_codes::OK) {
            return response.extract_json();
            }
            return pplx::task_from_result(json::value());
            }).then([](pplx::task<json::value> previousTask) {
                try {
                const json::value& v = previousTask.get();
                //std::wcout << v << std::endl;
                std::cout << v << std::endl;
                } catch (const std::exception& e) {
                std::wcout << e.what() << std::endl;
                }
                }).wait();

    return 0;
}
