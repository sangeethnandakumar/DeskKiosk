using CefSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace App
{
    public partial class Main : Form
    {
        const string SERVER_URL = "http://127.0.0.1:5000/";
        const string SERVER_LOC = @"C:\Users\Sangeeth Nandakumar\source\repos\DeskKiosk\DeskKiosk.Server\bin\Release\net8.0\publish";
        const string EXE_NAME = "DeskKiosk.Server.exe";

        public Main()
        {
            InitializeComponent();
            Browser.MenuHandler = new InvisibleMenuHandler();           
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (IsServerRunning())
                {
                    Browser.Load(SERVER_URL);
                }
                else
                {
                    StartServer();
                    Browser.Load(SERVER_URL);
                }
            });
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (IsLastInstance())
            {
                TerminateServer();
            }
        }

        private void StartServer()
        {
            if (!IsServerRunning())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(SERVER_LOC, EXE_NAME),
                    WorkingDirectory = SERVER_LOC,
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
        }

        private void TerminateServer()
        {
            string processName = Path.GetFileNameWithoutExtension(EXE_NAME);
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

        private bool IsServerRunning()
        {
            string processName = Path.GetFileNameWithoutExtension(EXE_NAME);
            Process[] processes = Process.GetProcessesByName(processName);

            return processes.Length > 0;
        }

        private bool IsLastInstance()
        {
            // Check if this is the last instance of the application running
            return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length == 1;
        }
    }
}
