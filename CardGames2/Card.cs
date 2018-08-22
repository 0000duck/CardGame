using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CardGames2
{
    class Card
    {
        public string Value { get; set; }
        public string Type { get; set; }

        public Card(string t, string v)
        {
            this.Type = t;
            this.Value = v;
        }

        public string Get() => JsonConvert.SerializeObject(this);
    }
}
