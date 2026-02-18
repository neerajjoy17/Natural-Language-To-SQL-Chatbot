namespace Bot.Models
{
    public class Match
    {
        public int id { get; set; }
        public int season { get; set; }
        public int week { get; set; }
        public DateTime date { get; set; }
        public string home { get; set; }
        public int homegoals { get; set; }
        public int awaygoals { get; set; }
        public string away { get; set; }
        public string result { get; set; }

    }
}
