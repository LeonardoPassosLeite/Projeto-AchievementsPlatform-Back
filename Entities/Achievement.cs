public class Achievement
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int PlayerId { get; set; }
    public Player Player { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; }
}
