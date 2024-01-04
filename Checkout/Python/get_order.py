from datetime import datetime
from datetime import timezone
from pandas import json_normalize
import hashlib
import base64
import requests

order_id = "CHECKOUT_ORDER_TO_FETCH"
merchant_id = "CHECKOUT_MERCHANT_ID"
secret_word = "CHECKOUT_SECRET_KEY"

class TestClass:

    def __init__(self):
        pass

    def get_request_headers(self, requests_body="", extra_headers=None):
        timestamp: str = datetime.now(timezone.utc).strftime("%Y-%m-%d %H:%M:%S")
        token = self.get_auth_token(timestamp, requests_body)
        #print(token)
        headers = {"Authorization": f"Svea {token}", "Timestamp": timestamp}
        if extra_headers:
            headers.update(extra_headers)
        return headers

    def get_auth_token(self, timestamp, request_body="") -> str:
        digest = request_body + secret_word + timestamp
        h = hashlib.sha512(digest.encode("utf-8")).hexdigest()
        auth = f"{merchant_id}:{h}"
        token = base64.b64encode(f"{auth}".encode('ascii')).decode('ascii')
        return token

print("Running GET request for Checkout (Python)")
testInstance = TestClass()
#my_headers = testInstance.get_request_headers(extra_headers={'Content-Type':'application/json'})
my_headers = testInstance.get_request_headers()
#print(my_headers)

url = "https://checkoutapistage.svea.com/api/orders/" + order_id
#url = "https://paymentadminapistage.svea.com/api/v1/orders/" + order_id
payload = {}

#print("Fetching order:", order_id)
response = requests.request("GET", url, headers=my_headers, data=payload)
#response = requests.get(url, headers=my_headers, data=payload)
#print(response.text)
#response.raise_for_status()

if response.status_code == 200:
    print("Success!")
else:
    print("Failed...")

if response:
    dictr = response.json()
    result = json_normalize(dictr)
    #print(result)
