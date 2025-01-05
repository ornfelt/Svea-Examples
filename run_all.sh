#!/usr/bin/env bash

OS=$(uname -s)
if [[ "$OS" == "Linux" || "$OS" == "Darwin" ]]; then
    # unix-like
    CXX=g++
    CPPFLAGS="-w -std=c++17 -lcurl -lcpprest -lcrypto -lssl"
else
    # Windows env
    CXX="g++"
    CPPFLAGS="-w -std=c++17 -LC:\path\to\OpenSSL\lib -LC:\path\to\cpprestsdk\lib -lcrypto -lssl -lcpprest -lcurl"
fi

# compilers / interpreters
CS_COMPILER="dotnet"
FS_COMPILER="dotnet"
VB_COMPILER="dotnet"
GO_COMPILER="go"
JAVA_COMPILER="javac"
PY="python"
JS="node"
PHP="php"
CARGO="cargo" # rustc can also be used

compile_and_run_csharp() {
    cd "Checkout/C#/create_order" && $CS_COMPILER build > /dev/null && $CS_COMPILER run && cd - > /dev/null
    cd "Checkout/C#/get_order" && $CS_COMPILER build > /dev/null && $CS_COMPILER run && cd - > /dev/null
    cd "PaymentGateway/C#/pg_request" && $CS_COMPILER build > /dev/null && $CS_COMPILER run && cd - > /dev/null
    cd "Webpay/C#/create_order" && $CS_COMPILER build > /dev/null && $CS_COMPILER run && cd - > /dev/null
    cd "Webpay/C#/get_order" && $CS_COMPILER build > /dev/null && $CS_COMPILER run && cd - > /dev/null
}

compile_and_run_cpp() {
    cd "Checkout/C++" && $CXX $CPPFLAGS create_order.cpp -o test && ./test && cd - > /dev/null
    cd "Checkout/C++" && $CXX $CPPFLAGS get_order.cpp -o test && ./test && cd - > /dev/null
    cd "PaymentGateway/C++" && $CXX $CPPFLAGS pg_create.cpp -o test && ./test && cd - > /dev/null
    cd "PaymentGateway/C++" && $CXX $CPPFLAGS pg_request.cpp -o test && ./test && cd - > /dev/null
    cd "Webpay/C++" && $CXX $CPPFLAGS create_order.cpp -o test && ./test && cd - > /dev/null
    cd "Webpay/C++" && $CXX $CPPFLAGS get_order.cpp -o test && ./test && cd - > /dev/null
}

compile_and_run_fsharp() {
    cd "Checkout/F#/create_order" && $FS_COMPILER build > /dev/null && $FS_COMPILER run && cd - > /dev/null
    cd "Checkout/F#/get_order" && $FS_COMPILER build > /dev/null && $FS_COMPILER run && cd - > /dev/null
    cd "PaymentGateway/F#/pg_request" && $FS_COMPILER build > /dev/null && $FS_COMPILER run && cd - > /dev/null
    cd "Webpay/F#/create_order" && $FS_COMPILER build > /dev/null && $FS_COMPILER run && cd - > /dev/null
    cd "Webpay/F#/get_order" && $FS_COMPILER build > /dev/null && $FS_COMPILER run && cd - > /dev/null
}

compile_and_run_go() {
    cd "Checkout/Go" && $GO_COMPILER build -o test create_order.go 2>/dev/null && ./test && cd - > /dev/null
    cd "Checkout/Go" && $GO_COMPILER build -o test get_order.go 2>/dev/null && ./test && cd - > /dev/null
    cd "PaymentGateway/Go" && $GO_COMPILER build -o test pg_request.go 2>/dev/null && ./test && cd - > /dev/null
    cd "Webpay/Go" && $GO_COMPILER build -o test create_order.go 2>/dev/null && ./test && cd - > /dev/null
    cd "Webpay/Go" && $GO_COMPILER build -o test get_order.go 2>/dev/null && ./test && cd - > /dev/null
}

compile_and_run_java() {
    cd "Checkout/Java" && java create_order.java && cd - > /dev/null
    cd "Checkout/Java" && java get_order.java && cd - > /dev/null
    cd "PaymentGateway/Java" && java pg_request.java && cd - > /dev/null
    cd "Webpay/Java" && java create_order.java && cd - > /dev/null
    cd "Webpay/Java" && java get_order.java && cd - > /dev/null
}

run_javascript() {
    cd "Checkout/Javascript" && $JS create_order.js && cd - > /dev/null
    cd "Checkout/Javascript" && $JS get_order.js && cd - > /dev/null
    cd "PaymentGateway/Javascript" && $JS pg_request.js && cd - > /dev/null
    cd "Webpay/Javascript" && $JS create_order.js && cd - > /dev/null
    cd "Webpay/Javascript" && $JS get_order.js && cd - > /dev/null
}

run_php() {
    cd "Checkout/Php" && $PHP create_order.php && cd - > /dev/null
    cd "Checkout/Php" && $PHP get_order.php && cd - > /dev/null
    cd "PaymentGateway/Php" && $PHP pg_request.php && cd - > /dev/null
    cd "Webpay/Php" && $PHP create_order.php && cd - > /dev/null
    cd "Webpay/Php" && $PHP get_order.php && cd - > /dev/null
}

run_python() {
    cd "Checkout/Python" && $PY create_order.py && cd - > /dev/null
    cd "Checkout/Python" && $PY get_order.py && cd - > /dev/null
    cd "PaymentGateway/Python" && $PY pg_request.py && cd - > /dev/null
    cd "Webpay/Python" && $PY create_order.py && cd - > /dev/null
    cd "Webpay/Python" && $PY get_order.py && cd - > /dev/null
}

compile_and_run_rust() {
    cd "Checkout/Rust/create_order" && RUSTFLAGS="-A warnings" $CARGO build 2> /dev/null && RUSTFLAGS="-A warnings" $CARGO run 2> /dev/null && cd - > /dev/null
    cd "Checkout/Rust/get_order" && RUSTFLAGS="-A warnings" $CARGO build 2> /dev/null && RUSTFLAGS="-A warnings" $CARGO run 2> /dev/null && cd - > /dev/null
    cd "PaymentGateway/Rust/pg_request" && RUSTFLAGS="-A warnings" $CARGO build 2> /dev/null && RUSTFLAGS="-A warnings" $CARGO run 2> /dev/null && cd - > /dev/null
    cd "Webpay/Rust/create_order" && RUSTFLAGS="-A warnings" $CARGO build 2> /dev/null && RUSTFLAGS="-A warnings" $CARGO run 2> /dev/null && cd - > /dev/null
    cd "Webpay/Rust/get_order" && RUSTFLAGS="-A warnings" $CARGO build 2> /dev/null && RUSTFLAGS="-A warnings" $CARGO run 2> /dev/null && cd - > /dev/null
}

compile_and_run_vb() {
    cd "Checkout/VB/create_order" && $VB_COMPILER build > /dev/null && $VB_COMPILER run && cd - > /dev/null
    cd "Checkout/VB/get_order" && $VB_COMPILER build > /dev/null && $VB_COMPILER run && cd - > /dev/null
    cd "PaymentGateway/VB/pg_request" && $VB_COMPILER build > /dev/null && $VB_COMPILER run && cd - > /dev/null
    cd "Webpay/VB/create_order" && $VB_COMPILER build > /dev/null && $VB_COMPILER run && cd - > /dev/null
    cd "Webpay/VB/get_order" && $VB_COMPILER build > /dev/null && $VB_COMPILER run && cd - > /dev/null
}

compile_and_run_all() {
    compile_and_run_csharp
    compile_and_run_cpp
    compile_and_run_fsharp
    compile_and_run_go
    compile_and_run_java
    run_javascript
    run_php
    run_python
    compile_and_run_rust
    compile_and_run_vb
}

if [ -f "./setup_local.sh" ]; then
    ./setup_local.sh
else
    #source setup.sh
    ./setup.sh
fi

normalize_language() {
    case "${1,,}" in
        js) echo "javascript" ;;
        cs|c#) echo "csharp" ;;
        cpp|c++) echo "cpp" ;;
        fs|f#) echo "fsharp" ;;
        vb|vb.net) echo "vb" ;;
        py) echo "python" ;;
        go) echo "go" ;;
        java) echo "java" ;;
        php) echo "php" ;;
        rust) echo "rust" ;;
        *) echo "$1" ;; # Return as-is for unsupported args
    esac
}

run_language() {
    case $1 in
        csharp)
            compile_and_run_csharp
            ;;
        cpp)
            compile_and_run_cpp
            ;;
        fsharp)
            compile_and_run_fsharp
            ;;
        go)
            compile_and_run_go
            ;;
        java)
            compile_and_run_java
            ;;
        javascript)
            run_javascript
            ;;
        php)
            run_php
            ;;
        python)
            run_python
            ;;
        rust)
            compile_and_run_rust
            ;;
        vb)
            compile_and_run_vb
            ;;
        *)
            echo "Unknown language: $1"
            ;;
    esac
}

# Check args
if [ $# -eq 0 ]; then
    # No arguments, run all
    compile_and_run_all
else
    for lang in "$@"; do
        normalized_lang=$(normalize_language "$lang")
        run_language "$normalized_lang"
    done
fi

if [ -f "./setup_local.sh" ]; then
    ./setup_local.sh clean
else
    ./setup.sh clean
fi

