const https = require('https');
const crypto = require('crypto');

const orderId = "8906830";
const merchantId = "124842";
const secretWord = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW";

class TestClass {
    createTimestamp() {
        return new Date().toISOString().replace(/T/, ' ').replace(/\..+/, '');
    }

    createAuthorizationToken() {
        const timestamp = this.createTimestamp();
        const hash = crypto.createHash('sha512');
        hash.update("" + secretWord + timestamp);
        return Buffer.from(merchantId + ':' + hash.digest('hex')).toString('base64');
    }

    getHeaders(extraHeaders = {}) {
        const timestamp = this.createTimestamp();
        const authToken = this.createAuthorizationToken();
        let headers = {
            'Authorization': `Svea ${authToken}`,
            'Timestamp': timestamp,
            ...extraHeaders
        };
        return headers;
    }

    async makeGetRequest(url, headers) {
        return new Promise((resolve, reject) => {
            https.get(url, { headers }, (resp) => {
                let data = '';
                resp.on('data', (chunk) => {
                    data += chunk;
                });
                resp.on('end', () => {
                    resolve(data);
                });
            }).on("error", (err) => {
                reject("Error: " + err.message);
            });
        });
    }
}

console.log("Running GET request for Checkout (Javascript)");
const testInstance = new TestClass();
const myHeaders = testInstance.getHeaders({'Content-Type': 'application/json'});
//const url = `https://paymentadminapistage.svea.com/api/v1/orders/${orderId}`;
const url = `https://checkoutapistage.svea.com/api/orders/${orderId}`;

testInstance.makeGetRequest(url, myHeaders)
    .then(response => {
        //console.log("Response: " + response + "\n");
        if (response) {
            console.log("Success!");
        } else {
            console.log("Failed...");
        }
    })
    .catch(error => console.log(error));
