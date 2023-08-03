from datetime import datetime
import hashlib
import base64
import requests

class TestClass:

    def __init__(self):
        pass

    def get_request_headers(self, requests_body="", extra_headers=None):
        timestamp: str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        token = self.get_auth_token(timestamp, requests_body)
        #print(token)
        headers = {"Authorization": f"Svea {token}", "Timestamp": timestamp}
        #headers = {"Authorization": f"Svea {token}"}
        if extra_headers:
            headers.update(extra_headers)
        return headers

    def get_auth_token(self, timestamp, request_body="") -> str:
        digest = request_body + self.svea_basic_settings.secret_key + timestamp
        h = hashlib.sha512(digest.encode("utf-8")).hexdigest()
        auth = f"{self.svea_basic_settings.merchant_id}:{h}"
        token = base64.b64encode(f"{auth}".encode('ascii')).decode('ascii')
        return token

testInstance = TestClass()

url = "https://checkoutapistage.svea.com/api/orders/1"

my_sco_headers = testInstance.get_request_headers(extra_headers={'Connection':'keep-alive'})
#print(my_sco_headers)

# Send request
payload={}
response = requests.request("GET", url, headers=my_sco_headers, data=payload)
print(response.text)
response.raise_for_status()