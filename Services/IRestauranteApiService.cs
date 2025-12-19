using System.Collections.Generic;
using System.Threading.Tasks;
using VoteAqui.DTOs;
using System.Text.Json;

namespace VoteAqui.Services
{
    public class RestauranteGanhadorDto
    {
        public string RestauranteNome { get; set; } = string.Empty;
        public int TotalVotos { get; set; }
    }

    public interface IRestauranteApiService
    {
        Task<List<RestauranteDto>> GetRestaurantesAsync();
        Task<bool> InserirRestauranteAsync(RestauranteDto restaurante);
        Task<VotoResponseDto> VotarAsync(VotoDto voto);
        Task<object> GetEstatisticasAsync();
        Task<bool> VerificarRestauranteEscolhidoEstaSemanaAsync(Guid restauranteId);
        Task<bool> SalvarCampeaoDoDiaAsync(UltimoCampeaoDto campeao);
        Task<UltimoCampeaoDto> ObterUltimoCampeaoAsync();
        bool VerificarHorarioBloqueioVotacao();
        Task<bool> GetBloqueioDataAsync(Guid userId);
        Task<object> GetContagemRestauranteGanhandoAsync();
        Task<bool> BloquearRestauranteAsync(Guid restauranteId, string NomeRestaurante);

    }

    public class RestauranteDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
    }

    public class VotoResponseDto
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }


  
}
