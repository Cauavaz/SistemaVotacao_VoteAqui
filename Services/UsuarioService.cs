using System.Net.Http.Json;
using VoteAqui.DTOs;

namespace VoteAqui.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly HttpClient _httpClient;

        public UsuarioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CadastrarUsuarioAsync(CadastroUsuarioDto usuario)
        {
            try
            {
                var apiUrl = "https://localhost:7295/api/Usuario/cadastrarUsuario";
                var response = await _httpClient.PostAsJsonAsync($"{apiUrl}", usuario);

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
                var apiUrl = "https://localhost:7295/api/Usuario/login";
                var response = await _httpClient.PostAsJsonAsync($"{apiUrl}", login);

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
