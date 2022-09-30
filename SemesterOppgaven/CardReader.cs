using ClassLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

class CardReader
{
    static void Main(string[] args)
    {
        bool noConnect;
        bool error = false;
        Socket comSocket;
        CardComms comms = new CardComms{Time=DateTime.Now,Number=1,CardID=1234,Pin="1111",Alarm_bool=false,Alarm_type =0,Lastuser = 4432, Need_validation = true};

        while (true)
        {
            comSocket = Connect2Server(out noConnect);
            if (!noConnect)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(comms, comms.GetType(), options);
                error = SharedMethod.SendString(comSocket, jsonString, out error);

                while (true)
                {
                    string mottattTekst = MottaTekst(comSocket, out error);
                    if (error) break;
                    Console.WriteLine(mottattTekst);
                }
            }
            Thread.Sleep(1000);
        }

        Console.WriteLine("Bryter forbindelsen med serveren ...");
        if (!noConnect)
        {
            comSocket.Shutdown(SocketShutdown.Both);
            comSocket.Close();
        }
    }

    static string MottaTekst(Socket kommSokkel, out bool error)
    {
        string svar = "";
        error = false;

        try
        {
            byte[] data = new byte[1024];
            int antallBytesMottatt = kommSokkel.Receive(data);

            if (antallBytesMottatt > 0) svar = Encoding.ASCII.GetString(data, 0, antallBytesMottatt);
            else error = true;
        }
        catch (Exception unntak)
        {
            Console.WriteLine("Feil: " + unntak.Message);
            error = true;
        }
        return svar;
    }
    private static Socket Connect2Server(out bool noConnect)
    {
        noConnect = false;
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
            Console.WriteLine("Link failed to establish");
            noConnect = true;
        }
        return clientSocket;
    }
}
