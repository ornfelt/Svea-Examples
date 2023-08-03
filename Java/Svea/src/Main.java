public class Main {
    public static void main(String[] args){

    	// Test PaymentGateway request
    	PaymentGateWayRequest pg = new PaymentGateWayRequest();
    	pg.makePostRequest();
    	
    	// Test Checkout requests
    	CheckoutApiClientRF checkoutClient = new CheckoutApiClientRF();
    	checkoutClient.runTests();

    	// Test Payment Admin requests
    	PmtApiClientRF paClient = new PmtApiClientRF();
    	paClient.runTests();
    }
}