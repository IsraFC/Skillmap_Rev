using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Skillmap.Services;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Collections.ObjectModel;
using Skillmap.Views;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Skillmap.ViewModels
{
    /// <summary>
    /// ViewModel encargado de mostrar los recursos educativos asociados a una materia específica.
    /// Incluye funcionalidades de búsqueda y navegación al detalle del recurso.
    /// </summary>
    public partial class SubjectResourcesViewModel : ObservableObject
    {
        private readonly HttpService _httpService;

        /// <summary>
        /// Lista de recursos visibles tras aplicar un filtro (si existe).
        /// </summary>
        [ObservableProperty] private ObservableCollection<ResourcePerSubjectOutputDTO> recursosFiltrados = new();

        /// <summary>
        /// Texto ingresado para filtrar los recursos por título.
        /// </summary>
        [ObservableProperty] private string textoBusqueda = string.Empty;

        /// <summary>
        /// Materia actualmente seleccionada, cuyos recursos se están visualizando.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NombreDocente))]
        [NotifyPropertyChangedFor(nameof(NombreMateria))]
        private SubjectOutputDTO? materiaSeleccionada;

        /// <summary>
        /// Nombre de la materia seleccionada (solo lectura).
        /// </summary>
        public string NombreMateria => MateriaSeleccionada?.Name ?? "";

        /// <summary>
        /// Nombre completo del docente asignado a la materia.
        /// </summary>
        public string NombreDocente => $"Docente: {MateriaSeleccionada?.TeacherFullName}";

        /// <summary>
        /// Lista completa de recursos sin filtrar, cargada desde la API.
        /// </summary>
        private List<ResourcePerSubjectOutputDTO> todosLosRecursos = new();

        /// <summary>
        /// Constructor que obtiene el servicio HTTP de forma inyectada.
        /// </summary>
        public SubjectResourcesViewModel()
        {
            _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
        }

        /// <summary>
        /// Carga los recursos educativos asociados a la materia seleccionada desde la API.
        /// Aplica el filtro si hay texto de búsqueda.
        /// </summary>
        [RelayCommand]
        private async Task CargarRecursos()
        {
            if (MateriaSeleccionada == null)
                return;

            try
            {
                var recursos = await _httpService.GetResourcesBySubject(MateriaSeleccionada.Id_Subject);
                todosLosRecursos = recursos;
                AplicarFiltro();
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudieron cargar los recursos: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Se ejecuta automáticamente cuando cambia el texto de búsqueda.
        /// Aplica el filtro a la lista de recursos.
        /// </summary>
        /// <param name="value">Nuevo texto ingresado.</param>
        partial void OnTextoBusquedaChanged(string value)
        {
            AplicarFiltro();
        }

        /// <summary>
        /// Filtra la lista de recursos según el texto de búsqueda actual.
        /// </summary>
        private void AplicarFiltro()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                RecursosFiltrados = new ObservableCollection<ResourcePerSubjectOutputDTO>(todosLosRecursos);
            }
            else
            {
                var texto = TextoBusqueda.ToLower();
                RecursosFiltrados = new ObservableCollection<ResourcePerSubjectOutputDTO>(
                    todosLosRecursos.Where(r => r.ResourceTitle.ToLower().Contains(texto)).ToList());
            }
        }

        /// <summary>
        /// Navega a la pantalla de detalle del recurso seleccionado.
        /// </summary>
        /// <param name="recurso">Recurso seleccionado.</param>
        [RelayCommand]
        private async Task VerMas(ResourcePerSubjectOutputDTO recurso)
        {
            await Shell.Current.Navigation.PushAsync(new ResourcesDetailPage(new ResourcesItem
            {
                Title = recurso.ResourceTitle,
                Description = recurso.Description,
                Link = recurso.Link,
                UploadDate = recurso.UploadDate
            }));
        }

        /// <summary>
        /// Genera y descarga un PDF con la lista de recursos de esta materia.
        /// (Versión corregida usando códigos Hexadecimales para evitar errores)
        /// </summary>
        [RelayCommand]
        public async Task ExportarPDF()
        {
            if (RecursosFiltrados == null || !RecursosFiltrados.Any())
            {
                await Shell.Current.DisplayAlert("Aviso", "No hay recursos para generar el reporte.", "OK");
                return;
            }

            try
            {
                QuestPDF.Settings.License = LicenseType.Community;

                var documento = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // 1. Configuración usando HEXADECIMALES para evitar conflictos de Color
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor("#FFFFFF");
                        page.DefaultTextStyle(x => x.FontSize(12));

                        // 2. Encabezado
                        page.Header().ShowOnce().Column(col =>
                        {
                            col.Item().Text("Reporte de Material Educativo").FontSize(20).Bold().FontColor("#1a73e8");
                            col.Item().Text($"Materia: {NombreMateria}").FontSize(16).SemiBold();
                            col.Item().Text(NombreDocente).FontSize(14).FontColor("#5f6368");
                            col.Item().PaddingTop(10).LineHorizontal(1).LineColor("#E0E0E0");
                        });

                        // 3. Tabla
                        page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(4);
                                columns.ConstantColumn(80);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(EstiloCelda).Text("Recurso").Bold();
                                header.Cell().Element(EstiloCelda).Text("Descripción").Bold();
                                header.Cell().Element(EstiloCelda).Text("Fecha").Bold();
                            });

                            foreach (var item in RecursosFiltrados)
                            {
                                table.Cell().Element(EstiloCelda).Text(item.ResourceTitle);
                                table.Cell().Element(EstiloCelda).Text(item.Description);
                                table.Cell().Element(EstiloCelda).Text(item.UploadDate.ToString("dd/MM/yyyy"));
                            }
                        });

                        // 4. Pie de página
                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                            x.Span(" | Generado por SkillMap");
                        });
                    });
                });

                // Guardar y abrir
                string nombreArchivo = $"Reporte_{NombreMateria.Replace(" ", "")}_{DateTime.Now:yyyyMMdd}.pdf";
                string rutaArchivo = Path.Combine(FileSystem.CacheDirectory, nombreArchivo);

                documento.GeneratePdf(rutaArchivo);

                await Launcher.Default.OpenAsync(new OpenFileRequest
                {
                    Title = "Abrir Reporte PDF",
                    File = new ReadOnlyFile(rutaArchivo)
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo crear el PDF: " + ex.Message, "OK");
            }
        }

        static QuestPDF.Infrastructure.IContainer EstiloCelda(QuestPDF.Infrastructure.IContainer container)
        {
            return container.BorderBottom(1).BorderColor("#E0E0E0").PaddingVertical(5).PaddingHorizontal(2);
        }
    }
}
