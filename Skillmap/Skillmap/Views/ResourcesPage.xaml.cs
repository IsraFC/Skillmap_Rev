using SkillmapLib1.Models;
using Skillmap.Services;
using System.Threading.Tasks;
using Skillmap.ViewModels;

namespace Skillmap.Views;

public partial class ResourcesPage : ContentPage
{
    private bool isNotStudent;
    public bool IsNotStudent { get => isNotStudent; set => isNotStudent = value; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ResourcesPage()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ResourcesViewModel vm)
        {
            vm.CargarRecursosCommand.Execute(null);
        }
    }
}