using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkillmapLib1.Models;
using Skillmap.Services;
using System.Collections.ObjectModel;

namespace Skillmap.ViewModels
{
    public partial class AllSemestersViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        [ObservableProperty]
        private ObservableCollection<SemesterItem> semestres = new();

        [ObservableProperty]
        private bool isNotStudent;

        public AllSemestersViewModel(HttpService httpService)
        {
            _httpService = httpService;
            CargarSemestres();
            VerificarRol();
        }

        private async void VerificarRol()
        {
            var rol = await SecureStorage.GetAsync("userRole");
            IsNotStudent = rol == "Admin" || rol == "Teacher";
        }

        [RelayCommand]
        public async void CargarSemestres()
        {
            try
            {
                var subjects = await _httpService.GetSubjects();

                var grouped = subjects
                    .OrderBy(s => int.Parse(s.Semester.Split('°')[0]))
                    .GroupBy(s => s.Semester)
                    .Select(g => new SemesterItem
                    {
                        Name = g.Key,
                        Subjects = g.ToList()
                    });

                Semestres = new ObservableCollection<SemesterItem>(grouped);
            }
            catch (Exception)
            {
                // Manejo opcional de errores aquí
            }
        }
    }
}
