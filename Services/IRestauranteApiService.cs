using System.Collections.Generic;
using System.Threading.Tasks;
using VoteAqui.DTOs;

namespace VoteAqui.Services
{
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
