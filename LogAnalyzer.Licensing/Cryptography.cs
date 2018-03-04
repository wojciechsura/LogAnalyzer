using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Licensing
{
    static class Cryptography
    {
        public static void SaveRSAKeys(RSAParameters parameters, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(parameters.D.Length);
            if (parameters.D.Length > 0)
                writer.Write(parameters.D);

            writer.Write(parameters.DP.Length);
            if (parameters.DP.Length > 0)
                writer.Write(parameters.DP);

            writer.Write(parameters.DQ.Length);
            if (parameters.DQ.Length > 0)
                writer.Write(parameters.DQ);

            writer.Write(parameters.Exponent.Length);
            if (parameters.Exponent.Length > 0)
                writer.Write(parameters.Exponent);

            writer.Write(parameters.InverseQ.Length);
            if (parameters.InverseQ.Length > 0)
                writer.Write(parameters.InverseQ);

            writer.Write(parameters.Modulus.Length);
            if (parameters.Modulus.Length > 0)
                writer.Write(parameters.Modulus);

            writer.Write(parameters.P.Length);
            if (parameters.P.Length > 0)
                writer.Write(parameters.P);

            writer.Write(parameters.Q.Length);
            if (parameters.Q.Length > 0)
                writer.Write(parameters.Q);
        }

        public static void SavePublicRSAKey(RSAParameters parameters, Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write((Int32)0);

            writer.Write((Int32)0);

            writer.Write((Int32)0);

            writer.Write(parameters.Exponent.Length);
            if (parameters.Exponent.Length > 0)
                writer.Write(parameters.Exponent);

            writer.Write((Int32)0);

            writer.Write(parameters.Modulus.Length);
            if (parameters.Modulus.Length > 0)
                writer.Write(parameters.Modulus);

            writer.Write((Int32)0);

            writer.Write((Int32)0);
        }

        public static RSAParameters LoadRSAKeys(Stream stream)
        {
            RSAParameters result = new RSAParameters();
            int len;

            BinaryReader reader = new BinaryReader(stream);

            len = reader.ReadInt32();
            if (len > 0)
                result.D = reader.ReadBytes(len);

            len = reader.ReadInt32();
            if (len > 0)
                result.DP = reader.ReadBytes(len);

            len = reader.ReadInt32();
            if (len > 0)
                result.DQ = reader.ReadBytes(len);

            len = reader.ReadInt32();
            if (len > 0)
                result.Exponent = reader.ReadBytes(len);

            len = reader.ReadInt32();
            if (len > 0)
                result.InverseQ = reader.ReadBytes(len);

            len = reader.ReadInt32();
            if (len > 0)
                result.Modulus = reader.ReadBytes(len);

            len = reader.ReadInt32();
            if (len > 0)
                result.P = reader.ReadBytes(len);

            len = reader.ReadInt32();
            if (len > 0)
                result.Q = reader.ReadBytes(len);

            return result;
        }
    }
}
