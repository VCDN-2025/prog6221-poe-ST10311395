"# prog6221-poe-ST10311395" 
please note the my final code is in zipped folder
# 🛡️ CyberGuardian – Your AI Cybersecurity Companion

CyberGuardian is a C# console-based chatbot that educates users on cybersecurity topics, assists with tasks like setting reminders, and features an interactive quiz. It uses natural language recognition and sentiment detection to create an intelligent and helpful experience for users seeking to improve their cyber awareness.

---

## 📌 Overview

CyberGuardian offers the following features:

- ✅ **Cybersecurity Task Assistant**  
  Add tasks such as updating passwords, enabling 2FA, or reviewing privacy settings. Optional reminders can be set.

- 🎯 **Interactive Quiz**  
  Answer multiple-choice and true/false questions to test your cybersecurity knowledge with real-time feedback.

- 🧠 **Natural Language Understanding**  
  Recognizes common commands and keywords like "add a task", "remind me to", or "start quiz" using flexible phrasing.

- 🗂️ **Activity Log**  
  Tracks all major chatbot actions (e.g., added tasks, set reminders, started quizzes, NLP detections) and allows users to review recent activity using commands like:
  - `show activity log`
  - `what have you done for me?`

- 🎧 **Voice Greeting**  
  Plays a welcome audio WAV file when the program starts.

- 🌐 **Formatted User Interface**  
  Uses colors, ASCII art, and typing effects to improve readability and engagement.

---

## 🛠️ Setup Instructions

To run **CyberGuardian** on your machine, follow these steps:

### 1. **Open in Visual Studio**

- Open **Visual Studio** (2022 or later recommended).
- Click on **File > Open > Project/Solution**.
- Navigate to the folder where your project is located.
- Open the `.sln` file (e.g., `CyberGuardian.sln`).

### 2. **Ensure the Following Files Exist in Your Project**

| File/Folder | Purpose |
|-------------|---------|
| `Program.cs` | Main application code containing the chatbot logic |
| `CyberTask` class | Represents task structure and reminder details |
| `README.md` | Project documentation |
| `Music/` | Folder for welcome audio files |
| `ElevenLabs_*.wav` | The actual WAV audio file played at startup |
| `Figgle` Package | Used for ASCII art title (installed via NuGet) |

> If `Figgle` is not installed yet, install it using:
```bash
Install-Package Figgle

