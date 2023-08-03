import java.io.UnsupportedEncodingException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Base64;
import java.util.Calendar;
import java.util.TimeZone;

import com.google.gson.FieldNamingPolicy;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

public class CheckoutUtil {
	
	// DateTime Format used to communicate with Svea's API (UTC/GMT)
	public static DateFormat dateTimeFmtGMT;
	// Date Format for JSon etc. 
	public static final String dfmtStr = "yyyy-MM-dd";
	public static final DateFormat dateFmt;
	public static Gson gson;
	
	static {
		dateTimeFmtGMT = new SimpleDateFormat("yyyy-MM-dd HH:mm");
		dateTimeFmtGMT.setTimeZone(TimeZone.getTimeZone("GMT"));
		
		GsonBuilder builder = new GsonBuilder().setPrettyPrinting().setDateFormat(dfmtStr);
		builder.setFieldNamingPolicy(FieldNamingPolicy.IDENTITY);
		builder.setLenient();
		gson = builder.create();
		
		dateFmt = new SimpleDateFormat(dfmtStr);
		
	}
	
	/**
	 * Calculates a MAC (Message Authentication Key) for given message and secret word.
	 * 
	 * @param merchantId	Merchant ID
	 * @param message		This is the request body (if any)
	 * @param secretWord	This is a presumably base64 encoded secret word that's given
	 * 						to you by Svea Ekonomi.
	 * @param timestamp		Timestamp as a string in format yyyy-MM-dd HH:mm:ss. Timezone must be UTC / GMT.
	 * @return
	 * @throws UnsupportedEncodingException
	 */
	public static String calculateAuthHeader(String merchantId, String message, String secretWord, String timestamp) throws UnsupportedEncodingException {

		
		String combined = message + secretWord + timestamp;
		String result = null;
	    try {
	         MessageDigest md = MessageDigest.getInstance("SHA-512");
	         byte[] bytes = md.digest(combined.getBytes("UTF-8"));
	         StringBuilder sb = new StringBuilder();
	         for(int i=0; i< bytes.length ;i++){
	            sb.append(Integer.toString((bytes[i] & 0xff) + 0x100, 16).substring(1));
	         }
	         result = merchantId + ":" + sb.toString();
	         return "Svea " + base64encodeMsg(result);
	        } 
	       catch (NoSuchAlgorithmException e){
	        e.printStackTrace();
	       }
	    return result;
		
	}

	/**
	 * Returns timestamp string using current time and UTC-timezone.
	 * 
	 * @return
	 */
	public static String getTimestampStr() {
		// Get time in UTC
		Calendar cal = Calendar.getInstance();
		String ts = CheckoutUtil.dateTimeFmtGMT.format(cal.getTime());
		return ts;
	}
	

	/**
	 * Encodes given message to Base64
	 * 
	 * @param message		An XML-message (plain text) to be encoded.
	 * @return				A base64 encoded message.
	 */
	
	public static String base64encodeMsg(String message) {
		return Base64.getEncoder().encodeToString(message.getBytes());
	}
	
	
	/**
	 * Decodes a base64 message. 
	 * 
	 * @param message
	 * @return
	 */
	public static String base64decodeMsg(String message) {
		
		StringBuffer buf = new StringBuffer();
		// Strip message from spaces, newlines etc
		char c;
		for (int i = 0; i<message.length(); i++) {
			c = message.charAt(i);
			switch(c) {
				case ' ' :
				case '\n' :
				case '\r' :
				case '\t' :
					continue;
				default:
					buf.append(c);
			}
		}
		String result = new String(Base64.getDecoder().decode(buf.toString())); 
		
		return result;
	}
}
