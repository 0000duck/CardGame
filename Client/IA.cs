using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class IA
    {
        public List<Card> Cards { get; set; }
        public int Crédit { get; set; }
        public int Mise { get; set; }

        public void ChangeMise(int nb)
        {
            Mise = Mise + nb;
            Crédit = Crédit - nb;
        }

        public void PlayTurn()
        {
            int value = 0;
            
            #region Affichage Cartes IA
            List<Card> doublons = Cards.GroupBy(c => c.Value).Where(g => g.Count() > 1).SelectMany(g => g).ToList();
            if (doublons.Count() > 1)
                Console.WriteLine("Doublon !");
            Console.WriteLine("Cartes de l'IA : ");
            foreach (Card c in Cards)
                Console.WriteLine(c.Value + c.Type);
            #endregion

            foreach (Card c in Cards)
            {
                switch(c.Value)
                {
                    case "As":
                        value += 14;
                        break;
                    case "Roi":
                        value += 13;
                        break;
                    case "Dame":
                        value += 12;
                        break;
                    case "Valet":
                        value += 11;
                        break;
                    default:
                        value += int.Parse(c.Value);
                        break;
                }
            }
            Console.WriteLine("Value de la main : " + value);
        }

        // public int PlaySecondTurn()
        //{

        //}
    }
}
