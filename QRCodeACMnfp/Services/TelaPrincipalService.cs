using OpenQA.Selenium;
using QRCodeACMnfp.Domain;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QRCodeACMnfp.Services
{
    internal sealed class TelaPrincipalService
    {
        internal static void AtualizarValoresTotaisNaTela(Label lancamentos, Label valorTotal, InformarcoesRelatorio informacoes)
        {
            lancamentos.Text = informacoes.Notas.Count.ToString();
            valorTotal.Text = informacoes.ValorTotal().ToString("F");
        }

        internal static void MostrarNoGridView(DataGridView dataGridView1, List<NotaFiscalValoresl> notas)
        {
            int linha = 0;
            int coluna = 0;
            foreach (NotaFiscalValoresl nota in notas)
            {
                if (linha >= 50)
                {
                    linha = 0;
                    coluna++;
                    if (coluna > 10)
                    {
                        throw new ArgumentOutOfRangeException("Não é mais possível lançar novos registros, " +
                            "Limite de campos foi alcançado.");
                    }
                    else
                    {
                        dataGridView1[coluna, linha].Value = nota.Valor.ToString();
                        linha++;
                    }
                }
                else
                {
                    dataGridView1[coluna, linha].Value = nota.Valor.ToString();
                    linha++;
                }
            }
        }

        internal static void AbrirNavegadorEmNfp(IWebDriver driver)
        {
           driver.Navigate().GoToUrl("https://www.nfp.fazenda.sp.gov.br/EntidadesFilantropicas/ListagemNotaEntidade.aspx");
        }

        internal static void ManualDoUsuario(string enderecoSave)
        {
            MessageBox.Show("MANUAL DO USUÁRIO: " +
                "\n\n 1) Ao iniciar a aplicação, uma tela do navegador Chrome vai abrir, faça o login e navegue nas informações até chegar no formulário 'Cadastro de doação de documento fiscal' normalmente." +
                " Em seguida selecione o modo de uso (automático ou manual). Por fim, escaneie o QRCode do documento. Fácil né ? :)" +
                "\n\n 2) Alguns documentos, apesar de possuir código de barras, podem apresentar falhas na impressão (o que gera a demora para leitura dos dados). Além disso," +
                "alguns documentos não são exatamente 'Cupom fiscal Eletronico', são de outra natureza, como 'Documento auxiliar da Nota Fiscal Eletrônica' por exemplo. Esses documentos não são" +
                " lidos pelo programa corretamente." +
                "\n\n 3) Quando escaneado, o programa preenche os campo da página com os valores mostrados nos respectivos campos do programa. " +
                "\n\n 4) As escolha 'automática' envia os dados para o site e 'Salva a Nota', estando apto a escanear a próxima." +
                "\n\n 5) A escolha 'manual' apenas preenche o formulário do Site, cabendo a você salvar a nota no site, e retornar ao programa no campo 'QRCode'." +
                "\n\n 6) O relatório quando emitido gera um arquivo do tipo excel no endereço " + enderecoSave + " com a data e hora atual." +
                "\n\n 7) Após salvar o Relatório, feche o programa no botão 'Fechar App' e inicie um novo processo." +
                "\n\n 8) Não é possível emitir o relatório se fechar o programa antes, sendo necessário preencher a planilha manualmente.");

        }

        internal static void InformacoesContato(LinkLabel linkLabel1)
        {
            var result = MessageBox.Show("Você será direcionado para o perfil profissional do Desenvolvedor.", "Info.", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
                VisitLink( linkLabel1);
        }

        internal static void LimpaFormulario(TextBox txtQRCode, TextBox txtCNPJ, TextBox txtData, TextBox txtExtrato, MaskedTextBox mskValor)
        {
            txtQRCode.Text.Trim();
            txtQRCode.Text = string.Empty;
            txtQRCode.ClearUndo();
            txtCNPJ.Clear();
            txtData.Clear();
            txtExtrato.Clear();
            mskValor.Clear();
        }

        internal static void PerguntaDeFecharAplicacao(IWebDriver driver)
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            var result = MessageBox.Show("Já verificou se o relatorio foi emitido?", "LEMBRETE!", buttons, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                driver.Dispose();
                Application.Exit();
            }
        }

        internal static DialogResult PerguntaDeLimparTodosRegistros()
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            var result = MessageBox.Show("Certeza desse comando?", "LEMBRETE!", buttons, MessageBoxIcon.Question);
            return result;
        }

        private static void VisitLink(LinkLabel linkLabel1)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.linkedin.com/in/anderson-silva-4b86413b/");
        }
    }
}
