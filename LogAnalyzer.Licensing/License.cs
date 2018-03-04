using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Xml.Serialization;

namespace LogAnalyzer.Licensing
{
    [Serializable]
    public class LicenseException : Exception
    {
        public LicenseException() 
        { 

        }

        public LicenseException(string message)
            : base(message)
        { 

        }

        public LicenseException(string message, Exception inner) : base(message, inner) 
        { 

        }
        protected LicenseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { 
        
        }
    }

    [XmlRoot("License")]
    public class License
    {
        private string username;

        private string module;

        private bool expires;

        private DateTime expirationDate;

        private byte[] signature;

        public static License LoadFromFile(string filename)
        {
            FileStream fs = null;

            try
            {
                fs = new FileStream(filename, FileMode.Open);

                XmlSerializer serializer = new XmlSerializer(typeof(License));
                License result = (License)serializer.Deserialize(fs);

                return result;
            }
            catch (Exception e)
            {
                throw new LicenseException("Cannot load license from file!", e);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public static void SaveToFile(License license, string filename)
        {
            if (license == null)
                throw new ArgumentNullException("license");

            FileStream fs = null;

            try
            {
                fs = new FileStream(filename, FileMode.Create);

                XmlSerializer serializer = new XmlSerializer(typeof(License));
                serializer.Serialize(fs, license);
            }
            catch (Exception e)
            {
                throw new LicenseException("Cannot save license to file!", e);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }

        public License()
        {
            username = "";
            module = "";
            expires = false;
            expirationDate = DateTime.Now;
            signature = null;
        }

        public bool VerifySignature(RSAParameters parameters)
        {
            RSACryptoServiceProvider provider = null;
            MemoryStream stream = null;

            try
            {
                provider = new RSACryptoServiceProvider();
                stream = new MemoryStream();

                provider.ImportParameters(parameters);
            
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(username);
                writer.Write(module);
                writer.Write(expires);
                writer.Write(expirationDate.ToBinary());

                SHA1Managed hashEngine = new SHA1Managed();
                byte[] hashedData = hashEngine.ComputeHash(stream.GetBuffer());
                return provider.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), signature);
            }
            finally
            {
                provider.Dispose();
                stream.Dispose();
            }
        }

        public void Sign(RSAParameters parameters)
        {
            RSACryptoServiceProvider provider = null;
            MemoryStream stream = null;

            try
            {
                provider = new RSACryptoServiceProvider();
                stream = new MemoryStream();

                provider.ImportParameters(parameters);

                if (provider.PublicOnly)
                    throw new InvalidOperationException("Provided RSA key is public only!");

                BinaryWriter writer = new BinaryWriter(stream);
                writer.Write(username);
                writer.Write(module);
                writer.Write(expires);
                writer.Write(expirationDate.ToBinary());

                SHA1Managed hashEngine = new SHA1Managed();
                byte[] hashedData = hashEngine.ComputeHash(stream.GetBuffer());

                signature = provider.SignHash(hashedData, CryptoConfig.MapNameToOID("SHA1"));                
            }
            finally
            {
                provider.Dispose();
                stream.Dispose();
            }
        }

        [XmlElement("Username")]
        public string Username
        {
            get 
            { 
                return username; 
            }
            set
            {
                username = value;
            }
        }

        [XmlElement("Module")]
        public string Module
        {
            get
            {
                return module;
            }
            set
            {
                module = value;
            }
        }

        [XmlElement("ExpirationDate")]
        public DateTime ExpirationDate
        {
            get
            {
                return expirationDate;
            }
            set
            {
                expirationDate = value;
            }
        }

        [XmlElement("Expires")]
        public bool Expires
        {
            get
            {
                return expires;
            }
            set
            {
                expires = value;
            }
        }

        [XmlElement("Signature")]
        public byte[] Signature
        {
            get
            {
                return signature;
            }
            set
            {
                signature = value;
            }
        }

        public bool Signed
        {
            get
            {
                return signature != null;
            }
        }
    }
}
