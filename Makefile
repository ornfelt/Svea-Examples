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

# Common compiler / interpreters
CS_COMPILER=dotnet
FS_COMPILER=dotnet
VB_COMPILER=dotnet
GO_COMPILER=go
JAVA_COMPILER=javac
PY=python
JS=node
PHP=php
CARGO=cargo # rustc can also be used

.PHONY: all csharp cpp fsharp go java javascript php python rust vb

all: csharp cpp fsharp go java javascript php python rust vb

csharp:
	@$(CS_COMPILER) build > /dev/null "Checkout/C#/get_order/get_order.csproj" && $(CS_COMPILER) run --project "Checkout/C#/get_order/get_order.csproj"
	@cd "Checkout/C#/create_order" && $(CS_COMPILER) build > /dev/null && $(CS_COMPILER) run && cd - > /dev/null

	@$(CS_COMPILER) build > /dev/null "PaymentGateway/C#/pg_request/pg_request.csproj" && $(CS_COMPILER) run --project "PaymentGateway/C#/pg_request/pg_request.csproj"

	@$(CS_COMPILER) build > /dev/null "Webpay/C#/get_order/get_order.csproj" && $(CS_COMPILER) run --project "Webpay/C#/get_order/get_order.csproj"
	@$(CS_COMPILER) build > /dev/null "Webpay/C#/create_order/create_order.csproj" && $(CS_COMPILER) run --project "Webpay/C#/create_order/create_order.csproj"

cpp:
	@$(CXX) $(CPPFLAGS) Checkout/C++/get_order.cpp -o Checkout/C++/get_order && Checkout/C++/get_order
	@cd "Checkout/C++" && $(CXX) $(CPPFLAGS) create_order.cpp -o create_order && ./create_order && cd - > /dev/null

	@$(CXX) $(CPPFLAGS) PaymentGateway/C++/pg_request.cpp -o PaymentGateway/C++/pg_request && PaymentGateway/C++/pg_request
	@$(CXX) $(CPPFLAGS) PaymentGateway/C++/pg_create.cpp -o PaymentGateway/C++/pg_create && PaymentGateway/C++/pg_create

	@$(CXX) $(CPPFLAGS) Webpay/C++/get_order.cpp -o Webpay/C++/get_order && Webpay/C++/get_order
	@$(CXX) $(CPPFLAGS) Webpay/C++/create_order.cpp -o Webpay/C++/create_order && Webpay/C++/create_order

fsharp:
	@$(FS_COMPILER) build > /dev/null "Checkout/F#/get_order/get_order.fsproj" && $(FS_COMPILER) run --project "Checkout/F#/get_order/get_order.fsproj"
	@cd "Checkout/F#/create_order" && $(FS_COMPILER) build > /dev/null && $(FS_COMPILER) run && cd - > /dev/null

	@$(FS_COMPILER) build > /dev/null "PaymentGateway/F#/pg_request/pg_request.fsproj" && $(FS_COMPILER) run --project "PaymentGateway/F#/pg_request/pg_request.fsproj"

	@$(FS_COMPILER) build > /dev/null "Webpay/F#/get_order/get_order.fsproj" && $(FS_COMPILER) run --project "Webpay/F#/get_order/get_order.fsproj"
	@$(FS_COMPILER) build > /dev/null "Webpay/F#/create_order/create_order.fsproj" && $(FS_COMPILER) run --project "Webpay/F#/create_order/create_order.fsproj"

go:
	@$(GO_COMPILER) build -o Checkout/Go/get_order Checkout/Go/get_order.go 2>/dev/null && Checkout/Go/get_order
	@cd "Checkout/Go" && $(GO_COMPILER) build -o create_order create_order.go 2>/dev/null && ./create_order && cd - > /dev/null 

	@$(GO_COMPILER) build -o PaymentGateway/Go/pg_request PaymentGateway/Go/pg_request.go 2>/dev/null && PaymentGateway/Go/pg_request

	@$(GO_COMPILER) build -o Webpay/Go/get_order Webpay/Go/get_order.go 2>/dev/null && Webpay/Go/get_order
	@$(GO_COMPILER) build -o Webpay/Go/create_order Webpay/Go/create_order.go 2>/dev/null && Webpay/Go/create_order

java:
	@java Checkout/Java/get_order.java
	@cd "Checkout/Java" && java create_order.java && cd - > /dev/null

	@java PaymentGateway/Java/pg_request.java

	@java Webpay/Java/get_order.java
	@java Webpay/Java/create_order.java

javascript:
	@$(JS) Checkout/Javascript/get_order.js
	@cd "Checkout/Javascript" && $(JS) create_order.js && cd - > /dev/null

	@$(JS) PaymentGateway/Javascript/pg_request.js

	@$(JS) Webpay/Javascript/get_order.js
	@$(JS) Webpay/Javascript/create_order.js

php:
	@$(PHP) Checkout/Php/get_order.php
	@cd "Checkout/Php" && $(PHP) create_order.php && cd - > /dev/null

	@$(PHP) PaymentGateway/Php/pg_request.php

	@$(PHP) Webpay/Php/get_order.php
	@$(PHP) Webpay/Php/create_order.php

python:
	@$(PY) Checkout/Python/get_order.py
	@cd Checkout/Python && $(PY) create_order.py && cd - > /dev/null

	@$(PY) PaymentGateway/Python/pg_request.py

	@$(PY) Webpay/Python/get_order.py
	@$(PY) Webpay/Python/create_order.py

rust:
	@cd Checkout/Rust/get_order && RUSTFLAGS="-A warnings" $(CARGO) build 2> /dev/null && RUSTFLAGS="-A warnings" $(CARGO) run 2> /dev/null && cd - > /dev/null
	@cd Checkout/Rust/create_order && RUSTFLAGS="-A warnings" $(CARGO) build 2> /dev/null && RUSTFLAGS="-A warnings" $(CARGO) run 2> /dev/null && cd - > /dev/null

	@cd PaymentGateway/Rust/pg_request && RUSTFLAGS="-A warnings" $(CARGO) build 2> /dev/null && RUSTFLAGS="-A warnings" $(CARGO) run 2> /dev/null && cd - > /dev/null

	@cd Webpay/Rust/get_order && RUSTFLAGS="-A warnings" $(CARGO) build 2> /dev/null && RUSTFLAGS="-A warnings" $(CARGO) run 2> /dev/null && cd - > /dev/null
	@cd Webpay/Rust/create_order && RUSTFLAGS="-A warnings" $(CARGO) build 2> /dev/null && RUSTFLAGS="-A warnings" $(CARGO) run 2> /dev/null && cd - > /dev/null

vb:
	@$(VB_COMPILER) build > /dev/null "Checkout/VB/get_order/get_order.vbproj" && $(VB_COMPILER) run --project "Checkout/VB/get_order/get_order.vbproj"
	@cd "Checkout/VB/create_order" && $(VB_COMPILER) build > /dev/null && $(VB_COMPILER) run && cd - > /dev/null

	@$(VB_COMPILER) build > /dev/null "PaymentGateway/VB/pg_request/pg_request.vbproj" && $(VB_COMPILER) run --project "PaymentGateway/VB/pg_request/pg_request.vbproj"

	@$(VB_COMPILER) build > /dev/null "Webpay/VB/get_order/get_order.vbproj" && $(VB_COMPILER) run --project "Webpay/VB/get_order/get_order.vbproj"
	@$(VB_COMPILER) build > /dev/null "Webpay/VB/create_order/create_order.vbproj" && $(VB_COMPILER) run --project "Webpay/VB/create_order/create_order.vbproj"
