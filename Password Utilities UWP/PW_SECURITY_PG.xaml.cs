using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Security_Utilities_UWP
{
    /// <summary>
    /// This Page is the Page used for checking the Security of your password
    /// </summary>
    public sealed partial class PW_SECURITY_PG : Page
    {
        public PW_SECURITY_PG()
        {
            this.InitializeComponent(); //Normal initizalization
        }

        private void Button_Click(object sender, RoutedEventArgs e) //Button Click
        {
            string pw = PW_INPUT.Password; //Get Password from Input field
            var _rating = CheckingPasswordStrength(pw); //Get Rating
            RATING_LBL.Text = "Security Rating: " + _rating.ToString().Replace("_", " "); //Set Text to Rating && removing the underscore from the var name
        }
        // This Enum declares the Values of how strong a password can be
        enum PasswordScore
        {
            Blank = 0,
            Very_Weak = 1,
            Weak = 2,
            Medium = 3,
            Strong = 4,
            Very_Strong = 5,
            OMEGA_STRONG = 6
        }

        private static PasswordScore CheckingPasswordStrength(string password)
        {
            int score = 1;
            if (password.Length < 1) // Length Check
                return PasswordScore.Blank;
            if (password.Length < 8) // Length Check
                return PasswordScore.Very_Weak;

            if (password.Length >= 12) // Length Check
                score++;
            if (password.Length >= 16) // Length Check
                score++;
            if (Regex.IsMatch(password, @"[0-9]+(\.[0-9][0-9]?)?", RegexOptions.ECMAScript))   // Number check
                score++;
            if (Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z]).+$", RegexOptions.ECMAScript)) // Lower and Upper case check
                score++;
            if (Regex.IsMatch(password, @"[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript)) // Special Character Check - some are still missing
                score++;
            return (PasswordScore)score;
        }
    }
}
