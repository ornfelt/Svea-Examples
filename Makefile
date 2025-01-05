# Run all:
# make

# Or run individual language(s):
# make cpp

ifeq ($(OS),Windows_NT)
    CXX=g++
    CPPFLAGS=-w -std=c++17 -LC:\path\to\OpenSSL\lib -LC:\path\to\cpprestsdk\lib -lcrypto -lssl -lcpprest -lcurl
else
    CXX=g++
    CPPFLAGS=-w -std=c++17 -lcurl -lcpprest -lcrypto -lssl
endif

# compilers / interpreters
CS_COMPILER=dotnet
FS_COMPILER=dotnet
VB_COMPILER=dotnet
GO_COMPILER=go
JAVA_COMPILER=javac
PY=python
JS=node
PHP=php
CARGO=cargo # rustc can also be used

.PHONY: all do-setup csharp cpp fsharp go java javascript php python rust vb do-cleanup

all: do-setup csharp cpp fsharp go java javascript php python rust vb do-cleanup

do-setup:
	#@./setup.sh
	@./setup_local.sh

do-cleanup:
	#@./setup.sh clean
	@./setup_local.sh clean

csharp:
	@cd "Checkout/C#/create_order" && $(CS_COMPILER) build > /dev/null && $(CS_COMPILER) run && cd - > /dev/null
	@cd "Checkout/C#/get_order" && $(CS_COMPILER) build > /dev/null && $(CS_COMPILER) run && cd - > /dev/null
	@cd "PaymentGateway/C#/pg_request" && $(CS_COMPILER) build > /dev/null && $(CS_COMPILER) run && cd - > /dev/null
	@cd "Webpay/C#/create_order" && $(CS_COMPILER) build > /dev/null && $(CS_COMPILER) run && cd - > /dev/null
	@cd "Webpay/C#/get_order" && $(CS_COMPILER) build > /dev/null && $(CS_COMPILER) run && cd - > /dev/null

cpp:
	@cd "Checkout/C++" && $(CXX) $(CPPFLAGS) create_order.cpp -o test && ./test && cd - > /dev/null
	@cd "Checkout/C++" && $(CXX) $(CPPFLAGS) get_order.cpp -o test && ./test && cd - > /dev/null
	@cd "PaymentGateway/C++" && $(CXX) $(CPPFLAGS) pg_create.cpp -o test && ./test && cd - > /dev/null
	@cd "PaymentGateway/C++" && $(CXX) $(CPPFLAGS) pg_request.cpp -o test && ./test && cd - > /dev/null
	@cd "Webpay/C++" && $(CXX) $(CPPFLAGS) create_order.cpp -o test && ./test && cd - > /dev/null
	@cd "Webpay/C++" && $(CXX) $(CPPFLAGS) get_order.cpp -o test && ./test && cd - > /dev/null

fsharp:
	@cd "Checkout/F#/create_order" && $(FS_COMPILER) build > /dev/null && $(FS_COMPILER) run && cd - > /dev/null
	@cd "Checkout/F#/get_order" && $(FS_COMPILER) build > /dev/null && $(FS_COMPILER) run && cd - > /dev/null
	@cd "PaymentGateway/F#/pg_request" && $(FS_COMPILER) build > /dev/null && $(FS_COMPILER) run && cd - > /dev/null
	@cd "Webpay/F#/create_order" && $(FS_COMPILER) build > /dev/null && $(FS_COMPILER) run && cd - > /dev/null
	@cd "Webpay/F#/get_order" && $(FS_COMPILER) build > /dev/null && $(FS_COMPILER) run && cd - > /dev/null

go:
	@cd "Checkout/Go" && $(GO_COMPILER) build -o test create_order.go 2>/dev/null && ./test && cd - > /dev/null
	@cd "Checkout/Go" && $(GO_COMPILER) build -o test get_order.go 2>/dev/null && ./test && cd - > /dev/null
	@cd "PaymentGateway/Go" && $(GO_COMPILER) build -o test pg_request.go 2>/dev/null && ./test && cd - > /dev/null
	@cd "Webpay/Go" && $(GO_COMPILER) build -o test create_order.go 2>/dev/null && ./test && cd - > /dev/null
	@cd "Webpay/Go" && $(GO_COMPILER) build -o test get_order.go 2>/dev/null && ./test && cd - > /dev/null

java:
	@cd "Checkout/Java" && java create_order.java && cd - > /dev/null
	@cd "Checkout/Java" && java get_order.java && cd - > /dev/null
	@cd "PaymentGateway/Java" && java pg_request.java && cd - > /dev/null
	@cd "Webpay/Java" && java create_order.java && cd - > /dev/null
	@cd "Webpay/Java" && java get_order.java && cd - > /dev/null

javascript:
	@cd "Checkout/Javascript" && $(JS) create_order.js && cd - > /dev/null
	@cd "Checkout/Javascript" && $(JS) get_order.js && cd - > /dev/null
	@cd "PaymentGateway/Javascript" && $(JS) pg_request.js && cd - > /dev/null
	@cd "Webpay/Javascript" && $(JS) create_order.js && cd - > /dev/null
	@cd "Webpay/Javascript" && $(JS) get_order.js && cd - > /dev/null

php:
	@cd "Checkout/Php" && $(PHP) create_order.php && cd - > /dev/null
	@cd "Checkout/Php" && $(PHP) get_order.php && cd - > /dev/null
	@cd "PaymentGateway/Php" && $(PHP) pg_request.php && cd - > /dev/null
	@cd "Webpay/Php" && $(PHP) create_order.php && cd - > /dev/null
	@cd "Webpay/Php" && $(PHP) get_order.php && cd - > /dev/null

python:
	@cd "Checkout/Python" && $(PY) create_order.py && cd - > /dev/null
	@cd "Checkout/Python" && $(PY) get_order.py && cd - > /dev/null
	@cd "PaymentGateway/Python" && $(PY) pg_request.py && cd - > /dev/null
	@cd "Webpay/Python" && $(PY) create_order.py && cd - > /dev/null
	@cd "Webpay/Python" && $(PY) get_order.py && cd - > /dev/null

rust:
	@cd "Checkout/Rust/create_order" && RUSTFLAGS="-A warnings" $(CARGO) build 2> /dev/null && RUSTFLAGS="-A warnings" $(CARGO) run 2> /dev/null && cd - > /dev/null
	@cd "Checkout/Rust/get_order" && RUSTFLAGS="-A warnings" $(CARGO) build 2> /dev/null && RUSTFLAGS="-A warnings" $(CARGO) run 2> /dev/null && cd - > /dev/null
	@cd "PaymentGateway/Rust/pg_request" && RUSTFLAGS="-A warnings" $(CARGO) build 2> /dev/null && RUSTFLAGS="-A warnings" $(CARGO) run 2> /dev/null && cd - > /dev/null
	@cd "Webpay/Rust/create_order" && RUSTFLAGS="-A warnings" $(CARGO) build 2> /dev/null && RUSTFLAGS="-A warnings" $(CARGO) run 2> /dev/null && cd - > /dev/null
	@cd "Webpay/Rust/get_order" && RUSTFLAGS="-A warnings" $(CARGO) build 2> /dev/null && RUSTFLAGS="-A warnings" $(CARGO) run 2> /dev/null && cd - > /dev/null

vb:
	@cd "Checkout/VB/create_order" && $(VB_COMPILER) build > /dev/null && $(VB_COMPILER) run && cd - > /dev/null
	@cd "Checkout/VB/get_order" && $(VB_COMPILER) build > /dev/null && $(VB_COMPILER) run && cd - > /dev/null
	@cd "PaymentGateway/VB/pg_request" && $(VB_COMPILER) build > /dev/null && $(VB_COMPILER) run && cd - > /dev/null
	@cd "Webpay/VB/create_order" && $(VB_COMPILER) build > /dev/null && $(VB_COMPILER) run && cd - > /dev/null
	@cd "Webpay/VB/get_order" && $(VB_COMPILER) build > /dev/null && $(VB_COMPILER) run && cd - > /dev/null
