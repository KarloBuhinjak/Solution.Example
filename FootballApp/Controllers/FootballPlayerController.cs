using Microsoft.AspNetCore.Mvc;

namespace FootballApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FootballPlayerController : ControllerBase
    {
        private static readonly List<FootballPlayer> Players = new List<FootballPlayer>
        {
            new FootballPlayer { Id = 1, Name = "Messi", Age = 34, Position = "Forward", Club = "FC Barcelona" },
            new FootballPlayer { Id = 2, Name = "Ronaldo", Age = 36, Position = "Forward", Club = "Manchester United" }
        };
        
        [HttpPost]
        public ActionResult<FootballPlayer> Post([FromBody] FootballPlayer player)
        {
            player.Id = Players.Count + 1; 
            Players.Add(player);
            return Ok(player);

        }
        
        [HttpGet]
        public IActionResult GetAllPlayers()
        {
            if (Players == null || Players.Count == 0)
            {
                return NoContent();
            }

            return Ok(Players);
        }
        
        [HttpGet("{id}")]
        public IActionResult GetPlayerById(int id)
        {
            foreach (var player in Players)
            {
                if (player.Id == id)
                {
                    return Ok(player); 
                }
            }

            return NotFound();
        }
        
        [HttpPut("{id}")]
        public IActionResult UpdatePlayer(int id, [FromBody] FootballPlayer updatedPlayer)
        {
            
            foreach (var player in Players)
            {
                if (player.Id == id)
                {
                    if (updatedPlayer.Name != null)
                    {
                        player.Name = updatedPlayer.Name;
                    }
                    if (updatedPlayer.Age != 0) 
                    {
                        player.Age = updatedPlayer.Age;
                    }
                    if (updatedPlayer.Position != null)
                    {
                        player.Position = updatedPlayer.Position;
                    }
                    if (updatedPlayer.Club != null)
                    {
                        player.Club = updatedPlayer.Club;
                    }

                    return Ok(player);

                }
            }

            return NotFound();
        }
        
        [HttpDelete("{id}")]
        public IActionResult DeletePlayer(int id)
        {
            FootballPlayer playerToRemove = null;
            foreach (var player in Players)
            {
                if (player.Id == id)
                {
                    playerToRemove = player;
                    break;
                }
            }

            if (playerToRemove == null)
            {
                return NotFound(); 
            }

            Players.Remove(playerToRemove);
            return Ok(playerToRemove);
        }
        
        
        
    }
}