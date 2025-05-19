namespace Skillmap.Views;

public partial class EditProfilePage : ContentPage
{
    public EditProfilePage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ViewModels.EditProfileViewModel vm)
        {
            vm.CargarDatosCommand.Execute(null);
        }
    }
}