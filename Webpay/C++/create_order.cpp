// Requires:
// sudo apt-get install libcurl4-openssl-dev
// or: yay -S libcurl-openssl-1.0
// Usage: g++ -o create_order create_order.cpp -lcurl && ./create_order

#include <iostream>
#include <string>
#include <curl/curl.h>
#include <algorithm>
#include <cctype>
#include <random>

// Callback function writes data to a std::string
static size_t WriteCallback(void *contents, size_t size, size_t nmemb, std::string *userp) {
    userp->append((char*)contents, size * nmemb);
    return size * nmemb;
}

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

int main() {
    CURL *curl;
    CURLcode res;
    std::string readBuffer;
    std::string randomOrderId = generate_random_string(8);

    curl_global_init(CURL_GLOBAL_ALL);

    curl = curl_easy_init();
    if(curl) {
        std::cout << "Running Create request for Webpay (C++)" << std::endl;
        const char *url = "https://webpaywsstage.svea.com/sveawebpay.asmx";
        const char *soapAction = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu";

        std::string soapEnvelope = R"(
        <soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:web="https://webservices.sveaekonomi.se/webpay">
            <soap:Header/>
            <soap:Body>
                <web:CreateOrderEu>
                    <web:request>
                        <web:Auth>
                            <web:ClientNumber>WEBPAY_CLIENT_ID</web:ClientNumber>
                            <web:Username>WEBPAY_PASSWORD</web:Username>
                            <web:Password>WEBPAY_PASSWORD</web:Password>
                        </web:Auth>
                        <web:CreateOrderInformation>
                            <web:ClientOrderNumber>my_order_id</web:ClientOrderNumber>
                            <web:OrderRows>
                                <web:OrderRow>
                                    <web:ArticleNumber>123</web:ArticleNumber>
                                    <web:Description>Some Product 1</web:Description>
                                    <web:PricePerUnit>293.6</web:PricePerUnit>
                                    <web:PriceIncludingVat>false</web:PriceIncludingVat>
                                    <web:NumberOfUnits>2</web:NumberOfUnits>
                                    <web:Unit>st</web:Unit>
                                    <web:VatPercent>25</web:VatPercent>
                                    <web:DiscountPercent>0</web:DiscountPercent>
                                    <web:DiscountAmount>0</web:DiscountAmount>
                                    <web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>
                                </web:OrderRow>
                                <web:OrderRow>
                                    <web:ArticleNumber>456</web:ArticleNumber>
                                    <web:Description>Some Product 2</web:Description>
                                    <web:PricePerUnit>39.2</web:PricePerUnit>
                                    <web:PriceIncludingVat>false</web:PriceIncludingVat>
                                    <web:NumberOfUnits>1</web:NumberOfUnits>
                                    <web:Unit>st</web:Unit>
                                    <web:VatPercent>25</web:VatPercent>
                                    <web:DiscountPercent>0</web:DiscountPercent>
                                    <web:DiscountAmount>0</web:DiscountAmount>
                                    <web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>
                                </web:OrderRow>
                            </web:OrderRows>
                            <web:CustomerIdentity>
                                <web:NationalIdNumber>4605092222</web:NationalIdNumber>
                                <web:Email>firstname.lastname@svea.com</web:Email>
                                <web:PhoneNumber>080000000</web:PhoneNumber>
                                <web:FullName>Tester Testsson</web:FullName>
                                <web:Street>Gatan 99</web:Street>
                                <web:ZipCode>12345</web:ZipCode>
                                <web:Locality>16733</web:Locality>
                                <web:CountryCode>SE</web:CountryCode>
                                <web:CustomerType>Individual</web:CustomerType>
                            </web:CustomerIdentity>
                            <web:OrderDate>2023-12-18T11:07:23</web:OrderDate>
                            <web:OrderType>Invoice</web:OrderType>
                        </web:CreateOrderInformation>
                    </web:request>
                </web:CreateOrderEu>
            </soap:Body>
        </soap:Envelope>
        )";

        size_t pos = soapEnvelope.find("my_order_id");
        if (pos != std::string::npos) {
            // Replace the substring with random_numb
            soapEnvelope.replace(pos, std::string("my_order_id").length(), randomOrderId);
        }

        struct curl_slist *headers = NULL;
        headers = curl_slist_append(headers, "Content-Type: application/soap+xml;charset=UTF-8");
        headers = curl_slist_append(headers, ("SOAPAction: " + std::string(soapAction)).c_str());

        curl_easy_setopt(curl, CURLOPT_URL, url);
        curl_easy_setopt(curl, CURLOPT_POSTFIELDS, soapEnvelope.c_str());
        curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, soapEnvelope.length());
        curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &readBuffer);

        res = curl_easy_perform(curl);

        if(res != CURLE_OK)
            fprintf(stderr, "curl_easy_perform() failed: %s\n", curl_easy_strerror(res));
        else {
            //std::cout << "Response: " << readBuffer << std::endl;
            long httpCode = 0;
            curl_easy_getinfo(curl, CURLINFO_RESPONSE_CODE, &httpCode);

            std::transform(readBuffer.begin(), readBuffer.end(), readBuffer.begin(), 
                           [](unsigned char c){ return std::tolower(c); });

            if (httpCode == 200 && readBuffer.find("accepted>true") != std::string::npos)
                std::cout << "Success!" << std::endl;
            else if (httpCode == 200)
                std::cout << "Success!" << std::endl;
            else
                std::cout << "Failed..." << std::endl;
        }

        curl_slist_free_all(headers);
        curl_easy_cleanup(curl);
        std::cout << "----------------------------------------------------------" << std::endl;
    }

    curl_global_cleanup();
    return 0;
}
