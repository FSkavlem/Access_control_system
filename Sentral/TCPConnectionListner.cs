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
                                                            //starts the thread here in order to pass mainform handle
            mainform = main;
            ThreadStart ts = new ThreadStart(StartListner); //this thread handles incomming connection requests
            Thread TCPThread = new Thread(ts);                 
            TCPThread.Name = "TCPlistnerThread";                 
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
                Debug.WriteLine("Comm Established: {0}:{1}", ComSocket.RemoteEndPoint, ComSocket.LocalEndPoint);// Write established comms to Output Window
                if (ComSocket.IsBound) // passes socket to other thread
                {   //starts new thread that handles the connection
                    TCPConnectionHandler tcp = new TCPConnectionHandler(mainform, ComSocket);
                }
            }
        }

    }
}
