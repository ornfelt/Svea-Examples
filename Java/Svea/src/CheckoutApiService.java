import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.Headers;
import retrofit2.http.POST;
import retrofit2.http.Path;

public interface CheckoutApiService {

	@Headers({
		"Content-type: application/json"
	})
	@GET("/api/orders/{orderId}")
	Call<ResponseBody> getOrder(
			@Header("Authorization")String authorization,
			@Header("Timestamp")String timestamp,
			@Path("orderId")String orderId);

	@Headers({
		"Content-type: application/json"
	})
	
	@POST("/api/orders/")
	Call<ResponseBody> createOrder(
			@Header("Authorization")String authorization,
			@Header("Timestamp")String timestamp,
			@Body()String body
			);
}
