using System;

namespace VoteAqui.DTOs
{
    public class BloqueioRestauranteDto
    {
        public Guid RestauranteId { get; set; }
        public string RestauranteNome { get; set; } = string.Empty;
    }
}
