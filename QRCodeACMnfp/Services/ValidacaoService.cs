using System;
using System.Linq;
using System.Windows.Forms;

namespace QRCodeACMnfp.Services
{
    class ValidacaoService
    {
        internal static void SepararCamposQRCode(string qRCodeAll, TextBox txtCNPJ, TextBox txtData, TextBox txtExtrato, MaskedTextBox mskValor)
        {
            if (qRCodeAll.Length > 60)
            {
                var qrCodeLimpo = qRCodeAll.Trim();
                var cnpj = qrCodeLimpo.Substring(6, 14);
                var extrato = qrCodeLimpo.Substring(31, 6);
                var dataAno = qrCodeLimpo.Substring(45, 4);
                var dataMes = qrCodeLimpo.Substring(49, 2);
                var dataDia = qrCodeLimpo.Substring(51, 2);
                var data = dataDia + dataMes + dataAno;

                var valor = new string(qrCodeLimpo.Substring(60).TakeWhile(c => c != '|').ToArray()); //precisa ser melhorado.
                txtCNPJ.Text = cnpj;
                txtData.Text = data;
                txtExtrato.Text = extrato;
                mskValor.Text = valor.Replace(".", ",");
            }
            else
            {
                throw new ArgumentException("Erro ao escanear elementos do QRCode. Verifique a compatibilidade" +
                    "do código ou se escaneou sem querer o código de barras próximo.");
            }
        }

        internal static bool TestaInformacoes(TextBox txtCNPJ, TextBox txtData, TextBox txtQRCode, MaskedTextBox mskValor)
        {
            var testaValor = float.TryParse(mskValor.Text.Replace(".", ","), out float valor);
            if (!testaValor)
            {
                throw new ArgumentException("Não foi possível transformar o valor em um número válido");
            }
            if (txtCNPJ.Text.Length != 14)
            {
                throw new ApplicationException("CNPJ da nota não possui 14 dígitos");
            }
            if (txtData.Text.Length != 8)
            {
                throw new ApplicationException("A data deve estár no formato ddMMYYYY sem espaço ou caracteres especiais.");
            }
            if (valor > 2000)
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                var result = MessageBox.Show("Deseja confirmar?", "VALOR ANORMAL!", buttons);
                if (result == System.Windows.Forms.DialogResult.No)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
