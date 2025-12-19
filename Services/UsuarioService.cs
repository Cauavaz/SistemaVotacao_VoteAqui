using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using VoteAqui.DTOs;

namespace VoteAqui.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UsuarioService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<bool> CadastrarUsuarioAsync(CadastroUsuarioDto usuario)
        {
            try
            {
                var baseUrl = _configuration.GetSection("ApiSettings:BaseUrl").Value;
                var apiUrl = $"{baseUrl}api/Usuario/cadastrarUsuario";
                var response = await _httpClient.PostAsJsonAsync(apiUrl, usuario);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao cadastrar usuário: {ex.Message}");
                return false;
            }
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto login)
        {
            try
            {
                var baseUrl = _configuration.GetSection("ApiSettings:BaseUrl").Value;
                var apiUrl = $"{baseUrl}api/Usuario/login";
                var response = await _httpClient.PostAsJsonAsync(apiUrl, login);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao fazer login: {ex.Message}");
                return null;
            }
        }
    }
}
