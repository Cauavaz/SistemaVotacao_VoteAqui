using VoteAqui.DTOs;

namespace VoteAqui.Services
{
    public interface IUsuarioService
    {
        Task<bool> CadastrarUsuarioAsync(CadastroUsuarioDto usuario);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto login);
    }
}
