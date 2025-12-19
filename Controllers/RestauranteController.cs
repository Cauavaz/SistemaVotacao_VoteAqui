using Microsoft.AspNetCore.Mvc;
using VoteAqui.Services;
using VoteAqui.DTOs;
using System.Threading.Tasks;

namespace VoteAqui.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RestauranteController : ControllerBase
    {
        private readonly IRestauranteApiService _restauranteApiService;

        public RestauranteController(IRestauranteApiService restauranteApiService)
        {
            _restauranteApiService = restauranteApiService;
        }

        [HttpGet("GetRestaurante")]
        public async Task<ActionResult<List<RestauranteDto>>> GetRestaurante()
        {
            var restaurantes = await _restauranteApiService.GetRestaurantesAsync();
            return Ok(restaurantes);
        }

        [HttpGet("GetBloqueioData")]
        public async Task<ActionResult<bool>> GetBloqueioData()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return BadRequest("Usuário não está logado");
            }
            
            var result = await _restauranteApiService.GetBloqueioDataAsync(userId);
            return Ok(result);
        }

        [HttpGet("GetContagemRestauranteGanhando")]
        public async Task<ActionResult<object>> GetContagemRestauranteGanhando()
        {
            var result = await _restauranteApiService.GetContagemRestauranteGanhandoAsync();
            return Ok(result);
        }

        [HttpPost("InserirRestaurante")]
        public async Task<IActionResult> InserirRestaurante([FromBody] RestauranteDto data)
        {
            try
            {
                string nome = data.Nome;
                string endereco = data.Endereco;

                var novoRestaurante = new RestauranteDto
                {
                    Id = Guid.NewGuid(),
                    Nome = nome,
                    Endereco = endereco
                };

                var sucesso = await _restauranteApiService.InserirRestauranteAsync(novoRestaurante);

                if (sucesso)
                {
                    return Ok(new
                    {
                        Sucesso = true,
                        Mensagem = "Restaurante inserido com sucesso!"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Sucesso = false,
                        Mensagem = "Falha ao inserir restaurante na API"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("BloquearRestaurante")]
        public async Task<IActionResult> BloquearRestaurante([FromBody] BloqueioRestauranteDto data)
        {
            try
            {
                if (data == null || data.RestauranteId == Guid.Empty)
                {
                    return BadRequest(new
                    {
                        Sucesso = false,
                        Mensagem = "Dados do restaurante inválidos"
                    });
                }

                var sucesso = await _restauranteApiService.BloquearRestauranteAsync(data.RestauranteId, data.RestauranteNome);

                if (sucesso)
                {
                    return Ok(new
                    {
                        Sucesso = true,
                        Mensagem = $"Restaurante Ganhador: ({data.RestauranteNome})"
                    });
                }
                else
               {
                    return BadRequest(new
                    {
                        Sucesso = false,
                        Mensagem = "Restaurante ja foi escolhido como ganhador essa semana!"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao bloquear restaurante: {ex.Message}"
                });
            }
        }

    }
}
