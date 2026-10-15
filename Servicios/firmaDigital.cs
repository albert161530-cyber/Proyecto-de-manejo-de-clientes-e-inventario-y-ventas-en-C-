using System;
using System.Collections.Generic;
using System.Linq;

using System.Security;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PLUNES
{
    public class firmaDigital
    {
        public static string FirmarXML(string xmlString, string rutaCertificado, string password)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.LoadXml(xmlString);

            X509Certificate2 cert = new X509Certificate2(
                rutaCertificado,
                password,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable
            );

            SignedXml signedXml = new SignedXml(doc);
            //signedXml.SigningKey = cert.GetRSAPrivateKey();
            signedXml.SigningKey = cert.PrivateKey; // Compatible con 4.5.1

            // SHA256 obligatorio DGII
            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA256Url;
            signedXml.SignedInfo.CanonicalizationMethod = SignedXml.XmlDsigCanonicalizationUrl;

            // Referencia al documento completo
            Reference reference = new Reference();
            reference.Uri = "";
            reference.DigestMethod = SignedXml.XmlDsigSHA256Url;
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());
            signedXml.AddReference(reference);

            // Agregar certificado dentro del XML
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(cert));
            signedXml.KeyInfo = keyInfo;

            // Generar firma
            signedXml.ComputeSignature();

            XmlElement xmlSignature = signedXml.GetXml();

            // Agregar nodo Signature al documento
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlSignature, true));

            return doc.OuterXml;
        }

        public static string Firmar_XML(string xmlString, string rutaCertificado, string password)
        {
            XmlDocument doc = new XmlDocument();
            doc.PreserveWhitespace = false;
            doc.LoadXml(xmlString);

            X509Certificate2 cert = new X509Certificate2(rutaCertificado, password);

            SignedXml signedXml = new SignedXml(doc);
            // signedXml.SigningKey = cert.GetRSAPrivateKey();  // frameWork 4.7 / 4.8
            signedXml.SigningKey = cert.PrivateKey; // Compatible con 4.5.1

            Reference reference = new Reference();
            reference.Uri = "";

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());

            signedXml.AddReference(reference);
            signedXml.ComputeSignature();

            XmlElement xmlDigitalSignature = signedXml.GetXml();
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            return doc.OuterXml;
        }
    }
}
