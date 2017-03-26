using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLoader.Shared
{
    class ProcessStarter
    {
        private CSharpCodeProvider _cSharpCodeProvider;
        public ProcessStarter()
        {
            _cSharpCodeProvider = new CSharpCodeProvider();
        }

        public void StartProcess(string connectionString, string filePath, string processSourceCode, CompilerParameters parameters)
        {
            deletePreviousDataUnloaderExeFiles();

            var results = _cSharpCodeProvider.CompileAssemblyFromSource(parameters, processSourceCode);

            if (results.Errors.Count != 0)
            {
                throw new Exception($"Compilation error of ressource file: {results.Errors[0].ErrorText}");
            }

            var process = new Process();
            process.StartInfo = new ProcessStartInfo(results.PathToAssembly, $"{filePath.Count(x => x == ' ')} {Process.GetCurrentProcess().Id.ToString()} {connectionString} {filePath}");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
        }

        //We have to delete them or else they will stack up each time we run a test.
        private void deletePreviousDataUnloaderExeFiles()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var dataunloaderfiles = Directory.GetFiles(currentDirectory, "uniquedataunloader*.exe");

            foreach (var file in dataunloaderfiles)
            {
                try
                {
                    File.Delete(file);
                }
                catch //If file can't be deleted. It means it is still in use, which is ok, don't do anything.
                {

                }
            }
        }
    }
}
