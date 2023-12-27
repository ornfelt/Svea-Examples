import asyncio
import requests
import base64
import hashlib
from xml.etree import ElementTree

class pg_request:

    def __init__(self):
        self._http_client = requests.Session()

    async def make_post_request_async(self):
        message_xml = """<?xml version="1.0" encoding="UTF-8"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>125123123</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>"""

        encoded_message = base64.b64encode(message_xml.encode('utf-8')).decode('utf-8')
        secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d"
        mac = self.get_sha512_hash(encoded_message + secret)

        print("mac:", mac)
        print("encodedMessage:", encoded_message)

        try:
            url = "https://webpaypaymentgatewaystage.svea.com/webpay/payment"
            content = {
                "merchantid": "1200",
                "message": encoded_message,
                "mac": mac
            }
            response = self._http_client.post(url, data=content)
            print("Response status code:", response.status_code)
            print("Response message:", response.text)
        except Exception as e:
            print(e)

    async def make_get_query_transaction_id_request_async(self):
        message_xml = """<?xml version="1.0" encoding="UTF-8"?><query><transactionid>900497</transactionid></query>"""

        encoded_message = base64.b64encode(message_xml.encode('utf-8')).decode('utf-8')
        secret = "27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d"
        mac = self.get_sha512_hash(encoded_message + secret)

        print("mac:", mac)
        print("encodedMessage:", encoded_message)

        try:
            url = "https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid"
            content = {
                "merchantid": "1200",
                "message": encoded_message,
                "mac": mac
            }
            response = self._http_client.post(url, data=content)

            print("Response status code:", response.status_code)
            print("Response message:", response.text)

            # Parsing the XML to find the 'message' element
            root = ElementTree.fromstring(response.text)
            message_element = root.find('.//message')
            if message_element is not None:
                encoded_message_part = message_element.text

                # Decoding the Base64 message part
                decoded_bytes = base64.b64decode(encoded_message_part)
                decoded_string = decoded_bytes.decode('utf-8')
                print("Decoded message:")
                print(decoded_string)
            else:
                print("Message element not found in response")
        except Exception as e:
            print(e)

    def get_sha512_hash(self, input_str):
        hash_obj = hashlib.sha512()
        hash_obj.update(input_str.encode('utf-8'))
        return hash_obj.hexdigest()

async def main():
    tester = pg_request()
    await tester.make_post_request_async()
    await tester.make_get_query_transaction_id_request_async()

if __name__ == "__main__":
    asyncio.run(main())
