namespace FootballDatabase
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Position { get; set; }
        public Guid ClubId { get; set; }
        
        
    }
}