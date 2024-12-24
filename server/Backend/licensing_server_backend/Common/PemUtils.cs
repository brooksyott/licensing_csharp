using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Licensing.Common
{
    public static class PemUtils
    {
        /// <summary>
        /// Extracts the base64-encoded key from a PEM string.
        /// Removes the header/footer lines and any whitespace.
        /// </summary>
        //public static string ExtractBase64Key(string pemString, string header, string footer)
        public static string ExtractBase64Key(string pemString)
        {
            //// Example header: "-----BEGIN PRIVATE KEY-----"
            //// Example footer: "-----END PRIVATE KEY-----"
            //// -----BEGIN RSA PRIVATE KEY-----
            //// -----END RSA PRIVATE KEY----
            //// Remove them and any whitespace.
            //var regex = new Regex($"-{5}{header}-{5}(.*?)-{5}{footer}-{5}", RegexOptions.Singleline);
            //var match = regex.Match(pemString);
            //if (!match.Success)
            //{
            //    //throw new ArgumentException("Invalid PEM format or header/footer mismatch.");
            //    Console.WriteLine("Invalid PEM format or header/footer mismatch.");
            //}

            string result = Regex.Replace(
                pemString,
                @"(?m)^-----BEGIN .*?-----\r?\n?|^-----END .*?-----\r?\n?",
                string.Empty
            );

            // The capturing group (.*?) should be the base64 content
            string pemContent = result
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Replace(" ", string.Empty);

            //Console.WriteLine("Original:\n" + pemString);
            //Console.WriteLine("Without header/footer:\n" + result);
            //Console.WriteLine("Returning:\n" + pemContent);

            return pemContent;
        }
    }

    public static class RsaKeyLoader
    {
        public static RSA LoadRsaPrivateKey(string pemContent)
        {
            // Read the entire .pem file
            //string pemContent = File.ReadAllText(pemFilePath);
            // -----BEGIN RSA PRIVATE KEY-----
            // ----END RSA PRIVATE KEY-----
            // Extract the base64-encoded content between the BEGIN/END lines
            string base64Key = PemUtils.ExtractBase64Key(
                pemContent
            );

            // Convert base64 to raw key bytes
            byte[] keyBytes = Convert.FromBase64String(base64Key);

            // Create an RSA instance
            RSA rsa = RSA.Create();

            // This method works for PKCS#8 keys
            rsa.ImportRSAPrivateKey(keyBytes, out _);

            return rsa;
        }

        public static RSA LoadRsaPublicKey(string pemContent)
        {
            // Read the entire .pem file
            //string pemContent = File.ReadAllText(pemFilePath);

            // Extract the base64-encoded content between the BEGIN/END lines
            string base64Key = PemUtils.ExtractBase64Key(
                pemContent
            );

            // Convert base64 to raw key bytes
            byte[] keyBytes = Convert.FromBase64String(base64Key);

            // Create an RSA instance
            RSA rsa = RSA.Create();

            // This method works for PKCS#8 keys
            rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);

            return rsa;
        }
    }
}
