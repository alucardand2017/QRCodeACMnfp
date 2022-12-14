using AForge.Video;
using AForge.Video.DirectShow;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Drawing;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;
using ClosedXML.Excel;
using System.Diagnostics;
using System.Globalization;

namespace QRCodeACMnfp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private IWebDriver driver = new ChromeDriver();
        private List<NotaFiscalValoresl> Notas = new List<NotaFiscalValoresl>();
        int lancamentos = 0;
        private int capacidadePlanilha = 400;
        private string enderecoSave = @"C:\Users\ander\Downloads\";

        //CONFIGURAÇÕES

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                SetupDataGridView();
                rdbManual.Checked = true;
                driver.Navigate().GoToUrl("https://www.nfp.fazenda.sp.gov.br/EntidadesFilantropicas/ListagemNotaEntidade.aspx");
                LimpaEFocaQRCode();
            }
            catch (Exception)
            {

                throw new WebDriverArgumentException("A versão do Navegador Chrome não está atualizada corretamente");
            }

        }


        //FUNÇÕES
        private void txtQRCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Enter)
                {
                    QuebrarCodigoQR(txtQRCode.Text);
                    bool testeForms = TestaInformacoes();
                    if (testeForms && Notas.Count < capacidadePlanilha)
                    {
                        SeleniumSetMethods.EnterText(driver, txtCNPJ.Text, txtData.Text, mskValor.Text.Replace(".", "").Replace(",", ""), txtExtrato.Text);
                        SeleniumSetMethods.SelectDropDown(driver, "ddlTpNota", "Cupom Fiscal", "Id");
                        if (rdbAutomatico.Checked == true)
                            SeleniumSetMethods.Click(driver, "btnSalvarNota", "Salvar Nota", "input");
                        Notas.Add(new NotaFiscalValoresl(mskValor.Text.Replace(".", ",")));
                        MostrarNoGridView();
                        lancamentos++;
                        AtualizarValoresTotaisNaTela();
                        txtQRCode.Text.Trim();
                        LimpaEFocaQRCode();
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Erro na visualização e estrutura da planilha");
                LimpaEFocaQRCode();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Erro de  aplicação.");
                LimpaEFocaQRCode();

            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Erro de argumento inválido!");
                LimpaEFocaQRCode();

            }
            catch (WebDriverArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Erro na captura do Botão.");
                LimpaEFocaQRCode();
            }
            catch (Exception)
            {
                MessageBox.Show("Verifique se está logado e na página de inserção de notas fiscais corretamente.", "Erro Geral no envio de dados para tabela", MessageBoxButtons.OK);
                LimpaEFocaQRCode();

            }
        }

        private void AtualizarValoresTotaisNaTela()
        {
            try
            {
                lblNLancamentos.Text = lancamentos.ToString();
                float soma = 0;
                foreach (NotaFiscalValoresl nota in Notas)
                    soma += float.Parse(nota.Valor);
                lblValorTotal.Text = soma.ToString("F");
            }
            catch (Exception)
            {
                MessageBox.Show("Houve problemas com a leitura dos dados. Erro na conversão de valores Totais no App.");
            }

        }

        private void LimpaEFocaQRCode()
        {
            FocoPrincipal();
            txtQRCode.Text = string.Empty;
            txtQRCode.ClearUndo();
            txtCNPJ.Clear();
            txtData.Clear();
            txtExtrato.Clear();
            mskValor.Clear();
        }

        private bool TestaInformacoes() //exception ok
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

        private void QuebrarCodigoQR(string qRCodeAll) //exception ok
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
                LimpaEFocaQRCode();
                throw new ArgumentException("Erro ao escanear elementos do QRCode. Verifique a compatibilidade" +
                    "do código ou se escaneou sem querer o código de barras próximo.");
            }
        }

        private void button2_Click(object sender, EventArgs e) //gera relatório
        {
            GerarRelatorio();
            LimpaEFocaQRCode();
        }

        //PLANILHA

        private void SetupDataGridView()
        {
            dataGridView1.ColumnCount = 11;
            dataGridView1.RowCount = 50;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.MultiSelect = false;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.SelectionMode = DataGridViewSelectionMode.CellSelect;

            dataGridView1.Name = "DataGridView1";
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView1.GridColor = Color.LightBlue;
            dataGridView1.RowHeadersVisible = false;

            new Font(dataGridView1.DefaultCellStyle.Font, FontStyle.Italic);
            dataGridView1.Dock = DockStyle.Fill;

            FormatarCelula();
        }

        private void FormatarCelula()
        {

            this.dataGridView1.Columns[0].DefaultCellStyle.Format = "N2";
            this.dataGridView1.Columns[1].DefaultCellStyle.Format = "N2";
            this.dataGridView1.Columns[2].DefaultCellStyle.Format = "N2";
            this.dataGridView1.Columns[3].DefaultCellStyle.Format = "N2";
            this.dataGridView1.Columns[4].DefaultCellStyle.Format = "N2";
            this.dataGridView1.Columns[5].DefaultCellStyle.Format = "N2";
            this.dataGridView1.Columns[6].DefaultCellStyle.Format = "N2";
            this.dataGridView1.Columns[7].DefaultCellStyle.Format = "N2";
            this.dataGridView1.Columns[8].DefaultCellStyle.Format = "N2";
            this.dataGridView1.Columns[9].DefaultCellStyle.Format = "N2";
        }

        private void MostrarNoGridView() //exception ok
        {
            int linha = 0;
            int coluna = 0;
            foreach (NotaFiscalValoresl nota in Notas)
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

        //CRIAR E SALVAR EXCEL

        private void GerarRelatorio()
        {
            try
            {
                DataTable dt = new DataTable();
                for (int i = 1; i <= 11; i++)
                {
                    dt.Columns.Add("Coluna " + i, typeof(float));
                }
                string dataRelatorio = DateTime.Now.ToString("ddMMyyyyHHmmss");
                using (var workbook = new XLWorkbook())
                {

                    foreach (DataGridViewColumn coluna in dataGridView1.Columns)
                    {
                        dt.Columns.Add(coluna.Name, typeof(float));
                    }
                    foreach (DataGridViewRow linha in dataGridView1.Rows)
                    {
                        DataRow drow = dt.NewRow();
                        foreach (DataGridViewCell cell in linha.Cells)
                        {
                            if (cell.Value == null)
                            {
                                cell.Value = 0f;
                            }
                            drow[cell.ColumnIndex] = cell.Value.ToString();
                        }
                        dt.Rows.Add(drow);
                    }

                    var worksheet = workbook.AddWorksheet("Plan1");
                    worksheet.Cell("A1").InsertData(dt);
                    worksheet.Cell("A51").FormulaA1 = "=SUM(A1:A50)";
                    worksheet.Cell("B51").FormulaA1 = "=SUM(B1:B50)";
                    worksheet.Cell("C51").FormulaA1 = "=SUM(C1:C50)";
                    worksheet.Cell("D51").FormulaA1 = "=SUM(D1:D50)";
                    worksheet.Cell("E51").FormulaA1 = "=SUM(E1:E50)";
                    worksheet.Cell("F51").FormulaA1 = "=SUM(F1:F50)";
                    worksheet.Cell("G51").FormulaA1 = "=SUM(G1:G50)";
                    worksheet.Cell("H51").FormulaA1 = "=SUM(H1:H50)";
                    worksheet.Cell("I51").FormulaA1 = "=SUM(I1:I50)";
                    worksheet.Cell("J51").FormulaA1 = "=SUM(J1:J50)";
                    worksheet.Cell("K51").FormulaA1 = "=SUM(A51:J51)";


                    workbook.SaveAs(enderecoSave + "Relatorio" + dataRelatorio + ".xlsx");
                    MessageBox.Show("Relatório Gerado em " + enderecoSave + " .");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro na geração do relatório");
            }

            //Process.Start(new ProcessStartInfo(@"C:\Users\ander\Downloads\Relatorio" + dataRelatorio + ".xlsx") { UseShellExecute = true });
        }

        //INFORMAÇÕES

        private void btnInformacoes_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MANUAL DO USUÁRIO: " +
                "\n\n 1) Ao iniciar a aplicação, uma tela do navegador Chrome vai abrir, faça o login e navegue nas informações até chegar no formulário 'Cadastro de doação de documento fiscal' normalmente." +
                "\n\n 2) Desmarque as caixas que fixam os campos já digitados no formulário antes de fazer a leitura do QR Code da nota fiscal com o leitor de mão." +
                "\n\n 3) Quando escaneado, o programa preenche os campo da página com os valores mostrados nos respectivos campos do programa. " +
                "\n\n 4) As escolha 'automática' envia os dados para o site e 'Salva a Nota', estando apto a escanear a próxima." +
                "\n\n 5) A escolha 'manual' apenas preenche o formulário do Site, cabendo a você salvar a nota no site." +
                "\n\n 6) O relatório quando emitido gera um arquivo do tipo excel no endereço " + enderecoSave + " com a data e hora atual." +
                "\n\n 7) Após salvar o Relatório, feche o programa no botão 'Fechar App' e inicie um novo processo." +
                "\n\n 8) Não é possível emitir o relatório se fechar o programa antes, sendo necessário preencher a planilha manualmente manualmente.");
            txtQRCode.Focus();
        }

        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {

            try
            {
                var result = MessageBox.Show("Você será direcionado para o perfil profissional do Desenvolvedor.", "Info.", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                    VisitLink();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Não foi possível visitar a página.");
            }
            FocoPrincipal();
        }

        private void VisitLink()
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.linkedin.com/in/anderson-silva-4b86413b/");
        }

        private void button1_Click_1(object sender, EventArgs e) //fecha a aplicação
        {
            try
            {
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                var result = MessageBox.Show("Já verificou se o relatorio foi emitido?", "LEMBRETE!", buttons, MessageBoxIcon.Question);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    driver.Dispose();
                    Application.Exit();
                }
                FocoPrincipal();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show("Erro ao fechar o aplicativo! => ", ex.Message);
            }

        }

        private void btnExcluirRegistro_Click(object sender, EventArgs e)
        {
            try
            {
                if (Notas.Count > 0)
                {
                    var nota = Notas.Last();
                    lancamentos--;
                    nota.Valor = 0.00f.ToString();
                    MostrarNoGridView();
                    Notas.Remove(nota);
                    AtualizarValoresTotaisNaTela();
                    MostrarNoGridView();
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


            LimpaEFocaQRCode();
        } //exclui 1 registro //trycatch

        private void button3_Click(object sender, EventArgs e) //exclui todos registros
        {
            try
            {
                LimparTodosDados();
                MessageBox.Show("Todos registros foram limpos.!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao limpar os registros. ", ex.Message);
            }
        }

        private void LimparTodosDados()
        {
            try
            {
                LimpaEFocaQRCode();
                lancamentos = 0;
                foreach (var nota in Notas)
                {
                    nota.Valor = 0.00f.ToString(); ;
                }
                MostrarNoGridView();
                Notas.Clear();
                AtualizarValoresTotaisNaTela();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao Limpar todos os dados. ", ex.Message);
            }

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


