using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ResourceAZ.Infrastructure
{
    class Reg
    {
        static public string ProgramNameIn = "РесурсАнода";
        static public string ProgramNameOut = "РесурсАнодаОтвет";
        //string Machine;


        //-------------------------------------------------------------------------------------------------------------
        // Получение данных из файла регистрации
        //-------------------------------------------------------------------------------------------------------------
        public static string GetInf()
        {
            string s;
            s = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ResourceAZ.lic";

            try
            {
                s = File.ReadAllText(s);
                s = Encryption.Decrypt(s, ProgramNameOut);
                s = Encryption.Decrypt(s, ProgramNameOut);
            }
            catch
            {
                return "";
            }
            //s = s.Substring(s.LastIndexOf("@") + 1);
            return s;
        }


        //----------------------------------------------------------------------------------------------
        // Класс для шифровки - дешифровки паролей
        //----------------------------------------------------------------------------------------------
        public class Encryption
        {
            public static string Encrypt(string clearText, string EncryptionKey = "wonDfTDseHFDVtgmFh")
            {
                byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);

                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x77, 0x41, 0x61, 0xb3, 0x87, 0xa1, 0x21, 0x14, 0x65, 0x1b, 0x78, 0x11, 0x99 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        clearText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return clearText;
            }

            //-----------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------
            public static byte[] DecryptByte(string cipherText, string EncryptionKey = "wonDfTDseHFDVtgmFh")
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x77, 0x41, 0x61, 0xb3, 0x87, 0xa1, 0x21, 0x14, 0x65, 0x1b, 0x78, 0x11, 0x99 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherBytes = ms.ToArray();
                    }
                }
                return cipherBytes;

            }

            //-----------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------
            public static string Decrypt(string cipherText, string EncryptionKey = "wonDfTDseHFDVtgmFh")
            {
                //            string EncryptionKey = "EUimfhd5AeRT7q1";
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x77, 0x41, 0x61, 0xb3, 0x87, 0xa1, 0x21, 0x14, 0x65, 0x1b, 0x78, 0x11, 0x99 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return cipherText;
            }

            //-----------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------
            public static string GetMACAddress()
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();

                string MACAddress = String.Empty;

                foreach (ManagementObject mo in moc)
                {
                    if (MACAddress == String.Empty)
                    { // only return MAC Address from first card
                        if ((bool)mo["IPEnabled"] == true) MACAddress = mo["MacAddress"].ToString();
                    }
                    mo.Dispose();
                }

                return MACAddress;
            }

            //-----------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------
            public static string UniqueMachineId()
            {
                StringBuilder builder = new StringBuilder();

                String query = "SELECT * FROM Win32_BIOS";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
                //  This should only find one
                foreach (ManagementObject item in searcher.Get())
                {
                    Object obj = item["Manufacturer"];
                    builder.Append(Convert.ToString(obj));
                    builder.Append(':');
                    obj = item["SerialNumber"];
                    builder.Append(Convert.ToString(obj));
                }

                return builder.ToString();
            }

            //-----------------------------------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------------------------------
            public static byte[] SymmetricEncrypt(string strText, SymmetricAlgorithm key)
            {
                // Create a memory stream.
                MemoryStream ms = new MemoryStream();

                // Create a CryptoStream using the memory stream and the
                // CSP(cryptoserviceprovider) DES key.
                CryptoStream crypstream = new CryptoStream(ms, key.CreateEncryptor(), CryptoStreamMode.Write);

                // Create a StreamWriter to write a string to the stream.
                StreamWriter sw = new StreamWriter(crypstream);

                // Write the strText to the stream.
                sw.WriteLine(strText);

                // Close the StreamWriter and CryptoStream.
                sw.Close();
                crypstream.Close();

                // Get an array of bytes that represents the memory stream.
                byte[] buffer = ms.ToArray();

                // Close the memory stream.
                ms.Close();

                // Return the encrypted byte array.
                return buffer;
            }

        }
    }
}
