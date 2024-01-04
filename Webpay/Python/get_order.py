import requests

def get_orders():
    print("Running GET request for Webpay (Python)")
    url = "https://webpayadminservicestage.svea.com/AdminService.svc/secure"
    soap_action = "http://tempuri.org/IAdminService/GetOrders"
    headers = {
        "Content-Type": "application/soap+xml;charset=UTF-8",
        "SOAPAction": soap_action
    }
    soap_envelope = """
    <soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:tem="http://tempuri.org/" xmlns:dat="http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service">
        <soap:Header xmlns:wsa="http://www.w3.org/2005/08/addressing">
            <wsa:Action>{soap_action}</wsa:Action>
            <wsa:To>{url}</wsa:To>
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
    """.format(soap_action=soap_action, url=url)

    response = requests.post(url, data=soap_envelope, headers=headers)

    #print("Response Code:", response.status_code)
    #print("Response:", response.text)
    if response.status_code == 200:
        print("Success!")
    else:
        print("Failed...")

if __name__ == "__main__":
    get_orders()
