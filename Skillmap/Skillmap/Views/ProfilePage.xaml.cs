namespace Skillmap.Views;
using Skillmap.ViewModels;

public partial class ProfilePage : ContentPage
{
	/// <summary>
	/// Default constructor
	/// </summary>
	public ProfilePage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProfileViewModel vm)
        {
            vm.CargarUsuarioCommand.Execute(null);
        }
    }
}