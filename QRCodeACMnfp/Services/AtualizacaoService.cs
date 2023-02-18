using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeACMnfp.Services
{
    internal sealed class AtualizacaoService
    {
        public string endereco { get; set; }
        internal static void AtualizaChrome()
        {
            Process.Start(new ProcessStartInfo(Environment.CurrentDirectory) { UseShellExecute = true });
        }
    }
}
