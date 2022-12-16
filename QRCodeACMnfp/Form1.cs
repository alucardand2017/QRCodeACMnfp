using System;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using QRCodeACMnfp.Services;
using QRCodeACMnfp.Domain;

namespace QRCodeACMnfp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //private IWebDriver driver = new ChromeDriver();
        private InformarcoesRelatorio relatorio = new InformarcoesRelatorio();
        private int capacidadePlanilha = 400;
        private string enderecoSave = @"C:\Users\ander\Downloads\";
        //CONFIGURAÇÕES
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
           
                TelaPrincipalService.AbrirNavegadorEmNfp(SeleniumSetMethods.DriverChrome);
                PlanilhaService.SetupDoGridView(dataGridView1);
                PlanilhaService.FormatarCelulaDoGridView(dataGridView1);
                rdbManual.Checked = true;
                LimparCamposFormulario();
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "A versão do Navegador Chrome está na versão 108.X ?");
            }

        }

        //FUNÇÕES
        private void txtQRCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    ValidacaoService.SepararCamposQRCode(txtQRCode.Text.Trim(), txtCNPJ, txtData, txtExtrato, mskValor);
                    bool testeForms = ValidacaoService.TestaInformacoes(txtCNPJ, txtData, txtQRCode, mskValor); 
                    if (testeForms && relatorio.Notas.Count < capacidadePlanilha)
                    {
                        SeleniumSetMethods.EnterText(SeleniumSetMethods.DriverChrome, txtCNPJ.Text, txtData.Text, mskValor.Text.Replace(".", "").Replace(",", ""), txtExtrato.Text);
                        SeleniumSetMethods.SelectDropDown(SeleniumSetMethods.DriverChrome, "ddlTpNota", "Cupom Fiscal", "Id");
                        if (rdbAutomatico.Checked == true)
                            SeleniumSetMethods.Click(SeleniumSetMethods.DriverChrome, "btnSalvarNota", "Salvar Nota", "input");
                        relatorio.Notas.Add(new NotaFiscalValoresl(mskValor.Text.Replace(".", ",")));
                        TelaPrincipalService.MostrarNoGridView(dataGridView1, relatorio.Notas);
                        TelaPrincipalService.AtualizarValoresTotaisNaTela(lblNLancamentos, lblValorTotal, relatorio);
                        LimparCamposFormulario();
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Erro na visualização e estrutura da planilha");
                LimparCamposFormulario();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Erro de  aplicação.");
                LimparCamposFormulario();

            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Erro de argumento inválido!");
                LimparCamposFormulario();
            }
            catch (WebDriverArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Erro na captura do Botão.");
                LimparCamposFormulario();
            }
            catch (Exception)
            {
                MessageBox.Show("Verifique se está logado e na página de inserção de notas fiscais corretamente.", "Erro Geral no envio de dados para tabela", MessageBoxButtons.OK);
                LimparCamposFormulario();
            }
        } //Entrada Principal

        private void LimparCamposFormulario()
        {
            TelaPrincipalService.LimpaFormulario(txtQRCode, txtCNPJ, txtData, txtExtrato, mskValor);
            FocoPrincipal();
        }
        private void button2_Click(object sender, EventArgs e) //gera relatório // exception ok
        {
            try
            {
                PlanilhaService.CriaPlanilha(dataGridView1, enderecoSave);
                LimparCamposFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro na geração do relatório");
            }
        }


        private void btnInformacoes_Click(object sender, EventArgs e)
        {
            try
            {
                TelaPrincipalService.ManualDoUsuario(enderecoSave);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Não foi possível visitar a página.");
            }
            finally
            {
                FocoPrincipal();
            }
        }

        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                TelaPrincipalService.InformacoesContato(linkLabel1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Não foi possível visitar a página.");
            }
            finally
            {
                FocoPrincipal();
            }
        }

        private void button1_Click_1(object sender, EventArgs e) //fecha a aplicação
        {
            try
            {
                TelaPrincipalService.PerguntaDeFecharAplicacao(SeleniumSetMethods.DriverChrome);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show("Erro ao fechar o aplicativo! => ", ex.Message);
            }
            finally
            {
                FocoPrincipal();
            }
        }

        private void btnExcluirRegistro_Click(object sender, EventArgs e)
        {
            try
            {
                if (relatorio.Notas.Count > 0)
                {
                    PlanilhaService.ZeraUltimaNotaLancada(relatorio); //zera para mostrar no grid
                    TelaPrincipalService.MostrarNoGridView(dataGridView1, relatorio.Notas);
                    PlanilhaService.RemoveUltimaNotaLancada(relatorio); //depois de mostrar zerado no grid removemos o registro.
                    TelaPrincipalService.AtualizarValoresTotaisNaTela(lblNLancamentos, lblValorTotal, relatorio);
                    TelaPrincipalService.MostrarNoGridView(dataGridView1, relatorio.Notas);
                    MessageBox.Show("Último registro foi excluído!");
                }
                else
                {
                    MessageBox.Show("Não há registros a serem excluídos", "Observação:", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao tentar excluir um registro dos arquivos. ", ex.Message);
            }
            finally
            {
                LimparCamposFormulario();
            }

        } //exclui 1 registro //trycatch

        private void button3_Click(object sender, EventArgs e) //exclui todos registros
        {
            try
            {
                if (TelaPrincipalService.PerguntaDeLimparTodosRegistros() == DialogResult.Yes)
                    LimparTodosDados();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao limpar os registros. ", ex.Message);
            }
            finally
            {
                FocoPrincipal();
            }
        }

        private void LimparTodosDados()
        {
            PlanilhaService.ZeraValoresDeNotas(relatorio.Notas);
            TelaPrincipalService.MostrarNoGridView(dataGridView1, relatorio.Notas);
            PlanilhaService.LimpaNotasDoRegistro(relatorio);
            TelaPrincipalService.AtualizarValoresTotaisNaTela(lblNLancamentos, lblValorTotal, relatorio);
            MessageBox.Show("Todos registros foram limpos.!");
            LimparCamposFormulario();
        }

        private void rdbAutomatico_CheckedChanged(object sender, EventArgs e)
        {
            FocoPrincipal();
        }

        private void rdbManual_CheckedChanged(object sender, EventArgs e)
        {
            FocoPrincipal();
        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {
            FocoPrincipal();
        }

        private void FocoPrincipal()
        {
            txtQRCode.Focus();
        }
    }
}


