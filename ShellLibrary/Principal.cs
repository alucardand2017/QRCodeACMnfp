namespace ShellLibrary
{
    public static class Principal
    {
        public static void AtualizaNavegador()
        {
            string sourceLastVersion = @"https://chromedriver.storage.googleapis.com/LATEST_RELEASE";
            string latestReleaseChromeDestination = Environment.CurrentDirectory + "\\LATEST_RELEASE.txt";

            string latestVersion = string.Empty;
            try
            {
                StreamReader sr = new StreamReader(latestReleaseChromeDestination);
                latestVersion = sr.ReadLine();
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

            string sourceDownloadChromeVersion = $"https://chromedriver.storage.googleapis.com/{latestVersion}/chromedriver_win32.zip";
            string destinationDownloadChromeVersion = Environment.CurrentDirectory + "\\chromedriver_win32.Zip";

            string chromedriverFolder = Environment.CurrentDirectory + "\\Chromedriver";
            string chromedriverZipFolder = destinationDownloadChromeVersion;

            try
            {
                Console.WriteLine("Mudando a política de Execução do PowerShell ... (1/8)");
                Console.Write(PowerShellHandler.Command("Set-ExecutionPolicy RemoteSigned"));
                Console.WriteLine("Concluído.\n");

                Console.WriteLine("Importando o modulo do powershell .. (2/8)");
                Console.Write(PowerShellHandler.Command("Import-Module Microsoft.PowerShell.Archive"));
                Console.WriteLine("Concluído.\n");

                Console.WriteLine("Buscando a última versão do Chrome ... (3/8)");
                Console.Write(BuscaInfoUltimaVersaoChrome(sourceLastVersion, latestReleaseChromeDestination));
                Console.WriteLine("Concluído.\n");

                Console.WriteLine("Iniciando download dos Arquivos ... (4/8)");
                Console.Write(DownloadLastVersionChrome(sourceDownloadChromeVersion, destinationDownloadChromeVersion));
                Console.WriteLine("Concluído.\n");

                Console.WriteLine("Extraindo arquivos ... (5/8)");
                Console.Write(ExtractArchives(chromedriverZipFolder, chromedriverFolder));
                Console.WriteLine("Conluído.\n");

                Console.WriteLine("Excluindo arquivos.Zip ... (6/8)");
                Console.Write(PowerShellHandler.Command(@"Remove-Item " + chromedriverZipFolder));
                Console.WriteLine("Concluido.\n");

                Console.WriteLine(@"Copiando na pasta raíz ... (7/8)");
                PowerShellHandler.Command($"Copy-Item  \"{chromedriverFolder}\\chromedriver.exe\" -Destination \"{Environment.CurrentDirectory}\"");
                Console.WriteLine("Concluído.\n");

                Console.WriteLine("Excluindo arquivos auxiliares ... (8/8)");
                Console.Write(PowerShellHandler.Command($"Remove-Item  {chromedriverFolder} -Recurse"));
                Console.WriteLine("Concluido.\n");



                //Console.WriteLine(@"Iniciado Valur ... (8/8)");
                //Console.WriteLine(PowerShellHandler.Command("Start-Process -FilePath \"C:\\Program Files (x86)\\Valur\\Valur\\Valur.exe\""));
                //Console.WriteLine("Concluído.");

            }
            catch (Exception e)
            {
                Console.WriteLine("ERRO. - " + e.Message);
            }

            static string BuscaInfoUltimaVersaoChrome(string source, string destination)
            {
                string command = $"Invoke-WebRequest -Uri {source} -OutFile {destination}";
                return PowerShellHandler.Command(command);
            }
            static object DownloadLastVersionChrome(string source, string destination)
            {
                string command = $"Invoke-WebRequest -Uri {source} -OutFile {destination}";
                return PowerShellHandler.Command(command);
            }

            static string ExtractArchives(string source, string destination)
            {
                string command = $"Expand-Archive -LiteralPath {source} -DestinationPath {destination} -Force";
                return PowerShellHandler.Command(command);
            }
        }
    }
}