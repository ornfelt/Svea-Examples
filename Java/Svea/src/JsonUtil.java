import java.text.DateFormat;
import java.text.SimpleDateFormat;

import com.google.gson.FieldNamingPolicy;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

public class JsonUtil {

	public static Gson gson;
	public static final String dfmtStr = "yyyy-MM-dd";
	public static final DateFormat dfmt;
	
	static {
		GsonBuilder builder = new GsonBuilder().setPrettyPrinting().setDateFormat(dfmtStr);
		builder.setFieldNamingPolicy(FieldNamingPolicy.IDENTITY);
		builder.setLenient();
		gson = builder.create();
		dfmt = new SimpleDateFormat(dfmtStr);
	}
}
