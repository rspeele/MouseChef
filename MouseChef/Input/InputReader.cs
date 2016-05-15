using System;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;

namespace MouseChef.Input
{
    public class InputReader : IDisposable
    {
        private readonly IEventProcessor _eventProcessor;
        private readonly Process _mouseMeat;
        private readonly Thread _reader;
        private bool _disposed;

        public InputReader(IEventProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
            _mouseMeat = Process.Start
                (new ProcessStartInfo
                {
                    FileName = "MouseMeat.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                });
            _reader = new Thread(ReadInput);
            _reader.Start();
        }

        private void ReadInput()
        {
            while (!_disposed)
            {
                var evtText = _mouseMeat.StandardOutput.ReadLine();
                var evt = JsonConvert.DeserializeObject<Event>(evtText);
                switch (evt.Type)
                {
                    case EventType.DeviceInfo:
                        _eventProcessor.DeviceInfo(evt.DeviceInfo);
                        break;
                    case EventType.Move:
                        _eventProcessor.Move(evt.Move);
                        break;
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _mouseMeat.Dispose();
            _reader.Join();
        }
    }
}
