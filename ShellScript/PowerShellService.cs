

    public static class PowerShellService
    {
      public static void Principal()
        {
            try
            {
                Console.WriteLine("Mudando a política de Execução do PowerShell ... (1/7)");
                Console.Write(PowerShellHandler.Command("Set-ExecutionPolicy RemoteSigned"));
                Console.WriteLine("Concluído.\n");

                Console.WriteLine("Importando o modulo do powershell .. (2/7)");
                Console.Write(PowerShellHandler.Command("Import-Module Microsoft.PowerShell.Archive"));
                Console.WriteLine("Concluído.\n");

                Console.WriteLine("Buscando a última versão do Chrome ... (3/7)");
                Console.Write(BuscaInfoUltimaVersaoChrome());
                Console.WriteLine("Concluído.\n");

                Console.WriteLine("Iniciando download dos Arquivos ... (4/7)");
                Console.Write(DownloadLastVersionChrome());
                Console.WriteLine("Concluído.\n");

                Console.WriteLine("Extraindo arquivos ... (5/7)");
                Console.Write(ExtractArchives());
                Console.WriteLine("Conluído.\n");

                Console.WriteLine("Excluindo arquivos.Zip ... (6/7)");
                Console.Write(PowerShellHandler.Command(@"Remove-Item D:\Downloads\chromedriver_win32.Zip"));
                Console.WriteLine("Concluido.\n");

                Console.WriteLine(@"Copiando na pasta raíz ... (7/7)");
                Console.WriteLine(PowerShellHandler.Command("Copy-Item \"D:\\Downloads\\Chromedriver\\chromedriver.exe\" -Destination \"C:\\Program Files (x86)\\Valur\\Valur\""));
                Console.WriteLine("Concluído.");
            }
            catch (Exception e)
            {
                Console.WriteLine("ERRO. - " + e.Message);
            }

            static string BuscaInfoUltimaVersaoChrome()
            {
                string destination = @"D:\Downloads\LATEST_RELEASE.txt";
                string source = @"https://chromedriver.storage.googleapis.com/LATEST_RELEASE";
                string command = $"Invoke-WebRequest -Uri {source} -OutFile {destination}";
                var output = PowerShellHandler.Command(command);
                return output;
            }
            static object DownloadLastVersionChrome()
            {
                string destination = @"D:\Downloads\chromedriver_win32.Zip";
                string source = @"https://chromedriver.storage.googleapis.com/110.0.5481.77/chromedriver_win32.zip";
                string command = $"Invoke-WebRequest -Uri {source} -OutFile {destination}";
                var chromeExe = PowerShellHandler.Command(command);
                return chromeExe;
            }

            static string ExtractArchives()
            {
                string source = @"'D:\Downloads\chromedriver_win32.Zip'";
                string destination = @"D:\Downloads\Chromedriver";
                string command = $"Expand-Archive -LiteralPath {source} -DestinationPath {destination} -Force";
                return PowerShellHandler.Command(command);
            }
        }

    }

