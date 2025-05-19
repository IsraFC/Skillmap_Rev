using Skillmap.ViewModels;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.InputDTO;
using Skillmap.Services;

namespace Skillmap.Views;

public partial class EditResourcePage : ContentPage
{
    public EditResourcePage()
	{
		InitializeComponent();
        BindingContext = new EditResourceViewModel(
            (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService)));
    }
}