import okhttp3.ResponseBody;
import retrofit2.Call;
import retrofit2.http.Body;
import retrofit2.http.GET;
import retrofit2.http.Header;
import retrofit2.http.Headers;
import retrofit2.http.POST;
import retrofit2.http.Path;

public interface PmtApiService {

	@Headers({
		"Content-type: application/json"
	})
	@GET("/api/v1/orders/{orderId}")
	Call<ResponseBody> getOrder(
			@Header("Authorization")String authorization,
			@Header("Timestamp")String timestamp,
			@Path("orderId")String orderId);

	@Headers({
		"Content-type: application/json"
	})
	@POST("/api/v1/orders/{orderId}/deliveries")
	Call<ResponseBody> deliverOrder(
			@Header("Authorization")String auth,
			@Header("Timestamp")String ts,
			@Path("orderId")String orderId,
			@Body()String jsonListOfLineIds);
}