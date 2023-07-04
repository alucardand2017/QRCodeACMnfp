using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO.Compression;
using InsideNotaFiscal;
namespace Valur.Utilities
{
    internal class ChromeUpdate
    {
        static readonly HttpClient client = new HttpClient();
        static string archiveNameZip = "chromedriver_win32.zip";
        static string archiveNameExe = "chromedriver.exe";
        static string archiveNameLicense = "LICENSE.chromedriver";
        public static async Task AtualizacaoChromiun()
        {
            try
            {
                string latestVersion = await GetVersionChromium(); //pega o numero da versão
                string sourceDownloadChromeVersion = GetUriChromeVersion(latestVersion); //baixa o zip dela
                string destinationDownloadChromeVersion = Environment.CurrentDirectory + $"\\{archiveNameZip}"; //monta o endereço do zip
                string chromedriverZipFolder = destinationDownloadChromeVersion; //diz que o endereço do driver zipado é o mesmo da versão de download
                await CopyZiFileInCurrentDirectory(sourceDownloadChromeVersion);
                ExtractorArchivesInLocal(destinationDownloadChromeVersion, Environment.CurrentDirectory);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Erro na busca datualização do Chrome: " + e.Message);
            }
        }
        private static string GetUriChromeVersion(string latestVersion)
        {
            return $"https://chromedriver.storage.googleapis.com/{latestVersion}/{archiveNameZip}";
        }
        private static void ExtractorArchivesInLocal(string destinationDownloadChromeVersion, string chromedriverFolder)
        {
            try
            {
                CheckDeleteExtractFoldersInDirectory(destinationDownloadChromeVersion, chromedriverFolder);
            }
            catch (Exception)
            {
                throw new ArgumentException("Erro ao extrair os arquivos para o sistema");
            }
        }
        private static void CheckDeleteExtractFoldersInDirectory(string destinationDownloadChromeVersion, string chromedriverFolder)
        {
            if (File.Exists(Environment.CurrentDirectory + $"\\{archiveNameExe}"))
                File.Delete(Environment.CurrentDirectory + $"\\{archiveNameExe}");
            if (File.Exists(Environment.CurrentDirectory + $"\\{archiveNameLicense}"))
                File.Delete(Environment.CurrentDirectory + $"\\{archiveNameLicense}");

            ZipFile.ExtractToDirectory(destinationDownloadChromeVersion, chromedriverFolder);

            if (File.Exists(Environment.CurrentDirectory + $"\\{archiveNameZip}"))
                File.Delete(Environment.CurrentDirectory + $"\\{archiveNameZip}");
        }
        private static async Task CopyZiFileInCurrentDirectory(string sourceDownloadChromeVersion)
        {
            try
            {
                using (var stream = await client.GetStreamAsync(sourceDownloadChromeVersion))
                {
                    using (var fileStream = new FileStream(archiveNameZip, FileMode.CreateNew))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }
            }
            catch (Exception)
            {
                throw new ArgumentException("Erro na realização do download dos arquivos do site da Google");
            }
           
        }
        public static async Task<string> GetVersionChromium()
        {
            try
            {
                using (HttpResponseMessage response = await client.GetAsync("https://chromedriver.storage.googleapis.com/LATEST_RELEASE"))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
            }
            catch (Exception)
            {

                throw new ArgumentException("Erro em adquirir informação da última versão do Google Chrome");
            }
           
        }
    }
}

