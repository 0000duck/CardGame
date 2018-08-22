using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using Newtonsoft.Json;

namespace CardGames2
{
    class Program
    {
        public static List<Plateau> parties = new List<Plateau>();

        static void Main(string[] args)
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Connect", PrintConnectRequest);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Change", ChangeCardRequest);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Restart", RestartAGame);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Mise", PlayerTurn);
            //Start listening for incoming connections
            Connection.StartListening(ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 8000));

            //Print out the IPs and ports we are now listening on
            Console.WriteLine("Server listening for TCP connection on:");
            foreach (System.Net.IPEndPoint localEndPoint in Connection.ExistingLocalListenEndPoints(ConnectionType.TCP))
                Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);

            //Let the user close the server
            Console.WriteLine("\nPress any key to close server.");
            Console.ReadKey(true);
            //We have used NetworkComms so we should ensure that we correctly call shutdown
            NetworkComms.Shutdown();
        }

        /// <summary>
        /// Print the connection request received from a client
        /// </summary>
        /// <param name="header">The packet header associated with the incoming message</param>
        /// <param name="connection">The connection used by the incoming message</param>
        /// <param name="message">The message to be printed to the console</param>
        private static void PrintConnectRequest(PacketHeader header, Connection connection, string message)
        {
            string clientIp = GetIp(connection.ToString());
            int clientPort = GetPort(connection.ToString());
            Console.WriteLine(connection.ToString() + " --> " + message);

            Message obj = JsonConvert.DeserializeObject<Message>(message);
            Player p = new Player() { Name = obj.Content, Ip = clientIp, Port = clientPort, Cards = new List<Card>(), Crédit = 10000 };
            Plateau plateau = new Plateau() { Player = p, Cards = new Deck(), Player2 = new IA() { Cards = new List<Card>(), Crédit = 10000 } };
            parties.Add(plateau);
            foreach(Card c in plateau.Cards.getCards(5))
            {
                plateau.Player.Cards.Add(c);
            }
            string returned_plat = JsonConvert.SerializeObject(plateau);
            string res = JsonConvert.SerializeObject(new Message() { Content = returned_plat });
            NetworkComms.SendObject("Start Turn", p.Ip, p.Port, res);
        }

        private static void ChangeCardRequest(PacketHeader header, Connection connection, string message)
        {
            string clientIp = GetIp(connection.ToString());
            int clientPort = GetPort(connection.ToString());
            Console.WriteLine(connection.ToString() + " --> " + message);

            Message msg = JsonConvert.DeserializeObject<Message>(message);
            Plateau p = JsonConvert.DeserializeObject<Plateau>(msg.Content);
            Plateau pFind = FindPlayer(clientIp, clientPort);
            if (p != pFind)
                pFind = p;
            Message msgRes = new Message() { Content = JsonConvert.SerializeObject(p.Player.Cards) };
            string res = JsonConvert.SerializeObject(msgRes);
            NetworkComms.SendObject("Card Changed", p.Player.Ip, p.Player.Port, res);
        }

        private static Plateau FindPlayer(string ip, int port)
        {
            foreach (Plateau p in parties)
                if (p.Player.Ip == ip && p.Player.Port == port)
                    return p;
            Plateau plateau = new Plateau()
            {
                Player = new Player() { Ip = ip, Port = port },
                Player2 = new IA(),
                Cards = new Deck()
            };
            parties.Add(plateau);
            return plateau;
        }

        private static void RestartAGame(PacketHeader header, Connection connection, string message)
        {
            string clientIp = GetIp(connection.ToString());
            int clientPort = GetPort(connection.ToString());
            Console.WriteLine(connection.ToString() + " --> " + message);

            Plateau p = FindPlayer(clientIp, clientPort);
            p.Cards = new Deck();
            p.Player.Cards = p.Cards.getCards(5);
            p.Player2 = new IA();
            p.Player2.Cards = p.Cards.getCards(5);

            string cards = JsonConvert.SerializeObject(p.Player.Cards);
            string res = JsonConvert.SerializeObject(new Message() { Content = cards });
            NetworkComms.SendObject("Start Turn", clientIp, clientPort, res);
        }

        private static void PlayerTurn(PacketHeader header, Connection connection, string message)
        {
            string clientIp = GetIp(connection.ToString());
            int clientPort = GetPort(connection.ToString());
            Console.WriteLine(connection.ToString() + " --> " + message);

            Message msg = JsonConvert.DeserializeObject<Message>(message);
            Plateau plateau = JsonConvert.DeserializeObject<Plateau>(msg.Content);

            for(int i = 0; i < parties.Count; i++)
                if (parties[i].Player.Ip == clientIp && parties[i].Player.Port == clientPort)
                    parties[i] = plateau;
            string res = JsonConvert.SerializeObject(new Message() { Content = "OK" });
            NetworkComms.SendObject("Play", clientIp, clientPort, res);
        }

        private static string GetIp(string ip)
        {
            string clientIp = ip.Split(':').First();
            return clientIp.Split(' ').Last();
        }

        private static int GetPort(string ip)
        {
            string port = ip.Split(':').Last();
            return int.Parse(port.Split(' ').First());
        }
    }
}
