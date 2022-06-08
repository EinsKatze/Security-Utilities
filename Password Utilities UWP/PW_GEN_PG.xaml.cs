using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Windows;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Text.RegularExpressions;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Password_Utilities_UWP
{
    /// <summary>
    /// This page is used to generate a password and copy it to your Clipboard.
    /// </summary>
    public sealed partial class PW_GEN_PG : Page
    {
        public PW_GEN_PG()
        {
            this.InitializeComponent(); // Normal Start
        }
        /// <summary>
        /// This Method generates a CRYPTOGRAPHICALLY SAFE string
        /// </summary>
        /// <param name="length">Length of the String</param>
        /// <param name="characterSet">Characters the String can contain</param>
        /// <returns>Cryptographically safe string</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars should be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }
        /// <summary>
        /// Generates a password and puts it into the Textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<char> charSet; // New Charset to declare what chars should be used in the password generating process
            charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";            
            if (SPECIAL_CHARS_TOGGLE.IsOn) // Check if the user wants special characters
            {
                charSet += "öäüÖÄÜ,;.:-_#'+*~´`ß?\\=})]([/{&%$§\"!^°<>|@€"; // Add SpecialChars to the Characterselection
            }
            if (NUMBER_TOGGLE.IsOn) // Check if the user wants numbers in the password
            {
                charSet += "1234567890"; // Add numbers to the Charselection
            }
            var pw = GetRandomString((int)PW_LEN_SLIDER.Value, charSet); // Generate the password and assign it to "pw"
            PW_RESULT.Text = pw; // Put the password into the TextBox
            
        }
        /// <summary>
        /// Copies the password to your clipboard and sends an notification that is has been copied to your clipboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var clipboardPW = new DataPackage(); // New Datapackage Variable because Clipboard.SetContent needs a DataPackage Input
            clipboardPW.SetText(PW_RESULT.Text); // Set the Value of the DataPackage Variable to the Password
            Clipboard.SetContent(clipboardPW); // Actually Copy the Value of the Variable to the Clipboard.
            new ToastContentBuilder() // New Notifiation
            .AddText("Copied Clipboard!") //Setting "Header" Text of Notification
            .AddText("Successfully copied the password to your Clipboard!") //Setting "Body" Text of Notification
            .Show(); //Show the Notification
        }
    }
}
