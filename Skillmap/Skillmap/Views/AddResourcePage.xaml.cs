using SkillmapLib1.Models;
using Skillmap.Services;
using SkillmapLib1.Models.DTO.OutputDTO;
using SkillmapLib1.Models.DTO.InputDTO;
using System.Net.Http.Json;
using Microsoft.Maui.Controls.Shapes;
using Skillmap.ViewModels;

namespace Skillmap.Views
{
    public partial class AddResourcePage : ContentPage
    {
        private readonly AddResourceViewModel viewModel;

        public AddResourcePage()
        {
            InitializeComponent();
            var httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
            BindingContext = viewModel = new AddResourceViewModel(httpService);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.CargarDatosAsync();
        }
    }
}
