using Microsoft.AspNetCore.Mvc;
using VoteAqui.Services;
using VoteAqui.DTOs;
using System.Threading.Tasks;
using VotoDto = VoteAqui.DTOs.VotoDto;

namespace VoteAqui.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VotacaoController: ControllerBase
    {
        private readonly IRestauranteApiService _restauranteApiService;

        public VotacaoController(IRestauranteApiService restauranteApiService)
        {
            _restauranteApiService = restauranteApiService;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> VotarRestaurante([FromBody] VotoDto voto)
        {
            // Verificar se está no horário de bloqueio (12:01 até 23:59)
            // teste caua Verificar se está no horário de bloqueio (12:01 até 23:59)

            if (_restauranteApiService.VerificarHorarioBloqueioVotacao())
            {
                return BadRequest(new
                {
                    Sucesso = false,
                    Mensagem = "Votação encerrada! Horário permitido: até 12:00."
                });
            }

            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return BadRequest(new
                {
                    Sucesso = false,
                    Mensagem = "Usuário não está logado"
                });
            }

            if (!Guid.TryParse(userIdString, out Guid userIdParsed))
            {
                return BadRequest(new
                {
                    Sucesso = false,
                    Mensagem = "ID de usuário inválido"
                });
            }

            if (voto == null || voto.RestauranteId == Guid.Empty)
            {
                return BadRequest(new
                {
                    Sucesso = false,
                    Mensagem = "RestauranteId inválido"
                });
            }

            voto.UsuarioId = userIdParsed;

            //// Validação: verificar se restaurante já foi escolhido esta semana
            //var resultadoValidacao = await _restauranteApiService.VerificarRestauranteEscolhidoEstaSemanaAsync(voto.RestauranteId);
            //if (!resultadoValidacao)
            //{
            //    return BadRequest(new
            //    {
            //        Sucesso = false,
            //        Mensagem = "Este restaurante já foi escolhido esta semana. Escolha outra opção!"
            //    });
            //}

            var resultado = await _restauranteApiService.VotarAsync(voto);

            if (resultado.Sucesso)
            {
                return Ok(new
                {
                    Sucesso = true,
                    Mensagem = "Voto registrado com sucesso!"
                });
            }

            return BadRequest(new
            {
                Sucesso = false,
                Mensagem = "Falha ao registrar voto"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetEstatisticas()
        {
            try
            {
                var estatisticas = await _restauranteApiService.GetEstatisticasAsync();
                
                if (estatisticas != null)
                {
                    return Ok(estatisticas);
                }
                else
                {
                    return BadRequest(new
                    {
                        Sucesso = false,
                        Mensagem = "Erro ao buscar estatísticas da API externa"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Sucesso = false,
                    Mensagem = $"Erro inesperado: {ex.Message}"
                });
            }
        }

        [HttpGet("status-votacao")]
        public IActionResult GetStatusVotacao()
        {
            var bloqueado = _restauranteApiService.VerificarHorarioBloqueioVotacao();
            
            return Ok(new
            {
                Bloqueado = bloqueado,
                Mensagem = bloqueado 
                    ? "Votação encerrada! Horário permitido: 00:00 até 12:00." 
                    : "Votação liberada!",
                HorarioAtual = DateTime.Now.ToString("HH:mm:ss"),
                HorarioPermitido = "00:00 - 12:00"
            });
        }

        [HttpPost("salvar-campeao")]
        public async Task<IActionResult> SalvarCampeaoDoDia([FromBody] UltimoCampeaoDto campeao)
        {
            try
            {
                var resultado = await _restauranteApiService.SalvarCampeaoDoDiaAsync(campeao);
                
                if (resultado)
                {
                    return Ok(new { Sucesso = true, Mensagem = "Campeão salvo com sucesso!" });
                }
                else
                {
                    return BadRequest(new { Sucesso = false, Mensagem = "Erro ao salvar campeão na API" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { Sucesso = false, Mensagem = $"Erro ao salvar campeão: {ex.Message}" });
            }
        }

        [HttpGet("obter-ultimo-campeao")]
        public async Task<IActionResult> ObterUltimoCampeao()
        {
            try
            {
                var campeao = await _restauranteApiService.ObterUltimoCampeaoAsync();
                return Ok(campeao);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Sucesso = false, Mensagem = $"Erro ao obter campeão: {ex.Message}" });
            }
        }
    }
}
