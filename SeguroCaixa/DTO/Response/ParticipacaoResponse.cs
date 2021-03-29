using SeguroCaixa.Models;

namespace SeguroCaixa.DTO.Response
{
    public class ParticipacaoResponse
    {
        public short NuTipoParticipacao { get; set; }
        public PessoaResponse PessoaResponse { get; set; }
    }
}
