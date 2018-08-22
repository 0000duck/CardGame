using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames2
{
    class Message
    {
        public string Content { get; set; }

        /// <summary>
        /// Affiche les règles du jeu
        /// </summary>
        /// <returns></returns>
        public Message getRegles()
        {
            return new Message()
            {
                Content = "Voici les règles du Choice Poker : " +
                "\n1. Chaque joueur possède 5 cartes et une mise de départ." +
                "\n2. Durant votre premier tour, choisissez les cartes que vous souhaitez remplacer." +
                "\n3. La première mise de 100 crédits est obligatoire." +
                "\n4. Vous ne pouvez pas vous coucher ni suivre la mise, vous pouvez seulement relance la mise." +
                "\n5. Le joueur avec la plus grosse mise choisit l'ordre de force de ce tour." +
                "\n   5.1 Les ordres de forces sont Stronger / Weaker." +
                "\n   5.2 Stronger signifie que la main la plus forte gagne et inversement pour Weaker." +
                "\n6. Tapez F1 pour obtenir l'aide de commandes" +
                "\n7. Votre mise et votre stratégie est très importante, un crédit de plus que votre adversaire peut vous faire gagner !"
            };
        }
    }
}
