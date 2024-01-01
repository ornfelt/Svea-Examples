#ifndef SveaAuth_H
#define SveaAuth_H

#include <cpprest/http_client.h>
#include <cpprest/http_listener.h>
#include <cpprest/json.h>
#include <openssl/hmac.h>
#include <openssl/evp.h>
#include <openssl/bio.h>
#include <iostream>
#include <iomanip>
#include <sstream>
#include <ctime>

#include <algorithm>
#include <string>

class SveaAuth {
public:
    SveaAuth() {}
    SveaAuth(const std::string& m_id, const std::string& secret)
        : merchant_id(m_id), secret_word(secret) {}

    void setMerchantId(const std::string& m_id) {
        merchant_id = m_id;
    }

    void setSecretWord(const std::string& secret) {
        secret_word = secret;
    }

    web::http::http_request get_request_headers(const std::string& request_body = "", const web::http::http_headers& extra_headers = web::http::http_headers()) {
        auto now = std::chrono::system_clock::now();
        auto now_c = std::chrono::system_clock::to_time_t(now);
        std::stringstream ss;
        ss << std::put_time(std::gmtime(&now_c), "%Y-%m-%d %H:%M:%S");
        std::string timestamp = ss.str();

        std::string token = get_auth_token(timestamp, request_body);
        web::http::http_request req;
        req.headers().add("Timestamp", timestamp);
        req.headers().add("Authorization", "Svea " + token);
        req.headers().add("Content-Type", "application/json");
        for (const auto& header : extra_headers) {
            req.headers().add(header.first, header.second);
        }
        return req;
    }

private:
    std::string merchant_id;
    std::string secret_word;

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

#endif // SveaAuth_H
