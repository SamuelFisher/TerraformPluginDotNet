using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace TerraformPluginDotNet
{
    public static class CertificateGenerator
    {
        private const int KeyStrength = 2048;

        public static AsymmetricKeyParameter GeneratePrivateKey()
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);

            // Generate Key
            var keyGenerationParameters = new KeyGenerationParameters(random, KeyStrength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            return keyPairGenerator.GenerateKeyPair().Private;
        }

        public static X509Certificate2 GenerateSelfSignedCertificate(string subjectName, string issuerName, AsymmetricKeyParameter issuerPrivKey)
        {
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            var certificateGenerator = new X509V3CertificateGenerator();

            // Serial Number
            var serialNumber = BigIntegers.CreateRandomInRange(BigInteger.One, BigInteger.ValueOf(long.MaxValue), random);
            certificateGenerator.SetSerialNumber(serialNumber);

            // Issuer and SN
            var subjectDN = new X509Name(subjectName);
            var issuerDN = new X509Name(issuerName);
            certificateGenerator.SetIssuerDN(issuerDN);
            certificateGenerator.SetSubjectDN(subjectDN);

            // SAN
            var subjectAltName = new GeneralNames(new GeneralName(GeneralName.DnsName, "localhost"));
            certificateGenerator.AddExtension(X509Extensions.SubjectAlternativeName, false, subjectAltName);

            // Validity
            var notBefore = DateTime.UtcNow.Date;
            var notAfter = notBefore.AddYears(2);
            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // Public Key
            var keyGenerationParameters = new KeyGenerationParameters(random, KeyStrength);
            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            // Sign certificate
            var signatureFactory = new Asn1SignatureFactory("SHA256WithRSA", issuerPrivKey, random);
            var certificate = certificateGenerator.Generate(signatureFactory);
            var x509 = new X509Certificate2(certificate.GetEncoded(), (string)null, X509KeyStorageFlags.Exportable);

            // Private key
            var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectKeyPair.Private);

            var seq = (Asn1Sequence)Asn1Object.FromByteArray(privateKeyInfo.ParsePrivateKey().GetDerEncoded());
            if (seq.Count != 9)
            {
                throw new PemException("Invalid RSA private key");
            }

            var rsa = RsaPrivateKeyStructure.GetInstance(seq);
            var rsaparams = new RsaPrivateCrtKeyParameters(rsa.Modulus, rsa.PublicExponent, rsa.PrivateExponent, rsa.Prime1, rsa.Prime2, rsa.Exponent1, rsa.Exponent2, rsa.Coefficient);

            var parms = DotNetUtilities.ToRSAParameters(rsaparams);
            var rsa1 = RSA.Create();
            rsa1.ImportParameters(parms);
            // https://github.com/dotnet/runtime/issues/23749
            var cert = x509.CopyWithPrivateKey(rsa1);

            return new X509Certificate2(cert.Export(X509ContentType.Pkcs12));
        }
    }
}
