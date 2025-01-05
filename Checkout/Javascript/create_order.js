const axios = require('axios');
const fs = require('fs').promises;
const crypto = require('crypto');

class SveaAuth {
    static async sendRequest() {
        console.log("Running Create request for Checkout (Javascript)");

        const url = "https://checkoutapistage.svea.com/api/orders";
        const randomOrderId = Array.from({ length: 15 }, () => Math.floor(Math.random() * 10)).join('');

        let body;
        try {
            let bodyStr = await fs.readFile('create_order_payload.json', 'utf8');
            bodyStr = bodyStr.replace("my_order_id", randomOrderId);
            body = JSON.parse(bodyStr);
        } catch (error) {
            console.error("Error:", error.message);
            return;
        }

        const headers = await SveaAuth.setHttpRequestHeaders("", JSON.stringify(body));
        try {
            const response = await axios.post(url, body, { headers });
            //console.log("response:", response);
            //console.log("response body:", response.data);
            //console.log("response status:", response.status);

            if (response.status === 200 || response.status === 201) {
                console.log("Success!");

                const orderIdRegex = /"OrderId":\s*(\d+)/i;
                const match = orderIdRegex.exec(JSON.stringify(response.data));

                if (match) {
                    const orderId = match[1];
                    //console.log(`Extracted OrderId: ${orderId}`);

                    try {
                        await fs.writeFile('./created_order_id.txt', orderId, 'utf8');
                        //console.log(`OrderId saved to ../created_order_id.txt`);
                    } catch (fileError) {
                        console.error(`Failed to save OrderId to file: ${fileError.message}`);
                    }
                } else {
                    console.error("OrderId not found in the response.");
                }

            } else {
                console.log("Failed...");
            }
        } catch (error) {
            console.error("Request failed:", error);
        }
    }

    static async setHttpRequestHeaders(operation, requestMessage) {
        const timestamp = new Date().toISOString();
        const token = SveaAuth.createAuthenticationToken(requestMessage, timestamp);

        return {
            "Authorization": "Svea " + token,
            "Timestamp": timestamp,
            "Content-Type": "application/json; charset=utf-8"
        };
    }

    static createAuthenticationToken(requestMessage, timestamp) {
        const merchantId = "CHECKOUT_MERCHANT_ID";
        const secretKey = "CHECKOUT_SECRET_KEY";

        const hash = crypto.createHash('sha512');
        hash.update(requestMessage + secretKey + timestamp);
        const hashString = hash.digest('hex');
        const authToken = Buffer.from(`${merchantId}:${hashString}`).toString('base64');

        return authToken;
    }
}

SveaAuth.sendRequest();

