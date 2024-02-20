using Bogus;
using FluentAssertions;
using PokerTournamentManagement.App;
using PokerTournamentManagement.App.Models;

namespace PokerTournamentManagement.Tests
{
    public class TournamentTests
    {
        private readonly Faker<Player> playerFaker;

        private readonly List<BlindLevel> blindLevels;

        public TournamentTests()
        {
            playerFaker = new Faker<Player>()
                .CustomInstantiator(f => new Player(f.Person.FullName));
            blindLevels =
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
        }

        [Fact]
        public void BuyIn_IncrementsBuyInCount_And_AddsToBuyInAmount()
        {
            // Arrange
            var player = new Player("Victor");
            var initialBuyInCount = player.BuyInCount;
            var initialBuyInAmount = player.BuyInAmount;
            var buyInAmount = 100m;

            // Act
            var result = player.BuyIn(buyInAmount);

            // Assert
            result.Should().BeTrue();
            player.BuyInCount.Should().Be(initialBuyInCount + 1);
            player.BuyInAmount.Should().Be(initialBuyInAmount + buyInAmount);
        }

        [Fact]
        public void BuyIn_ShouldRejectNegativeBuyInAmount()
        {
            // Arrange
            var player = new Player("Victor");
            var buyInAmount = -100m;

            // Act
            var result = player.BuyIn(buyInAmount);

            // Assert
            result.Should().BeFalse();
            player.BuyInCount.Should().Be(0);
            player.BuyInAmount.Should().Be(0m);
        }

        [Fact]
        public void RegisterPlayer_ShouldRegisterUpToMaxPlayers()
        {
            // Arrange
            var config = new TournamentConfig(BuyInAmount: 100m, MaxBuyIns: 2, MaxPlayers: 18, MaxTableSize: 6, blindLevels);
            var tournament = new Tournament(config);
            var players = playerFaker.Generate(18);

            // Act
            players.ForEach(player => tournament.RegisterPlayer(player));

            // Assert
            tournament.Players.Should().HaveCount(18);
            foreach (var table in tournament.Tables)
            {
                table.Players.Should().NotBeEmpty();
            }
        }

        [Fact]
        public void BlindLevel_ShouldIncrementCorrectly()
        {
            // Arrange
            var config = new TournamentConfig(BuyInAmount: 100m, MaxBuyIns: 2, MaxPlayers: 18, MaxTableSize: 6, blindLevels);
            var tournament = new Tournament(config);

            // Act
            tournament.IncrementBlindLevel();
            tournament.IncrementBlindLevel();
            tournament.IncrementBlindLevel();
            tournament.IncrementBlindLevel();

            // Assert
            tournament.CurrentBlindLevel.Level.Should().Be(5);
            tournament.CurrentBlindLevel.SmallBlind.Should().Be(250);
            tournament.CurrentBlindLevel.BigBlind.Should().Be(500);
        }

        [Fact]
        public void EliminatePlayer_ShouldEliminateCorrectPlayer()
        {
            // Arrange
            int maxPlayers = 18;
            int maxTableSize = 6;
            var config = new TournamentConfig(BuyInAmount: 100m, MaxBuyIns: 2, maxPlayers, maxTableSize, blindLevels);
            var tournament = new Tournament(config);
            var players = playerFaker.Generate(maxPlayers);

            foreach (var player in players)
            {
                tournament.RegisterPlayer(player);
            }

            var playerToEliminate = players[5];

            // Act
            var result = tournament.EliminatePlayer(playerToEliminate);

            // Arrage
            result.Should().BeTrue();
            tournament.Players.Should().NotContain(playerToEliminate);

            foreach (var table in tournament.Tables)
            {
                table.Players.Should().NotContain(playerToEliminate);
            }
        }

        [Fact]
        public void RedistributePlayers_ShouldEvenlyDistributePlayersAcrossTables()
        {
            // Arrange
            int maxPlayers = 18;
            int maxTableSize = 6;
            var config = new TournamentConfig(BuyInAmount: 100m, MaxBuyIns: 2, maxPlayers, maxTableSize, blindLevels);
            var tournament = new Tournament(config);
            var players = playerFaker.Generate(maxPlayers);

            // Act
            foreach (var player in players)
            {
                tournament.RegisterPlayer(player);
            }

            var table = tournament.Tables.First();
            var playersToRemove = new List<Player>(table.Players);

            for (var i = 0; i < 5; i++)
            {
                tournament.EliminatePlayer(playersToRemove[i]);
            }

            tournament.RedistributePlayers();

            // Assert
            tournament.Tables.Should().HaveCount(3);
            foreach (var t in tournament.Tables)
            {
                t.Players.Should().HaveCountGreaterThanOrEqualTo(2);
            }
        }

    }
}