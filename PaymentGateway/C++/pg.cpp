// Linux usage:
// install: libcpprest-dev libssl-dev
// g++ -std=c++17 pg.cpp -o pg -lcrypto -lssl -lcpprest

// Windows usage:
// ./vcpkg install cpprestsdk cpprestsdk:x64-windows
// g++ -std=c++17 pg_request.cpp -o pg_request.exe -LC:\path\to\OpenSSL\lib -LC:\path\to\cpprestsdk\lib -lcrypto -lssl -lcpprest

#include <cpprest/http_client.h>
#include <cpprest/filestream.h>
#include <openssl/sha.h>
#include <openssl/bio.h>
#include <openssl/evp.h>
#include <openssl/buffer.h>
#include <iostream>
#include <sstream>
#include <iomanip>
#include <string>

class pg_request {
public:
    pg_request() {}

    void make_post_request() {
        std::string message_xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>125123123</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>";
        std::string encoded_message = base64Encode(reinterpret_cast<const unsigned char*>(message_xml.c_str()), message_xml.length());
        std::string secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
        std::string mac = getSha512Hash(encoded_message + secret);

        std::cout << "mac: " << mac << std::endl;
        std::cout << "encodedMessage: " << encoded_message << std::endl;

        try {
            web::http::client::http_client client(U("https://webpaypaymentgatewaystage.svea.com/webpay/payment"));
            web::http::http_request request(web::http::methods::POST);

            request.headers().set_content_type(U("application/x-www-form-urlencoded"));

            web::http::uri_builder builder;
            builder.append_query(U("merchantid"), U("1200"));
            builder.append_query(U("message"), utility::conversions::to_string_t(encoded_message));
            builder.append_query(U("mac"), utility::conversions::to_string_t(mac));

            request.set_request_uri(builder.to_uri());

            web::http::http_response response = client.request(request).get();

            std::cout << "Response status code: " << response.status_code() << std::endl;
            auto body = response.extract_string().get();
            std::cout << "Response message: " << body << std::endl;
        } catch (const std::exception &e) {
            std::cout << e.what() << std::endl;
        }
    }

private:
    std::string getSha512Hash(const std::string& input) {
        unsigned char hash[SHA512_DIGEST_LENGTH];
        SHA512_CTX sha512;
        SHA512_Init(&sha512);
        SHA512_Update(&sha512, input.c_str(), input.size());
        SHA512_Final(hash, &sha512);
        std::stringstream ss;
        for (int i = 0; i < SHA512_DIGEST_LENGTH; i++) {
            ss << std::hex << std::setw(2) << std::setfill('0') << (int)hash[i];
        }
        return ss.str();
    }

    std::string base64Encode(const unsigned char* buffer, size_t length) {
        BIO* b64 = BIO_new(BIO_f_base64());
        BIO* bio = BIO_new(BIO_s_mem());
        BIO_set_flags(b64, BIO_FLAGS_BASE64_NO_NL); // No newlines
        bio = BIO_push(b64, bio);

        BIO_write(bio, buffer, length);
        BIO_flush(bio);

        BUF_MEM* bufferPtr;
        BIO_get_mem_ptr(bio, &bufferPtr);
        BIO_set_close(bio, BIO_NOCLOSE);
        
        std::string encodedData(bufferPtr->data, bufferPtr->length);

        BIO_free_all(bio);
        return encodedData;
    }
};

int main() {
    pg_request tester;
    tester.make_post_request();
    return 0;
}
