public class ChatService
{
    private List<ChatTeam> teams;
    private ChatQueue fifoQueue;
    private ChatQueue overflowQueue;
    private int maxQueueLength;
    private int maxConcurrency;
    private Dictionary<UserRole, double> seniorityMultipliers;

    public ChatService()
    {
        // Initialize chat teams
        teams = new List<ChatTeam>
        {
            new ChatTeam { TeamName = "Team A", Agents = new List<UserRole> { UserRole.TeamLead, UserRole.MidLevel, UserRole.MidLevel, UserRole.Junior } },
            new ChatTeam { TeamName = "Team B", Agents = new List<UserRole> { UserRole.Senior, UserRole.MidLevel, UserRole.Junior, UserRole.Junior } },
            new ChatTeam { TeamName = "Team C", Agents = new List<UserRole> { UserRole.MidLevel, UserRole.MidLevel } },
        };

        // Initialize queues
        fifoQueue = new ChatQueue { Sessions = new Queue<ChatSession>() };
        overflowQueue = new ChatQueue { Sessions = new Queue<ChatSession>() };

        // Define parameters
        maxQueueLength = (int)(teams.Sum(team => team.Agents.Count) * 1.5);
        maxConcurrency = 10;

        seniorityMultipliers = new Dictionary<UserRole, double>
        {
            { UserRole.Junior, 0.4 },
            { UserRole.MidLevel, 0.6 },
            { UserRole.Senior, 0.8 },
            { UserRole.TeamLead, 0.5 },
        };
    }

    public void Run()
    {
        // Initialize your chat service and other components

        while (true)
        {
            // Continuously monitor chat sessions and handle other service tasks
            MonitorChatSessions();
            AssignChatsToAgents();

            // Sleep for a short interval before checking again (adjust the interval as needed)
            Thread.Sleep(1000);
        }
    }

    public string CreateChatSession()
    {
        // Check if the maximum queue length is reached
        if (fifoQueue.Sessions.Count + overflowQueue.Sessions.Count >= maxQueueLength)
        {
            if (IsDuringOfficeHours())
            {
                // If during office hours and the queue is full, add to overflow queue
                if (overflowQueue.Sessions.Count < maxQueueLength)
                {
                    var session = new ChatSession
                    {
                        SessionID = Guid.NewGuid().ToString(),
                        Status = ChatSessionStatus.Waiting,
                        PollCount = 0,
                        AssignedAgent = UserRole.Junior, // Overflow agents are considered Junior
                    };
                    overflowQueue.Sessions.Enqueue(session);
                    return "Chat session added to overflow queue during office hours.";
                }
                else
                {
                    return "Chat session refused due to maximum queue length being reached (office hours).";
                }
            }
            else
            {
                return "Chat session refused due to maximum queue length being reached (outside office hours).";
            }
        }

        // Check if the FIFO queue is full
        if (fifoQueue.Sessions.Count < maxQueueLength)
        {
            var session = new ChatSession
            {
                SessionID = Guid.NewGuid().ToString(),
                Status = ChatSessionStatus.Waiting,
                PollCount = 0,
                AssignedAgent = UserRole.Junior, // Initially assigned to a Junior agent
            };
            fifoQueue.Sessions.Enqueue(session);
            return "Chat session added to FIFO queue.";
        }
        else
        {
            if (IsDuringOfficeHours())
            {
                // If during office hours and the FIFO queue is full, add to overflow queue
                if (overflowQueue.Sessions.Count < maxQueueLength)
                {
                    var session = new ChatSession
                    {
                        SessionID = Guid.NewGuid().ToString(),
                        Status = ChatSessionStatus.Waiting,
                        PollCount = 0,
                        AssignedAgent = UserRole.Junior, // Overflow agents are considered Junior
                    };
                    overflowQueue.Sessions.Enqueue(session);
                    return "Chat session added to overflow queue during office hours.";
                }
                else
                {
                    return "Chat session refused due to maximum queue length being reached (office hours).";
                }
            }
            else
            {
                return "Chat session refused due to maximum queue length being reached (outside office hours).";
            }
        }
    }

    private UserRole DetermineNextAgentRole(Dictionary<UserRole, int> lastAssignedIndex, UserRole currentRole)
    {
        // Define the order of seniority (from lowest to highest)
        List<UserRole> seniorityOrder = new List<UserRole>
    {
        UserRole.Junior,
        UserRole.MidLevel,
        UserRole.Senior,
        UserRole.TeamLead
    };

        // Find the index of the current role in the seniority order
        int currentRoleIndex = seniorityOrder.IndexOf(currentRole);

        // Start searching for the next available agent with the next seniority level
        for (int i = 1; i < seniorityOrder.Count; i++)
        {
            // Calculate the next seniority level to consider
            int nextRoleIndex = (currentRoleIndex + i) % seniorityOrder.Count;
            UserRole nextRole = seniorityOrder[nextRoleIndex];

            // Check if there are available agents of the next seniority level
            if (lastAssignedIndex[nextRole] < teams.FindAll(t => t.Agents.Contains(nextRole)).Count - 1)
            {
                return nextRole;
            }
        }

        // If no agents of higher seniority are available, stay with the current role
        return currentRole;
    }

    private void AssignChatsToAgents()
    {
        // Initialize dictionaries to keep track of the last assigned index for each role
        Dictionary<UserRole, int> lastAssignedIndex = new Dictionary<UserRole, int>
    {
        { UserRole.Junior, -1 },
        { UserRole.MidLevel, -1 },
        { UserRole.Senior, -1 },
        { UserRole.TeamLead, -1 },
    };

        // Loop through the chat sessions in the FIFO queue
        foreach (ChatSession session in fifoQueue.Sessions.ToList())
        {
            // Determine the role of the next agent to be assigned based on round-robin and seniority
            UserRole nextRoleToAssign = DetermineNextAgentRole(lastAssignedIndex, session.AssignedAgent);

            if (nextRoleToAssign != UserRole.Junior)
            {
                // Find the next agent with the assigned role from the chat teams
                ChatTeam team = teams.Find(t => t.Agents.Contains(nextRoleToAssign));

                if (team != null)
                {
                    // Assign the chat session to the next agent with the specified role
                    session.AssignedAgent = nextRoleToAssign;
                    lastAssignedIndex[nextRoleToAssign] = (lastAssignedIndex[nextRoleToAssign] + 1) % team.Agents.Count;

                    // Remove the session from the FIFO queue and start the chat
                    fifoQueue.Sessions.Dequeue();
                    StartChat(session, nextRoleToAssign);
                }
            }
        }
    }


    private void MonitorChatSessions()
    {
        foreach (ChatSession session in fifoQueue.Sessions.ToList())
        {
            if (session.Status == ChatSessionStatus.Active)
            {
                // If the session is active, increment the poll count
                session.PollCount++;

                // Check if the session has exceeded the maximum allowed poll count
                if (session.PollCount >= 3)
                {
                    // Mark the session as inactive
                    session.Status = ChatSessionStatus.Inactive;
                }
            }
        }
    }


    private void StartChat(ChatSession session, UserRole assignedAgent)
    {
        // Simulate starting a chat with a user
        Console.WriteLine($"Starting chat session {session.Id} with {assignedAgent}...");

        // Simulate a chat conversation (you would need to replace this with your actual chat communication logic)
        for (int i = 1; i <= 5; i++)
        {
            // Simulate user message
            Console.WriteLine($"User: Hello, this is user message {i}");

            // Simulate agent response
            Console.WriteLine($"{assignedAgent}: Hi, this is agent {assignedAgent}. This is agent response {i}");
        }

        // Simulate ending the chat session
        Console.WriteLine($"Chat session {session.Id} with {assignedAgent} ended.");

        // You would typically implement the actual chat communication here with your chat system or messaging platform
    }


    private void PollChat(ChatSession session)
    {
        // Simulate a user polling the chat session
        Console.WriteLine($"User is polling chat session {session.Id}...");

        // Check if the chat session is still active
        if (session.Status == ChatSessionStatus.Active)
        {
            // Simulate sending a response from the agent
            Console.WriteLine($"Agent ({session.AssignedAgent}): We are here to help. Please wait...");

            // Reset the poll count since the user received a response
            session.PollCount = 0;
        }
        else
        {
            // The chat session is marked as inactive; notify the user
            Console.WriteLine("Chat session is inactive. You can start a new chat session if needed.");
        }

        // You would typically implement actual polling and response handling logic here based on your chat system or messaging platform
    }


    private bool IsDuringOfficeHours()
    {
        // Define office hours (e.g., 9 AM to 5 PM)
        TimeSpan officeHoursStart = new TimeSpan(9, 0, 0); // 9 AM
        TimeSpan officeHoursEnd = new TimeSpan(17, 0, 0);  // 5 PM

        // Get the current time
        TimeSpan currentTime = DateTime.Now.TimeOfDay;

        // Check if the current time is within office hours
        return currentTime >= officeHoursStart && currentTime <= officeHoursEnd;
    }
}