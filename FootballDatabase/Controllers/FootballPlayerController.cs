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
        private static readonly List<Player> Players = new List<Player>
        {
            
        };
        

        
        
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding player: {ex.Message}");
            }
        }
        
        [HttpGet]
        public IActionResult GetAllPlayers()
        {
            var players = new List<Player>();
            using var connection = new NpgsqlConnection(connectionString);
            var commandText = "SELECT * FROM \"Player\";";
            
            using var command = new NpgsqlCommand(commandText, connection);
            
            connection.Open();

            using var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var player = new Player();
                    player.Id = Guid.Parse(reader[0].ToString());
                    player.Name = reader[1].ToString();
                    player.Age = Convert.ToInt32(reader[2]);
                    player.Position = reader[3].ToString();
                    player.ClubId = Guid.Parse(reader[4].ToString());
                    players.Add(player);
                }
                
            }
            if (players.Count == 0)
            {
                return NoContent();
            }

            return Ok(players);
            
            
        }
        
        [HttpGet("{id}")]
        public IActionResult GetPlayer(Guid id)
        {
            using var connection = new NpgsqlConnection(connectionString);
            var commandText = "SELECT * FROM \"Player\" WHERE \"Id\" = @Id;";
            using var command = new NpgsqlCommand(commandText, connection);
            command.Parameters.AddWithValue("@Id", id);
    
            connection.Open();

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var player = new Player();
                player.Id = Guid.Parse(reader[0].ToString());
                player.Name = reader[1].ToString();
                player.Age = Convert.ToInt32(reader[2]);
                player.Position = reader[3].ToString();
                player.ClubId = Guid.Parse(reader[4].ToString());
                return Ok(player);
            }
            else
            {
                return NotFound();
            }
        }
        
        [HttpPut("{id}")]
        public IActionResult UpdatePlayer(Guid id, [FromBody] Player updatedPlayer)
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
        
        [HttpDelete("{id}")]
        public IActionResult DeletePlayer(Guid id)
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
        
        
        
    //     if (Players == null || Players.Count == 0)
    //     {
    //         return NoContent();
    //     }
    //
    // return Ok(Players);
        
        // [HttpGet("{id}")]
        // public IActionResult GetPlayer(int id)
        // {
        //     foreach (var player in Players)
        //     {
        //         if (player.Id == id)
        //         {
        //             return Ok(player); 
        //         }
        //     }
        //
        //     return NotFound();
        // }
        
        // [HttpPut("{id}")]
        // public IActionResult UpdatePlayer(int id, [FromBody] FootballPlayer updatedPlayer)
        // {
        //     
        //     foreach (var player in Players)
        //     {
        //         if (player.Id == id)
        //         {
        //             if (updatedPlayer.Name != null)
        //             {
        //                 player.Name = updatedPlayer.Name;
        //             }
        //             if (updatedPlayer.Age != 0) 
        //             {
        //                 player.Age = updatedPlayer.Age;
        //             }
        //             if (updatedPlayer.Position != null)
        //             {
        //                 player.Position = updatedPlayer.Position;
        //             }
        //             if (updatedPlayer.Club != null)
        //             {
        //                 //Console.WriteLine(updatedPlayer.);
        //                 player.Club = updatedPlayer.Club;
        //             }
        //
        //             return Ok(player);
        //
        //         }
        //     }
        //
        //     return NotFound();
        // }
        
        // [HttpDelete("{id}")]
        // public IActionResult DeletePlayer(int id)
        // {
        //     FootballPlayer playerToRemove = null;
        //     foreach (var player in Players)
        //     {
        //         if (player.Id == id)
        //         {
        //             playerToRemove = player;
        //             break;
        //         }
        //     }
        //
        //     if (playerToRemove == null)
        //     {
        //         return NotFound(); 
        //     }
        //
        //     Players.Remove(playerToRemove);
        //     return Ok(playerToRemove);
        // }
        
        
        
    }
}