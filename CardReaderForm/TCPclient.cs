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
/*********************************************handles for other threads(delegates)***************************************/
    public delegate void PinAnswerFromDB(object sender, PinAnswerFromDBEventArgs e);
    public delegate void ServerConnectStatus(object sender, bool x);
    class TCPclient
    {
/******************************************************public vars******************************************************/
        Socket comSocket;
        CardReaderForm mainform;
        public event PinAnswerFromDB _PinAnswerFromDB;
        public event EventHandler ServerConnectStatus;
        public int doornr;

        public void RunCLient(CardReaderForm f,int DOORnr)
        {
            /* 
             * this method starts the TCPclient 
             */ 
            mainform = f;                                              //passes mainform window handle to clas
            doornr = DOORnr;                                           //sets inital door nr
            ThreadStart ts = new ThreadStart(StartTCPClient);          //creates a new thread start
            Thread TCPThread = new Thread(ts);                         //makes a new thread based on thread start
            TCPThread.Name = "TCPclientThread";                        //gives the thread a name        
            TCPThread.IsBackground = true;
            TCPThread.Start();                                         //starts the thread
        }

/*****************************************************eventhandlers****************************************************/
        private void Mainform_NewAccesRequest(object? sender, NewAccessRequestEventArgs e)      
        {
            /* 
             * this event happends when CardReaderForm publishes a NewAccessRequest. 
             * and sends its to json deserilizer with given packageid
             */
            CardInfo cardInfo = e.carddata;
            cardInfo.DoorNr = GetDoorNr();
            sendClassAsJSON_String(PackageIdentifier.PinValidation, cardInfo, ref comSocket);
        }
        private void OnProcessExit(object? sender, EventArgs e)
        {
            /* 
             * this event happends when procces is closing down, sends a closing down report to server
             * waiting 100ms to ensure traffic sent before closing down. Should be ASYNC
             */
            sendClassAsJSON_String(PackageIdentifier.ClosingDown, "", ref comSocket);
            Thread.Sleep(100);
        }
        private void Mainform_PublishAlarm(PublishAlarmEventArgs e)
        {
            AlarmEvent alarmEvent = e.alarmevent;
            sendClassAsJSON_String(PackageIdentifier.AlarmEvent, alarmEvent, ref comSocket);
        }
/*****************************************************TCPCLIENT*******************************************************/
        private void StartTCPClient()
        {
           /* 
            * The TCPclient in primarely built around what is received from the tcp pipe, this is done by having a constant while loop
            * listeneing to any incomming traffic on connected comsocket. If disconnected, will try reconnect every 1 second.
            * In all the communcation between server and client there is package identifiers that
            * lets us easily identify what class to deserialize the json object into.
            */
            bool error = false; 
            CardInfo lastuser = new CardInfo();                                                                   //makes a placeholder for lastuser
            mainform.NewAccesRequest += Mainform_NewAccesRequest;                                                 //subscribes to event from cardreaderform when a new access try is flagged
            mainform.PublishAlarm += Mainform_PublishAlarm;                                                       //subscribes to event from cardreaderform when a new alarm is to be sent to server
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);                               //event when proccess is ending
            while (true)                                
            {
                comSocket = Connect2Server();                                                                     //this method establishes connection to Sentral server 
                while (comSocket.Connected)                                                                       //loops that runs aslong as connected to server
                {                       
                    ServerConnectStatus?.Invoke(true, new EventArgs());                                           //sends established connection status to cardreaderform UI
                    string receivedString = Messages.ReceiveString(comSocket, out error);                         //gets recived string from tcp pipe
                    string packageID =Messages.GetPackageIdentifier(ref receivedString, out receivedString);      //seperates out the packageid from the json string.
                    switch (packageID)                                                                            //switch based on the received packageID
                    {
                        case PackageIdentifier.ServerACK:                                                         //packageid: when server Acknowledge is received
                            Debug.WriteLine("{0} received from: {1}", receivedString, comSocket.RemoteEndPoint);  //print the server endpoint in output for debugging purposes
                            break;
                        case PackageIdentifier.PinValidation:                                                     //packageid: this is when a returned pinvalidation from server arrived
                            ReturnCardComms x = JsonSerializer.Deserialize<ReturnCardComms>(receivedString);      //unpacks the received json string into a class
                            PinAnswerFromDB handler = _PinAnswerFromDB;                                           //creates and event
                            handler?.Invoke(this, new PinAnswerFromDBEventArgs(x));                               //flags the event, subscriber is CardReaderForm              
                            break;
                        case PackageIdentifier.RequestNumber:                                                     //not implemented
                            break;
                        case PackageIdentifier.ResetAlarm:                                                        //not implemented
                            break;
                        default:
                            break;
                    }
                    Thread.Sleep(20);                                                                             //sleep the thread 20 ms iot.wait for cardreaderform
                }
             ServerConnectStatus?.Invoke(false, new EventArgs());                                                 //sends not connected to cardreaderform UI
             Thread.Sleep(1000);                                                                                  //Wait 1000MS before trying to reconnect
            }
        }
/*****************************************************FUNCTIONS*******************************************************/
        private int GetDoorNr() => mainform.Invoke((mainform.GetDoorNr));              //gets the doornr from cardreaderform UI
        private void sendClassAsJSON_String(string packageID, object class2send, ref Socket socket)
        {
            /* 
             * this is a generic method that sends a class as json string. It takes the packageID(6digits) and 
             * adds its to the front of the json string. This way we always know that the first 6 digits are packageID
             */
            bool error;                                                         //legacy not in use
            string jsonString = JsonSerializer.Serialize(class2send);           //makes a jsonstring from a class represented as object for generic
            jsonString = Messages.AddPackageIdentifier(packageID, jsonString);  //adds the packageidentifier to the json string
            var complete = Messages.SendString(socket, jsonString, out error);  //send the complete string. Error and complete bool is legacy code
        }
        private static Socket Connect2Server()
        {
            /* 
             * this method connects the TCPclient to the TCP server based on the server information
             * located in the class.cs public static class SentralInfo
             */
            Socket clientSocket = new Socket(AddressFamily.InterNetwork,       //generates a TCP socket
                SocketType.Stream,
                ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse(SentralInfo.ipaddress), SentralInfo.port); //sets ip address and port to server from Sentralinfo class
            try
            {
                clientSocket.Connect(serverEP);                                //tries to connect the TCP socket based on endpoint.

            }
            catch (Exception e)
            {
                Debug.WriteLine("Link failed to establish");                  //if it fails prints to output window for debugging purposes.
            }
            return clientSocket;
        }
    }
}
