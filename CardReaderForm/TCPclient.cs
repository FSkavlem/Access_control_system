using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.IO.Ports;
using System.Text;
using System.Diagnostics;
using System.Dynamic;


namespace CardReaderForm
{
    public delegate void PinAnswerFromDB(object sender, PinAnswerFromDBEventArgs e);
    public class PinAnswerFromDBEventArgs : EventArgs
    {
        public PinAnswerFromDBEventArgs(ReturnCardComms returnCardComms)
        {
            this.returnCardComms = returnCardComms;
        }

        public ReturnCardComms returnCardComms { get; set; }
        
    }
  
    class TCPclient
    {

        Socket comSocket;
        CardReaderForm mainform;
        public event PinAnswerFromDB _PinAnswerFromDB;

        public void RunCLient(CardReaderForm f)
        {
            mainform = f;
            ThreadStart ts = new ThreadStart(StartTCPClient);
            Thread TCPThread = new Thread(ts);
            TCPThread.Name = "TCPclientThread";
            TCPThread.IsBackground = true;
            TCPThread.Start();
        }

        //eventHandlers
        private void Mainform_NewAccesRequest(object? sender, NewAccessRequestEventArgs e)
        {
            CardInfo cardInfo = e.carddata;
            sendClassAsJSON_String(PackageIdentifier.PinValidation, cardInfo, ref comSocket);
        }
        private void Mainform_PublishAlarm(PublishAlarmEventArgs e)
        {
            AlarmEvent alarmEvent = e.alarmevent;
            sendClassAsJSON_String(PackageIdentifier.AlarmEvent, alarmEvent, ref comSocket);
        }


        void StartTCPClient()
        {
            Door door;
            bool error = false;
            
            bool newAccessTry = false;
            Tuple<bool,int> alarm;
    
            CardInfo lastuser = new CardInfo();

            //subscribe to events
            mainform.NewAccesRequest += Mainform_NewAccesRequest;
            mainform.PublishAlarm += Mainform_PublishAlarm;


            while (true)
            {
                comSocket = Connect2Server();
                
                while (comSocket.Connected)
                {
                    string receivedString = ClassLibrary.Message.ReceiveString(comSocket, out error);
                    string packageID = ClassLibrary.Message.GetPackageIdentifier(ref receivedString, out receivedString);
                    switch (packageID)
                    {
                        case PackageIdentifier.ServerACK:
                            Debug.WriteLine("{0} received from: {1}", receivedString, comSocket.RemoteEndPoint);
                            break;
                        case PackageIdentifier.PinValidation:
                            ReturnCardComms x = JsonSerializer.Deserialize<ReturnCardComms>(receivedString);
                                        
                            PinAnswerFromDB handler = _PinAnswerFromDB;
                            handler?.Invoke(this, new PinAnswerFromDBEventArgs(x)); //fire event
                            break;
                        case PackageIdentifier.RequestNumber:
                            break;
                        case PackageIdentifier.ResetAlarm:
                            break;
                        default:
                            break;
                    }
            Thread.Sleep(20);
                }
             Thread.Sleep(1000); //Wait 1000MS before trying to reconnect
            }
        }

        private void sendClassAsJSON_String(string packageID, object class2send, ref Socket socket)
        {
            bool error;
            string jsonString = JsonSerializer.Serialize(class2send);
            jsonString = ClassLibrary.Message.AddPackageIdentifier(packageID, jsonString);
            var complete = ClassLibrary.Message.SendString(socket, jsonString, out error);
        }
        private static Socket Connect2Server()
        {
            Socket clientSocket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            try
            {
                clientSocket.Connect(serverEP);

            }
            catch (Exception e)
            {
                Debug.WriteLine("Link failed to establish");
            }
            return clientSocket;
        }
    }
}
