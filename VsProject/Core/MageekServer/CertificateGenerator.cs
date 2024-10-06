using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

public class CertificateGenerator
{
    public static (string certificatePath, string password) GenerateSelfSignedCertificate(string certPath)
    {
        var randomPassword = GenerateRandomPassword();
        using (RSA rsa = RSA.Create(2048))
        {
            var certRequest = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            certRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
            certRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certRequest.PublicKey, false));
            using (X509Certificate2 cert = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1)))
            {
                byte[] pfxBytes = cert.Export(X509ContentType.Pfx, randomPassword);
                File.WriteAllBytes(certPath, pfxBytes);
            }
        }
        return (certPath, randomPassword);
    }

    private static string GenerateRandomPassword(int length = 16)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        var random = new RNGCryptoServiceProvider();
        var buffer = new byte[length];
        random.GetBytes(buffer);
        var passwordChars = new char[length];
        for (int i = 0; i < length; i++)
        {
            passwordChars[i] = validChars[buffer[i] % validChars.Length];
        }
        return new string(passwordChars);
    }
}
