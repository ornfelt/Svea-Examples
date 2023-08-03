import java.io.File;
import java.net.URL;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.List;

import okhttp3.ResponseBody;

import retrofit2.Call;
import retrofit2.Response;
import retrofit2.Retrofit;
import retrofit2.converter.scalars.ScalarsConverterFactory;

public class PmtApiClientRF {

	public static DateFormat dfmt = new SimpleDateFormat("yyyy-MM-dd");
	
	private String merchantId;
	private String secretWord;
	private String serverName;
	
	private	Retrofit	retroFit = null;
	private PmtApiService service = null;

	public void runTests() {
		init();
		System.out.println("\n---------------------------------------------\n");
		testGetOrder();
		// More tests...
	}
	
	private void testGetOrder() {
		System.out.println("Trying to fetch order through PA!");
		try {
			Long orderId = 8906830L;
			PAOrder order = getOrder(orderId);
			
			if (order == null) {
				System.out.println("No such order: " + orderId);
				return;
			}
			
			System.out.println("PA Order fetched!");
			System.out.println(JsonUtil.gson.toJson(order));
			
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	/**
	 * Initializes client with values
	 * 
	 */
	public void init() {
		init("https://paymentadminapistage.svea.com", "124842", "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW");
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

		service = retroFit.create(PmtApiService.class);		
	}
	
	
	public PAOrder getOrder(Long orderId) throws Exception {
		String ts = PmtApiUtil.getTimestampStr();
		String auth = PmtApiUtil.calculateAuthHeader(merchantId, "", secretWord, ts);
		
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
			return PmtApiUtil.gson.fromJson(resultMsg, PAOrder.class);
		} else {
			return null;
		}
	}
	
	public String deliverCompleteOrder(Long orderId) throws Exception {
		PAOrder order = getOrder(orderId);
		if (order==null) return("Can't deliver order " + orderId + " (not found)");
		
		if ("Delivered".equals(order.getOrderStatus())) {
			return orderId + " already delivered";
		}
		
		String ts = PmtApiUtil.getTimestampStr();
		List<Long> lines = new ArrayList<Long>();
		
		List<OrderRow> rowList = order.getOrderRows();
		for (OrderRow or : rowList) {
			if (!or.isCancelled()) {
				lines.add(or.getOrderRowId());
			}
		}
		
		String body = "{ \"orderRowIds\": " + JsonUtil.gson.toJson(lines) + " }";
		String auth = PmtApiUtil.calculateAuthHeader(merchantId, body, secretWord, ts);

		Call<ResponseBody> call = service.deliverOrder(auth, ts, orderId.toString(), body);
		Response<ResponseBody> response = call.execute();
		
		String resultMsg = null; 

		if (response.errorBody()!=null && response.errorBody().string()!=null && response.errorBody().string().length()>0) {
			System.out.println(response.errorBody().string());
			resultMsg = response.errorBody().string();
		} else {
			if (response.code()==200) {
				resultMsg = response.body().string();
				System.out.println(response.message());
				System.out.println(resultMsg);
				System.out.println(response.raw().toString());
			} else {
				resultMsg = response.message();
				System.out.println(response.code() + " : " + response.message());
			}
		}		

		if (resultMsg!=null && resultMsg.trim().length()>0) {
			return resultMsg;
		} else {
			return null;
		}
	}
	
	/**
	 * Tells Svea that this order is delivered and should be billed.
	 * No order rows are supplied, meaning all deliverable rows should be delivered.
	 * 
	 * @param orderId		The order to be delivered
	 * @return				
	 * @throws Exception
	 */
	public String deliverCompleteOrderNoCheck(Long orderId) throws Exception {
		String ts = PmtApiUtil.getTimestampStr();
		List<Long> lines = new ArrayList<Long>();
		
		String body = "{\"orderRowIds\":" + JsonUtil.gson.toJson(lines) + "}";
		String auth = PmtApiUtil.calculateAuthHeader(merchantId, body, secretWord, ts);

		Call<ResponseBody> call = service.deliverOrder(auth, ts, orderId.toString(), body);
		Response<ResponseBody> response = call.execute();
		
		String resultMsg = null; 

		if (response.errorBody()!=null && response.errorBody().string()!=null && response.errorBody().string().length()>0) {
			System.out.println(response.errorBody().string());
			resultMsg = response.errorBody().string();
		} else {
			if (response.code()==200) {
				resultMsg = response.body().string();
				System.out.println(response.message());
				System.out.println(resultMsg);
				System.out.println(response.raw().toString());
			} else {
				resultMsg = response.message();
				System.out.println(response.code() + " : " + response.message());
			}
		}		

		if (resultMsg!=null && resultMsg.trim().length()>0) {
			return resultMsg;
		} else {
			return null;
		}
	}
}
