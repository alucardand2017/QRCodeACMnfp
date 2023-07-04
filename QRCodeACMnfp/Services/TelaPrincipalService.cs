using OpenQA.Selenium;
using InsideNotaFiscal.Domain;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace InsideNotaFiscal.Services
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
            try
            {
                driver.Navigate().GoToUrl("https://www.nfp.fazenda.sp.gov.br/EntidadesFilantropicas/ListagemNotaEntidade.aspx");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Erro ao tentar abrir o site com esse driver: ", ex.Message);
            }
        }

        internal static void ManualDoUsuario(string enderecoSave)
        {
            MessageBox.Show("MANUAL DO USUÁRIO: " +
                "\n\n 1) Ao iniciar a aplicação, uma tela do navegador 'Google Chrome vai abrir', faça o login e navegue no site até " +
                "chegar no formulário 'Cadastro de doação de documento fiscal'." +

                "\n\n 2) As escolha 'automática' envia os dados para o site e 'Salva a Nota', estando apto a escanear a próxima." +

                "\n\n 3)) A escolha 'manual' apenas preenche o formulário do Site, cabendo a você salvar a nota no site, e retornar ao programa no campo 'QRCode'." +

                "\n\n Obs: Alguns documentos, apesar de possuírem código de barras, podem apresentar falhas na impressão (o que gera a demora" +
                " para leitura dos dados). Além disso, alguns documentos não são 'Cupom fiscal Eletronico', e sim de outra natureza, como 'Documento auxiliar da Nota " +
                "Fiscal Eletrônica', entre outros. Esses documentos não são lidos pelo programa corretamente." +

                "\n\n 6) O relatório quando emitido gera um arquivo do tipo excel no endereço " + enderecoSave + " com a data e hora atual como nome (ex: 04072023120559. = 04/07/2023 as 12:05:59)");

        }

        internal static void InformacoesContato(LinkLabel linkLabel1)
        {
            var result = MessageBox.Show("Você será direcionado para o perfil profissional do Desenvolvedor.", "Info.", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
                VisitLink(linkLabel1);
        }

        internal static void FecharAplicacao(IWebDriver driver)
        {
            driver.Dispose();
            Application.Exit();
        }

        internal static DialogResult PerguntaDeLimparTodosRegistros()
        {
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            var result = MessageBox.Show("Deseja realmente limpar todos os campos do relatório?", "Atenção!", buttons, MessageBoxIcon.Question);
            return result;
        }

        private static void VisitLink(LinkLabel linkLabel1)
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.linkedin.com/in/anderson-silva-4b86413b/");
        }

        internal static void LimpaFormulario(Formulario formulario)
        {
            formulario.Qrcode.Text.Trim();
            formulario.Qrcode.Text = string.Empty;
            formulario.Qrcode.ClearUndo();
            formulario.Cnpj.Clear();
            formulario.Data.Clear();
            formulario.Extrato.Clear();
            formulario.Valor.Clear();
        }
    }
}
