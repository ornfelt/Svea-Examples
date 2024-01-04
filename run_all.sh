#!/usr/bin/env bash

# Detecting OS
OS=$(uname -s)
if [[ "$OS" == "Linux" || "$OS" == "Darwin" ]]; then
    # Unix-like environment
    CXX=g++
    CPPFLAGS="-w -std=c++17 -lcurl -lcpprest -lcrypto -lssl"
else
    # Windows environment
    CXX="g++"
    CPPFLAGS="-w -std=c++17 -LC:\path\to\OpenSSL\lib -LC:\path\to\cpprestsdk\lib -lcrypto -lssl -lcpprest -lcurl"
fi

# Common compiler / interpreters
CS_COMPILER="dotnet"
FS_COMPILER="dotnet"
VB_COMPILER="dotnet"
GO_COMPILER="go"
JAVA_COMPILER="javac"
PY="python"
JS="node"
PHP="php"
CARGO="cargo" # rustc can also be used

# Compile and run C#
compile_and_run_csharp() {
    #csexec="Checkout/C#/test" && $CS_COMPILER "Checkout/C#/get_order.cs" -out:$csexec && mono $csexec

    cd "Checkout/C#/get_order" && $CS_COMPILER build > /dev/null && $CS_COMPILER run && cd - > /dev/null
    cd "Checkout/C#/create_order" && $CS_COMPILER build > /dev/null && $CS_COMPILER run && cd - > /dev/null

    cd "PaymentGateway/C#/pg_request" && $CS_COMPILER build > /dev/null && $CS_COMPILER run && cd - > /dev/null

    cd "Webpay/C#/get_order" && $CS_COMPILER build > /dev/null && $CS_COMPILER run && cd - > /dev/null
    cd "Webpay/C#/create_order" && $CS_COMPILER build > /dev/null && $CS_COMPILER run && cd - > /dev/null
}

# Compile and run C++
compile_and_run_cpp() {
    CPPEXEC="Checkout/C++/test"
    $CXX $CPPFLAGS Checkout/C++/get_order.cpp -o $CPPEXEC && $CPPEXEC
    CPPEXEC="test"
    cd "Checkout/C++" && $CXX $CPPFLAGS create_order.cpp -o $CPPEXEC && ./$CPPEXEC && cd - > /dev/null

    CPPEXEC="PaymentGateway/C++/test"
    $CXX $CPPFLAGS PaymentGateway/C++/pg_request.cpp -o $CPPEXEC && $CPPEXEC
    $CXX $CPPFLAGS PaymentGateway/C++/pg_create.cpp -o $CPPEXEC && $CPPEXEC

    CPPEXEC="Webpay/C++/test"
    $CXX $CPPFLAGS Webpay/C++/get_order.cpp -o $CPPEXEC && $CPPEXEC
    $CXX $CPPFLAGS Webpay/C++/create_order.cpp -o $CPPEXEC && $CPPEXEC
}

# Compile and run F#
compile_and_run_fsharp() {
    #F_EXEC="Checkout/F#/test.exe" && $FS_COMPILER "Checkout/F#/get_order.fs" -o $F_EXEC && mono $F_EXEC

    cd "Checkout/F#/get_order" && $FS_COMPILER build > /dev/null && $FS_COMPILER run && cd - > /dev/null
    cd "Checkout/F#/create_order" && $FS_COMPILER build > /dev/null && $FS_COMPILER run && cd - > /dev/null

    cd "PaymentGateway/F#/pg_request" && $FS_COMPILER build > /dev/null && $FS_COMPILER run && cd - > /dev/null

    cd "Webpay/F#/get_order" && $FS_COMPILER build > /dev/null && $FS_COMPILER run && cd - > /dev/null
    cd "Webpay/F#/create_order" && $FS_COMPILER build > /dev/null && $FS_COMPILER run && cd - > /dev/null
}

# Compile and run Go
compile_and_run_go() {
    GO_EXEC="Checkout/Go/test"
    $GO_COMPILER build -o $GO_EXEC Checkout/Go/get_order.go 2>/dev/null && ./$GO_EXEC
    cd "Checkout/Go" && $GO_COMPILER build -o $GO_EXEC create_order.go 2>/dev/null && ./$GO_EXEC && cd - > /dev/null

    GO_EXEC="PaymentGateway/Go/test"
    $GO_COMPILER build -o $GO_EXEC PaymentGateway/Go/pg_request.go 2>/dev/null && ./$GO_EXEC

    GO_EXEC="Webpay/Go/test"
    $GO_COMPILER build -o $GO_EXEC Webpay/Go/get_order.go 2>/dev/null && ./$GO_EXEC
    $GO_COMPILER build -o $GO_EXEC Webpay/Go/create_order.go 2>/dev/null && ./$GO_EXEC
}

# Compile and run Java
compile_and_run_java() {
    #java Checkout/Java/get_order.java 2>&1 | grep -v "warning"
    java Checkout/Java/get_order.java

    cd "Checkout/Java" && java create_order.java && cd - > /dev/null

    java PaymentGateway/Java/pg_request.java

    java Webpay/Java/get_order.java
    java Webpay/Java/create_order.java
}

# Run JavaScript
run_javascript() {
    $JS Checkout/Javascript/get_order.js
    cd "Checkout/Javascript" && $JS create_order.js && cd - > /dev/null

    $JS PaymentGateway/Javascript/pg_request.js

    $JS Webpay/Javascript/get_order.js
    $JS Webpay/Javascript/create_order.js
}

# Run PHP
run_php() {
    $PHP Checkout/Php/get_order.php
    cd "Checkout/Php" && $PHP create_order.php && cd - > /dev/null

    $PHP PaymentGateway/Php/pg_request.php

    $PHP Webpay/Php/get_order.php
    $PHP Webpay/Php/create_order.php
}

# Run Python
run_python() {
    $PY Checkout/Python/get_order.py
    cd "Checkout/Python" && $PY create_order.py && cd - > /dev/null

    $PY PaymentGateway/Python/pg_request.py

    $PY Webpay/Python/get_order.py
    $PY Webpay/Python/create_order.py
}

# Compile and run Rust
compile_and_run_rust() {
    cd Checkout/Rust/get_order && RUSTFLAGS="-A warnings" $CARGO build 2> /dev/null && RUSTFLAGS="-A warnings" $CARGO run 2> /dev/null && cd - > /dev/null
    cd Checkout/Rust/create_order && RUSTFLAGS="-A warnings" $CARGO build 2> /dev/null && RUSTFLAGS="-A warnings" $CARGO run 2> /dev/null && cd - > /dev/null

    cd PaymentGateway/Rust/pg_request && RUSTFLAGS="-A warnings" $CARGO build 2> /dev/null && RUSTFLAGS="-A warnings" $CARGO run 2> /dev/null && cd - > /dev/null

    cd Webpay/Rust/get_order && RUSTFLAGS="-A warnings" $CARGO build 2> /dev/null && RUSTFLAGS="-A warnings" $CARGO run 2> /dev/null && cd - > /dev/null
    cd Webpay/Rust/create_order && RUSTFLAGS="-A warnings" $CARGO build 2> /dev/null && RUSTFLAGS="-A warnings" $CARGO run 2> /dev/null && cd - > /dev/null
}

compile_and_run_vb() {
    cd "Checkout/VB/get_order" && $VB_COMPILER build > /dev/null && $VB_COMPILER run && cd - > /dev/null
    cd "Checkout/VB/create_order" && $VB_COMPILER build > /dev/null && $VB_COMPILER run && cd - > /dev/null

    cd "PaymentGateway/VB/pg_request" && $VB_COMPILER build > /dev/null && $VB_COMPILER run && cd - > /dev/null

    cd "Webpay/VB/get_order" && $VB_COMPILER build > /dev/null && $VB_COMPILER run && cd - > /dev/null
    cd "Webpay/VB/create_order" && $VB_COMPILER build > /dev/null && $VB_COMPILER run && cd - > /dev/null
}

# Running all
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

./setup.sh
#source setup.sh

# Check arguments
if [ $# -eq 0 ]; then
    # No arguments, run all languages
    compile_and_run_all
else
    # Loop through arguments and run the selected language(s)
    for lang in "$@"; do
        case $lang in
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
                echo "Unknown language: $lang"
                ;;
        esac
    done
fi

./setup.sh clean
