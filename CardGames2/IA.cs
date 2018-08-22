using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames2
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
    }
}
