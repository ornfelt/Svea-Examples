import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URI;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.io.IOException;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.text.SimpleDateFormat;
import java.util.Base64;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;

public class get_order {

    private static final String ORDER_ID = "8906830";
    private static final String MERCHANT_ID = "124842";
    private static final String SECRET_WORD = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW";
    //private static final String BASE_URL = "https://paymentadminapistage.svea.com/api/v1/orders/";
    private static final String BASE_URL = "https://checkoutapistage.svea.com/api/orders/";
    private static final String CONTENT_TYPE = "application/json";

    public static void main(String[] args) {
        System.out.println("Running GET request for Checkout (Java)");
        try {
            String url = BASE_URL + ORDER_ID;
            Map<String, String> myHeaders = getRequestHeaders("", null);

            //URL obj = new URL(url);
            URL obj = URI.create(url).toURL();
            HttpURLConnection con = (HttpURLConnection) obj.openConnection();
            con.setRequestMethod("GET");

            for (Map.Entry<String, String> entry : myHeaders.entrySet()) {
                con.setRequestProperty(entry.getKey(), entry.getValue());
            }

            int responseCode = con.getResponseCode();
            //System.out.println("Response Code : " + responseCode);
            if (responseCode == 200)
                System.out.println("Success!");
            else
                System.out.println("Failed...");

            if (responseCode == HttpURLConnection.HTTP_OK) {
                BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream()));
                String inputLine;
                StringBuffer response = new StringBuffer();

                while ((inputLine = in.readLine()) != null) {
                    response.append(inputLine);
                }
                in.close();

                //System.out.println(response.toString());
            } else {
                System.out.println("Error: Request returned non-OK status code");
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public static Map<String, String> getRequestHeaders(String requestBody, Map<String, String> extraHeaders) {
        String timestamp = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(new Date());
        String token = getAuthToken(timestamp, requestBody);
        Map<String, String> headers = new HashMap<>();
        headers.put("Authorization", "Svea " + token);
        headers.put("Timestamp", timestamp);

        if (extraHeaders != null) {
            headers.putAll(extraHeaders);
        }

        return headers;
    }

    public static String getAuthToken(String timestamp, String requestBody) {
        String digest = requestBody + SECRET_WORD + timestamp;
        MessageDigest md;
        try {
            md = MessageDigest.getInstance("SHA-512");
            byte[] hashed = md.digest(digest.getBytes());
            String auth = MERCHANT_ID + ":" + bytesToHex(hashed);
            return Base64.getEncoder().encodeToString(auth.getBytes());
        } catch (NoSuchAlgorithmException e) {
            e.printStackTrace();
            return null;
        }
    }

    private static String bytesToHex(byte[] bytes) {
        StringBuilder sb = new StringBuilder();
        for (byte b : bytes) {
            sb.append(String.format("%02x", b));
        }
        return sb.toString();
    }
}
