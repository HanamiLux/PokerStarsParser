using System.Collections.Generic;
using System.Linq;

namespace pokerTestParser
{
    internal class PokerInfoHand
    {
        public string Name { get; set; }
        public decimal SummaryPot { get; set; }
        public List<Player> Players { get; set; }

        public PokerInfoHand(string name, ICollection<Player> players)
        {
            Name = name;
            Players = players.ToList();
            CountSummaryPot();
        }
        
        private void CountSummaryPot()
        {
            SummaryPot = Players.Sum(x => x.SummaryPlayerPot);
        }
    }
}
