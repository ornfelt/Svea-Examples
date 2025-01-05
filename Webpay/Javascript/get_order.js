// npm install axios

const fs = require('fs'); // Import the File System module

const axios = require('axios');

async function getOrders() {
    console.log("Running GET request for Webpay (Javascript)");
    try {
        const url = "https://webpayadminservicestage.svea.com/AdminService.svc/secure";
        const soapAction = "http://tempuri.org/IAdminService/GetOrders";

        let soapEnvelope = `<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:tem="http://tempuri.org/" xmlns:dat="http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service">
            <soap:Header xmlns:wsa="http://www.w3.org/2005/08/addressing">
                <wsa:Action>${soapAction}</wsa:Action>
                <wsa:To>${url}</wsa:To>
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
                                <dat:SveaOrderId>WEBPAY_ORDER_TO_FETCH_VALUE</dat:SveaOrderId>
                            </dat:GetOrderInformation>
                        </dat:OrdersToRetrieve>
                    </tem:request>
                </tem:GetOrders>
            </soap:Body>
        </soap:Envelope>`;

        let sveaOrderId;
        try {
            const filePath = './created_order_id.txt';
            if (fs.existsSync(filePath)) {
                sveaOrderId = fs.readFileSync(filePath, 'utf8').trim();
                //console.log(`Using SveaOrderId: ${sveaOrderId}`);
                soapEnvelope = soapEnvelope.replace("WEBPAY_ORDER_TO_FETCH_VALUE", sveaOrderId);
            } else {
                throw new Error(`File not found: ${filePath}`);
            }
        } catch (error) {
            console.error("Error reading SveaOrderId from file:", error.message);
            return;
        }

        const config = {
            headers: {
                'Content-Type': 'application/soap+xml;charset=UTF-8',
                'SOAPAction': soapAction
            }
        };

        const response = await axios.post(url, soapEnvelope, config);
        //console.log("Response Code:", response.status);
        //console.log("Response:", response.data);

        if (response.status === 200)
            console.log("Success!");
        else
            console.log("Failed...");
    } catch (error) {
        console.error(error);
    }
}

getOrders();

