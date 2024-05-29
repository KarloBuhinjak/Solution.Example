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
        public ActionResult<FootballPlayer> Post(FootballPlayer player)
        {
            Console.WriteLine(player.Name);
            player.Id = Players.Count + 1; // Automatski dodijeli ID novom igraču
            Players.Add(player);
            return CreatedAtAction(nameof(Get), new { id = player.Id }, player);
        }
        
        [HttpGet]
        public IEnumerable<FootballPlayer> Get()
        {
            return Players;
        }
        
        
        
    }
}