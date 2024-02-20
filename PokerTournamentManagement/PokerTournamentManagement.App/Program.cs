using PokerTournamentManagement.App;
using PokerTournamentManagement.App.Models;

Console.WriteLine("Welcome to the Poker Tournament Manager");

List<BlindLevel> blindLevels =
    [
        new(1, 25, 50, TimeSpan.FromMinutes(20)),
        new(2, 50, 100, TimeSpan.FromMinutes(20)),
        new(3, 100, 200, TimeSpan.FromMinutes(20)),
        new(4, 200, 400, TimeSpan.FromMinutes(20)),
        new(5, 250, 500, TimeSpan.FromMinutes(20)),
        new(6, 500, 1000, TimeSpan.FromMinutes(20)),
        new(7, 2000, 4000, TimeSpan.FromMinutes(20)),
        new(8, 2500, 5000, TimeSpan.FromMinutes(20)),
        new(9, 3000, 6000, TimeSpan.FromMinutes(20)),
        new(10, 4000, 8000, TimeSpan.FromMinutes(20))
    ];

Console.Write("Enter buy-in amount: ");
decimal buyInAmount = decimal.Parse(Console.ReadLine());

Console.Write("Enter maximum buy-ins: ");
int maxBuyIns = int.Parse(Console.ReadLine());

Console.Write("Enter maximum players: ");
int maxPlayers = int.Parse(Console.ReadLine());

Console.Write("Enter maximum table size: ");
int maxTableSize = int.Parse(Console.ReadLine());

TournamentConfig tournamentConfig = new (buyInAmount, maxBuyIns, maxPlayers, maxTableSize, blindLevels);

Tournament tournament = new(tournamentConfig);

Console.WriteLine("Tournament configuration complete. Starting tournament...");