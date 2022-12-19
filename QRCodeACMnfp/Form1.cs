using System;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using QRCodeACMnfp.Services;
using QRCodeACMnfp.Domain;
using DocumentFormat.OpenXml.Bibliography;

namespace QRCodeACMnfp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }        
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                PlanilhaService.CriarDiretorio();
                TelaPrincipalService.AbrirNavegadorEmNfp(NavegadorView.DriverChrome);
                PlanilhaService.SetupDoGridView(dataGridView1);
                PlanilhaService.FormatarCelulaDoGridView(dataGridView1);
                rdbManual.Checked = true; //modo manual ativado de inicio
                LimparCamposFormulario();
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "A versão do Navegador Chrome está na versão 108.X ?");
            }

        }

        private void txtQRCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) //Entrada Principal
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    Formulario formulario = new Formulario( txtQRCode, txtCNPJ, txtData, txtExtrato, mskValor);

                    //ValidacaoService.SepararCamposQRCode(txtQRCode.Text.Trim(), txtCNPJ, txtData, txtExtrato, mskValor);
                    ValidacaoService.SepararCamposQRCode(formulario);
                    //bool testeForms = ValidacaoService.TestaInformacoes(txtCNPJ, txtData, txtQRCode, mskValor);
                    bool testeForms = ValidacaoService.TestaInformacoes(formulario);
                    if (testeForms && PlanilhaService.Relatorio.Notas.Count < PlanilhaService.capacidadePlanilha)
                    {
                        NavegadorView.EnterText(NavegadorView.DriverChrome, txtCNPJ.Text, txtData.Text, mskValor.Text.Replace(".", "").Replace(",", ""), txtExtrato.Text);
                        NavegadorView.SelectDropDown(NavegadorView.DriverChrome, "ddlTpNota", "Cupom Fiscal", "Id");
                        if (rdbAutomatico.Checked == true)
                            NavegadorView.Click(NavegadorView.DriverChrome, "btnSalvarNota", "Salvar Nota", "input");
                        PlanilhaService.Relatorio.Notas.Add(new NotaFiscalValoresl(mskValor.Text.Replace(".", ",")));
                        TelaPrincipalService.MostrarNoGridView(dataGridView1, PlanilhaService.Relatorio.Notas);
                        TelaPrincipalService.AtualizarValoresTotaisNaTela(lblNLancamentos, lblValorTotal, PlanilhaService.Relatorio);
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
                MessageBox.Show(ex.Message, "Erro da aplicação.");
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
            catch (Exception ex)
            {
                MessageBox.Show("Verifique se está logado e na página de inserção de notas fiscais corretamente e se está na página de inserção de dados da Nota Fiscal. \n\n" + ex.Message, "Erro Geral no envio de dados para tabela", MessageBoxButtons.OK);
                LimparCamposFormulario();
            }
        } 

        private void LimparCamposFormulario()
        {
            TelaPrincipalService.LimpaFormulario(txtQRCode, txtCNPJ, txtData, txtExtrato, mskValor);
            FocoPrincipal();
        }

        private void button2_Click(object sender, EventArgs e) //gera relatório // exception ok
        {
            try
            {
                PlanilhaService.CriarRelatorio(dataGridView1, PlanilhaService.enderecoSave);
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
                TelaPrincipalService.ManualDoUsuario(PlanilhaService.enderecoSave);
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
                TelaPrincipalService.PerguntaDeFecharAplicacao(NavegadorView.DriverChrome);
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
                if (PlanilhaService.Relatorio.Notas.Count > 0)
                {
                    PlanilhaService.ZeraUltimaNotaLancada(PlanilhaService.Relatorio); //zera para mostrar no grid
                    TelaPrincipalService.MostrarNoGridView(dataGridView1, PlanilhaService.Relatorio.Notas);
                    PlanilhaService.RemoveUltimaNotaLancada(PlanilhaService.Relatorio); //depois de mostrar zerado no grid removemos o registro.
                    TelaPrincipalService.AtualizarValoresTotaisNaTela(lblNLancamentos, lblValorTotal, PlanilhaService.Relatorio);
                    TelaPrincipalService.MostrarNoGridView(dataGridView1, PlanilhaService.Relatorio.Notas);
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
            PlanilhaService.ZeraValoresDeNotas(PlanilhaService.Relatorio.Notas);
            TelaPrincipalService.MostrarNoGridView(dataGridView1, PlanilhaService.Relatorio.Notas);
            PlanilhaService.LimpaNotasDoRegistro(PlanilhaService.Relatorio);
            TelaPrincipalService.AtualizarValoresTotaisNaTela(lblNLancamentos, lblValorTotal, PlanilhaService.Relatorio);
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


