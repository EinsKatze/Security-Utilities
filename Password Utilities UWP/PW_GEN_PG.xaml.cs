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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Password_Utilities_UWP
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class PW_GEN_PG : Page
    {
        public PW_GEN_PG()
        {
            this.InitializeComponent();
        }
        public static string GetRandomString(int length, IEnumerable<char> characterSet)
        {
            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<char> charSet;
            bool specialChars = SPECIAL_CHARS_TOGGLE.IsOn;
            if (specialChars)
            {
                charSet = "abcdefghijklmopqrstuvwxyzöäüABCDEFGHIJKLMOPQRSTUVWXYZÖÄÜ,;.:-_#'+*~´`ß?\\0=}9)]8([7/{6&5%4$3§2\"1!^°<>|@€";
            }
            else
            {
                charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            }
            var pw = GetRandomString((int)PW_LEN_SLIDER.Value, charSet);
            PW_RESULT.Text = pw;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var clipboardPW = new DataPackage();
            clipboardPW.SetText(PW_RESULT.Text);
            Clipboard.SetContent(clipboardPW);
            new ToastContentBuilder()
            .AddText("Copied Clipboard!")
            .AddText("Successfully copied the password to your Clipboard!")
            .Show();
        }
    }
}
