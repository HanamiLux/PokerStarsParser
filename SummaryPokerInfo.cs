using System.Collections.Generic;
using System.Linq;

namespace pokerTestParser
{
    internal class SummaryPokerInfo
    {
        public int TotalHands { get; set; }
        public decimal SummaryPot { get; set; }
        public List<PokerInfoHand> HandsList { get; }
        public SummaryPokerInfo(List<PokerInfoHand> handsList)
        {
            HandsList = handsList;
            TotalHands = handsList.Count;
            SummaryPot = handsList.Sum(hand => hand.SummaryPot);
        }
    }
}
