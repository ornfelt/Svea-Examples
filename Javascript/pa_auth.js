function createHash(requestBody, timestamp, secret)
{
   if (httpMethod == 'GET' || !requestBody) {
        requestBody = ''; 
    } 
    var signatureRawData = [requestBody, secret, timestamp].join("");
    console.log("signatureData: "+ signatureRawData);
    var hash = CryptoJS.SHA512(signatureRawData);
    
    var checkoutMerchantId = pm.environment.get("checkoutMerchantId");
    console.log("merchantid: " + checkoutMerchantId);
    var j = [checkoutMerchantId, ':', hash.toString()].join("");
   
    var words  = CryptoJS.enc.Utf8.parse(j);
    var hashInBase64 =  CryptoJS.enc.Base64.stringify(words);
    
    return hashInBase64;
}

var date = new Date();
var formattedDate = date.getFullYear()+'-'+(date.getMonth()+1)+'-'+date.getDate()+' '+date.getUTCHours() + ":" + date.getMinutes();

var httpMethod = "GET";
var secret = pm.environment.get("checkoutSecret");
console.log("secret: " + secret);
pm.globals.set("timestamp", "" + formattedDate);

var hmac = createHash(pm.request.body, formattedDate, secret);
console.log(" requestBody: "+ '(empty)' +" time: "+ formattedDate+" secret: "+ secret);
pm.globals.set("HMAC", "" + hmac); 