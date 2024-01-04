const crypto = require('crypto');

class pg_request {
    constructor() {
        this.secret = "PG_SECRET_KEY";
    }

    async makeGetQueryTransactionIdRequestAsync() {
        const transactionId = PG_ORDER_TO_FETCH;
        const messageXML = `<?xml version="1.0" encoding="UTF-8"?><query><transactionid>${transactionId}</transactionid></query>`;
        const encodedMessage = btoa(messageXML);
        const mac = this.getSha512Hash(encodedMessage + this.secret);

        //console.log("mac:", mac);
        //console.log("encodedMessage:", encodedMessage);

        try {
            const url = "https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid";
            const content = {
                merchantid: "PG_MERCHANT_ID",
                message: encodedMessage,
                mac: mac
            };

            const response = await this.makeHttpRequest(url, content);
            const responseBody = await response.text();

            //console.log("Response status code:", response.status);
            //console.log("Response message:", responseBody);

            if (response.status === 200)
                console.log("Success!");
            else
                console.log("Failed...");
        } catch (e) {
            console.error(e.message);
        }
    }

    async makePostRequestAsync() {
        const randomRefNo = Math.floor(Math.random() * (9999999 - 1000000) + 1000000);
        const messageXML = `<?xml version="1.0" encoding="UTF-8"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>${randomRefNo}</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>`;
        const encodedMessage = btoa(messageXML);
        const mac = this.getSha512Hash(encodedMessage + this.secret);

        //console.log("mac:", mac);
        //console.log("encodedMessage:", encodedMessage);

        try {
            const url = "https://webpaypaymentgatewaystage.svea.com/webpay/payment";
            const content = {
                merchantid: "PG_MERCHANT_ID",
                message: encodedMessage,
                mac: mac
            };

            const response = await this.makeHttpRequest(url, content);
            const responseBody = await response.text();

            //console.log("Response status code:", response.status);
            //console.log("Response message:", responseBody);
            if (responseBody.toLowerCase().includes("enter your card details") || responseBody.toLowerCase().includes("select card type"))
                console.log("Success!");
            else
                console.log("Failed...");
        } catch (e) {
            console.error(e.message);
        }
    }

    getSha512Hash(input) {
        return crypto.createHash('sha512').update(input).digest('hex');
    }

    // Browser env
    //async getSha512Hash(input) {
    //    const encoder = new TextEncoder();
    //    const data = encoder.encode(input);
    //    const hashBuffer = await crypto.subtle.digest('SHA-512', data);
    //    return this.arrayBufferToHex(hashBuffer);
    //}

    //arrayBufferToHex(buffer) {
    //    return Array.from(new Uint8Array(buffer))
    //        .map(b => b.toString(16).padStart(2, '0'))
    //        .join('');
    //}

    async makeHttpRequest(url, postData) {
        const response = await fetch(url, {
            method: 'POST',
            body: new URLSearchParams(postData),
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        });

        //if (!response.ok) {
        //    throw new Error(`HTTP error! Status: ${response.status}`);
        //}

        return response;
    }
}

// Usage
async function runRequests() {
    const tester = new pg_request();
    console.log("Running GET request for PaymentGateway (Javascript)");
    await tester.makeGetQueryTransactionIdRequestAsync();
    console.log("Running Create request for PaymentGateway (Javascript)");
    await tester.makePostRequestAsync();
    console.log("----------------------------------------------------------");
}

runRequests().catch(console.error);
