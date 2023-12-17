// npm install axios

const axios = require('axios');

async function getOrders() {
    try {
        const url = "https://webpayadminservicestage.svea.com/AdminService.svc/secure";
        const soapAction = "http://tempuri.org/IAdminService/GetOrders";

        const soapEnvelope = `<soap:Envelope xmlns:soap="http://www.w3.org/2003/05/soap-envelope" xmlns:tem="http://tempuri.org/" xmlns:dat="http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service">
            <soap:Header xmlns:wsa="http://www.w3.org/2005/08/addressing">
                <wsa:Action>${soapAction}</wsa:Action>
                <wsa:To>${url}</wsa:To>
            </soap:Header>
            <soap:Body>
                <tem:GetOrders>
                    <tem:request>
                        <dat:Authentication>
                            <dat:Password>sverigetest</dat:Password>
                            <dat:Username>sverigetest</dat:Username>
                        </dat:Authentication>
                        <dat:OrdersToRetrieve>
                            <dat:GetOrderInformation>
                                <dat:ClientId>79021</dat:ClientId>
                                <dat:OrderType>Invoice</dat:OrderType>
                                <dat:SveaOrderId>9731563</dat:SveaOrderId>
                            </dat:GetOrderInformation>
                        </dat:OrdersToRetrieve>
                    </tem:request>
                </tem:GetOrders>
            </soap:Body>
        </soap:Envelope>`;

        const config = {
            headers: {
                'Content-Type': 'application/soap+xml;charset=UTF-8',
                'SOAPAction': soapAction
            }
        };

        const response = await axios.post(url, soapEnvelope, config);

        console.log("Response Code:", response.status);
        console.log("Response:", response.data);
    } catch (error) {
        console.error(error);
    }
}

getOrders();
