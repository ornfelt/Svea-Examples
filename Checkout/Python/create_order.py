import asyncio
import base64
from datetime import datetime
import hashlib
import httpx
import json
import random

class SveaAuth:
    @staticmethod
    async def send_request():
        print("Running Create request for Checkout (Python)")
        async with httpx.AsyncClient() as client:
            url = "https://checkoutapistage.svea.com/api/orders"
            random_order_id = ''.join(random.choices('0123456789', k=8))

            #body = {
            #    "countryCode": "SE",
            #    "currency": "SEK",
            #    "locale": "sv-SE",
            #    "clientOrderNumber": "my_order_id",
            #    "merchantSettings": {
            #        "CheckoutValidationCallBackUri": "https://your.domain/validation-callback/{checkout.order.uri}",
            #        "PushUri": "https://your.domain/push-callback/{checkout.order.uri}",
            #        "TermsUri": "https://your.domain/terms/",
            #        "CheckoutUri": "https://your.domain/checkout-callback/",
            #        "ConfirmationUri": "https://your.domain/confirmation-callback/",
            #        "ActivePartPaymentCampaigns": None,
            #        "PromotedPartPaymentCampaign": 0
            #    },
            #    "cart": {
            #        "Items": [
            #            {
            #                "ArticleNumber": "ABC80",
            #                "Name": "Computer",
            #                "Quantity": 300,
            #                "UnitPrice": 500000,
            #                "DiscountPercent": 1000,
            #                "VatPercent": 2500,
            #                "Unit": None,
            #                "TemporaryReference": None,
            #                "RowNumber": 1,
            #                "MerchantData": None
            #            },
            #            {
            #                "ArticleNumber": "ABC81",
            #                "Name": "AnotherComputer",
            #                "Quantity": 200,
            #                "UnitPrice": 400000,
            #                "DiscountAmount": 10000,
            #                "VatPercent": 2500,
            #                "Unit": None,
            #                "TemporaryReference": None,
            #                "RowNumber": 2,
            #                "MerchantData": None
            #            }
            #        ]
            #    },
            #    "presetValues": [
            #        {
            #            "TypeName": "EmailAddress",
            #            "Value": "test.person@svea.com",
            #            "IsReadonly": True
            #        }
            #    ],
            #    "identityFlags": None,
            #    "requireElectronicIdAuthentication": False,
            #    "partnerKey": None,
            #    "merchantData": None
            #}

            # Attempt to read the JSON payload from file
            try:
                with open('create_order_payload.json', 'r') as file:
                    #body = json.load(file)
                    body_str = file.read()
                    # Replace the placeholder with the random order ID
                    body_str = body_str.replace("my_order_id", random_order_id)
                    body = json.loads(body_str)
            except FileNotFoundError:
                print("Error: File not found.")
                return
            except json.JSONDecodeError:
                print("Error: File is not a valid JSON.")
                return

            headers = await SveaAuth.set_http_request_headers(client, "", json.dumps(body))
            response = await client.post(url, json=body, headers=headers)
            #print("response:", response)
            #print("response body:", response.text)
            if response.status_code == 200 or response.status_code == 201:
                print("Success!")
            else:
                print("Failed...")
        print("----------------------------------------------------------")

    @staticmethod
    async def set_http_request_headers(client, operation, request_message):
        client.timeout = 30
        timestamp = datetime.utcnow().strftime("%Y-%m-%d %H:%M:%S")
        token = SveaAuth.create_authentication_token(request_message, timestamp)
        return {
            "Authorization": "Svea " + token,
            "Timestamp": timestamp,
            "Content-Type": "application/json; charset=utf-8"
        }

    @staticmethod
    def create_authentication_token(request_message, timestamp):
        merchant_id = "124842"
        secret_key = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW"
        sha512 = hashlib.sha512()
        sha512.update((request_message + secret_key + timestamp).encode('utf-8'))
        hash_string = sha512.hexdigest()
        auth_token = base64.b64encode((merchant_id + ":" + hash_string).encode('utf-8')).decode('utf-8')
        return auth_token

async def main():
    await SveaAuth.send_request()

asyncio.run(main())
