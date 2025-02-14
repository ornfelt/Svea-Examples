const https = require('https');
const crypto = require('crypto');
const fs = require('fs').promises;

const merchantId = "CHECKOUT_MERCHANT_ID";
const secretWord = "CHECKOUT_SECRET_KEY";

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

async function main() {
    let orderId = "";
    try {
        const orderIdData = await fs.readFile('./created_order_id.txt', 'utf8');
        orderId = orderIdData.trim();
        //console.log(`Using OrderId: ${orderId}`);
    } catch (err) {
        console.error(`Failed to read OrderId from file: ${err.message}`);
        return;
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
}

main();

