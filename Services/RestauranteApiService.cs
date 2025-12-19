using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using VoteAqui.DTOs;
using static System.Net.WebRequestMethods;

namespace VoteAqui.Services
{
    public class RestauranteApiService : IRestauranteApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RestauranteApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<RestauranteDto>> GetRestaurantesAsync()
        {
            try
            {
                var apiUrl = "https://localhost:7295/api/Restaurante";
                var response = await _httpClient.GetFromJsonAsync<List<RestauranteDto>>($"{apiUrl}");
                return response ?? new List<RestauranteDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar restaurantes: {ex.Message}");
                return new List<RestauranteDto>();
            }
        }

        public async Task<bool> GetBloqueioDataAsync(Guid userId)
        {
            try
            {
                var apiUrl = $"https://localhost:7295/api/restaurante/meu-status-votos?userId={userId}";
                var response = await _httpClient.GetFromJsonAsync<object>(apiUrl);

                if (response is JsonElement json)
                {
                    return json.GetBoolean();
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar restaurantes: {ex.Message}");
                return false;
            }
        }



        public async Task<bool> InserirRestauranteAsync(RestauranteDto restaurante)
        {
            try
            {
                var apiUrl = "https://localhost:7295/api/Restaurante";
                var response = await _httpClient.PostAsJsonAsync($"{apiUrl}", restaurante);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao inserir restaurante: {ex.Message}");
                return false;
            }
        }

        public async Task<VotoResponseDto> VotarAsync(VotoDto voto)
        {
            try
            {
                Console.WriteLine($"Enviando voto para API externa: RestauranteId={voto?.RestauranteId}, UsuarioId={voto?.UsuarioId}");
                
                var apiUrl = "https://localhost:7295/api/Voto/votar";
                var response = await _httpClient.PostAsJsonAsync($"{apiUrl}", voto);
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Voto registrado com sucesso");
                    return new VotoResponseDto
                    {
                        Sucesso = true,
                        Mensagem = "Voto registrado com sucesso!"
                    };
                }
                else
                {
                    Console.WriteLine($"Erro na API externa: {response.StatusCode}");
                    return new VotoResponseDto
                    {
                        Sucesso = false,
                        Mensagem = "Erro ao registrar voto na API externa"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao chamar API externa: {ex.Message}");
                return new VotoResponseDto
                {
                    Sucesso = false,
                    Mensagem = "Falha ao registrar voto na API"
                };
            }
        }

        public async Task<bool> VerificarRestauranteEscolhidoEstaSemanaAsync(Guid restauranteId)
        {
            try
            {
                var url = $"https://localhost:7295/api/voto/verificacao/restaurante-semana?restauranteId={restauranteId}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return false;

                return await response.Content.ReadFromJsonAsync<bool>();
            }
            catch
            {
                return false;
            }
        }



        public async Task<object> GetEstatisticasAsync()
        {
            try
            {
                Console.WriteLine("Buscando estatísticas da API externa");
                
                var response = await _httpClient.GetAsync("https://localhost:7295/api/Voto/estatisticas");
                
                if (response.IsSuccessStatusCode)
                {
                    var estatisticas = await response.Content.ReadFromJsonAsync<object>();
                    Console.WriteLine("Estatísticas obtidas com sucesso");
                    return estatisticas;
                }
                else
                {
                    Console.WriteLine($"Erro na API externa: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar estatísticas: {ex.Message}");
                return null;
            }
        }

        public async Task<object> Login()
        {
            try
            {
                Console.WriteLine("Buscando estatísticas da API externa");

                var response = await _httpClient.GetAsync("https://localhost:7295/api/Voto/estatisticas");

                if (response.IsSuccessStatusCode)
                {
                    var estatisticas = await response.Content.ReadFromJsonAsync<object>();
                    Console.WriteLine("Estatísticas obtidas com sucesso");
                    return estatisticas;
                }
                else
                {
                    Console.WriteLine($"Erro na API externa: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar estatísticas: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SalvarCampeaoDoDiaAsync(UltimoCampeaoDto campeao)
        {
            try
            {
                var logEntry = $"[{DateTime.Now:dd/MM/yyyy HH:mm:ss}] CAMPEÃO DO DIA: {campeao.RestauranteNome} - {campeao.TotalVotos} votos\n";
                
                // Salvar em arquivo de log
                var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "campeoes.log");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));
                
                await System.IO.File.AppendAllTextAsync(logPath, logEntry);
                
                var ultimoCampeaoPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "ultimo_campeao.txt");
                var ultimoCampeaoText = $"{campeao.RestauranteNome}|{campeao.TotalVotos}|{DateTime.Now:dd/MM/yyyy}|{DateTime.Now:HH:mm}";
                await System.IO.File.WriteAllTextAsync(ultimoCampeaoPath, ultimoCampeaoText);
                
                Console.WriteLine($"Campeão salvo no log: {campeao.RestauranteNome}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar campeão no log: {ex.Message}");
                return false;
            }
        }

        public async Task<UltimoCampeaoDto> ObterUltimoCampeaoAsync()
        {
            try
            {
                var ultimoCampeaoPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "ultimo_campeao.txt");
                
                if (System.IO.File.Exists(ultimoCampeaoPath))
                {
                    var content = await System.IO.File.ReadAllTextAsync(ultimoCampeaoPath);
                    var parts = content.Split('|');
                    
                    if (parts.Length >= 4)
                    {
                        return new UltimoCampeaoDto
                        {
                            RestauranteNome = parts[0],
                            TotalVotos = int.Parse(parts[1]),
                            DataVotacao = parts[2],
                            HorarioVotacao = parts[3]
                        };
                    }
                }
                
                return new UltimoCampeaoDto 
                { 
                    RestauranteNome = "Nenhum campeão registrado", 
                    TotalVotos = 0,
                    DataVotacao = "-",
                    HorarioVotacao = "-"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter campeão do log: {ex.Message}");
                return new UltimoCampeaoDto 
                { 
                    RestauranteNome = "Erro ao carregar campeão", 
                    TotalVotos = 0,
                    DataVotacao = "-",
                    HorarioVotacao = "-"
                };
            }
        }

        public bool VerificarHorarioBloqueioVotacao()
        {
            var agora = DateTime.Now;
            var hora = agora.Hour;
            var minuto = agora.Minute;
            
            // Bloquear das 12:01 até 23:59
            if (hora > 14 || (hora == 14 && minuto > 57))
            {
                return true; // Bloqueado
            }
            
            return false; // Liberado
        }

        }
}
