using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RegProgramm
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string ProgramNameIn = "РесурсАнода";
        static string ProgramNameOut = "РесурсАнодаОтвет";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Файлы запроса (*.gen)|*.gen";

            if (dlg.ShowDialog() == false)
                return;

            tbGet.Content = dlg.FileName;

        }

        private void BtReg_Click(object sender, RoutedEventArgs e)
        {
            int Days = 0;

            if(!int.TryParse(tbDays.Text, out Days))
            {
                MessageBox.Show("Некорректное количество дней","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string s = File.ReadAllText(tbGet.Content.ToString());
            try
            {
                DateTime dt = DateTime.Now.AddDays(Days);

                s = Encryption.Decrypt(s, ProgramNameIn);
                s = Encryption.Decrypt(s, ProgramNameIn) + "@" + dt.ToString();

                s = Encryption.Encrypt(s, ProgramNameOut);
                s = Encryption.Encrypt(s, ProgramNameOut);

                string saveFile = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\ResourceAZ.lic";
                File.WriteAllText(saveFile, s);
                tbAnswer.Content = saveFile;
            }
            catch
            {
                MessageBox.Show("Содержимое файле неверно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //btSend.IsEnabled = true;

        }

        private void TbGet_Drop(object sender, DragEventArgs e)
        {
            string[] s = e.Data.GetFormats();

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // можно же перетянуть много файлов, так что....
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                tbGet.Content = files[0];
            }
        }
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
