using System.Net;
using Microsoft.AspNetCore.Mvc;


namespace FootballApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FootballPlayerController : ControllerBase
    {
        private static readonly List<FootballPlayer> Players = new List<FootballPlayer>
        {
            
        };
        
        [HttpPost]
        public IActionResult Post([FromBody] FootballPlayer player)
        {
            if (player == null)
            {
                return BadRequest("Player is null");
            }

            if (Players == null || !Players.Any())
            {
                player.Id = 1;
            }
            else
            {
                player.Id = Players.Max(p => p.Id) + 1;
            }
            Players.Add(player);
            return Ok(new { message = "Player added successfully", player });
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
        public IActionResult GetPlayer(int id)
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
                        //Console.WriteLine(updatedPlayer.);
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