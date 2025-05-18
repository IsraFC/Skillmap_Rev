namespace Skillmap.Pages
{
    public partial class CrearRecursoPage : ContentPage
    {
        public CrearRecursoPage()
        {
            InitializeComponent();
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            // Validación básica
            if (string.IsNullOrWhiteSpace(IdRecursoEntry.Text) ||
                string.IsNullOrWhiteSpace(TituloEntry.Text) ||
                string.IsNullOrWhiteSpace(DescripcionEditor.Text) ||
                TipoRecursoPicker.SelectedIndex == -1 ||
                MateriaPicker.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(RutaArchivoEntry.Text))
            {
                await DisplayAlert("Campos incompletos", "Por favor llena todos los campos obligatorios marcados con *.", "Aceptar");
                return;
            }

            // Simular guardado exitoso
            string titulo = TituloEntry.Text;
            string tipo = TipoRecursoPicker.SelectedItem.ToString();
            string materia = MateriaPicker.SelectedItem.ToString();
            bool publico = EsPublicoCheckBox.IsChecked;

            await DisplayAlert("Recurso guardado", $"Se ha registrado el recurso \"{titulo}\" como {tipo} en {materia}.", "OK");

            // Aquí podrías limpiar campos o navegar
        }
    }
}
