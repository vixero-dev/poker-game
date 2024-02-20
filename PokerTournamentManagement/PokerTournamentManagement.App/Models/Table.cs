namespace PokerTournamentManagement.App
{
    internal record Table(int MaxPlayers)
    {
        public List<Player> Players { get; } = [];
    }
}
