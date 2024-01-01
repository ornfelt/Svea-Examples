#include <algorithm>
#include <cctype>
#include <iostream>
#include <random>
#include <sstream>
#include <string>
#include <iomanip>
#include <string>
#include <curl/curl.h>
#include <openssl/sha.h>
#include <openssl/bio.h>
#include <openssl/evp.h>
#include <openssl/buffer.h>

// Usage:
// g++ -o pg_request pg_request.cpp -lcurl -lssl -lcrypto && ./pg_request

class pg_request {
public:
    void makeGetQueryTransactionIdRequest() {
        int transactionId = 900497;
        std::string messageXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><query><transactionid>" + std::to_string(transactionId) + "</transactionid></query>";
        std::string encodedMessage = base64Encode(reinterpret_cast<const unsigned char*>(messageXML.c_str()), messageXML.length());
        std::string secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
        std::string mac = getSha512Hash(encodedMessage + secret);

        std::string postData = "merchantid=1200&message=" + encodedMessage + "&mac=" + mac;
        performHttpRequest("https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid", postData, false);
    }

    void makePostRequest() {
        std::random_device rd;
        std::mt19937 gen(rd());
        std::uniform_int_distribution<> distr(1000000, 9999999);
        int randomRefNo = distr(gen);
        std::string messageXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>" + std::to_string(randomRefNo) + "</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>";

        std::string encodedMessage = base64Encode(reinterpret_cast<const unsigned char*>(messageXML.c_str()), messageXML.length());
        std::string secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
        std::string mac = getSha512Hash(encodedMessage + secret);
        //std::cout << "Base64 Encoded Message (Post): " << encodedMessage << std::endl;
        //std::cout << "sha512 hashed message (Post): " << mac << std::endl;

        std::string postData = "merchantid=1200&message=" + encodedMessage + "&mac=" + mac;
        performHttpRequest("https://webpaypaymentgatewaystage.svea.com/webpay/payment", postData, true);
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

    static size_t writeFunction(void* ptr, size_t size, size_t nmemb, std::string* data) {
        data->append((char*)ptr, size * nmemb);
        return size * nmemb;
    }

    void performHttpRequest(const std::string& url, const std::string& postData, bool isPost) {
        CURL* curl = curl_easy_init();
        if (curl) {
            curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
            curl_easy_setopt(curl, CURLOPT_POSTFIELDS, postData.c_str());
            curl_easy_setopt(curl, CURLOPT_SSLVERSION, CURL_SSLVERSION_TLSv1_2);
            curl_easy_setopt(curl, CURLOPT_FOLLOWLOCATION, 1L);
            curl_easy_setopt(curl, CURLOPT_TIMEOUT, 30L); // 30 seconds
            //curl_easy_setopt(curl, CURLOPT_HTTP_VERSION, CURL_HTTP_VERSION_1_0);
            //curl_easy_setopt(curl, CURLOPT_FAILONERROR, true);

            // Response information
            std::string responseString;
            std::string headerString;
            curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, writeFunction);
            curl_easy_setopt(curl, CURLOPT_WRITEDATA, &responseString);
            curl_easy_setopt(curl, CURLOPT_HEADERDATA, &headerString);

            // Set headers
            struct curl_slist* headers = NULL;
            headers = curl_slist_append(headers, "Content-Type: application/x-www-form-urlencoded");
            curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);

            // Enable verbose output for debugging
            //curl_easy_setopt(curl, CURLOPT_VERBOSE, 1L);

            CURLcode res = curl_easy_perform(curl);
            if (res != CURLE_OK) {
                std::cerr << "curl_easy_perform() failed: " << curl_easy_strerror(res) << std::endl;
            } else {
                long httpCode = 0;
                curl_easy_getinfo(curl, CURLINFO_RESPONSE_CODE, &httpCode);
                //std::cout << "Response status code: " << httpCode << std::endl;
                //std::cout << "Response body: " << responseString << std::endl;

                std::transform(responseString.begin(), responseString.end(), responseString.begin(),
                        [](unsigned char c){ return std::tolower(c); });

                // Since we receive an iframe and the status tends to return 400 - verify the HTML content instead
                if (isPost && responseString.find("enter your card details") != std::string::npos || responseString.find("select card type") != std::string::npos)
                    std::cout << "Success!" << std::endl;
                else if (httpCode == 200)
                    std::cout << "Success!" << std::endl;
                else
                    std::cout << "Failed..." << std::endl;

                // Additional processing of the response can be done here

                // Clean up headers list
                curl_slist_free_all(headers);
                curl_easy_cleanup(curl);
            }
        }
    }
};

int main() {
    pg_request pgRequest;
    std::cout << "Running GET request for PaymentGateway (C++)" << std::endl;
    pgRequest.makeGetQueryTransactionIdRequest();
    // This doesn't quite work for some reason. See pg_create.cpp that uses another http client for a working post request.
    //pgRequest.makePostRequest();
    return 0;
}
