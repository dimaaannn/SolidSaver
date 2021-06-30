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
    public interface ISwConnector
    {
        ISldWorks SwApp { get; }

        bool IsComConnected { get; }
        bool Connect();

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
            _logger.LogInformation("Begin connection to SW");
            _swProcess = GetProcess(SW_PROCESS_NAME);
            if (_swProcess == null)
                return false;

            SwApp = Activator.CreateInstance(typeof(ISldWorks)) as ISldWorks;
            IsComConnected = SwApp != null;

            if (IsComConnected)
            {
                ComConnected?.Invoke(this, EventArgs.Empty);
                _swProcess.EnableRaisingEvents = true;
                _swProcess.Disposed += OnSwProcessDisposed;
            }
            if (IsComConnected)
                _logger.LogDebug("Com connection established");
            else
                _logger.LogWarning("Com connection failed");
            return IsComConnected;
        }


        private Process GetProcess(string processName)
        {
            Process[] ProcessList = Process.GetProcessesByName(processName);
            Process firstProcess = ProcessList.FirstOrDefault();
            _logger.LogInformation("GetSwProcess = {proc}", firstProcess);
            return firstProcess;
        }

        private void OnSwProcessDisposed(object sender, EventArgs e)
        {
            IsComConnected = false;
            SwApp = null;
            _swProcess.Disposed -= OnSwProcessDisposed;
            ComDisposed?.Invoke(this, EventArgs.Empty);
        }
    }
}
