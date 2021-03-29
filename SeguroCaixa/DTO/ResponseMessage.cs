namespace SeguroCaixa.DTO
{
    public class ResponseMessage
    {
        
        public ResponseMessage(string Mensagem, int StatusCode)
        {
            this.Mensagem = Mensagem;
            this.StatusCode = StatusCode;
        }

        public ResponseMessage(string Mensagem)
        {
            this.Mensagem = Mensagem;
            
        }

        public ResponseMessage() {}

        public string Mensagem { get; set; }
        public int StatusCode { get; set; }
    }
}
