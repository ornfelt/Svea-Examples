import requests
import random
import re
import os

def main():
    print("Running Create request for Webpay (Python)")
    try:
        url = "https://webpaywsstage.svea.com/sveawebpay.asmx"
        action = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu"
        headers = {
            "Content-Type": "application/soap+xml; charset=utf-8",
            "SOAPAction": action
        }
        random_order_id = ''.join(random.choices('0123456789', k=8))

        soap_template = """
        <soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:web="https://webservices.sveaekonomi.se/webpay" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
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
        """

        soap_envelope = soap_template.replace("my_order_id", random_order_id)
        response = requests.post(url, headers=headers, data=soap_envelope)
        #print("Response Code:", response.status_code)
        #print("Response:")
        #print(response.text)

        if response.status_code == 200 and "accepted>true" in response.text.lower():
            print("Success!")
            svea_order_id_match = re.search(r"<(?:\w+:)?SveaOrderId>(\d+)</(?:\w+:)?SveaOrderId>", response.text)
            if svea_order_id_match:
                svea_order_id = svea_order_id_match.group(1)
                #print(f"SveaOrderId extracted: {svea_order_id}")

                output_file = "./created_order_id.txt"
                os.makedirs(os.path.dirname(output_file), exist_ok=True)
                with open(output_file, "w") as f:
                    f.write(svea_order_id)
                #print(f"SveaOrderId saved to {output_file}")
            else:
                print("Failed to extract SveaOrderId.")
        else:
            print("Failed...")
        print("----------------------------------------------------------")
        
    except Exception as e:
        print(e)

if __name__ == "__main__":
    main()

