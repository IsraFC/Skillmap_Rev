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
                var response = await _client.GetAsync("User");
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

        // SUBJECTS ------------------------------------------------------------------
        public async Task<List<SubjectOutputDTO>> GetSubjects()
        {
            var response = await _client.GetAsync("Subject");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var subjects = await JsonSerializer.DeserializeAsync<List<SubjectOutputDTO>>(stream, _serializerOptions);
            return subjects ?? [];
        }

        public async Task<HttpResponseMessage> CreateSubject(SubjectInputDTO subject)
        {
            return await _client.PostAsJsonAsync("Subject", subject, _serializerOptions);
        }

        public async Task<HttpResponseMessage> UpdateSubject(int id, SubjectInputDTO subject)
        {
            return await _client.PutAsJsonAsync($"Subject/{id}", subject, _serializerOptions);
        }

        public async Task DeleteSubject(int id)
        {
            await _client.DeleteAsync($"Subject/{id}");
        }

        // RESOURCE FEEDBACK ---------------------------------------------------------
        public async Task<List<ResourceFeedbackOutputDTO>> GetFeedbacks()
        {
            var response = await _client.GetAsync("ResourceFeedback");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var feedbacks = await JsonSerializer.DeserializeAsync<List<ResourceFeedbackOutputDTO>>(stream, _serializerOptions);
            return feedbacks ?? [];
        }

        public async Task<HttpResponseMessage> CreateFeedback(ResourceFeedbackInputDTO feedback)
        {
            return await _client.PostAsJsonAsync("ResourceFeedback", feedback, _serializerOptions);
        }

        public async Task<HttpResponseMessage> UpdateFeedback(int idResource, string username, ResourceFeedbackInputDTO feedback)
        {
            return await _client.PutAsJsonAsync($"ResourceFeedback/{idResource}/{username}", feedback, _serializerOptions);
        }

        public async Task DeleteFeedback(int idResource, string username)
        {
            await _client.DeleteAsync($"ResourceFeedback/{idResource}/{username}");
        }

        // SUBJECT-RESOURCE ----------------------------------------------------------
        public async Task<HttpResponseMessage> CreateSubjectResource(SubjectResourceInputDTO sr)
        {
            return await _client.PostAsJsonAsync("SubjectResource", sr, _serializerOptions);
        }

        public async Task DeleteSubjectResource(int idSubject, int idResource)
        {
            await _client.DeleteAsync($"SubjectResource/{idSubject}/{idResource}");
        }

        // RESOURCE TYPES ------------------------------------------------------------
        public async Task<List<ResourceType>> GetResourceTypes()
        {
            var response = await _client.GetAsync("ResourceType");
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var types = await JsonSerializer.DeserializeAsync<List<ResourceType>>(stream, _serializerOptions);
            return types ?? [];
        }

        public async Task<HttpResponseMessage> CreateResourceType(ResourceType type)
        {
            return await _client.PostAsJsonAsync("ResourceType", type, _serializerOptions);
        }

        public async Task DeleteResourceType(string id)
        {
            await _client.DeleteAsync($"ResourceType/{id}");
        }
    }
}
