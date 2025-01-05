import java.net.*;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.net.http.HttpResponse.BodyHandlers;
import java.net.URLEncoder;
import java.io.*;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Base64;
import java.util.Random;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.xml.sax.InputSource;

public class pg_request {

    public static void main(String[] args) throws IOException, NoSuchAlgorithmException {
        System.out.println("Running Create request for PaymentGateway (Java)");
        makePostRequest();
        System.out.println("Running GET request for PaymentGateway (Java)");
        makeGetQueryTransactionIdRequest();
        System.out.println("----------------------------------------------------------");
    }

    private static void makeGetQueryTransactionIdRequest() throws IOException, NoSuchAlgorithmException {
        int transactionId = PG_ORDER_TO_FETCH;
        String messageXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><query><transactionid>" + transactionId + "</transactionid></query>";
        String encodedMessage = Base64.getEncoder().encodeToString(messageXML.getBytes());
        String secret = "PG_SECRET_KEY";
        String mac = getSha512Hash(encodedMessage + secret);

        //System.out.println("mac: " + mac);
        //System.out.println("encodedMessage: " + encodedMessage);

        //URL url = new URL("https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid");
        URL url = URI.create("https://webpaypaymentgatewaystage.svea.com/webpay/rest/querytransactionid").toURL();
        HttpURLConnection conn = (HttpURLConnection) url.openConnection();
        conn.setRequestMethod("POST");
        conn.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
        conn.setDoOutput(true);

        String data = "merchantid=PG_MERCHANT_ID&message=" + URLEncoder.encode(encodedMessage, "UTF-8") + "&mac=" + URLEncoder.encode(mac, "UTF-8");

        try (DataOutputStream wr = new DataOutputStream(conn.getOutputStream())) {
            wr.writeBytes(data);
            wr.flush();
        }

        int responseCode = conn.getResponseCode();
        //System.out.println("Response status code: " + responseCode);

        try (BufferedReader in = new BufferedReader(new InputStreamReader(conn.getInputStream()))) {
            String inputLine;
            StringBuilder content = new StringBuilder();
            while ((inputLine = in.readLine()) != null) {
                content.append(inputLine);
            }

            //System.out.println("Response message: " + content.toString());
            if (responseCode == 200)
                System.out.println("Success!");
            else
                System.out.println("Failed...");
        }
    }

    private static void makePostRequest() throws IOException, NoSuchAlgorithmException {
        int randomRefNo = new Random().nextInt(9000000) + 1000000;
        String messageXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><payment><paymentmethod>SVEACARDPAY</paymentmethod><currency>SEK</currency><amount>500</amount><vat>100</vat><customerrefno>" + randomRefNo + "</customerrefno><returnurl>https://webpaypaymentgatewaystage.svea.com/webpay-admin/admin/merchantresponsetest.xhtml</returnurl><lang>en</lang></payment>";
        String encodedMessage = Base64.getEncoder().encodeToString(messageXML.getBytes());
        String secret = "PG_SECRET_KEY";
        String mac = getSha512Hash(encodedMessage + secret);

        //System.out.println("mac: " + mac);
        //System.out.println("encodedMessage: " + encodedMessage);

        //URL url = new URL("https://webpaypaymentgatewaystage.svea.com/webpay/payment");
        URL url = URI.create("https://webpaypaymentgatewaystage.svea.com/webpay/payment").toURL();
        HttpURLConnection conn = (HttpURLConnection) url.openConnection();
        conn.setRequestMethod("POST");
        conn.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
        conn.setDoOutput(true);

        String data = "merchantid=PG_MERCHANT_ID&message=" + URLEncoder.encode(encodedMessage, "UTF-8") + "&mac=" + URLEncoder.encode(mac, "UTF-8");

        try (DataOutputStream wr = new DataOutputStream(conn.getOutputStream())) {
            wr.writeBytes(data);
            wr.flush();
        }

        //int responseCode = conn.getResponseCode();
        //System.out.println("Response status code: " + responseCode);

        try (BufferedReader in = new BufferedReader(new InputStreamReader(conn.getInputStream()))) {
            String inputLine;
            StringBuilder content = new StringBuilder();
            while ((inputLine = in.readLine()) != null) {
                content.append(inputLine);
            }

            String responseContent = content.toString();
            //System.out.println(responseContent);
            // Since we receive an iframe and the status tends to return 400 - verify the HTML content instead
            if (responseContent.toLowerCase().contains("enter your card details") || responseContent.toLowerCase().contains("select card type"))
                System.out.println("Success!");
            else
                System.out.println("Failed...");
        }
    }

    private static String getSha512Hash(String input) throws NoSuchAlgorithmException {
        MessageDigest md = MessageDigest.getInstance("SHA-512");
        byte[] bytes = md.digest(input.getBytes(StandardCharsets.UTF_8));
        StringBuilder sb = new StringBuilder();
        for (byte aByte : bytes) {
            sb.append(Integer.toString((aByte & 0xff) + 0x100, 16).substring(1));
        }
        return sb.toString();
    }
}
