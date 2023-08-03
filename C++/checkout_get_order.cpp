#include <iostream>
#include <ctime>
#include <chrono>
#include <sstream>
#include <iomanip>
#include <openssl/sha.h>
#include <openssl/bio.h>
#include <openssl/evp.h>
#include <openssl/buffer.h>
#include <cpprest/http_client.h>
#include <cpprest/asyncrt_utils.h>

/**
This code uses C++ REST SDK (Casablanca) to handle HTTP requests. You'll need to install and configure the C++ REST SDK to build and run this code. OpenSSL is also used for generating SHA-512 hashes and base64 encoding. Make sure to link your OpenSSL libraries during compilation.
*/

class TestClass {
public:
    web::http::http_headers get_request_headers(std::string requests_body="", web::http::http_headers extra_headers = web::http::http_headers()) {
        auto timestamp = get_current_timestamp();
        auto token = get_auth_token(timestamp, requests_body);

        web::http::http_headers headers;
        headers[U("Authorization")] = U("Svea ") + token;
        headers[U("Timestamp")] = timestamp;

        for (const auto& header : extra_headers) {
            headers[header.first] = header.second;
        }

        return headers;
    }

    std::string get_auth_token(std::string timestamp, std::string request_body="") {
        std::string digest = request_body + svea_basic_settings.secret_key + timestamp;
        std::string h = sha512_digest(digest);
        std::string auth = svea_basic_settings.merchant_id + ":" + h;
        std::string token = base64_encode(auth);
        return token;
    }

private:
    struct {
        std::string merchant_id = "";
        std::string secret_key = "";
    } svea_basic_settings;

    std::string get_current_timestamp() {
        auto now = std::chrono::system_clock::now();
        std::time_t now_c = std::chrono::system_clock::to_time_t(now);
        std::tm* now_tm = std::localtime(&now_c);

        std::ostringstream oss;
        oss << std::put_time(now_tm, "%Y-%m-%d %H:%M:%S");
        return oss.str();
    }

    std::string sha512_digest(const std::string& data) {
        unsigned char hash[SHA512_DIGEST_LENGTH];
        SHA512(reinterpret_cast<const unsigned char*>(data.c_str()), data.length(), hash);

        std::ostringstream oss;
        oss << std::hex;
        for (const auto& byte : hash) {
            oss << std::setw(2) << std::setfill('0') << static_cast<int>(byte);
        }
        return oss.str();
    }

    std::string base64_encode(const std::string& data) {
        BIO *bio, *b64;
        BUF_MEM *bptr;

        b64 = BIO_new(BIO_f_base64());
        bio = BIO_new(BIO_s_mem());
        bio = BIO_push(b64, bio);

        BIO_write(bio, data.c_str(), data.length());
        BIO_flush(bio);
        BIO_get_mem_ptr(bio, &bptr);

        std::string result(bptr->data, bptr->length);
        BIO_free_all(bio);

        return result;
    }
};

int main() {
    TestClass testInstance;

    utility::string_t url = U("https://checkoutapistage.svea.com/api/orders/1");

    web::http::http_headers my_sco_headers = testInstance.get_request_headers(web::http::http_headers{{U("Connection"), U("keep-alive")}});

    web::http::client::http_client client(url);
    web::http::http_request request(methods::GET);
    request.headers() = my_sco_headers;

    // Send request
    web::http::http_response response = client.request(request).get();
    std::wcout << response.to_string() << std::endl;
    response.headers().set_content_length(0);
    response.headers().set_content_type(U("text/plain"));

    return 0;
}