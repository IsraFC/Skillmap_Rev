using Skillmap.Models;

namespace Skillmap.Views;

public partial class ResourcesPage : ContentPage
{
	private List<ResourcesItem> allResources = new List<ResourcesItem>
	{
		new ResourcesItem {Id = 1, Title = "Presentaci�n FB Ads", 
			Description = "Esta presentaci�n proporciona una gu�a completa sobre la publicidad en Facebook Ads, desde los fundamentos hasta estrategias avanzadas. Se abordan temas como la creaci�n de anuncios efectivos, la segmentaci�n precisa del p�blico objetivo, la optimizaci�n del presupuesto publicitario y el an�lisis de m�tricas clave para mejorar el rendimiento de las campa�as. Tambi�n se incluyen estudios de caso y mejores pr�cticas para lograr conversiones m�s eficientes y maximizar el retorno de inversi�n (ROI).",
			Link = "https://example.com/fbads", UploadDate = new DateOnly(2024, 3, 1)},
		new ResourcesItem {Id = 2, Title = "Presentaci�n Google Ads",
			Description = "Este recurso es una presentaci�n detallada sobre el uso de Google Ads como plataforma de publicidad digital. Se explica paso a paso c�mo configurar campa�as de b�squeda, display y video, eligiendo palabras clave adecuadas y creando anuncios atractivos. Adem�s, se ense�an estrategias de puja, segmentaci�n por audiencia y c�mo utilizar herramientas como Google Analytics para medir la efectividad de las campa�as. Incluye ejemplos pr�cticos y recomendaciones para mejorar la calidad del anuncio y reducir costos innecesarios.", 
			Link = "https://example.com/googleads", UploadDate = new DateOnly(2024, 2, 15)},
		new ResourcesItem {Id = 3, Title = "Presentaci�n de SEO",
            Description = "En esta presentaci�n se profundiza en las mejores pr�cticas de optimizaci�n para motores de b�squeda (SEO), explicando c�mo mejorar la visibilidad de un sitio web en los resultados de b�squeda de Google y otros buscadores. Se analizan factores clave como la optimizaci�n de contenido, la importancia de las palabras clave, estrategias de link building y el impacto de la velocidad de carga en el posicionamiento. Tambi�n se incluyen herramientas �tiles para monitorear el rendimiento SEO y c�mo adaptarse a los constantes cambios en los algoritmos de b�squeda.", 
			Link = "https://example.com/seo", UploadDate = new DateOnly(2024, 1, 20)}
	};

	/// <summary>
	/// Default constructor
	/// </summary>
	public ResourcesPage()
	{
		InitializeComponent();
		resourcesCollectionView.ItemsSource = allResources;
	}
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue.ToLower();
        resourcesCollectionView.ItemsSource = allResources.Where(r => r.Title.ToLower().Contains(searchText)).ToList();
    }

    private async void OnViewMorePressed(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.BindingContext is ResourcesItem selectedFavorite)
        {
            await Navigation.PushAsync(new ResourcesDetailPage(new ResourcesItem
            {
                Title = selectedFavorite.Title,
                Description = selectedFavorite.Description,
                Link = "https://www.example.com",
                UploadDate = selectedFavorite.UploadDate,
            }));
        }
    }
}