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
            } else {
                console.log("Failed...");
            }
        } catch (error) {
            console.error("Request failed:", error);
        }
        console.log("----------------------------------------------------------");
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
        const merchantId = "124842";
        const secretKey = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW";
        
        const hash = crypto.createHash('sha512');
        hash.update(requestMessage + secretKey + timestamp);
        const hashString = hash.digest('hex');
        const authToken = Buffer.from(`${merchantId}:${hashString}`).toString('base64');

        return authToken;
    }
}

SveaAuth.sendRequest();
