#!/bin/bash

# Set credentials and some valid Order IDs
CHECKOUT_MERCHANT_ID_VALUE="124842"
CHECKOUT_SECRET_KEY_VALUE="1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW"
CHECKOUT_ORDER_TO_FETCH_VALUE="8906830"
WEBPAY_CLIENT_ID_VALUE="79021"
WEBPAY_PASSWORD_VALUE="sverigetest"
WEBPAY_ORDER_TO_FETCH_VALUE="9731563"
PG_MERCHANT_ID_VALUE="1200"
PG_SECRET_KEY_VALUE="27f18bfcbe4d7f39971cb3460fbe7234a82fb48f985cf22a068fa1a685fe7e6f93c7d0d92fee4e8fd7dc0c9f11e2507300e675220ee85679afa681407ee2416d"
PG_ORDER_TO_FETCH_VALUE="900497"

process_files() {
    local folder=$1
    local pattern=$2
    local replacement=$3
    # Expanded list of extensions
    local extensions=("cs" "cpp" "fs" "go" "java" "js" "php" "py" "rs" "vb")

    for ext in "${extensions[@]}"; do
        find "$folder/" -type f -name "*.$ext" -exec sed -i "s/$pattern/$replacement/g" {} +
    done
}

if [[ $1 == "clean" ]]; then
    echo "Cleaning up credentials and order IDs..."
    # Remove Checkout info
    process_files "Checkout" "$CHECKOUT_MERCHANT_ID_VALUE" "CHECKOUT_MERCHANT_ID"
    process_files "Checkout" "$CHECKOUT_SECRET_KEY_VALUE" "CHECKOUT_SECRET_KEY"
    process_files "Checkout" "$CHECKOUT_ORDER_TO_FETCH_VALUE" "CHECKOUT_ORDER_TO_FETCH"
    # Remove Webpay info
    process_files "Webpay" "$WEBPAY_CLIENT_ID_VALUE" "WEBPAY_CLIENT_ID"
    process_files "Webpay" "$WEBPAY_PASSWORD_VALUE" "WEBPAY_PASSWORD"
    process_files "Webpay" "$WEBPAY_ORDER_TO_FETCH_VALUE" "WEBPAY_ORDER_TO_FETCH"
    # Remove PaymentGateway info
    process_files "PaymentGateway" "$PG_MERCHANT_ID_VALUE" "PG_MERCHANT_ID"
    process_files "PaymentGateway" "$PG_SECRET_KEY_VALUE" "PG_SECRET_KEY"
    process_files "PaymentGateway" "$PG_ORDER_TO_FETCH_VALUE" "PG_ORDER_TO_FETCH"
else
    echo "Setting up credentials and order IDs..."
    # Set Checkout info
    process_files "Checkout" "CHECKOUT_MERCHANT_ID" "$CHECKOUT_MERCHANT_ID_VALUE"
    process_files "Checkout" "CHECKOUT_SECRET_KEY" "$CHECKOUT_SECRET_KEY_VALUE"
    process_files "Checkout" "CHECKOUT_ORDER_TO_FETCH" "$CHECKOUT_ORDER_TO_FETCH_VALUE"
    # Set Webpay info
    process_files "Webpay" "WEBPAY_CLIENT_ID" "$WEBPAY_CLIENT_ID_VALUE"
    process_files "Webpay" "WEBPAY_PASSWORD" "$WEBPAY_PASSWORD_VALUE"
    process_files "Webpay" "WEBPAY_ORDER_TO_FETCH" "$WEBPAY_ORDER_TO_FETCH_VALUE"
    # Set PaymentGateway info
    process_files "PaymentGateway" "PG_MERCHANT_ID" "$PG_MERCHANT_ID_VALUE"
    process_files "PaymentGateway" "PG_SECRET_KEY" "$PG_SECRET_KEY_VALUE"
    process_files "PaymentGateway" "PG_ORDER_TO_FETCH" "$PG_ORDER_TO_FETCH_VALUE"
fi
