var currentMerchant = pm.environment.get("currentMerchant") || "SE";
var merchantId = pm.environment.get(`checkoutMerchant${currentMerchant}`);
var secretKey = pm.environment.get(`checkoutSecretKey${currentMerchant}`);
console.log("MerchantID: " + merchantId);
console.log("Secret: " + secretKey);

var message = pm.request.body ? pm.request.body.toString() : "";

var hashString = CryptoJS.SHA512(`${message}${secretKey}`).toString();
hashString = hashString.replace(/\-/, "");


pm.request.addHeader({
    name: "Authorization",
    key: "Authorization",
    value: "Svea " + btoa(`${merchantId}:${hashString}`)
});