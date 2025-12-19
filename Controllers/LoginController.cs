using Microsoft.AspNetCore.Mvc;
using VoteAqui.DTOs;
using VoteAqui.Models;
using VoteAqui.Services;

namespace VoteAqui.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioService _usuarioService;

        public LoginController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastroUsuarioDto usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var sucesso = await _usuarioService.CadastrarUsuarioAsync(usuario);

                    if (sucesso)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "Usuário cadastrado com sucesso!"
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "Falha ao cadastrar usuário na API"
                        });
                    }
                }
                else
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(new { 
                        success = false, 
                        message = "Dados inválidos",
                        errors = errors 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Erro ao cadastrar usuário: " + ex.Message 
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resultado = await _usuarioService.LoginAsync(login);

                    if (resultado != null)
                    {
                        HttpContext.Session.SetString("UserEmail", login.Email);
                        HttpContext.Session.SetString("UserId", resultado.Id.ToString());

                        return Ok(new
                        {
                            success = true,
                            message = "Login realizado com sucesso!",
                            userEmail = login.Email
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            success = false,
                            message = "E-mail ou senha inválidos"
                        });
                    }
                }
                else
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage);
                    
                    return BadRequest(new { 
                        success = false, 
                        message = "Dados inválidos",
                        errors = errors 
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Erro ao fazer login: " + ex.Message 
                });
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
