using SkillmapLib1.Models.DTO.OutputDTO;
using Skillmap.Services;


namespace Skillmap.Views;

public partial class SubjectResourcesPage : ContentPage
{
    private readonly HttpService _httpService;
    private SubjectOutputDTO _subject;
    private List<ResourcePerSubjectOutputDTO> _allResources = new();

    public SubjectResourcesPage(SubjectOutputDTO subject)
	{
		InitializeComponent();
        _httpService = (HttpService)App.Current.Handler.MauiContext.Services.GetService(typeof(HttpService));
        _subject = subject;

        SubjectTitleLabel.Text = $"{_subject.Name}";
        TeacherNameLabel.Text = $"Docente: {_subject.TeacherFullName}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadResourcesBySubject();
    }

    private async Task LoadResourcesBySubject()
    {
        try
        {
            var all = await _httpService.GetResourcesBySubject(_subject.Id_Subject);
            _allResources = all;
            resourcesCollectionView.ItemsSource = _allResources;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudieron cargar los recursos: {ex.Message}", "OK");
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue?.ToLower() ?? "";
        resourcesCollectionView.ItemsSource = _allResources
            .Where(r => r.ResourceTitle.ToLower().Contains(searchText))
            .ToList();
    }

    private async void OnViewMorePressed(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is ResourcePerSubjectOutputDTO resource)
        {
            await Navigation.PushAsync(new ResourcesDetailPage(new SkillmapLib1.Models.ResourcesItem
            {
                Title = resource.ResourceTitle,
                Description = resource.Description,
                Link = resource.Link,
                UploadDate = resource.UploadDate
            }));
        }
    }

}