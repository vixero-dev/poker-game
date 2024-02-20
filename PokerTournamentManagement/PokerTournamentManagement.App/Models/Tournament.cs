using PokerTournamentManagement.App.Models;

namespace PokerTournamentManagement.App
{
    internal class Tournament
    {
        public decimal TotalPrizePool { get; private set; } = 0m;
        public List<Player> Players { get; } = [];
        public List<Table> Tables { get; } = [];
        public BlindLevel CurrentBlindLevel { get; private set; }

        private readonly TournamentConfig _config;

        public Tournament(TournamentConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            CurrentBlindLevel = _config.BlindLevels.First();

            CreateTables();
        }

        public bool RegisterPlayer(Player player)
        {
            if (Players.Count >= _config.MaxPlayers || Players.Contains(player)) return false;

            var random = new Random();
            var availableTables = Tables.Where(table => table.Players.Count < table.MaxPlayers).ToList();

            var table = availableTables[random.Next(availableTables.Count)];

            Players.Add(player);
            TotalPrizePool += player.BuyInAmount;
            table.Players.Add(player);

            return true;
        }

        public bool IncrementBlindLevel()
        {
            var currentIndex = _config.BlindLevels.ToList().FindIndex(bl => bl.Level == CurrentBlindLevel.Level);
            if (currentIndex + 1 < _config.BlindLevels.Count())
            {
                CurrentBlindLevel = _config.BlindLevels.ElementAt(currentIndex + 1);
            }

            return true;
        }

        public bool EliminatePlayer(Player player)
        {
            if (!Players.Contains(player)) return false;

            Players.Remove(player);

            foreach (var table in Tables)
            {
                if (table.Players.Contains(player))
                {
                    table.Players.Remove(player);
                    break;
                }
            }

            return true;
        }

        public bool RedistributePlayers()
        {
            int totalPlayers = Players.Count;
            int numberOfTables = Tables.Count;
            int idealTableSize = totalPlayers / numberOfTables;

            var tablesWithSpace = Tables.Where(table => table.Players.Count < idealTableSize).ToList();

            foreach (var table in Tables.Where(table => table.Players.Count > idealTableSize))
            {
                while (table.Players.Count > idealTableSize && tablesWithSpace.Count != 0)
                {
                    var playerToMove = table.Players.Last();
                    table.Players.Remove(playerToMove);
                    var targetTable = tablesWithSpace.First();
                    targetTable.Players.Add(playerToMove);

                    if (targetTable.Players.Count >= idealTableSize)
                    {
                        tablesWithSpace.Remove(targetTable);
                    }
                }
            }

            return true;
        }


        private void CreateTables()
        {
            int numberOfTables = (int)Math.Ceiling((double)_config.MaxPlayers / _config.MaxTableSize);
            for (int i = 0; i < numberOfTables; i++)
            {
                Tables.Add(new Table(_config.MaxTableSize));
            }
        }
    }
}
