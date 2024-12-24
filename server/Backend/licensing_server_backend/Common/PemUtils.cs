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


        public static string ExportPrivateKey(RSA rsa)
        {
            // Export the private key in PKCS#1 format (compatible with OpenSSL)
            var privateKeyBytes = rsa.ExportRSAPrivateKey();
            return ConvertToPem(privateKeyBytes, "RSA PRIVATE KEY");
        }

        public static string ExportPublicKey(RSA rsa)
        {
            // Export the public key in X.509 format (compatible with OpenSSL)
            var publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
            return ConvertToPem(publicKeyBytes, "PUBLIC KEY");
        }

        private static string ConvertToPem(byte[] keyData, string keyType)
        {
            // Convert to Base64 format
            string base64Key = Convert.ToBase64String(keyData, Base64FormattingOptions.InsertLineBreaks);

            // Wrap the key with PEM headers and footers
            return $"-----BEGIN {keyType}-----\n{base64Key}\n-----END {keyType}-----";
        }
    }
}
