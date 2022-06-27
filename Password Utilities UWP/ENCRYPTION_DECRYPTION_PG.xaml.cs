using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Security_Utilities_UWP
{
    public sealed partial class ENCRYPTION_DECRYPTION_PG : Page
    {
        public ENCRYPTION_DECRYPTION_PG()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string computeThis = TXT_INPUT.Text;
            string key = INPUT_KEY.Text;
            var item = ((ComboBoxItem)METHOD_SELECTION.SelectedItem).Content.ToString();
            
            if(item == "Encrypt")
            {
                OUTPUT.Text = EncryptString(computeThis, key);
            }
            if(item == "Decrypt")
            {
                OUTPUT.Text = DecryptString(computeThis, key);
            }
        }

        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        private const int Keysize = 128;

        // This constant determines the number of iterations for the password bytes generation function.
        private const int DerivationIterations = 1000;

        public static string EncryptString(string clearText, string Key)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                byte[] IV = GenerateBitsOfRandomEntropy(15);
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, IV);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(IV) + Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string DecryptString(string cipherText, string Key)
        {
            byte[] IV = Convert.FromBase64String(cipherText.Substring(0, 20));
            cipherText = cipherText.Substring(20).Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, IV);
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    try
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                    catch (CryptographicException)
                    {
                        cipherText = "ERROR: Wrong Decryption Key.";
                    }
                }
            }
            return cipherText;
        }

        private static byte[] GenerateBitsOfRandomEntropy(int size)
        {
            // 32 Bytes will give us 256 bits.
            // 16 Bytes will give us 128 bits.
            var randomBytes = new byte[size];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        private void CopyToClipboardBtn_Click(object sender, RoutedEventArgs e)
        {
            var clipboardPW = new DataPackage(); // New Datapackage Variable because Clipboard.SetContent needs a DataPackage Input
            clipboardPW.SetText(OUTPUT.Text); // Set the Value of the DataPackage Variable to the Password
            Clipboard.SetContent(clipboardPW); // Actually Copy the Value of the Variable to the Clipboard.
        }
    }
}
