namespace Skillmap.Pages;

public partial class EditProfilePage : ContentPage
{
	public EditProfilePage()
	{
		InitializeComponent();
	}

    private async void OnSaveClicked(object sender, EventArgs e)
    {
		await DisplayAlert("Perfil Actualizado", "Tu perfil se ha actualizado correctamente", "Ok");
		await Navigation.PopAsync();
    }
}