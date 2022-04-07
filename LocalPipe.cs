/*
Copyright 2022 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace MeshCentralSatellite
{
    public class LocalPipeServer
    {
        private String name;
        private byte[] buffer = new byte[4096];
        private NamedPipeServerStream pipeServer;
        public bool connected = false;

        public delegate void onMessageHandler(string msg);
        public event onMessageHandler onMessage;
        public delegate void onStateChangeHandler(bool connected);
        public event onStateChangeHandler onStateChange;

        public LocalPipeServer(string name)
        {
            this.name = name;
            pipeServer = new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            pipeServer.BeginWaitForConnection(new AsyncCallback(processConnection), null);
        }

        public void Dispose()
        {
            try { pipeServer.Close(); } catch (Exception) { }
            pipeServer = null;
        }

        private void processConnection(IAsyncResult ar)
        {
            try { pipeServer.EndWaitForConnection(ar); } catch (Exception) { }
            connected = true;
            if (onStateChange != null) { onStateChange(connected); }
            try { pipeServer.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(processRead), null); } catch (Exception) { }
        }

        public void Send(string msg)
        {
            if ((pipeServer != null) && (pipeServer.IsConnected))
            {
                byte[] data = UTF8Encoding.UTF8.GetBytes(msg);
                pipeServer.Write(data, 0, data.Length);
            }
        }

        private void processRead(IAsyncResult ar)
        {
            int len = 0;
            try { len = pipeServer.EndRead(ar); } catch (Exception) { }
            if (len > 0)
            {
                onMessage(UTF8Encoding.UTF8.GetString(buffer, 0, len));
                try { pipeServer.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(processRead), null); } catch (Exception) { }
            }
            else
            {
                connected = false;
                if (onStateChange != null) { onStateChange(connected); }
                pipeServer.BeginWaitForConnection(new AsyncCallback(processConnection), null);
            }
        }
    }

    public class LocalPipeClient
    {
        private String name;
        private byte[] buffer = new byte[4096];
        private NamedPipeClientStream pipeClient;

        public delegate void onMessageHandler(string msg);
        public event onMessageHandler onMessage;

        public LocalPipeClient(string name)
        {
            this.name = name;
        }

        public async void Start()
        {
            pipeClient = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
            await pipeClient.ConnectAsync();
            startReading();
        }

        public void Dispose()
        {
            try { pipeClient.Close(); } catch (Exception) { }
            pipeClient = null;
        }

        private void startReading()
        {
            try { pipeClient.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(processRead), null); } catch (Exception) { }
        }

        private void processRead(IAsyncResult ar)
        {
            int len = 0;
            try { len = pipeClient.EndRead(ar); } catch (Exception) { }
            if (len > 0)
            {
                onMessage(UTF8Encoding.UTF8.GetString(buffer, 0, len));
                try { pipeClient.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(processRead), null); } catch (Exception) { }
            }
            else
            {
                Start();
            }
        }

        public void Send(string msg)
        {
            if ((pipeClient != null) && (pipeClient.IsConnected))
            {
                byte[] data = UTF8Encoding.UTF8.GetBytes(msg);
                pipeClient.Write(data, 0, data.Length);
            }
        }
    }

}
