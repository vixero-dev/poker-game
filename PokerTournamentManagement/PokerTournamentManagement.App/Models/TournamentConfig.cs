using PokerTournamentManagement.App.Models;

namespace PokerTournamentManagement.App
{
    internal record TournamentConfig(decimal BuyInAmount, int MaxBuyIns, int MaxPlayers, int MaxTableSize, IEnumerable<BlindLevel> BlindLevels);
}