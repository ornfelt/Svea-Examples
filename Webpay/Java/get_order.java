import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URI;
import java.nio.charset.StandardCharsets;

public class get_order {
	
    public static void main(String[] args) {
        System.out.println("Running GET request for Webpay (Java)");
        try {
            String url = "https://webpayadminservicestage.svea.com/AdminService.svc/secure";
            String action = "http://tempuri.org/IAdminService/GetOrders";

            String soapEnvelope = "<soap:Envelope xmlns:soap=\"http://www.w3.org/2003/05/soap-envelope\" xmlns:tem=\"http://tempuri.org/\" xmlns:dat=\"http://schemas.datacontract.org/2004/07/DataObjects.Admin.Service\">\r\n" +
                "    <soap:Header xmlns:wsa=\"http://www.w3.org/2005/08/addressing\">\r\n" +
                "        <wsa:Action>http://tempuri.org/IAdminService/GetOrders</wsa:Action>\r\n" +
                "        <wsa:To>https://webpayadminservicestage.svea.com/AdminService.svc/secure</wsa:To>\r\n" +
                "    </soap:Header>\r\n" +
                "    <soap:Body>\r\n" +
                "        <tem:GetOrders>\r\n" +
                "            <tem:request>\r\n" +
                "                <dat:Authentication>\r\n" +
                "                    <dat:Password>sverigetest</dat:Password>\r\n" +
                "                    <dat:Username>sverigetest</dat:Username>\r\n" +
                "                </dat:Authentication>\r\n" +
                "                <dat:OrdersToRetrieve>\r\n" +
                "                    <dat:GetOrderInformation>\r\n" +
                "                        <dat:ClientId>79021</dat:ClientId>\r\n" +
                "                        <dat:OrderType>Invoice</dat:OrderType>\r\n" +
                "                        <dat:SveaOrderId>9731563</dat:SveaOrderId>\r\n" +
                "                    </dat:GetOrderInformation>\r\n" +
                "                </dat:OrdersToRetrieve>\r\n" +
                "            </tem:request>\r\n" +
                "        </tem:GetOrders>\r\n" +
                "    </soap:Body>\r\n" +
                "</soap:Envelope>";

            //URL obj = new URL(url);
            URL obj = URI.create(url).toURL();
            HttpURLConnection con = (HttpURLConnection) obj.openConnection();

            con.setRequestMethod("GET");
            con.setRequestProperty("Content-Type", "application/soap+xml;charset=UTF-8");
            con.setRequestProperty("SOAPAction", action);

            con.setDoOutput(true);
            DataOutputStream wr = new DataOutputStream(con.getOutputStream());
            wr.writeBytes(soapEnvelope);
            wr.flush();
            wr.close();

            int responseCode = con.getResponseCode();
            //System.out.println("Response Code : " + responseCode);

            BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream(), StandardCharsets.UTF_8));
            String inputLine;
            StringBuilder response = new StringBuilder();

            while ((inputLine = in.readLine()) != null) {
                response.append(inputLine);
            }
            in.close();

            //System.out.println("Response:");
            //System.out.println(response.toString());
            if (responseCode == 200)
                System.out.println("Success!");
            else
                System.out.println("Failed...");
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
