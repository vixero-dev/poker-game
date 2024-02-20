namespace PokerTournamentManagement.App
{
    internal class Player(string name)
    {
        public string Name { get; } = name;
        public int BuyInCount { get; private set; } = 0;
        public decimal BuyInAmount { get; private set; } = 0m;

        public bool BuyIn(decimal buyInAmount)
        {
            if (buyInAmount <= 0)
            {
                return false;
            }

            BuyInCount++;
            BuyInAmount += buyInAmount;

            return true;
        }
    }
}
