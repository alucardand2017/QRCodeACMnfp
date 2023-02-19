using System.Collections.Generic;

namespace QRCodeACMnfp.Domain
{
    class InformarcoesRelatorio
    {
        public List<NotaFiscalValoresl> Notas { get; set; } = new List<NotaFiscalValoresl>();
       
        public float ValorTotal()
        {
            float soma = 0f;
            foreach (var item in Notas)
            {
                soma += float.Parse(item.Valor);
            }
            return soma;
        }
    }
}
