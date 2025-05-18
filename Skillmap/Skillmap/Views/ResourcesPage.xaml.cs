using Skillmap.Models;

namespace Skillmap.Views;

public partial class ResourcesPage : ContentPage
{
	private List<ResourcesItem> allResources = new List<ResourcesItem>
	{
		new ResourcesItem {Id = 1, Title = "Presentación FB Ads", 
			Description = "Esta presentación proporciona una guía completa sobre la publicidad en Facebook Ads, desde los fundamentos hasta estrategias avanzadas. Se abordan temas como la creación de anuncios efectivos, la segmentación precisa del público objetivo, la optimización del presupuesto publicitario y el análisis de métricas clave para mejorar el rendimiento de las campañas. También se incluyen estudios de caso y mejores prácticas para lograr conversiones más eficientes y maximizar el retorno de inversión (ROI).",
			Link = "https://example.com/fbads", UploadDate = new DateOnly(2024, 3, 1)},
		new ResourcesItem {Id = 2, Title = "Presentación Google Ads",
			Description = "Este recurso es una presentación detallada sobre el uso de Google Ads como plataforma de publicidad digital. Se explica paso a paso cómo configurar campañas de búsqueda, display y video, eligiendo palabras clave adecuadas y creando anuncios atractivos. Además, se enseñan estrategias de puja, segmentación por audiencia y cómo utilizar herramientas como Google Analytics para medir la efectividad de las campañas. Incluye ejemplos prácticos y recomendaciones para mejorar la calidad del anuncio y reducir costos innecesarios.", 
			Link = "https://example.com/googleads", UploadDate = new DateOnly(2024, 2, 15)},
		new ResourcesItem {Id = 3, Title = "Presentación de SEO",
            Description = "En esta presentación se profundiza en las mejores prácticas de optimización para motores de búsqueda (SEO), explicando cómo mejorar la visibilidad de un sitio web en los resultados de búsqueda de Google y otros buscadores. Se analizan factores clave como la optimización de contenido, la importancia de las palabras clave, estrategias de link building y el impacto de la velocidad de carga en el posicionamiento. También se incluyen herramientas útiles para monitorear el rendimiento SEO y cómo adaptarse a los constantes cambios en los algoritmos de búsqueda.", 
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