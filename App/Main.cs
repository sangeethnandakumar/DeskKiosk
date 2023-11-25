using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace App
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            StartServer();
            Browser.Load("http://127.0.0.1:5000/");
        }

        private void StartServer()
        {
            TerminateProcessByName("DeskKiosk.Server");
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Path.Combine(@"DeskKiosk.Server.exe"),
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                RedirectStandardError = false
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
        }


        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            TerminateProcessByName("DeskKiosk.Server");
        }

        static void TerminateProcessByName(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                    Console.WriteLine($"Process {process.ProcessName} terminated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error terminating process {process.ProcessName}: {ex.Message}");
                }
            }
        }
    }
}
