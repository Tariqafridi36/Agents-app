
public enum UserRole
{
    Junior,
    MidLevel,
    Senior,
    TeamLead,
}

// public class ChatSession
// {
//     public string SessionID { get; set; }
//     public string Status { get; set; }
//     public int PollCount { get; set; }
//     public UserRole AssignedAgent { get; set; }
// }

public class ChatSession
{
    public int Id { get; set; }
    public string UserId { get; set; } // User identifier
    public string SessionID { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int PollCount { get; set; }
    public UserRole AssignedAgent { get; set; }
    public ChatSessionStatus Status { get; set; }

    public ChatSession()
    {
        // Default constructor
    }

    public ChatSession(int id, string userId)
    {
        Id = id;
        UserId = userId;
        StartTime = DateTime.UtcNow;
        Status = ChatSessionStatus.Active;
    }
}

public enum ChatSessionStatus
{
    Active,
    Inactive,
    Completed,
    Waiting
}
