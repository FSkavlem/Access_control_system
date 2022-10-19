using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Sentral
{
    public class TCPConnectionListner
    {
        
        public static MainActivity mainform { get; set; }
        public TCPConnectionListner(MainActivity main)
        {
            mainform = main;
            ThreadStart ts = new ThreadStart(StartListner);
            Thread TCPThread = new Thread(ts);
            TCPThread.Name = "TCPclientThread";
            TCPThread.IsBackground = true;
            TCPThread.Start();
        }
        private static void StartListner()
        {
            Socket ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            ListenSocket.Bind(serverEP);
            ListenSocket.Listen(10);

            while (true)
            {
                Socket ComSocket = ListenSocket.Accept(); // blokkerende metode
                // Write established comms to Output Window
                Debug.WriteLine("Comm Established: {0}:{1}", ComSocket.RemoteEndPoint, ComSocket.LocalEndPoint);
                // passes socket to other thread
                if (ComSocket.IsBound)
                {   //start new connection handler
                    TCPConnectionHandler tcp = new TCPConnectionHandler(mainform, ComSocket);
                }
            }
        }

    }
}
