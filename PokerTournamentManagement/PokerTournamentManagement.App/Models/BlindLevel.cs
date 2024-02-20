namespace PokerTournamentManagement.App.Models
{
    internal record BlindLevel(int Level, int SmallBlind, int BigBlind, TimeSpan Duration);
}
