namespace pokerTestParser
{
    internal class Player
    {
        public string Name { get; set; }
        public decimal SummaryPlayerPot { get; set; }
        public decimal StartBalance { get; set; }

        public Player() { }
        public Player(string name, decimal startBalance, decimal summaryPlayerPot)
        {
            Name = name;
            StartBalance = startBalance;
            SummaryPlayerPot = summaryPlayerPot;
        }
    }
}
