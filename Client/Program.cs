using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;


namespace Client
{
    class Program
    {
        public static string serverIp = "127.0.0.1";
        public static int serverPort = 8000;
        public static Plateau plateau = new Plateau() { Cards = new Deck()};
        public static bool run = false;

        static void Main(string[] args)
        {
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Start Turn", PrintIncomingMsg);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Card Changed", StartSecondTurn);
            NetworkComms.AppendGlobalIncomingPacketHandler<string>("Play", PlayTurn);

            while (true)
            {
                //Check if user wants to go around the loop
                Console.WriteLine("\n .quit to quit.\n.rules for the rules.\n.cards for see your cards.\n.monney for see your money.\n.start to start a new game!");
                var a = Console.ReadLine();
                switch (a)
                {
                    case ".quit":
                        run = false;
                        NetworkComms.Shutdown();
                        Environment.Exit(0);
                        break;
                    case ".rules":
                        Console.Clear();
                        Message msg = new Message().getRegles();
                        Console.WriteLine(msg.Content);
                        break;
                    case ".cards":
                        Console.Clear();
                        if (run == true)
                            foreach (Card c in plateau.Player.Cards)
                                Console.WriteLine(c.Value + c.Type);
                        else
                            Console.WriteLine("Pas de cartes en main...");
                        break;
                    case ".monney":
                        Console.WriteLine("Crédits disponibles : ");
                        break;
                    case ".start":
                        if (run == true)
                            RestartGame();
                        else
                        {
                            Console.WriteLine("Entrez votre pseudonyme : ");
                            string pseudo = Console.ReadLine();
                            connectToServer(serverIp, serverPort, pseudo);
                        }
                        break;
                    default:
                        break;

                }
                Thread.Sleep(3000);
            }
        }

        private static void connectToServer(string ip, int port, string pseudo)
        {
            Message msg = new Message() { Content = pseudo };
            string json = JsonConvert.SerializeObject(msg);
            //Send the message in a single line
            NetworkComms.SendObject("Connect", ip, port, json);
        }

        /// <summary>
        /// Print the connection request received from a client
        /// </summary>
        /// <param name="header">The packet header associated with the incoming message</param>
        /// <param name="connection">The connection used by the incoming message</param>
        /// <param name="message">The message to be printed to the console</param>
        private static void PrintIncomingMsg(PacketHeader header, Connection connection, string message)
        {
            Console.Clear();
            Message msg = JsonConvert.DeserializeObject<Message>(message);
            plateau = JsonConvert.DeserializeObject<Plateau>(msg.Content);
            run = true;
            foreach(Card c in plateau.Player.Cards)
            {
                Console.WriteLine(c.Value + c.Type);
            }
            Console.WriteLine("\nSelect the cards you wish to change (ex : 1-3-5 change cards 1, 3 and 5.");
            string userRes = Console.ReadLine();
            foreach(char c in userRes)
            {
                switch(c)
                {
                    case '1':
                        plateau.Player.Cards[0] = plateau.Cards.getACard();
                        break;
                    case '2':
                        plateau.Player.Cards[1] = plateau.Cards.getACard();
                        break;
                    case '3':
                        plateau.Player.Cards[2] = plateau.Cards.getACard();
                        break;
                    case '4':
                        plateau.Player.Cards[3] = plateau.Cards.getACard();
                        break;
                    case '5':
                        plateau.Player.Cards[4] = plateau.Cards.getACard();
                        break;
                    default:
                        break;
                }
            }
            plateau.Player2.Cards = plateau.Cards.getCards(5);
            string change = JsonConvert.SerializeObject(plateau);
            string res = JsonConvert.SerializeObject(new Message() { Content = change });
            NetworkComms.SendObject("Change", serverIp, serverPort, res);
        }

        private static void StartSecondTurn(PacketHeader header, Connection connection, string message)
        {
            Message msg = JsonConvert.DeserializeObject<Message>(message);
            List<Card> cards = JsonConvert.DeserializeObject<List<Card>>(msg.Content);
            if (plateau.Player.Cards != cards)
                plateau.Player.Cards = cards;
            plateau.Player.ChangeMise(100);
            plateau.Player2.ChangeMise(100);
            foreach (Card c in cards)
            {
                Console.WriteLine(c.Value + c.Type);
            }
            bool played = false;
            while(played == false)
            {
                Console.WriteLine("\nCrédits disponibles : " + plateau.Player.Crédit);
                Console.WriteLine("Mise Actuelle : " + plateau.Player.Mise);
                Console.WriteLine("Votre adversaire possède " + plateau.Player2.Crédit + " crédits et a miser " + plateau.Player2.Mise);
                Console.WriteLine("Combien misez-vous ?");
                string Ures = Console.ReadLine();
                try
                {
                    int.TryParse(Ures, out int nb);
                    if (nb == 0)
                    {
                        Console.WriteLine("Laisser le choix de force à l'adversaire ? ( press y or n )");
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.Y)
                        {
                            played = true;
                        }
                    } else
                    {
                        if (nb >= plateau.Player.Crédit)
                            plateau.Player.ChangeMise(plateau.Player.Crédit);
                        else
                            plateau.Player.ChangeMise(nb);
                        plateau.Player2.PlayTurn();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Impossible de définir un nombre !");
                }

            }
            string change = JsonConvert.SerializeObject(plateau);
            string res = JsonConvert.SerializeObject(new Message() { Content = change });
            NetworkComms.SendObject("Mise", serverIp, serverPort, res);
        }

        private static void PlayTurn(PacketHeader header, Connection connection, string message)
        {
            Console.Clear();

            foreach (Card c in plateau.Player.Cards)
                Console.WriteLine(c.Value + c.Type);
            Console.WriteLine("\nCrédits disponibles : " + plateau.Player.Crédit);
            Console.WriteLine("Mise Actuelle : " + plateau.Player.Mise);
            Console.WriteLine("Votre adversaire possède " + plateau.Player2.Mise + " et a miser " + plateau.Player2.Mise);
            bool played = false;
            while (played == false)
            {
                Console.WriteLine("Combien misez-vous ?");
                string Ures = Console.ReadLine();
                try
                {
                    int.TryParse(Ures, out int nb);
                    if (nb == 0)
                    {
                        Console.WriteLine("Laisser le choix de force à l'adversaire ? ( press y or n )");
                        var key = Console.ReadKey();
                        if (key.Key == ConsoleKey.Y)
                        {
                            played = true;
                        }
                    }
                    else
                    {
                        if (nb >= plateau.Player.Crédit)
                            plateau.Player.ChangeMise(plateau.Player.Crédit);
                        else
                            plateau.Player.ChangeMise(nb);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Impossible de définir un nombre !");
                }

            }
            string change = JsonConvert.SerializeObject(plateau);
            string res = JsonConvert.SerializeObject(new Message() { Content = change });
            NetworkComms.SendObject("Mise", serverIp, serverPort, res);
        }

        private static void RestartGame()
        {
            Console.WriteLine("Démarrer une nouvelle partie ? ( press y or n )");
            var key = Console.ReadKey();
            if(key.Key == ConsoleKey.Y)
            {
                string json = JsonConvert.SerializeObject(new Message() { Content = "Restart" });
                NetworkComms.SendObject("Restart", serverIp, serverPort, json);
            } else if(key.Key == ConsoleKey.N)
            {
                Console.Clear();
                run = false;
            }
        }
    }
}
