namespace TournamentWebService.Core.Publisher
{
    public class StartChatMessage
    {
        public string TYPE { get; set; } = "START_CHAT";
        public StartChatData DATA { get; set; }
    }

    public class StartChatData
    {
        public string room { get; set; }
        public string type { get; set; }
        public StartChatData( string room, string type)
        {
            this.room = room;
            this.type = type;
        }
    }

    public class EndChatMessage
    {
        public string TYPE { get; set; } = "SAVE_MESSAGES";
        public EndChatData DATA { get; set; }
    }

    public class EndChatData
    {
        public string room { get; set; }
        public EndChatData(string room)
        {
            this.room = room;
        }
    }
}
