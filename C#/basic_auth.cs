using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SveaAuthentication
{
    class Program
    {
        static void Main(string[] args)
        {
            string a;
            string b;
            CreateAuthenticationToken(out a, out b);

            Console.Write(a);
            Console.WriteLine();
            Console.Write(b);
            Console.ReadKey();
        }

        static void CreateAuthenticationToken(out string token, out string timestamp, string message = null)
        {
            const int merchantId = 124842;
            const string secretKey = "1NDxpT2WQ4PW6Ud95rLWKD98xVr45Q8O9Vd52nomC7U9B18jp7lHCu7nsiTJO1NWXjSx26vE41jJ4rul7FUP1cGKXm4wakxt3iF7k63ayleb1xX9Di2wW46t9felsSPW";
            message = message ?? string.Empty;
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            using (var sha512 = SHA512.Create())
            {
                var hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(message + secretKey + timestamp));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                token = Convert.ToBase64String(Encoding.UTF8.GetBytes(merchantId + ":" + hashString));
            }
        }
    }
}
