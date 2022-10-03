using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.IO.Ports;
using System.Text;
using System.Diagnostics;


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
            Door door = new Door();
            bool error = false;
            Socket comSocket;
            bool newAccessTry = false;

            GetAccessTry newAccessTryUpdate = new GetAccessTry(mainform.GetNewAccessTry);
            SetAccessTryEvent setAccessTryEvent = new SetAccessTryEvent(mainform.SetNewAccessTry);
            GetCardID getCardID = new GetCardID(mainform.getCardIDfromForm);
            GetPublicDoor getPublicDoor = new GetPublicDoor(mainform.GetPublicDoor);
            SetPinValidation setPinValidation = new SetPinValidation(mainform.SetPinValidation);

            while (true)
            {
                comSocket = Connect2Server(); //no need for 
                while (comSocket.Connected)
                {
                    newAccessTry = (bool)mainform.Invoke(newAccessTryUpdate);
                    door = (Door)mainform.Invoke(getPublicDoor);

                    if (newAccessTry)
                    {
                        int cardid = (int)mainform.Invoke(getCardID);
                        CardInfo cardInfo = new CardInfo { CardID = cardid, Number = door.nodeNum, PinEntered = door.enteredPin, Time = door.time };

                        string jsonString = JsonSerializer.Serialize(cardInfo);
                        jsonString = ClassLibrary.Message.AddPackageIdentifier(PackageIdentifier.PinValidation, jsonString);
                        var complete = ClassLibrary.Message.SendString(comSocket, jsonString, out error);
                        mainform.Invoke(setAccessTryEvent, false);
                    }
                    if (comSocket.Available > 0)
                    {
                        string? receivedString = ClassLibrary.Message.ReceiveString(comSocket, out error);
                        string packageID = ClassLibrary.Message.GetPackageIdentifier(ref receivedString, out receivedString);
                        switch (packageID)
                        {
                            case PackageIdentifier.ServerACK:
                                Debug.WriteLine("{0} received from: {1}", receivedString, comSocket.RemoteEndPoint);
                                break;
                            case PackageIdentifier.PinValidation:
                                ReturnCardComms returnCardComms = JsonSerializer.Deserialize<ReturnCardComms>(receivedString);
                                mainform.Invoke(setPinValidation, returnCardComms.Validation);
                                break;
                            case PackageIdentifier.RequestNumber:
                            default:
                                break;
                        }
                    }
                }
                Thread.Sleep(1000); //Wait 1000MS before trying to reconnect
            }
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
