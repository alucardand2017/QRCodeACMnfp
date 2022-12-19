using ClosedXML.Excel;
using QRCodeACMnfp.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace QRCodeACMnfp.Services
{

    internal sealed class PlanilhaService
    {
        public static InformarcoesRelatorio Relatorio { get; set; } = new InformarcoesRelatorio();

        public static readonly int capacidadePlanilha = 400;

        public static readonly string enderecoSave = @"C:\Dados_De_Relatorio\";

        internal static void CriarDiretorio()
        {
            if (!Directory.Exists(enderecoSave))
            {
                Directory.CreateDirectory(enderecoSave);
            }

        }

        internal static void SetupDoGridView(DataGridView dataGridView1)
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
        }

        internal static void FormatarCelulaDoGridView(DataGridView dataGridView1)
        {
            dataGridView1.Columns[0].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[1].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[2].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[3].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[4].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[5].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[6].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[7].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[8].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[9].DefaultCellStyle.Format = "N2";
            dataGridView1.Columns[10].DefaultCellStyle.Format = "N2";

        }

        internal static void CriarRelatorio(DataGridView dataGridView1)
        {
            DataTable dt = new DataTable();
            string dataRelatorio = InserirDataNomeRelatorio();
            string nomeRelatorio = "Relatorio_" + dataRelatorio + ".xlsx";
            string enderecoCompleto = enderecoSave + nomeRelatorio;
            CriaColuna(dt);
            using (var workbook = new XLWorkbook())
            {
                CriaEstruturaRelatorio(dataGridView1, dt);
                InsereDadosRelatorio(dt, workbook);
                SalvarRelatorio(enderecoCompleto, workbook);
                InformaUsuario(enderecoCompleto);
                AbreAplicativoPadrao(enderecoCompleto);
            }
        }

        internal static void ZeraValoresDeNotas(List<NotaFiscalValoresl> notas)
        {
            foreach (var nota in notas)
                nota.Valor = "0,00".ToString();
        }

        internal static void ZeraUltimaNotaLancada(InformarcoesRelatorio relatorio)
        {
            var nota = relatorio.Notas.Last();
            nota.Valor = "0,00".ToString();
        }

        internal static void RemoveUltimaNotaLancada(InformarcoesRelatorio relatorio)
        {
            relatorio.Notas.Remove(relatorio.Notas.Last());
        }

        internal static void LimpaNotasDoRegistro(InformarcoesRelatorio relatorio)
        {
            relatorio.Notas.Clear();
        }

        private static void AbreAplicativoPadrao(string endereco)
        {
            Process.Start(new ProcessStartInfo(endereco) { UseShellExecute = true });
        }

        private static void InformaUsuario(string endereco)
        {
            MessageBox.Show("Relatório Gerado em " + endereco);
        }

        private static void SalvarRelatorio(string enderecoRelatorio, XLWorkbook workbook)
        {
            workbook.SaveAs(enderecoRelatorio);
        }

        private static void InsereDadosRelatorio(DataTable dt, XLWorkbook workbook)
        {
            var worksheet = workbook.AddWorksheet("Plan1");
            worksheet.Cell("A1").InsertData(dt);
            SetupTabela(worksheet);
        }

        private static void SetupTabela(IXLWorksheet wb)
        {
            wb.ColumnWidth = 15f;


            wb.Cell("A51").FormulaA1 = "=SUM(A1:A50)";
            wb.Cell("B51").FormulaA1 = "=SUM(B1:B50)";
            wb.Cell("C51").FormulaA1 = "=SUM(C1:C50)";
            wb.Cell("D51").FormulaA1 = "=SUM(D1:D50)";
            wb.Cell("E51").FormulaA1 = "=SUM(E1:E50)";
            wb.Cell("F51").FormulaA1 = "=SUM(F1:F50)";
            wb.Cell("G51").FormulaA1 = "=SUM(G1:G50)";
            wb.Cell("H51").FormulaA1 = "=SUM(H1:H50)";
            wb.Cell("I51").FormulaA1 = "=SUM(I1:I50)";
            wb.Cell("J51").FormulaA1 = "=SUM(J1:J50)";
            wb.Cell("K51").FormulaA1 = "=SUM(A51:J51)";

            wb.Cells("A1:K50").Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Black)
                .Font.SetFontName("Calibri")
                .Font.SetFontSize(11)
                ;
            wb.Cells("A1:K51").Style.NumberFormat.Format = "0.00";
        }

        private static void CriaEstruturaRelatorio(DataGridView dataGridView1, DataTable dt)
        {
            foreach (DataGridViewColumn coluna in dataGridView1.Columns)
            {
                dt.Columns.Add(coluna.Name, typeof(float));
            }
            foreach (DataGridViewRow linha in dataGridView1.Rows)
            {
                PreencherCelulaValoresPadrao(dt, linha);
            }
        }

        private static void PreencherCelulaValoresPadrao(DataTable dt, DataGridViewRow linha)
        {
            DataRow drow = dt.NewRow();
            foreach (DataGridViewCell cell in linha.Cells)
            {
                if (cell.Value == null)
                {
                    cell.Value = "0.00";
                }
                drow[cell.ColumnIndex] = cell.Value.ToString();
            }
            dt.Rows.Add(drow);
        }

        private static string InserirDataNomeRelatorio()
        {
            return DateTime.Now.ToString("ddMMyyyyHHmmss");
        }

        private static void CriaColuna(DataTable dt)
        {
            for (int i = 1; i <= 11; i++)
            {
                dt.Columns.Add("Coluna " + i, typeof(float));
            }
        }
    }
}
