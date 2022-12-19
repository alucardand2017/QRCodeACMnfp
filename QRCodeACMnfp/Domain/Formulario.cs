using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QRCodeACMnfp.Domain
{
    internal class Formulario
    {
        public TextBox Qrcode { get; set; }
        public TextBox Cnpj { get; set; }
        public TextBox Data { get; set; }
        public TextBox Extrato { get; set; }
        public MaskedTextBox Valor { get; set; }

        public Formulario(TextBox qrcode, TextBox cnpj, TextBox data, TextBox extrato, MaskedTextBox valor)
        {
            Qrcode = qrcode;
            Cnpj = cnpj;
            Data = data;
            Extrato = extrato;
            Valor = valor;
        }
    }
}
