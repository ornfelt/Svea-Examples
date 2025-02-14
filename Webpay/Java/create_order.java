import java.io.OutputStream;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URI;
import java.util.Random;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.nio.file.Files;
import java.nio.file.Paths;

public class create_order {

    public static void main(String[] args) {
        System.out.println("Running Create request for Webpay (Java)");
        try {
            String url = "https://webpaywsstage.svea.com/sveawebpay.asmx";
            String action = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu";
            Random random = new Random();
            StringBuilder randomOrderId = new StringBuilder();
            for (int i = 0; i < 8; i++) {
                randomOrderId.append(random.nextInt(10));
            }

			String soapTemplate = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:web=\"https://webservices.sveaekonomi.se/webpay\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">"
			+ "<soap:Header/>"
			+ "<soap:Body>"
			+ "<web:CreateOrderEu>"
			+ "<web:request>"
			+ "<web:Auth>"
			+ "<web:ClientNumber>WEBPAY_CLIENT_ID</web:ClientNumber>"
			+ "<web:Username>WEBPAY_PASSWORD</web:Username>"
			+ "<web:Password>WEBPAY_PASSWORD</web:Password>"
			+ "</web:Auth>"
			+ "<web:CreateOrderInformation>"
			+ "<web:ClientOrderNumber>my_order_id</web:ClientOrderNumber>"
			+ "<web:OrderRows>"
			+ "<web:OrderRow>"
			+ "<web:ArticleNumber>123</web:ArticleNumber>"
			+ "<web:Description>Some Product 1</web:Description>"
			+ "<web:PricePerUnit>293.6</web:PricePerUnit>"
			+ "<web:PriceIncludingVat>false</web:PriceIncludingVat>"
			+ "<web:NumberOfUnits>2</web:NumberOfUnits>"
			+ "<web:Unit>st</web:Unit>"
			+ "<web:VatPercent>25</web:VatPercent>"
			+ "<web:DiscountPercent>0</web:DiscountPercent>"
			+ "<web:DiscountAmount>0</web:DiscountAmount>"
			+ "<web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>"
			+ "</web:OrderRow>"
			+ "<web:OrderRow>"
			+ "<web:ArticleNumber>456</web:ArticleNumber>"
			+ "<web:Description>Some Product 2</web:Description>"
			+ "<web:PricePerUnit>39.2</web:PricePerUnit>"
			+ "<web:PriceIncludingVat>false</web:PriceIncludingVat>"
			+ "<web:NumberOfUnits>1</web:NumberOfUnits>"
			+ "<web:Unit>st</web:Unit>"
			+ "<web:VatPercent>25</web:VatPercent>"
			+ "<web:DiscountPercent>0</web:DiscountPercent>"
			+ "<web:DiscountAmount>0</web:DiscountAmount>"
			+ "<web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>"
			+ "</web:OrderRow>"
			+ "</web:OrderRows>"
			+ "<web:CustomerIdentity>"
			+ "<web:NationalIdNumber>4605092222</web:NationalIdNumber>"
			+ "<web:Email>firstname.lastname@svea.com</web:Email>"
			+ "<web:PhoneNumber>080000000</web:PhoneNumber>"
			+ "<web:FullName>Tester Testsson</web:FullName>"
			+ "<web:Street>Gatan 99</web:Street>"
			+ "<web:ZipCode>12345</web:ZipCode>"
			+ "<web:Locality>16733</web:Locality>"
			+ "<web:CountryCode>SE</web:CountryCode>"
			+ "<web:CustomerType>Individual</web:CustomerType>"
			+ "</web:CustomerIdentity>"
			+ "<web:OrderDate>2023-12-18T11:07:23</web:OrderDate>"
			+ "<web:OrderType>Invoice</web:OrderType>"
			+ "</web:CreateOrderInformation>"
			+ "</web:request>"
			+ "</web:CreateOrderEu>"
			+ "</soap:Body>"
			+ "</soap:Envelope>";

            String soapEnvelope = soapTemplate.replace("my_order_id", randomOrderId.toString());

            //URL obj = new URL(url);
            URL obj = URI.create(url).toURL();
            HttpURLConnection con = (HttpURLConnection) obj.openConnection();

            // Setting basic post request
            con.setRequestMethod("POST");
            con.setRequestProperty("Content-Type", "application/soap+xml;charset=UTF-8");

            if (action != null && !action.isEmpty()) {
                con.setRequestProperty("SOAPAction", action);
            }

            con.setDoOutput(true);
            OutputStream os = con.getOutputStream();
            os.write(soapEnvelope.getBytes());
            os.flush();
            os.close();

            int responseCode = con.getResponseCode();
            //System.out.println("Response Code : " + responseCode);

            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream()));
            String inputLine;
            StringBuffer response = new StringBuffer();

            while ((inputLine = in.readLine()) != null) {
                response.append(inputLine);
            }
            in.close();

            //System.out.println("Response:");
            //System.out.println(response.toString());
            if (responseCode == 200 && response.toString().toLowerCase().contains("accepted>true")) {
                System.out.println("Success!");

                String responseString = response.toString();
                Pattern sveaOrderIdPattern = Pattern.compile("<(?:\\w+:)?SveaOrderId>(\\d+)</(?:\\w+:)?SveaOrderId>", Pattern.CASE_INSENSITIVE);
                Matcher matcher = sveaOrderIdPattern.matcher(responseString);

                if (matcher.find()) {
                    String sveaOrderId = matcher.group(1);
                    //System.out.println("SveaOrderId extracted: " + sveaOrderId);

                    try {
                        Files.write(Paths.get("./created_order_id.txt"), sveaOrderId.getBytes());
                        //System.out.println("SveaOrderId saved to ../created_order_id.txt");
                    } catch (Exception e) {
                        System.out.println("Failed to save SveaOrderId: " + e.getMessage());
                    }
                } else {
                    System.out.println("Failed to extract SveaOrderId.");
                }
            } else
                System.out.println("Failed...");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
