using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames2
{
    class Deck
    {
        public List<Card> cards = new List<Card>();
        private List<string> types = new List<string>();
        
        /// <summary>
        /// Initialise toutes les cartes du deck
        /// </summary>
        public Deck()
        {
            types.Add(" de coeur");
            types.Add(" de carreau");
            types.Add(" de trèfle");
            types.Add(" de pique");

            foreach(string type in types)
            {
                for(int i = 1; i < 14; i++)
                {
                    switch(i)
                    {
                        case 11:
                            cards.Add(new Card(type, "Valet"));
                            break;
                        case 12:
                            cards.Add(new Card(type, "Dame"));
                            break;
                        case 13:
                            cards.Add(new Card(type, "Roi"));
                            break;
                        default:
                            cards.Add(new Card(type, i.ToString()));
                            break;
                    }
                }
            }
        }

        public void ShowCards()
        {
            foreach(Card c in cards)
            {
                Console.WriteLine(c.Get());
            }
        }

        public List<Card> getCards(int nb)
        {
            Random rdm = new Random();
            List<Card> res = new List<Card>();
            for(int i = 0; i < nb; i++)
            {
                int nbrdm = rdm.Next(0, 51);
                res.Add(cards[nbrdm]);
                cards.Remove(cards[nbrdm]);
            }
            return res;
        }
    }
}
