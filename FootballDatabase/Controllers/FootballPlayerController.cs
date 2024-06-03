using System.Net;
using Microsoft.AspNetCore.Mvc;
using Npgsql;


namespace FootballDatabase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FootballPlayerController : ControllerBase
    {
        private readonly string connectionString = "Host=localhost;Port=5432;Database=Football;Username=postgres;Password=lozinka;";
        private static readonly List<Player> Players = new List<Player> { };
        
        [HttpPost]
        public IActionResult AddPlayer([FromBody] Player player)
        {
            if (player == null)
            {
                return BadRequest("Player data is null.");
            }

            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var commandText = "INSERT INTO \"Player\" (\"Id\", \"Name\", \"Age\", \"Position\", \"ClubId\") " +
                                  "VALUES (@Id, @Name, @Age, @Position, @ClubId);";

                using var command = new NpgsqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@Id", Guid.NewGuid()); // Generiranje novog ID-a za igrača
                command.Parameters.AddWithValue("@Name", player.Name);
                command.Parameters.AddWithValue("@Age", player.Age);
                command.Parameters.AddWithValue("@Position", player.Position);
                command.Parameters.AddWithValue("@ClubId", player.ClubId );

                command.ExecuteNonQuery();

                return Ok("Player added successfully.");
            }
            catch (NpgsqlException npgEx)
            {
                return StatusCode(500, $"Database error: {npgEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [HttpGet]
        public IActionResult GetAllPlayers()
        {
            var players = new List<Player>();
    
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                var commandText = "SELECT * FROM \"Player\";";
        
                using var command = new NpgsqlCommand(commandText, connection);
        
                connection.Open();
                using var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var player = new Player
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            Position = reader.GetString(reader.GetOrdinal("Position")),
                            ClubId = reader.GetGuid(reader.GetOrdinal("ClubId"))
                        };
                        players.Add(player);
                    }
                }
                if (players.Count == 0)
                {
                    return NoContent();
                }

                return Ok(players);
            }
            catch (NpgsqlException npgEx)
            {
                return StatusCode(500, $"Database error: {npgEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [HttpGet("clubs")]
        public IActionResult GetAllClubsWithPlayers()
        {
            try
            {
                var clubs = new List<Club>();

                using var connection = new NpgsqlConnection(connectionString);
                connection.Open();

                var commandText = @"
                    SELECT c.""Id"" AS ClubId, c.""Name"" AS ClubName, c.""Country"", 
                           p.""Id"" AS PlayerId, p.""Name"" AS PlayerName, p.""Age"", p.""Position"", p.""ClubId""
                    FROM ""Club"" c
                    LEFT JOIN ""Player"" p ON c.""Id"" = p.""ClubId"";";

                using var command = new NpgsqlCommand(commandText, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var clubId = reader.GetGuid(reader.GetOrdinal("ClubId"));
                    var club = clubs.Find(c => c.Id == clubId);

                    if (club == null)
                    {
                        club = new Club
                        {
                            Id = clubId,
                            Name = reader.GetString(reader.GetOrdinal("ClubName")),
                            Country = reader.GetString(reader.GetOrdinal("Country")),
                            Players = new List<Player>()
                        };
                        clubs.Add(club);
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("PlayerId")))
                    {
                        var player = new Player
                        {
                            Id = reader.GetGuid(reader.GetOrdinal("PlayerId")),
                            Name = reader.GetString(reader.GetOrdinal("PlayerName")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            Position = reader.GetString(reader.GetOrdinal("Position")),
                            ClubId = clubId
                        };
                        club.Players.Add(player);
                    }
                }

                return Ok(clubs);
            }
            catch (NpgsqlException npgEx)
            {
                return StatusCode(500, $"Database error: {npgEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        
        [HttpGet("{id}")]
        public IActionResult GetPlayer(Guid id)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                var commandText = "SELECT * FROM \"Player\" WHERE \"Id\" = @Id;";
                using var command = new NpgsqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var player = new Player
                    {
                        Id = Guid.Parse(reader[0].ToString()),
                        Name = reader[1].ToString(),
                        Age = Convert.ToInt32(reader[2]),
                        Position = reader[3].ToString(),
                        ClubId = Guid.Parse(reader[4].ToString())
                    };
                    return Ok(player);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (NpgsqlException npgEx)
            {
                return StatusCode(500, $"Database error: {npgEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [HttpPut("{id}")]
        public IActionResult UpdatePlayer(Guid id, [FromBody] Player updatedPlayer)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                var commandText = "UPDATE \"Player\" SET \"Name\" = @Name, \"Age\" = @Age, \"Position\" = @Position, \"ClubId\" = @ClubId WHERE \"Id\" = @Id;";
                using var command = new NpgsqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", updatedPlayer.Name);
                command.Parameters.AddWithValue("@Age", updatedPlayer.Age);
                command.Parameters.AddWithValue("@Position", updatedPlayer.Position);
                command.Parameters.AddWithValue("@ClubId", updatedPlayer.ClubId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok("Player updated successfully");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (NpgsqlException npgEx)
            {
               
                return StatusCode(500, $"Database error: {npgEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeletePlayer(Guid id)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);
                var commandText = "DELETE FROM \"Player\" WHERE \"Id\" = @Id;";
                using var command = new NpgsqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok("Player deleted successfully");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (NpgsqlException npgEx)
            {
                return StatusCode(500, $"Database error: {npgEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}