namespace Skillmap.Pages
{
    public partial class CrearUsuarioPage : ContentPage
    {
        public CrearUsuarioPage()
        {
            InitializeComponent();

            PasswordEntry.TextChanged += (s, e) =>
            {
                var length = e.NewTextValue?.Length ?? 0;
                if (length == 0)
                {
                    PasswordStrengthBar.WidthRequest = 0;
                    PasswordStrengthBar.BackgroundColor = Color.FromArgb("#dc3545");
                }
                else if (length < 4)
                {
                    PasswordStrengthBar.WidthRequest = 40;
                    PasswordStrengthBar.BackgroundColor = Color.FromArgb("#dc3545");
                }
                else if (length < 8)
                {
                    PasswordStrengthBar.WidthRequest = 80;
                    PasswordStrengthBar.BackgroundColor = Color.FromArgb("#fd7e14");
                }
                else
                {
                    PasswordStrengthBar.WidthRequest = 160;
                    PasswordStrengthBar.BackgroundColor = Color.FromArgb("#28a745");
                }
            };
        }
    }
}
