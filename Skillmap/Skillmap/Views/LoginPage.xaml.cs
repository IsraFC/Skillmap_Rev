using Skillmap.Services;
using Microsoft.Maui.Controls;

namespace Skillmap.Views;

public partial class LoginPage : ContentPage
{
    private bool isAdmin;
    public bool IsAdmin { get => isAdmin; set => isAdmin = value; }
    public LoginPage()
	{
		InitializeComponent();
    }
}