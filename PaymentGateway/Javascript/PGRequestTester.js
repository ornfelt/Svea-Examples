const https = require('https');
const crypto = require('crypto');

class PgRequest {
    constructor() {
        this.httpClient = https;
    }

    makePostRequestAsync() {
        const messageXML = `<?xml version="1.0" encoding="UTF-8"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>125123123</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>`;
        const encodedMessage = Buffer.from(messageXML).toString('base64');
        const secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
        const mac = this.getSha512Hash(encodedMessage + secret);

        console.log("mac:", mac);
        console.log("encodedMessage:", encodedMessage);

        const postData = `merchantid=1200&message=${encodedMessage}&mac=${mac}`;
        const options = {
            hostname: 'webpaypaymentgatewaystage.svea.com',
            port: 443,
            path: '/webpay/payment',
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'Content-Length': Buffer.byteLength(postData)
            }
        };

        const req = this.httpClient.request(options, (res) => {
            console.log(`Response status code: ${res.statusCode}`);
            res.on('data', (d) => {
                process.stdout.write(d);
            });
        });

        req.on('error', (e) => {
            console.error(e);
        });

        req.write(postData);
        req.end();
    }

    makeGetQueryTransactionIdRequestAsync() {
        const messageXML = `<?xml version="1.0" encoding="UTF-8"?><query><transactionid>900497</transactionid></query>`;
        const encodedMessage = Buffer.from(messageXML).toString('base64');
        const secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d";
        const mac = this.getSha512Hash(encodedMessage + secret);

        console.log("mac:", mac);
        console.log("encodedMessage:", encodedMessage);

        const postData = `merchantid=1200&message=${encodedMessage}&mac=${mac}`;
        const options = {
            hostname: 'webpaypaymentgatewaystage.svea.com',
            port: 443,
            path: '/webpay/rest/querytransactionid',
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'Content-Length': Buffer.byteLength(postData)
            }
        };

        const req = this.httpClient.request(options, (res) => {
            console.log(`Response status code: ${res.statusCode}`);
            let dataChunks = [];
            res.on('data', (chunk) => {
                dataChunks.push(chunk);
            });
            res.on('end', () => {
                const responseMessage = Buffer.concat(dataChunks).toString();
                console.log("Response message:", responseMessage);
                // Additional code to parse and process the XML response can be added here.
            });
        });

        req.on('error', (e) => {
            console.error(e);
        });

        req.write(postData);
        req.end();
    }

    getSha512Hash(input) {
        return crypto.createHash('sha512').update(input).digest('hex');
    }
}

// Usage
const tester = new PgRequest();
tester.makePostRequestAsync();
tester.makeGetQueryTransactionIdRequestAsync();
