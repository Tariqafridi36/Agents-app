public class Agent
{
    public int Id { get; set; }
    public string Name { get; set; }
    public AgentSeniority Seniority { get; set; }
    public int ConcurrentChats { get; set; } // Maximum number of chats the agent can handle simultaneously

    public Agent()
    {
        // Default constructor
    }

    public Agent(int id, string name, AgentSeniority seniority, int concurrentChats)
    {
        Id = id;
        Name = name;
        Seniority = seniority;
        ConcurrentChats = concurrentChats;
    }
}

public enum AgentSeniority
{
    Junior,
    MidLevel,
    Senior,
    TeamLead
}
