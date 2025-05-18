using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.InputDTO;
using SkillmapLib1.Models.DTO.OutputDTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Skillmap.Services
{
    public class HttpService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _serializerOptions;
        private string? _authorizationKey;

        public HttpService(HttpClient client)
        {
            _client = client;
            _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // Inicializa el cliente con token y encabezados
        public async Task<bool> InitializeClient(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(_authorizationKey))
                {
                    var response = await Login(email, password);
                    if (!response.IsSuccessStatusCode)
                        return false;

                    var contentStream = await response.Content.ReadAsStreamAsync();
                    var token = await JsonSerializer.DeserializeAsync<UserTokenOutputDTO>(contentStream, _serializerOptions);
                    _authorizationKey = token?.AccessToken;

                    if (!string.IsNullOrEmpty(_authorizationKey))
                    {
                        await SecureStorage.SetAsync("accessToken", _authorizationKey);

                        var user = await GetCurrentUser();
                        if (user != null)
                        {
                            await SecureStorage.SetAsync("userName", user.UserName);
                            await SecureStorage.SetAsync("userRole", user.Role);
                        }
                    }
                }

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authorizationKey);
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en InitializeClient: {ex.Message}");
                return false;
            }
        }

        // LOGIN
        public async Task<HttpResponseMessage>  Login(string email, string password)
        {
            var loginDto = new LoginInputDTO { Email = email, Password = password };
            return await _client.PostAsJsonAsync("login", loginDto, _serializerOptions);
        }

        // OBTENER USUARIO AUTENTICADO
        public async Task<LoggedInUserOutputDTO?> GetCurrentUser()
        {
            try
            {
                var response = await _client.GetAsync("api/User");
                response.EnsureSuccessStatusCode();

                var stream = await response.Content.ReadAsStreamAsync();
                var user = await JsonSerializer.DeserializeAsync<LoggedInUserOutputDTO>(stream, _serializerOptions);

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetCurrentUser: {ex.Message}");
                return null;
            }
        }

        // OBTENER USUARIOS
        public async Task<List<UserWithRoleOutputDTO>> GetAllUsers()
        {
            var response = await _client.GetAsync("api/User/all");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var users = await JsonSerializer.DeserializeAsync<List<UserWithRoleOutputDTO>>(stream, _serializerOptions);
            return users ?? [];
        }

        // SUBJECTS ------------------------------------------------------------------
        public async Task<List<SubjectOutputDTO>> GetSubjects()
        {
            await EnsureAuthorized();

            var response = await _client.GetAsync("api/Subject");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var subjects = await JsonSerializer.DeserializeAsync<List<SubjectOutputDTO>>(stream, _serializerOptions);
            return subjects ?? [];
        }

        public async Task<HttpResponseMessage> CreateSubject(SubjectInputDTO subject)
        {
            return await _client.PostAsJsonAsync("api/Subject", subject, _serializerOptions);
        }

        public async Task<HttpResponseMessage> UpdateSubject(int id, SubjectInputDTO subject)
        {
            return await _client.PutAsJsonAsync($"api/Subject/{id}", subject, _serializerOptions);
        }

        public async Task DeleteSubject(int id)
        {
            await _client.DeleteAsync($"api/Subject/{id}");
        }

        // RESOURCE FEEDBACK ---------------------------------------------------------
        public async Task<List<ResourceFeedbackOutputDTO>> GetFeedbacks()
        {
            var response = await _client.GetAsync("api/ResourceFeedback");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var feedbacks = await JsonSerializer.DeserializeAsync<List<ResourceFeedbackOutputDTO>>(stream, _serializerOptions);
            return feedbacks ?? [];
        }

        public async Task<HttpResponseMessage> CreateFeedback(ResourceFeedbackInputDTO feedback)
        {
            return await _client.PostAsJsonAsync("api/ResourceFeedback", feedback, _serializerOptions);
        }

        public async Task<HttpResponseMessage> UpdateFeedback(int idResource, string username, ResourceFeedbackInputDTO feedback)
        {
            return await _client.PutAsJsonAsync($"api/ResourceFeedback/{idResource}/{username}", feedback, _serializerOptions);
        }

        public async Task DeleteFeedback(int idResource, string username)
        {
            await _client.DeleteAsync($"api/ResourceFeedback/{idResource}/{username}");
        }

        // SUBJECT-RESOURCE ----------------------------------------------------------
        public async Task<HttpResponseMessage> CreateSubjectResource(SubjectResourceInputDTO sr)
        {
            return await _client.PostAsJsonAsync("api/SubjectResource", sr, _serializerOptions);
        }

        public async Task<List<SubjectResource>> GetAllSubjectResources()
        {
            var response = await _client.GetAsync("api/SubjectResource");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<List<SubjectResource>>(stream, _serializerOptions);

            return result ?? [];
        }

        public async Task DeleteSubjectResource(int idSubject, int idResource)
        {
            await _client.DeleteAsync($"api/SubjectResource/{idSubject}/{idResource}");
        }

        // RESOURCE TYPES ------------------------------------------------------------
        public async Task<List<ResourceType>> GetResourceTypes()
        {
            var response = await _client.GetAsync("api/ResourceType");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var types = await JsonSerializer.DeserializeAsync<List<ResourceType>>(stream, _serializerOptions);
            return types ?? [];
        }

        public async Task<HttpResponseMessage> CreateResourceType(ResourceType type)
        {
            return await _client.PostAsJsonAsync("api/ResourceType", type, _serializerOptions);
        }

        public async Task DeleteResourceType(string id)
        {
            await _client.DeleteAsync($"api/ResourceType/{id}");
        }

        // RESOURCES ------------------------------------------------------------------
        public async Task<List<ResourcesItem>> GetResources()
        {
            var response = await _client.GetAsync("api/Resource");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var resources = await JsonSerializer.DeserializeAsync<List<ResourcesItem>>(stream, _serializerOptions);
            return resources ?? [];
        }

        // Obtener un recurso por id
        public async Task<ResourcesItem?> GetResourceById(int id)
        {
            var response = await _client.GetAsync($"api/Resource/{id}");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<ResourcesItem>(stream, _serializerOptions);
        }

        public async Task<List<ResourcePerSubjectOutputDTO>> GetResourcesBySubject(int subjectId)
        {
            var response = await _client.GetAsync($"api/SubjectResource/subject/{subjectId}");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var list = await JsonSerializer.DeserializeAsync<List<ResourcePerSubjectOutputDTO>>(stream, _serializerOptions);
            return list ?? [];
        }

        public async Task<bool> RestoreSession()
        {
            try
            {
                var token = await SecureStorage.GetAsync("accessToken");

                if (!string.IsNullOrEmpty(token))
                {
                    _authorizationKey = token;
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authorizationKey);
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        // Crear recurso
        public async Task<ResourcesItem?> CreateResource(ResourcesItem resource)
        {
            var response = await _client.PostAsJsonAsync("api/Resource", resource, _serializerOptions);

            if (!response.IsSuccessStatusCode)
                return null;

            var stream = await response.Content.ReadAsStreamAsync();
            var created = await JsonSerializer.DeserializeAsync<ResourcesItem>(stream, _serializerOptions);
            return created;
        }

        // Obtener recursos por materia
        public async Task<List<ResourcePerSubjectOutputDTO>> GetResourcesBySubjectAll()
        {
            var response = await _client.GetAsync("api/SubjectResource");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var list = await JsonSerializer.DeserializeAsync<List<ResourcePerSubjectOutputDTO>>(stream, _serializerOptions);
            return list ?? [];
        }


        // Actualizar recurso
        public async Task<HttpResponseMessage> UpdateResource(int id, ResourcesItem resource)
        {
            return await _client.PutAsJsonAsync($"api/Resource/{id}", resource);
        }

        // Eliminar recurso
        public async Task DeleteResource(int id)
        {
            await _client.DeleteAsync($"api/Resource/{id}");
        }

        private async Task EnsureAuthorized()
        {
            if (string.IsNullOrEmpty(_authorizationKey))
            {
                var token = await SecureStorage.GetAsync("accessToken");
                if (!string.IsNullOrEmpty(token))
                {
                    _authorizationKey = token;
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authorizationKey);
                    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }
            }
        }
    }
}
