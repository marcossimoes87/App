using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace SeguroCaixa.DTO
{
    public class ResponseError : ResponseMessage
    {
        public int Codigo { get; set; }
        public string Mensagem { get; set; }
        public string[] Detalhes { get; set; }
        public string version { get; set; }     

        public ResponseError(string userMessage)
        {
            this.Mensagem = userMessage;
            this.version = "1.0.0";
            this.StatusCode = 409;
        }
        public static ResponseError FromModelStateError(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(v => v.Errors);
            return new ResponseError("Erro no modelo enviado")
            {
                Codigo = 100,
                Mensagem = "Houve erro(s) na validação da requisição",
                Detalhes = errors.Select(e => e.ErrorMessage).ToArray(),
            };
        }
    }
}
