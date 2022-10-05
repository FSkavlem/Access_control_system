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
    class TCPclient
    {
        CardReaderForm mainform;
        public void RunCLient(CardReaderForm f)
        {
            mainform = f;
            ThreadStart ts = new ThreadStart(StartTCPClient);
            Thread TCPThread = new Thread(ts);
            TCPThread.Name = "TCPclientThread";
            TCPThread.IsBackground = true;
            TCPThread.Start();
        }
       
        void StartTCPClient()
        {
            Door door;
            bool error = false;
            Socket comSocket;
            bool newAccessTry = false;
            Tuple<bool,int> alarm;
            bool resetalarm = false;
            bool alarmSent = false;
            CardInfo lastuser = new CardInfo();

            while (true)
            {
                comSocket = Connect2Server();
                
                while (comSocket.Connected)
                {
                    //-------get global Vars------
                    newAccessTry = GetGlobalNewAccessTry();   
                    door = GetDoor();
                    alarm = GetGlobalAlarm();
                    resetalarm = GetResetAlarm();
                    //-------get global Vars------   

                    if ((bool)alarm.Item1 & !alarmSent) Statemachine(States.SendAlarmEvent);
                    if (resetalarm) Statemachine(States.ResetAlarm);
                    if (newAccessTry) Statemachine(States.AccessTry);
                    if (comSocket.Available > 0) Statemachine(States.TrafficOnTCPsocket);

                    void Statemachine(int currentstate)
                    {
                        switch (currentstate)
                        {
                            case States.SendAlarmEvent:
                                AlarmEvent alarmEvent = new AlarmEvent { Alarm_type = alarm.Item2, Time = door.time, DoorNumber = door.nodeNum, LastUser = lastuser };
                                sendClassAsJSON_String(PackageIdentifier.AlarmEvent, alarmEvent, ref comSocket);
                                alarmSent = true;
                                SetGlobalResetAlarm(true);
                                break;
                            case States.ResetAlarm:
                                alarmSent = false; 
                                break;
                            case States.AccessTry:
                                int cardid = GetGlobalCardID();
                                CardInfo cardInfo = new CardInfo { CardID = cardid, DoorNr = door.nodeNum, PinEntered = door.enteredPin, Time = door.time };
                                sendClassAsJSON_String(PackageIdentifier.PinValidation, cardInfo, ref comSocket);
                                SetGlobalCard_AccessTry(cardInfo, false); //sett newAccessTry to false and passes CardInfo Sent to Main 
                                lastuser = cardInfo;
                                break;
                            case States.TrafficOnTCPsocket:
                                string receivedString = ClassLibrary.Message.ReceiveString(comSocket, out error);
                                string packageID = ClassLibrary.Message.GetPackageIdentifier(ref receivedString, out receivedString);
                                switch (packageID)
                                {
                                    case PackageIdentifier.ServerACK:
                                        Debug.WriteLine("{0} received from: {1}", receivedString, comSocket.RemoteEndPoint);
                                        break;
                                    case PackageIdentifier.PinValidation:
                                        ReturnCardComms returnCardComms = JsonSerializer.Deserialize<ReturnCardComms>(receivedString);
                                        SetGlobalPinValidation(returnCardComms);
                                        break;
                                    case PackageIdentifier.RequestNumber:
                                        break;
                                    case PackageIdentifier.ResetAlarm:
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        Thread.Sleep(20);
                    }
                }
             Thread.Sleep(1000); //Wait 1000MS before trying to reconnect
            }
        }

        private void SetGlobalResetAlarm(bool x) => mainform.Invoke(new SetResetAlarm(mainform.SetResetAlarm),x);
        private int GetGlobalCardID() => (int)mainform.Invoke(new GetCardID(mainform.getCardIDfromForm));
        private void SetGlobalPinValidation(ReturnCardComms? returnCardComms)
        {
            SetPinValidation setPinValidation = new SetPinValidation(mainform.SetPinValidation);
            mainform.Invoke(setPinValidation, returnCardComms.Validation);
        }
        private void SetGlobalCard_AccessTry(CardInfo cardInfo, bool v)
        {
            SetAccessTryEvent setAccessTryEvent = new SetAccessTryEvent(mainform.SetNewAccessTry);
            SetCardInfo updateCardinfo = new SetCardInfo(mainform.SetCardInfo);
            mainform.Invoke(updateCardinfo, cardInfo);
            mainform.Invoke(setAccessTryEvent, false);
        }
        private bool GetGlobalNewAccessTry() => (bool)mainform.Invoke(new GetAccessTry(mainform.GetNewAccessTry));
        private Door GetDoor() => (Door)mainform.Invoke(new GetPublicDoor(mainform.GetPublicDoor));
        private Tuple<bool, int> GetGlobalAlarm() => (Tuple<bool, int>)mainform.Invoke(new GetAlarm(mainform.GetAlarm));
        private bool GetResetAlarm() => (bool)mainform.Invoke(new GetResetAlarm(mainform.GetResetAlarm));
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
