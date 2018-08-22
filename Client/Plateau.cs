using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Plateau
    {
        public Player Player { get; set; }
        public IA Player2 { get; set; }
        public Deck Cards { get; set; }
    }
}
