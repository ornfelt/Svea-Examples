import java.io.IOException;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Random;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.Base64;
import java.time.format.DateTimeFormatter;
import java.time.ZonedDateTime;

public class SveaAuth {
    private static final String merchantId = "CHECKOUT_MERCHANT_ID";
    private static final String secretKey = "CHECKOUT_SECRET_KEY";

    public static void main(String[] args) throws IOException, InterruptedException, ExecutionException {
        System.out.println("Running Create request for Checkout (Java)");
        sendRequest().get();
    }

    public static CompletableFuture<Void> sendRequest() {
        HttpClient client = HttpClient.newHttpClient();
        String url = "https://checkoutapistage.svea.com/api/orders";
        Random random = new Random();
        StringBuilder randomOrderId = new StringBuilder();
        for (int i = 0; i < 15; i++) {
            randomOrderId.append(random.nextInt(10));
        }

        String bodyStr;
        try {
            Path filePath = Paths.get("create_order_payload.json");
            bodyStr = Files.readString(filePath);
            bodyStr = bodyStr.replace("my_order_id", randomOrderId.toString());
        } catch (IOException e) {
            System.err.println("Error reading the JSON file: " + e.getMessage());
            return CompletableFuture.completedFuture(null);
        }

        HttpRequest request;
        try {
            request = HttpRequest.newBuilder()
                .uri(URI.create(url))
                .header("Content-Type", "application/json; charset=utf-8")
                .headers(setHttpRequestHeaders(bodyStr))
                .POST(HttpRequest.BodyPublishers.ofString(bodyStr))
                .build();
        } catch (NoSuchAlgorithmException e) {
            System.err.println("Error creating request headers: " + e.getMessage());
            return CompletableFuture.completedFuture(null);
        }

        return client.sendAsync(request, HttpResponse.BodyHandlers.ofString())
            .thenApply(response -> {
                //System.out.println("response: " + response);
                //System.out.println("response body: " + response.body());
                //System.out.println("response status: " + response.statusCode());

                if (response.statusCode() == 200 || response.statusCode() == 201) {
                    System.out.println("Success!");
                } else {
                    System.out.println("Failed...");
                }
                System.out.println("----------------------------------------------------------");
                return null;
            });
    }

    private static String[] setHttpRequestHeaders(String requestMessage) throws NoSuchAlgorithmException {
        String timestamp = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss").format(ZonedDateTime.now());
        String token = createAuthenticationToken(requestMessage, timestamp);
        return new String[]{
            "Authorization", "Svea " + token,
            "Timestamp", timestamp
        };
    }

    private static String createAuthenticationToken(String requestMessage, String timestamp) throws NoSuchAlgorithmException {
        MessageDigest sha512 = MessageDigest.getInstance("SHA-512");
        sha512.update((requestMessage + secretKey + timestamp).getBytes());
        String hashString = bytesToHex(sha512.digest());
        String authToken = Base64.getEncoder().encodeToString((merchantId + ":" + hashString).getBytes());
        return authToken;
    }

    private static String bytesToHex(byte[] hash) {
        StringBuilder hexString = new StringBuilder();
        for (byte b : hash) {
            String hex = Integer.toHexString(0xff & b);
            if (hex.length() == 1) hexString.append('0');
            hexString.append(hex);
        }
        return hexString.toString();
    }
}
