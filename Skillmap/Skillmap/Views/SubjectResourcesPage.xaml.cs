using SkillmapLib1.Models.DTO.OutputDTO;
using Skillmap.Services;
using Skillmap.ViewModels;

namespace Skillmap.Views;

public partial class SubjectResourcesPage : ContentPage
{
    public SubjectResourcesPage(SubjectOutputDTO subject)
	{
		InitializeComponent();
        if (BindingContext is SubjectResourcesViewModel vm)
        {
            vm.MateriaSeleccionada = subject;
            vm.CargarRecursosCommand.Execute(null);
        }
    }
}