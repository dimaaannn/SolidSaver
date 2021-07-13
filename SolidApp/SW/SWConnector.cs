using Microsoft.Extensions.Logging;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidApp.SW
{
    public interface ISwConnector : IDisposable
    {
        ISldWorks SwApp { get; }

        bool IsComConnected { get; }
        bool Connect();
        void Disconnect();

        event EventHandler ComConnected;
        event EventHandler ComDisposed;
    }

    public class SWConnector : ISwConnector
    {
        public SWConnector(ILogger<SWConnector> logger)
        {
            _logger = logger;
        }

        private readonly ILogger _logger;
        private Process _swProcess;
        private const string SW_PROCESS_NAME = "SLDWORKS";
        private const string SW_PROG_ID = "SldWorks.Application";

        public ISldWorks SwApp { get; private set; }
        public bool IsComConnected { get; private set; }

        public event EventHandler ComConnected;
        public event EventHandler ComDisposed;

        public bool Connect()
        {
            using(var log = _logger.BeginScope("Com Connection to {processName}", SW_PROCESS_NAME))
            {
                _logger.LogDebug("Begin connection to SW");
                _swProcess = GetProcess(SW_PROCESS_NAME);
                if (_swProcess == null)
                    return false;
                Type progType = Type.GetTypeFromProgID(SW_PROG_ID);
                SwApp = Activator.CreateInstance(progType) as ISldWorks;
                IsComConnected = SwApp != null;
                if (IsComConnected)
                {
                    ComConnected?.Invoke(this, EventArgs.Empty);
                    _swProcess.EnableRaisingEvents = true;
                    _swProcess.Disposed += OnSwProcessDisposed;
                }
                if (IsComConnected)
                    _logger.LogInformation("Com connection established");
                else
                    _logger.LogWarning("Com connection failed");
            }
            return IsComConnected;
        }

        public void Disconnect()
        {
            _swProcess.Dispose();
            _logger.LogInformation("Process {name} disposed", SW_PROCESS_NAME);
        }

        private Process GetProcess(string processName)
        {
            Process[] ProcessList = Process.GetProcessesByName(processName);
            Process firstProcess = ProcessList.FirstOrDefault();
            if (firstProcess == null)
                _logger.LogWarning($"GetProcess: Process not found");
            else
                _logger.LogDebug("Process {proc} is found", firstProcess?.ProcessName);
            return firstProcess;
        }

        private void OnSwProcessDisposed(object sender, EventArgs e)
        {
            Dispose();
        }

        public void Dispose()
        {
            IsComConnected = false;
            SwApp = null;
            _swProcess.Disposed -= OnSwProcessDisposed;
            ComDisposed?.Invoke(this, EventArgs.Empty);
        }
    }
}
