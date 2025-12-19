namespace VoteAqui.DTOs
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public int Senha { get; set; }
    }
}
