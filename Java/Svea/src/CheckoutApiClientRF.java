import java.io.File;
import java.net.URL;
import java.text.DateFormat;
import java.text.SimpleDateFormat;

import okhttp3.ResponseBody;

import retrofit2.Call;
import retrofit2.Response;
import retrofit2.Retrofit;
import retrofit2.converter.scalars.ScalarsConverterFactory;

public class CheckoutApiClientRF {

	public static DateFormat dfmt = new SimpleDateFormat("yyyy-MM-dd");
	
	private String merchantId;
	private String secretWord;
	private String serverName;
	
	private	Retrofit	retroFit = null;
	private CheckoutApiService service = null;
	
	public void runTests() {
		init();
		testGetOrder();
		System.out.println("\n---------------------------------------------\n");
		testCreateOrder();
	}
	
	// Try to fetch order
	public void testGetOrder() {
		System.out.println("Trying to fetch order!");
		try {
			String result = getOrder(8909760L);
			
			if (result!=null) {
				Order o = getOrderFromString(result);
				System.out.println("CheckoutOrderNumber: " + o.getOrderId());
				System.out.println("ClientOrderNumber: " + o.getClientOrderNumber());
			}
			System.out.println("Get Order result: " + result);
			
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	// Try to create order
	public void testCreateOrder() {
		System.out.println("Trying to create order!");
		try {
			MerchantSettings ms = new MerchantSettings();
			ms.setTermsUri("http://webshop.se/terms");
			ms.setCheckoutUri("http://webshop.se/checkout");
			ms.setConfirmationUri("http://webshop.se/confirm");
			ms.setPushUri("https://webshop.se/svea/checkoutpush/{checkout.order.uri}");
			
			Cart cart = new Cart();
			CartItem item = new CartItem();
			item.setArticleNumber("100");
			item.setName("Röda skor");
			item.setQuantity(1);
			item.setUnit("par");
			item.setUnitPrice(10000);
			item.setVatPercent(2500);
			cart.addItem(item);
			
			String locale = "sv-SE";
			String currency = "SEK";
			String countryCode = "SE";
			String clientOrderNumber = "100002";
			
			Order order = new Order();
			order.setMerchantSettings(ms);
			order.setCart(cart);
			order.setLocale(locale);
			order.setCurrency(currency);
			order.setCountryCode(countryCode);
			order.setClientOrderNumber(clientOrderNumber);

			Customer customer = new Customer();
			customer.setNationalId("194608142222");
			
			order.setCustomer(customer);
			
			Address address = new Address();
			address.setFullName("Therese Persson");
			address.setFirstName("Therese");
			address.setLastName("Persson");
			address.setStreetAddress("Testgatan 1");
			address.setCoAddress("c/o Eriksson, Erik");
			address.setPostalCode("99999");
			address.setCity("Stan");
			address.setCountryCode("SE");
			
			order.setShippingAddress(address);
			order.setBillingAddress(address);
			
			String result = createOrder(order);
			System.out.println("Create Order result: " + result);
			
			if (result!=null) {
				Order resultOrder = JsonUtil.gson.fromJson(result, Order.class);
				System.out.println(resultOrder.toString());
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	/**
	 * Initializes client with values
	 * 
	 */
	public void init() {
		init("https://checkoutapistage.svea.com", "124842", "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW");
	}
	
	/**
	 * Initializes client
	 */
	public void init(String serverName, String merchantId, String secretWord) {
		this.serverName = serverName;
		this.merchantId = merchantId;
		this.secretWord = secretWord;
		
		// Disable SNI to prevent SSL-name problem
		// System.setProperty("jsse.enableSNIExtension", "false");
		
		ScalarsConverterFactory converter = ScalarsConverterFactory.create();
		
		retroFit = new Retrofit.Builder().baseUrl(this.serverName)
				.addConverterFactory(converter)
				.build();

		service = retroFit.create(CheckoutApiService.class);		
	}

	public String getOrder(Long orderId) throws Exception {
		String ts = CheckoutUtil.getTimestampStr();
		String auth = CheckoutUtil.calculateAuthHeader(merchantId, "", secretWord, ts);
		
		Call<ResponseBody> call = service.getOrder(auth, ts, orderId.toString());
		
		Response<ResponseBody> response = call.execute();
		
		String resultMsg = null; 

		if (response.errorBody()!=null) {
			System.out.println(response.errorBody().string());
			resultMsg = response.errorBody().string();
		} else {
			resultMsg = response.body().string();
			System.out.println(response.message());
			System.out.println(resultMsg);
			System.out.println(response.raw().toString());
		}		

		if (resultMsg!=null && resultMsg.trim().length()>0) {
			return resultMsg;
		} else {
			return null;
		}
	}
	
	
	/**
	 * Creates an order 
	 * @param ms
	 * @param cart
	 * @param locale
	 * @param currency
	 * @param countryCode
	 * @param clientOrderNumber
	 * @return
	 * @throws Exception
	 */
	public String createOrder(Order order ) throws Exception {
		StringBuffer body = new StringBuffer();
		body.append(order.toString());

		String ts = CheckoutUtil.getTimestampStr();
		String auth = CheckoutUtil.calculateAuthHeader(merchantId, body.toString(), secretWord, ts);
		
		//System.out.println("ts: " + ts);
		//System.out.println("auth: " + auth);
		//System.out.println("body: " + body.toString());
		
		Call<ResponseBody> call = service.createOrder(auth, ts, body.toString());
		Response<ResponseBody> response = call.execute();
		
		String resultMsg = null; 

		if (response.errorBody()!=null) {
			System.out.println(response.errorBody().string());
			resultMsg = response.errorBody().string();
		} else {
			resultMsg = response.body().string();
			System.out.println(response.message());
			System.out.println(resultMsg);
			System.out.println(response.raw().toString());
		}		

		if (resultMsg!=null && resultMsg.trim().length()>0) {
			return resultMsg;
		} else {
			return null;
		}
	}
	
	public Order getOrderFromString(String orderStr) {
		Order order = JsonUtil.gson.fromJson(orderStr, Order.class);
		return order;
	}
}