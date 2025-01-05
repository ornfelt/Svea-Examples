#!/bin/bash

# Credentials
CHECKOUT_MERCHANT_ID_VALUE=""
CHECKOUT_SECRET_KEY_VALUE=""
#CHECKOUT_ORDER_TO_FETCH_VALUE="8906830" # Use some valid order for your merchant id
WEBPAY_CLIENT_ID_VALUE=""
WEBPAY_PASSWORD_VALUE=""
#WEBPAY_ORDER_TO_FETCH_VALUE="9731563" # Use some valid order for your client id
PG_MERCHANT_ID_VALUE=""
PG_SECRET_KEY_VALUE=""
PG_ORDER_TO_FETCH_VALUE="900497" # Use some valid order for your merchant id

process_files() {
    local folder=$1
    local pattern=$2
    local replacement=$3
    local extensions=("cs" "cpp" "fs" "go" "java" "js" "php" "py" "rs" "vb")

    for ext in "${extensions[@]}"; do
        find "$folder/" -type f -name "*.$ext" -exec sed -i "s/$pattern/$replacement/g" {} +
    done
}

if [[ $1 == "clean" ]]; then
    echo "Cleaning up credentials..."

    # Remove Checkout info
    process_files "Checkout" "$CHECKOUT_MERCHANT_ID_VALUE" "CHECKOUT_MERCHANT_ID"
    process_files "Checkout" "$CHECKOUT_SECRET_KEY_VALUE" "CHECKOUT_SECRET_KEY"
    #process_files "Checkout" "$CHECKOUT_ORDER_TO_FETCH_VALUE" "CHECKOUT_ORDER_TO_FETCH"

    # Remove Webpay info
    process_files "Webpay" "$WEBPAY_CLIENT_ID_VALUE" "WEBPAY_CLIENT_ID"
    process_files "Webpay" "$WEBPAY_PASSWORD_VALUE" "WEBPAY_PASSWORD"
    #process_files "Webpay" "$WEBPAY_ORDER_TO_FETCH_VALUE" "WEBPAY_ORDER_TO_FETCH"

    # Remove PaymentGateway info
    process_files "PaymentGateway" "$PG_MERCHANT_ID_VALUE" "PG_MERCHANT_ID"
    process_files "PaymentGateway" "$PG_SECRET_KEY_VALUE" "PG_SECRET_KEY"
    process_files "PaymentGateway" "$PG_ORDER_TO_FETCH_VALUE" "PG_ORDER_TO_FETCH"
else
    echo "Setting up credentials..."

    # Set Checkout info
    process_files "Checkout" "CHECKOUT_MERCHANT_ID" "$CHECKOUT_MERCHANT_ID_VALUE"
    process_files "Checkout" "CHECKOUT_SECRET_KEY" "$CHECKOUT_SECRET_KEY_VALUE"
    #process_files "Checkout" "CHECKOUT_ORDER_TO_FETCH" "$CHECKOUT_ORDER_TO_FETCH_VALUE"

    # Set Webpay info
    process_files "Webpay" "WEBPAY_CLIENT_ID" "$WEBPAY_CLIENT_ID_VALUE"
    process_files "Webpay" "WEBPAY_PASSWORD" "$WEBPAY_PASSWORD_VALUE"
    #process_files "Webpay" "WEBPAY_ORDER_TO_FETCH" "$WEBPAY_ORDER_TO_FETCH_VALUE"

    # Set PaymentGateway info
    process_files "PaymentGateway" "PG_MERCHANT_ID" "$PG_MERCHANT_ID_VALUE"
    process_files "PaymentGateway" "PG_SECRET_KEY" "$PG_SECRET_KEY_VALUE"
    process_files "PaymentGateway" "PG_ORDER_TO_FETCH" "$PG_ORDER_TO_FETCH_VALUE"
fi

