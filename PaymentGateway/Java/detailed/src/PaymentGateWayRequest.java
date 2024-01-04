import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.Reader;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLEncoder;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Base64;
import java.util.LinkedHashMap;
import java.util.Map;

/**
 * This class attempts to perform a POST request to Svea Payment Gateway Stage with test credentials from Svea tech site.
 * @author Jonas Örnfelt
 *
 */

public class PaymentGateWayRequest {

	public PaymentGateWayRequest() {
	}
	
	// Call this method to make request (make sure you have a file xmltest.xml in source folder)
	public void makePostRequest() {
	        URL url;
	        String messageXML = "";
	        
	        try {
				messageXML = readFile("test.xml", StandardCharsets.UTF_8);
			} catch (IOException e1) { e1.printStackTrace(); }
	        
	        String encodedMessage = Base64.getEncoder().encodeToString(messageXML.getBytes());
	        
	        String secret = "PG_SECRET_KEY";
	        
	        String mac = get_SHA_512_SecurePassword((encodedMessage + secret), "");
	        
	        System.out.println("mac: " + mac);
	        System.out.println("encodedMessage: " + encodedMessage);
	        
			try {
				url = new URL("https://webpaypaymentgatewaystage.svea.com/webpay/payment");
			
				Map<String,Object> params = new LinkedHashMap<>();
				params.put("merchantid", "PG_MERCHANT_ID");
				params.put("message", encodedMessage);
				params.put("mac", mac);

				StringBuilder postData = new StringBuilder();
				for (Map.Entry<String,Object> param : params.entrySet()) {
					if (postData.length() != 0) postData.append('&');
					postData.append(URLEncoder.encode(param.getKey(), "UTF-8"));
					postData.append('=');
					postData.append(URLEncoder.encode(String.valueOf(param.getValue()), "UTF-8"));
				}
				byte[] postDataBytes = postData.toString().getBytes("UTF-8");

				HttpURLConnection conn = (HttpURLConnection)url.openConnection();
				conn.setRequestMethod("POST");
				conn.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
				conn.setRequestProperty("Content-Length", String.valueOf(postDataBytes.length));
				conn.setDoOutput(true);
				conn.getOutputStream().write(postDataBytes);

				Reader in = new BufferedReader(new InputStreamReader(conn.getInputStream(), "UTF-8"));
				
				// print response
				for (int c; (c = in.read()) >= 0;)
					System.out.print((char)c);
				
			} catch (MalformedURLException e) {
				e.printStackTrace();
			} catch (IOException e) {
				e.printStackTrace();
			}
	}
	
	// Get SHA512 encrypted string
	private String get_SHA_512_SecurePassword(String passwordToHash, String salt){
		String generatedPassword = null;
		try {
			MessageDigest md = MessageDigest.getInstance("SHA-512");
			md.update(salt.getBytes(StandardCharsets.UTF_8));
			byte[] bytes = md.digest(passwordToHash.getBytes(StandardCharsets.UTF_8));
			StringBuilder sb = new StringBuilder();
			for(int i=0; i < bytes.length ;i++){
				sb.append(Integer.toString((bytes[i] & 0xff) + 0x100, 16).substring(1));
			}
			generatedPassword = sb.toString();
		} catch (NoSuchAlgorithmException e) {
			e.printStackTrace();
		}
		return generatedPassword;
	}
	
	// Read file and get string of the content
	private String readFile(String path, Charset encoding) throws IOException {
		//System.out.println("trying to read file: " + path);
		byte[] encoded = Files.readAllBytes(Paths.get(path));
		return new String(encoded, encoding);
	}
}
