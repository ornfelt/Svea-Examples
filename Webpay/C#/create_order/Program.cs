﻿using System;
using System.Text;
using System.Net;

class Test
{

    static void Main()
    {
        Console.WriteLine("Running Create request for Webpay (C#)");
        try
        {
            var client = new HttpClient();
            var randomOrderId = GenerateRandomOrderId();
            var url = "https://webpaywsstage.svea.com/sveawebpay.asmx";
            var action = "https://webservices.sveaekonomi.se/webpay/CreateOrderEu";

            string soapEnvelope = @"
    <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:web=""https://webservices.sveaekonomi.se/webpay"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
        <soap:Header/>
        <soap:Body>
            <web:CreateOrderEu>
                <web:request>
                    <web:Auth>
                        <web:ClientNumber>WEBPAY_CLIENT_ID</web:ClientNumber>
                        <web:Username>WEBPAY_PASSWORD</web:Username>
                        <web:Password>WEBPAY_PASSWORD</web:Password>
                    </web:Auth>
                    <web:CreateOrderInformation>
                        <web:ClientOrderNumber>my_order_id</web:ClientOrderNumber>
                        <web:OrderRows>
                            <web:OrderRow>
                                <web:ArticleNumber>123</web:ArticleNumber>
                                <web:Description>Some Product 1</web:Description>
                                <web:PricePerUnit>293.6</web:PricePerUnit>
                                <web:PriceIncludingVat>false</web:PriceIncludingVat>
                                <web:NumberOfUnits>2</web:NumberOfUnits>
                                <web:Unit>st</web:Unit>
                                <web:VatPercent>25</web:VatPercent>
                                <web:DiscountPercent>0</web:DiscountPercent>
                                <web:DiscountAmount>0</web:DiscountAmount>
                                <web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>
                            </web:OrderRow>
                            <web:OrderRow>
                                <web:ArticleNumber>456</web:ArticleNumber>
                                <web:Description>Some Product 2</web:Description>
                                <web:PricePerUnit>39.2</web:PricePerUnit>
                                <web:PriceIncludingVat>false</web:PriceIncludingVat>
                                <web:NumberOfUnits>1</web:NumberOfUnits>
                                <web:Unit>st</web:Unit>
                                <web:VatPercent>25</web:VatPercent>
                                <web:DiscountPercent>0</web:DiscountPercent>
                                <web:DiscountAmount>0</web:DiscountAmount>
                                <web:DiscountAmountIncludingVat>false</web:DiscountAmountIncludingVat>
                            </web:OrderRow>
                        </web:OrderRows>
                        <web:CustomerIdentity>
                            <web:NationalIdNumber>4605092222</web:NationalIdNumber>
                            <web:Email>firstname.lastname@svea.com</web:Email>
                            <web:PhoneNumber>080000000</web:PhoneNumber>
                            <web:FullName>Tester Testsson</web:FullName>
                            <web:Street>Gatan 99</web:Street>
                            <web:ZipCode>12345</web:ZipCode>
                            <web:Locality>16733</web:Locality>
                            <web:CountryCode>SE</web:CountryCode>
                            <web:CustomerType>Individual</web:CustomerType>
                        </web:CustomerIdentity>
                        <web:OrderDate>2023-12-18T11:07:23</web:OrderDate>
                        <web:OrderType>Invoice</web:OrderType>
                    </web:CreateOrderInformation>
                </web:request>
            </web:CreateOrderEu>
        </soap:Body>
    </soap:Envelope>";

            soapEnvelope = soapEnvelope.Replace("my_order_id", randomOrderId);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/soap+xml;charset=UTF-8";

            // Add the SOAPAction to the header if it's required by the server
            if (!string.IsNullOrEmpty(action))
            {
                request.Headers.Add("SOAPAction", action);
            }

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(soapEnvelope);
            }

            var response = (HttpWebResponse)request.GetResponse();
            //Console.WriteLine("Response Code : " + (int)response.StatusCode);

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                string responseContent = streamReader.ReadToEnd();
                //Console.WriteLine("Response:");
                //Console.WriteLine(responseContent);
                if ((int)response.StatusCode == 200 && responseContent.ToLower().Contains("accepted>true"))
                {
                    Console.WriteLine("Success!");
                }
                else
                {
                    Console.WriteLine("Failed...");
                }
            }
            Console.WriteLine("----------------------------------------------------------");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static string GenerateRandomOrderId()
    {
        var random = new Random();
        var orderId = new StringBuilder();
        for (int i = 0; i < 8; i++)
        {
            orderId.Append(random.Next(0, 10)); // Append a random digit
        }
        return orderId.ToString();
    }
}
