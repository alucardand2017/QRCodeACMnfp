using AForge.Video;
using AForge.Video.DirectShow;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Drawing;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace QRCodeACMnfp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private FilterInfoCollection filterInfoCollection;
        private VideoCaptureDevice captureDevice;
        private IWebDriver driver = new ChromeDriver();
        private List<NotaFiscal> Notas = new List<NotaFiscal>();
        private float Soma;

        private Button addNewRowButton = new Button();
        private Button deleteRowButton = new Button();

        private void Form1_Load(object sender, EventArgs e)
        {

            SetupLayout();
            SetupDataGridView();
            FormatarCelulasPlanilha();

            rdbCapturaManual.Checked = true;
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo filter in filterInfoCollection)
                cboDevice.Items.Add(filter.Name);
            cboDevice.SelectedIndex = 0;
            driver.Navigate().GoToUrl("https://www.nfp.fazenda.sp.gov.br/EntidadesFilantropicas/ListagemNotaEntidade.aspx");
        }
        //CONFIGURAÇÕES
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (captureDevice != null)
                if (captureDevice.IsRunning)
                    captureDevice.Stop();
        }



        private void button1_Click(object sender, EventArgs e) //Enviar para Página
        {
            bool testeForms = TestaInformacoes();
            if (testeForms)
            {
                try
                {
                    SeleniumSetMethods.EnterText(driver, txtCNPJ.Text, txtData.Text, mskValor.Text.Replace(".", "").Replace(",", ""), txtExtrato.Text);
                    SeleniumSetMethods.SelectDropDown(driver, "ddlTpNota", "Cupom Fiscal", "Id");
                    //SeleniumSetMethods.Click(driver, "btnSalvarNota", "Salvar Nota", "submit");
                }
                catch (Exception)
                {
                    MessageBox.Show("Erro na hora de enviar as informações a página, verifique se está logad@" +
                        " no ambiente correto (página aberta pelo próprio programa quando iniciado.");
                    throw;
                }
            }
            else
            {
                MessageBox.Show("Não foi possível obter todos os campos desse QRCode. \n " +
                    "Por gentileza, digite essa nota manualmente no formulário");
            }
        }

        private bool TestaInformacoes()
        {
            var testaValor = float.TryParse(mskValor.Text, out float valor);
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            if (txtCNPJ.Text.Length != 14)
            {
                MessageBox.Show("CNPJ da nota não possui 14 dígitos");
                return false;
            }
            if (txtData.Text.Length != 8)
            {
                MessageBox.Show("A data deve estár no formato ddMMYYYY sem espaço ou caracteres especiais.");
                return false;
            }
            if (valor > 10000.0)
            {
                var result = MessageBox.Show("valor elevado ou negativo!", "Deseja confirmar o valor?", buttons);
                if (result == System.Windows.Forms.DialogResult.No)
                {
                    return false;
                }
            }
            if (valor <= 0f)
            {
                MessageBox.Show("A data deve estár no formato ddMMYYYY sem espaço ou caracteres especiais.");
                return false;
            }
            return true;
        }

        private void btn_Captura_Click(object sender, EventArgs e)
        {
            captureDevice = new VideoCaptureDevice(filterInfoCollection[cboDevice.SelectedIndex].MonikerString);
            captureDevice.NewFrame += CaptureNewFrame;
            captureDevice.Start();
            timer1.Start();
        }

        private void CaptureNewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }

       

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                BarcodeReader barcodeReader = new BarcodeReader();
                Result result = barcodeReader.Decode((Bitmap)pictureBox1.Image);
                if (result != null)
                {
                    txtQRCode.Text = result.ToString();
                    string QRCodeAll = txtQRCode.Text;
                    QuebrarCodigoQR(QRCodeAll);
                    timer1.Stop();

                    if (captureDevice.IsRunning)
                        captureDevice.Stop();
                }
            }
        }

        private void QuebrarCodigoQR(string qRCodeAll)
        {
            if (qRCodeAll.Length > 60)
            {
                var qrCodeLimpo = qRCodeAll;
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
                mskValor.Text = valor;
            }
            else
            {
                MessageBox.Show("CQ code não foi lido corretamente ou é incompatível com essa aplicação.");

            }

        }

        private void rdbCapturaManual_CheckedChanged(object sender, EventArgs e)
        {
            if (btnEnviar.Enabled == false)
                btnEnviar.Enabled = true;
            if (btn_Captura.Enabled == false)
                btn_Captura.Enabled = true;
        }

        private void rbtCapturaAutomatica_CheckedChanged(object sender, EventArgs e)
        {
            if (btnEnviar.Enabled == true)
                btnEnviar.Enabled = false;
            if (btn_Captura.Enabled == true)
                btn_Captura.Enabled = false;
        }

        private void button1_Click_1(object sender, EventArgs e) //fecha a aplicação
        {
            driver.Dispose();
            if (captureDevice != null)
                if (captureDevice.IsRunning)
                    captureDevice.Stop();
            timer1.Stop();
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e) //gera relatório
        {
            MessageBox.Show("Função ainda não disponível nessa versão.");
        }

        private void cboDevice_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void linkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Não foi possível visitar a página. :(");
            }
        }
        private void VisitLink()
        {
            linkLabel1.LinkVisited = true;
            System.Diagnostics.Process.Start("https://www.linkedin.com/in/anderson-silva-4b86413b/");
        }

        private void txtQRCode_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtQRCode_Enter(object sender, EventArgs e)
        {
        }

        private void txtQRCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }



        //PLANILHA
        private void songsDataGridView_CellFormatting(object sender,
      System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        {
            if (e != null)
            {
                this.dataGridView1.Columns[0].DefaultCellStyle.Format = "C";
                this.dataGridView1.Columns[1].DefaultCellStyle.Format = "C";
                this.dataGridView1.Columns[2].DefaultCellStyle.Format = "C";
                this.dataGridView1.Columns[3].DefaultCellStyle.Format = "C";
                this.dataGridView1.Columns[4].DefaultCellStyle.Format = "C";
                this.dataGridView1.Columns[5].DefaultCellStyle.Format = "C";
                this.dataGridView1.Columns[6].DefaultCellStyle.Format = "C";
                this.dataGridView1.Columns[7].DefaultCellStyle.Format = "C";
                this.dataGridView1.Columns[8].DefaultCellStyle.Format = "C";
                this.dataGridView1.Columns[9].DefaultCellStyle.Format = "C";

            }
        }
        private void SetupLayout()
        {
            //this.Size = new Size(600, 500);

            //addNewRowButton.Text = "Add Row";
            //addNewRowButton.Location = new Point(10, 10);
            //addNewRowButton.Click += new EventHandler(addNewRowButton_Click);

            //deleteRowButton.Text = "Delete Row";
            //deleteRowButton.Location = new Point(100, 10);
            //deleteRowButton.Click += new EventHandler(deleteRowButton_Click);

            //dataGridView1.Controls.Add(addNewRowButton);
            //dataGridView1.Controls.Add(deleteRowButton);
            //dataGridView1.Height = 50;
            //dataGridView1.Dock = DockStyle.Bottom;

            //Controls.Add(this.buttonPanel);
        }

        private void deleteRowButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void addNewRowButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SetupDataGridView()
        {

            dataGridView1.ColumnCount = 11;
            dataGridView1.RowCount = 51;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);

            dataGridView1.Name = "DataGridView1";
            dataGridView1.AutoSizeRowsMode =  DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridView1.GridColor = Color.LightBlue;
            dataGridView1.RowHeadersVisible = false;


            new Font(dataGridView1.DefaultCellStyle.Font, FontStyle.Italic);

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.Dock = DockStyle.Fill;

            dataGridView1.CellFormatting += new DataGridViewCellFormattingEventHandler(songsDataGridView_CellFormatting);
        }

        private void FormatarCelulasPlanilha()
        {
            string[] row51 = { "=SOMA(A1:A50)", "=SOMA(B1:B50)", "=SOMA(C1:C50)", "=SOMA(D1:D50)", "=SOMA(E1:E50)", "=SOMA(F1:F50)", "=SOMA(G1:G50)", "=SOMA(H1:H50)", "=SOMA(I1:I50)", "=SOMA(J1:J50)", };
            string cellSoma = "=SOMA(A51:J51)";
            dataGridView1.Rows.Insert(50, row51);
            dataGridView1[10, 50].Value = cellSoma;

            dataGridView1.Columns[0].DisplayIndex = 3;
            dataGridView1.Columns[1].DisplayIndex = 4;
            dataGridView1.Columns[2].DisplayIndex = 0;
            dataGridView1.Columns[3].DisplayIndex = 1;
            dataGridView1.Columns[4].DisplayIndex = 2;
        }
    }

}


