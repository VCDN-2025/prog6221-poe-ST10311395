/* 
    CyberGuardian ChatBot - Full Version
    Author: Katelyn Narain - ST10311395
    Date: 26 May 2025
    
    References:
    - C# Dictionary and List usage: Microsoft Docs - https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic
    - Figgle ASCII Art Library: https://www.nuget.org/packages/Figgle/
    - System.Media.SoundPlayer documentation: https://learn.microsoft.com/en-us/dotnet/api/system.media.soundplayer
    - Chatbot conversation flow inspired by best practices from natural language processing guidance
    - Error handling patterns based on .NET best practices
    - Typing effect implementation and UI formatting influenced by community discussions
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Threading;
using Figgle;

// Class representing a cybersecurity task
class CyberTask
{
    public string Title { get; set; }              // Task title
    public string Description { get; set; }        // Task description/details
    public DateTime? ReminderDate { get; set; }    // Optional reminder date

    // Custom string representation of the task, including reminder info if present
    public override string ToString()
    {
        string reminderInfo = ReminderDate.HasValue ? $" (Reminder: {ReminderDate.Value.ToShortDateString()})" : "";
        return $"- {Title}: {Description}{reminderInfo}";
    }
}

class Program
{
    static List<CyberTask> TaskList = new List<CyberTask>();  // List to store all user tasks
    static Random rand = new Random();

    // Activity log stores recent actions with timestamps and descriptions
    static List<string> ActivityLog = new List<string>();

    // Various dictionaries for chatbot responses, keywords, topics, and user memory
    static Dictionary<string, string> responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "how are you", "I'm a bot, but I'm fully operational and ready to help you stay safe online!" },
        { "what's your purpose", "My mission is to educate and empower you with cybersecurity knowledge." },
        { "what can i ask you about", "You can ask me about password safety, phishing, safe browsing, and general cybersecurity tips." },
        { "safe browsing", "Avoid clicking unknown links, use HTTPS websites, and keep your browser up to date." }
    };

    static Dictionary<string, string> keywordResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "password", "Make sure to use strong, unique passwords for each account. Consider using a password manager." },
        { "scam", "Watch out for online scams. If something sounds too good to be true, it probably is." },
        { "privacy", "Protect your privacy by limiting what you share online and reviewing app permissions regularly." }
    };

    static Dictionary<string, List<string>> topicResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
    {
        {
            "phishing", new List<string>
            {
                "Watch out for emails with urgent requests or attachments.",
                "Always verify the sender's email address before clicking links.",
                "Don’t enter credentials on suspicious login pages.",
                "Phishing often mimics trusted brands—double-check URLs.",
                "Enable 2FA to protect accounts even if your password is phished."
            }
        },
        {
            "password safety", new List<string>
            {
                "Use a mix of letters, numbers, and symbols in your passwords.",
                "Avoid using the same password across multiple sites.",
                "Consider using a trusted password manager.",
                "Change your passwords regularly, especially after a breach."
            }
        }
    };

    // Stores user preferences or context to personalize responses
    static Dictionary<string, string> userMemory = new Dictionary<string, string>();

    // Sentiment-based responses to detect and respond to user feelings
    static Dictionary<string, string> sentimentResponses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "worried", "It's completely understandable to feel that way. Remember, staying informed helps you stay safe!" },
        { "frustrated", "I know cybersecurity can be challenging. I'm here to help you step-by-step." },
        { "curious", "That's great! Curiosity is the first step toward becoming cybersecurity savvy." }
    };

    static void Main()
    {
        try
        {
            DrawBorder("CyberGuardian");   // Draw ASCII title border

            // Play welcome audio (wav file)
            PlayAudio(@"C:\\Users\\Katelyn Narain\\source\\repos\\ChatBot\\Music\\ElevenLabs_2025-04-23T15_55_15_Rachel_pre_sp100_s50_sb75_se0_b_m2.wav");

            // Ask for user name and greet
            Console.Write("Please enter your name: ");
            string userName = Console.ReadLine();

            ShowWelcomeMessage(userName);

            // Start main chat loop
            StartChat(userName);

            // Ending message after user exits
            Console.ForegroundColor = ConsoleColor.Green;
            Divider("Session Ended");
            TypeText($"\nThank you for using CyberGuardian, {userName}!");
            TypeText("Stay safe, stay smart. Press Enter to exit.");
            Console.ResetColor();
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            // Catch any unexpected errors globally
            DisplayError("An unexpected error occurred: " + ex.Message);
        }
    }

    // Displays a stylized ASCII art banner using Figgle
    static void DrawBorder(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Magenta;
        string asciiArt = FiggleFonts.Slant.Render(title);
        int width = asciiArt.Split('\n')[0].Length;

        Console.WriteLine(new string('═', width));
        Console.WriteLine(asciiArt);
        Console.WriteLine(new string('═', width));
        Console.WriteLine("      [:: Your AI Guide to Cybersecurity ::]");
        Console.WriteLine(new string('═', width));
        Console.ResetColor();
    }

    // Plays a WAV audio file synchronously
    static void PlayAudio(string soundFilePath)
    {
        try
        {
            if (!File.Exists(soundFilePath))
            {
                DisplayError("Audio file not found: " + soundFilePath);
                return;
            }

            using (SoundPlayer player = new SoundPlayer(soundFilePath))
            {
                player.Load();
                player.PlaySync();
            }
        }
        catch (Exception ex)
        {
            DisplayError("Audio could not be played: " + ex.Message);
        }
    }

    // Shows a personalized welcome message with padding and border
    static void ShowWelcomeMessage(string userName)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        string[] lines =
        {
            $"Welcome, {userName}!",
            "I'm CyberGuardian, your cybersecurity companion.",
            "Ask me anything about staying safe online."
        };

        int maxLen = 0;
        foreach (string line in lines)
            if (line.Length > maxLen)
                maxLen = line.Length;

        string border = new string('*', maxLen + 6);
        Console.WriteLine("\n" + border);

        foreach (string line in lines)
        {
            Console.Write("*  ");
            TypeText(line.PadRight(maxLen), 15);  // Print text with typing effect
            Console.WriteLine("  *");
        }

        Console.WriteLine(border);
        Console.ResetColor();
    }

    // Main interactive chat loop
    static void StartChat(string userName)
    {
        Divider("Chat Help");
        Console.ForegroundColor = ConsoleColor.DarkCyan;

        // Show user commands and tips
        Console.WriteLine("You can ask about:");
        Console.WriteLine(" - Password safety");
        Console.WriteLine(" - Scams");
        Console.WriteLine(" - Privacy");
        Console.WriteLine(" - Phishing");
        Console.WriteLine(" - Safe browsing");
        Console.WriteLine(" - My purpose");
        Console.WriteLine(" - What can I help with");
        Console.WriteLine("Type 'exit' to leave the chat.");
        Console.WriteLine("Type 'start quiz' to begin the cybersecurity quiz.");
        Console.WriteLine("Type 'show activity log' or 'what have you done for me?' to view recent actions.");
        Console.WriteLine("------------------------------\n");
        Console.ResetColor();

        string currentTopic = null; // Tracks ongoing conversation topic

        while (true)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{userName}: ");
            string input = Console.ReadLine();

            if (input == null)
            {
                TypeText("CyberGuardian: It seems like something went wrong with the input. Let's continue whenever you're ready.");
                continue;
            }

            input = input.Trim();

            // Exit command
            if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                break;

            if (string.IsNullOrWhiteSpace(input))
            {
                DisplayError("CyberGuardian: I didn't quite catch that. Could you please say something?");
                continue;
            }

            string inputLower = input.ToLower();

            // Check if user wants to see the activity log
            if (inputLower == "show activity log" || inputLower == "what have you done for me?" || inputLower == "show my actions" || inputLower == "task summary")
            {
                ShowActivityLog();
                continue;
            }

            // NLP-enhanced recognition of user commands and intents

            // Reminders via natural language
            if (inputLower.Contains("remind me to") || inputLower.Contains("add a reminder to"))
            {
                HandleAddReminder(input);
                LogAction($"Added reminder via NLP command: '{input}'");
                continue;
            }

            // Adding tasks via flexible phrasing
            if (inputLower.StartsWith("add task") || inputLower.StartsWith("add a task") || inputLower.Contains("create task"))
            {
                string taskTitle = ExtractTaskTitle(inputLower);
                if (!string.IsNullOrWhiteSpace(taskTitle))
                {
                    AddTaskFlow("add task - " + taskTitle);
                    LogAction($"Added task via NLP command: '{taskTitle}'");
                }
                else
                {
                    DisplayError("CyberGuardian: Please specify the task title after 'add task'. For example, 'Add task - update password'.");
                }
                continue;
            }

            // Start the quiz commands
            if (inputLower.Contains("start quiz") || inputLower.Contains("cyber quiz") || inputLower.Contains("quiz game") || inputLower.Contains("quiz"))
            {
                LogAction("Quiz started.");
                StartCyberQuiz();
                LogAction("Quiz completed.");
                continue;
            }

            // Show tasks command
            if (inputLower.Contains("show tasks") || inputLower.Contains("list tasks") || inputLower.Contains("my tasks"))
            {
                ShowTasks();
                LogAction("Displayed task list.");
                continue;
            }

            // Provide phishing tips if user mentions phishing
            if (inputLower.Contains("phishing"))
            {
                currentTopic = "phishing";
                string tip = topicResponses[currentTopic][rand.Next(topicResponses[currentTopic].Count)];
                TypeText($"CyberGuardian: Here's a tip on phishing: {tip}");
                LogAction("Provided phishing tip.");
                continue;
            }

            // More detailed natural conversation handling starts here
            try
            {
                // User states interest in a topic
                if (inputLower.StartsWith("i'm interested in ") || inputLower.StartsWith("i am interested in "))
                {
                    string interest = input.Substring(input.IndexOf("in ") + 3).Trim();
                    userMemory["interest"] = interest;
                    TypeText($"CyberGuardian: Great! I'll remember that you're interested in {interest}.");
                    currentTopic = interest.ToLower();
                    LogAction($"User interest noted: {interest}");
                    continue;
                }

                // Sentiment detection responses
                bool sentimentHandled = false;
                foreach (var sentiment in sentimentResponses)
                {
                    if (inputLower.Contains(sentiment.Key))
                    {
                        TypeText($"CyberGuardian: {sentiment.Value}");
                        LogAction($"Handled sentiment expression: '{sentiment.Key}'");
                        sentimentHandled = true;
                        break;
                    }
                }
                if (sentimentHandled) continue;

                Console.ForegroundColor = ConsoleColor.Green;

                // Personalized tip if user asks for a tip on their interest
                if (userMemory.ContainsKey("interest") && inputLower.Contains("tip"))
                {
                    string interest = userMemory["interest"];
                    TypeText($"CyberGuardian: As someone interested in {interest}, here's a tip:");

                    if (topicResponses.ContainsKey(interest.ToLower()))
                    {
                        string personalizedTip = topicResponses[interest.ToLower()][rand.Next(topicResponses[interest.ToLower()].Count)];
                        TypeText($"CyberGuardian: {personalizedTip}");
                        LogAction($"Provided personalized tip for interest: {interest}");
                    }
                    else if (keywordResponses.ContainsKey(interest.ToLower()))
                    {
                        TypeText($"CyberGuardian: {keywordResponses[interest.ToLower()]}");
                        LogAction($"Provided keyword tip for interest: {interest}");
                    }
                    else
                    {
                        TypeText("CyberGuardian: I don't have specific tips on that yet, but I’ll remember it for the future!");
                        LogAction($"No tip available for interest: {interest}");
                    }
                    currentTopic = interest.ToLower();
                    continue;
                }

                // User asks for more explanation on current topic
                if (currentTopic != null && (
                    inputLower.Contains("more") ||
                    inputLower.Contains("explain") ||
                    inputLower.Contains("i don't understand") ||
                    inputLower.Contains("what do you mean") ||
                    inputLower.Contains("huh") ||
                    inputLower.Contains("i'm confused")))
                {
                    if (topicResponses.ContainsKey(currentTopic))
                    {
                        string nextTip = topicResponses[currentTopic][rand.Next(topicResponses[currentTopic].Count)];
                        TypeText($"CyberGuardian: Here's more on {currentTopic}: {nextTip}");
                        LogAction($"Provided follow-up explanation for topic: {currentTopic}");
                        continue;
                    }
                }

                // Static known responses
                if (responses.TryGetValue(inputLower, out string staticResponse))
                {
                    TypeText($"CyberGuardian: {staticResponse}");
                    LogAction($"Responded with static response for: '{inputLower}'");
                    currentTopic = null;
                }
                // Varied topic responses with random selection
                else if (topicResponses.TryGetValue(inputLower, out List<string> variedList))
                {
                    string randomResponse = variedList[rand.Next(variedList.Count)];

                    if (userMemory.ContainsKey("interest") &&
                        string.Equals(userMemory["interest"], input, StringComparison.OrdinalIgnoreCase))
                    {
                        TypeText($"CyberGuardian: As someone interested in {input}, here's a tip for you:");
                    }

                    TypeText($"CyberGuardian: {randomResponse}");
                    LogAction($"Provided topic response for: '{inputLower}'");
                    currentTopic = inputLower;
                }
                else
                {
                    // Check for keyword matches inside user input
                    bool keywordFound = false;
                    foreach (var pair in keywordResponses)
                    {
                        if (inputLower.Contains(pair.Key))
                        {
                            TypeText($"CyberGuardian: {pair.Value}");
                            LogAction($"Provided keyword response for: '{pair.Key}'");
                            currentTopic = pair.Key;
                            keywordFound = true;
                            break;
                        }
                    }

                    // No recognized command or keyword found
                    if (!keywordFound)
                    {
                        TypeText("CyberGuardian: I'm not sure I understand. Can you try rephrasing?");
                        LogAction($"Unrecognized input: '{inputLower}'");
                        currentTopic = null;
                    }
                }

                Console.ResetColor();
            }
            catch (Exception)
            {
                // Graceful error handling during chat processing
                DisplayError("CyberGuardian encountered an error while processing your input. Please try again.");
                LogAction("Error processing user input.");
                continue;
            }
        }
    }

    /// <summary>
    /// Adds an entry to the activity log with current timestamp and limits to last 10 entries.
    /// </summary>
    /// <param name="actionDescription">Short description of the action</param>
    static void LogAction(string actionDescription)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        ActivityLog.Add($"[{timestamp}] {actionDescription}");

        // Keep only the last 10 log entries for clarity
        if (ActivityLog.Count > 10)
            ActivityLog.RemoveAt(0);
    }

    /// <summary>
    /// Displays the last 10 entries of the activity log to the user
    /// </summary>
    static void ShowActivityLog()
    {
        Divider("Activity Log (Last 10 Actions)");
        if (ActivityLog.Count == 0)
        {
            TypeText("CyberGuardian: No actions recorded yet.");
            return;
        }

        // Print each logged action with numbering
        int count = ActivityLog.Count;
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"{i + 1}. {ActivityLog[i]}");
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Attempts to extract a task title from flexible user input for adding tasks
    /// </summary>
    /// <param name="inputLower">Lowercase user input string</param>
    /// <returns>Extracted task title or null if none found</returns>
    static string ExtractTaskTitle(string inputLower)
    {
        string[] prefixes = { "add task", "add a task", "create task" };
        foreach (var prefix in prefixes)
        {
            int idx = inputLower.IndexOf(prefix);
            if (idx >= 0)
            {
                string after = inputLower.Substring(idx + prefix.Length).Trim();
                if (after.StartsWith("-"))
                    after = after.Substring(1).Trim();
                return after;
            }
        }
        return null;
    }

    /// <summary>
    /// Handles natural language reminders, extracts reminder time and task, and adds to task list
    /// </summary>
    /// <param name="input">User input string</param>
    static void HandleAddReminder(string input)
    {
        string lower = input.ToLower();
        int remindIdx = lower.IndexOf("remind me to");
        if (remindIdx < 0)
            remindIdx = lower.IndexOf("add a reminder to");

        if (remindIdx < 0)
        {
            TypeText("CyberGuardian: Sorry, I couldn't find a reminder phrase.");
            return;
        }

        string reminderPart = input.Substring(remindIdx + (lower.Contains("remind me to") ? 12 : 16)).Trim();

        DateTime? reminderDate = null;
        string taskTitle = reminderPart;

        // Simple parsing: handle "tomorrow"
        if (reminderPart.EndsWith(" tomorrow", StringComparison.OrdinalIgnoreCase))
        {
            reminderDate = DateTime.Now.AddDays(1);
            taskTitle = reminderPart.Substring(0, reminderPart.Length - " tomorrow".Length).Trim();
        }
        // Handle phrases like "in 3 days"
        else if (reminderPart.StartsWith("in "))
        {
            string[] parts = reminderPart.Substring(3).Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2 && int.TryParse(parts[0], out int amount))
            {
                string unit = parts[1].ToLower();
                reminderDate = unit switch
                {
                    "day" or "days" => DateTime.Now.AddDays(amount),
                    "week" or "weeks" => DateTime.Now.AddDays(amount * 7),
                    _ => null
                };
                taskTitle = null; // No clear task title found in this simple case
            }
        }

        if (taskTitle != null && !string.IsNullOrWhiteSpace(taskTitle))
        {
            AddTaskFlow("add task - " + taskTitle);
        }
        if (reminderDate.HasValue)
        {
            LogAction($"Reminder set via NLP command for '{taskTitle ?? "unknown task"}' on {reminderDate.Value.ToShortDateString()}");
            TypeText($"CyberGuardian: Got it! I’ll remind you on {reminderDate.Value.ToShortDateString()}.");
        }
        else
        {
            TypeText("CyberGuardian: Reminder noted without specific time.");
        }
    }

    // The flow to add a task, now enhanced to log actions and reminder setting
    static void AddTaskFlow(string input)
    {
        // Validate input format
        if (string.IsNullOrWhiteSpace(input) || !input.ToLower().StartsWith("add task -"))
        {
            DisplayError("CyberGuardian: Please follow the format 'Add task - [Title]'. Try again.");
            return;
        }

        string title = input.Substring("add task -".Length).Trim();

        // Provide default or keyword-based descriptions
        string defaultDescription = "Review your account privacy settings to ensure your data is protected.";

        string description = title.ToLower() switch
        {
            string t when t.Contains("privacy") => defaultDescription,
            string t when t.Contains("password") => "Update your passwords and ensure they are strong and unique.",
            string t when t.Contains("2fa") || t.Contains("two-factor") => "Set up two-factor authentication for added security.",
            string t when t.Contains("phishing") => "Learn how to identify and report phishing emails.",
            _ => $"Task: {title} - Please stay cyber-aware."
        };

        Console.ForegroundColor = ConsoleColor.Yellow;
        TypeText($"CyberGuardian: Task added with the description: \"{description}\".");
        Console.ResetColor();

        Console.Write("CyberGuardian: Would you like a reminder? (e.g., Remind me in 3 days): ");
        string reminderInput = Console.ReadLine()?.Trim();

        DateTime? reminderDate = null;

        if (!string.IsNullOrWhiteSpace(reminderInput) && reminderInput.ToLower().Contains("remind me"))
        {
            // Parse reminder time, e.g. "remind me in 3 days"
            string reminderText = reminderInput.ToLower().Replace("remind me in", "").Trim();
            string[] parts = reminderText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2 && int.TryParse(parts[0], out int amount))
            {
                string unit = parts[1];
                reminderDate = unit switch
                {
                    "day" or "days" => DateTime.Now.AddDays(amount),
                    "week" or "weeks" => DateTime.Now.AddDays(amount * 7),
                    _ => null
                };
            }

            if (reminderDate.HasValue)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                TypeText($"CyberGuardian: Got it! I’ll remind you in {reminderDate.Value.Subtract(DateTime.Now).Days} days.");
                Console.ResetColor();

                // Log reminder setting
                LogAction($"Reminder set for task '{title}' on {reminderDate.Value.ToShortDateString()}");
            }
            else
            {
                DisplayError("CyberGuardian: Sorry, I couldn't understand the reminder format.");
            }
        }
        else
        {
            TypeText("CyberGuardian: No reminder set.");
        }

        // Add task to task list
        TaskList.Add(new CyberTask
        {
            Title = title,
            Description = description,
            ReminderDate = reminderDate
        });

        // Log that task was added
        LogAction($"Task added: '{title}'");
    }

    // Show all current tasks with color formatting
    static void ShowTasks()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Divider("Your Cybersecurity Tasks");

        if (TaskList.Count == 0)
        {
            TypeText("CyberGuardian: You have no tasks at the moment.");
        }
        else
        {
            foreach (var task in TaskList)
            {
                Console.WriteLine(task);
            }
        }

        Console.ResetColor();
    }

    // Represents a quiz question with options, correct answer, and explanation
    class QuizQuestion
    {
        public string Question { get; set; }
        public string[] Options { get; set; } // Possible answers labeled A, B, C, D etc.
        public char CorrectOption { get; set; } // Correct answer letter
        public string Explanation { get; set; } // Explanation shown after answer
    }

    // Starts the interactive cybersecurity quiz
    static void StartCyberQuiz()
    {
        var questions = new List<QuizQuestion>
        {
            new QuizQuestion
            {
                Question = "What should you do if you receive an email asking for your password?",
                Options = new[] { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                CorrectOption = 'C',
                Explanation = "Reporting phishing emails helps prevent scams and protects others."
            },
            new QuizQuestion
            {
                Question = "True or False: Using the same password for every account is safe.",
                Options = new[] { "True", "False" },
                CorrectOption = 'B',
                Explanation = "False! Reusing passwords increases risk if one account is breached."
            },
            new QuizQuestion
            {
                Question = "Which is a strong password example?",
                Options = new[] { "123456", "Password!", "Winter2025$", "qwerty" },
                CorrectOption = 'C',
                Explanation = "\"Winter2025$\" includes uppercase, numbers, and a special character — good job!"
            },
            new QuizQuestion
            {
                Question = "What does 2FA stand for?",
                Options = new[] { "Two-Factor Authentication", "Two-Firewall Access", "Two-Factor Access", "Twice-Filtered Authentication" },
                CorrectOption = 'A',
                Explanation = "2FA adds an extra layer of security — always enable it when possible."
            },
            new QuizQuestion
            {
                Question = "Which site is safer to enter personal information?",
                Options = new[] { "http://example.com", "https://example.com" },
                CorrectOption = 'B',
                Explanation = "HTTPS encrypts your data, making it safer from interception."
            }
        };

        int score = 0;

        Divider("Cybersecurity Quiz Time!");
        TypeText("CyberGuardian: Let's begin! Answer each question by typing A, B, C, or D (or T/F).");

        for (int i = 0; i < questions.Count; i++)
        {
            var q = questions[i];
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nQuestion {i + 1}: {q.Question}");

            for (int optIndex = 0; optIndex < q.Options.Length; optIndex++)
            {
                char label = (char)('A' + optIndex);
                Console.WriteLine($"{label}) {q.Options[optIndex]}");
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Your answer: ");
            string answer = Console.ReadLine()?.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(answer))
            {
                TypeText("CyberGuardian: You didn't enter an answer. Let's skip to the next one.");
                continue;
            }

            char answerChar = answer[0];
            if (answerChar == q.CorrectOption)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                TypeText("CyberGuardian: Correct! " + q.Explanation);
                score++;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                TypeText($"CyberGuardian: Oops! That’s not right. {q.Explanation}");
            }

            Console.ResetColor();
        }

        Divider("Quiz Complete");
        Console.ForegroundColor = ConsoleColor.Cyan;
        TypeText($"CyberGuardian: You got {score} out of {questions.Count} correct!");

        if (score == questions.Count)
            TypeText("CyberGuardian: 💪 Great job! You're a cybersecurity pro!");
        else if (score >= 3)
            TypeText("CyberGuardian: 👍 Nice! You’re on the right track. Keep learning to stay safe.");
        else
            TypeText("CyberGuardian: 🔒 Keep practicing — knowledge is your best defense!");

        Console.ResetColor();
    }

    // Prints text with a simulated typing effect (character-by-character delay)
    static void TypeText(string text, int delay = 30)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay);
        }
        Console.WriteLine();
    }

    // Prints a message in red color to indicate an error
    static void DisplayError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    // Prints a decorative divider with a title for UI separation
    static void Divider(string title)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        string line = new string('-', 40);
        Console.WriteLine($"\n{line}");
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"[ {title} ]");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"{line}\n");
        Console.ResetColor();
    }
}

