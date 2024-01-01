import java.io.OutputStream;
import java.io.InputStream;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URI;

public class create_order {

    public static void main(String[] args) {
        System.out.println("Running Create request for Webpay (Java)");
        try {
            String url = "https://webpaywsstage.svea.com/sveawebpay.asmx";
            String action = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu";

			String soapEnvelope = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:web=\"https://webservices.sveaekonomi.se/webpay\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">"
			+ "<soap:Header/>"
			+ "<soap:Body>"
			+ "<web:CreateOrderEu>"
			+ "<web:request>"
			+ "<web:Auth>"
			+ "<web:ClientNumber>79021</web:ClientNumber>"
			+ "<web:Username>sverigetest</web:Username>"
			+ "<web:Password>sverigetest</web:Password>"
			+ "</web:Auth>"
			+ "<web:CreateOrderInformation>"
			+ "<web:ClientOrderNumber>MyTestingOrder123</web:ClientOrderNumber>"
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
            if (responseCode == 200 && response.toString().toLowerCase().contains("accepted>true"))
                System.out.println("Success!");
            else
                System.out.println("Failed...");
            System.out.println("----------------------------------------------------------");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
