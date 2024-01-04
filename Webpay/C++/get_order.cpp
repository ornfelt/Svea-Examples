// Requires:
// sudo apt-get install libcurl4-openssl-dev
// or: yay -S libcurl-openssl-1.0
// Usage: g++ -o get_order get_order.cpp -lcurl && ./get_order

#include <iostream>
#include <string>
#include <curl/curl.h>
#include <algorithm>
#include <cctype>

static size_t WriteCallback(void *contents, size_t size, size_t nmemb, std::string *s) {
    size_t newLength = size * nmemb;
    try {
        s->append((char*)contents, newLength);
    } catch(std::bad_alloc &e) {
        // Handle memory problem
        return 0;
    }
    return newLength;
}

int main() {
    CURL *curl;
    CURLcode res;
    std::string readBuffer;

    curl_global_init(CURL_GLOBAL_DEFAULT);

    curl = curl_easy_init();
    if(curl) {
        std::cout << "Running GET request for Webpay (C++)" << std::endl;
        std::string url = "https://webpayadminservicestage.svea.com/AdminService.svc/secure";
        std::string soapAction = "http://tempuri.org/IAdminService/GetOrders";
        std::string soapEnvelope = R"(
        <soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:tem="http://tempuri.org/" xmlns:dat="http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service">
            <soap:Header xmlns:wsa="http://www.w3.org/2005/08/addressing">
                <wsa:Action>http://tempuri.org/IAdminService/GetOrders</wsa:Action>
                <wsa:To>https://webpayadminservicestage.svea.com/AdminService.svc/secure</wsa:To>
            </soap:Header>
            <soap:Body>
                <tem:GetOrders>
                    <tem:request>
                        <dat:Authentication>
                            <dat:Password>WEBPAY_PASSWORD</dat:Password>
                            <dat:Username>WEBPAY_PASSWORD</dat:Username>
                        </dat:Authentication>
                        <dat:OrdersToRetrieve>
                            <dat:GetOrderInformation>
                                <dat:ClientId>WEBPAY_CLIENT_ID</dat:ClientId>
                                <dat:OrderType>Invoice</dat:OrderType>
                                <dat:SveaOrderId>WEBPAY_ORDER_TO_FETCH</dat:SveaOrderId>
                            </dat:GetOrderInformation>
                        </dat:OrdersToRetrieve>
                    </tem:request>
                </tem:GetOrders>
            </soap:Body>
        </soap:Envelope>
        )";

        struct curl_slist *headers = NULL;
        headers = curl_slist_append(headers, "Content-Type: application/soap+xml;charset=UTF-8");
        headers = curl_slist_append(headers, ("SOAPAction: " + soapAction).c_str());

        curl_easy_setopt(curl, CURLOPT_URL, url.c_str());
        curl_easy_setopt(curl, CURLOPT_POSTFIELDS, soapEnvelope.c_str());
        curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, (long)soapEnvelope.length());
        curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &readBuffer);
        //curl_easy_setopt(curl, CURLOPT_VERBOSE, 1L); // Enable verbose output
        curl_easy_setopt(curl, CURLOPT_FOLLOWLOCATION, 1L); // Follow redirects if any
        // curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0L); // Disable SSL verification (only for debugging)

        res = curl_easy_perform(curl);

        if(res != CURLE_OK)
            fprintf(stderr, "curl_easy_perform() failed: %s\n", curl_easy_strerror(res));
        else {
            //std::cout << "Response: " << readBuffer << std::endl;
            long httpCode = 0;
            curl_easy_getinfo(curl, CURLINFO_RESPONSE_CODE, &httpCode);

            std::transform(readBuffer.begin(), readBuffer.end(), readBuffer.begin(), 
                           [](unsigned char c){ return std::tolower(c); });

            if (httpCode == 200)
                std::cout << "Success!" << std::endl;
            else
                std::cout << "Failed..." << std::endl;
        }

        curl_easy_cleanup(curl);
        curl_slist_free_all(headers);
    }

    curl_global_cleanup();

    return 0;
}
