const axios = require('axios')
const moment = require('moment')

// setup
const merchantId = "";
const secret = "";
let token = ''
let timestamp = ''
// test order
const order = JSON.stringify({
		"countrycode": 'SE',
		"currency": 'SEK',
		"locale": 'sv-SE',
		"clientordernumber": 129782183212,
		"merchantdata": 'Test string from merchant',
		"merchantsettings": {
		"termsuri": 'http://localhost:51898/terms',
		"checkouturi": 'http://localhost:51925/',
		"confirmationuri": 'http://localhost:51925/checkout/confirm',
		"pushuri": 'https://localhost:51925/push.php?svea_order_id={checkout.order.uri}'
		},
		"cart": {
		"items": [{
		"articlenumber": '11',
		"name": 'aa',
		"quantity": 200,
		"unitprice": 12300,
		"vatpercent": 2500,
		"unit": 'st',
		"temporaryreference": '1',
		"merchantdata": 'Size: S'
		},
		{
			"articlenumber": '22',
			"name": 'bb',
			"quantity": 300,
			"unitprice": 25000,
			"vatpercent": 2500,
			"unit": 'pcs',
			"temporaryreference": '2',
			"merchantdata": null
		}
		]
		},
		"presetvalues": [{
			"typename": 'emailAddress',
			"value": 'test@yourdomain.se',
			"isreadonly": false
		},
		{
			"typename": 'postalCode',
			"value": '99999',
			"isreadonly": false
		}
		]
})

function generateToken() {
	console.log("Generating token...");
	timestamp = moment.utc().format('yyyy-MM-DD HH:mm:ss')
		let h = require("crypto")
		.createHash("sha512")
		.update(order + secret + timestamp)
		.digest("hex");

	let hash = h.toUpperCase()
		let encoded = Buffer.from(merchantId + ':' + hash).toString('base64')
		token = encoded
		console.log("DONE!");
}

generateToken();

var config = {
method: 'post',
		url: 'https://checkoutapistage.svea.com/api/orders',
		headers: { 
			'Content-Type': 'application/json',
			'Accept': '*/*',
			'Authorization': `Svea ${token}`,
			'Timestamp': timestamp
		},
data : order
};

axios(config)
	.then(function (response) {
			console.log(JSON.stringify(response.data));
			})
.catch(function (error) {
		console.log(error);
		});