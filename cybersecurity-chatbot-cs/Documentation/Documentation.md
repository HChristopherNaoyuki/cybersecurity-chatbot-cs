# Cybersecurity Awareness Chatbot (C# Console Application)

## Table of Contents

- [Overview](#overview)
- [Purpose and Goals](#purpose-and-goals)
- [Main Features](#main-features)
- [Supported Cybersecurity Topics](#supported-cybersecurity-topics)
- [Architecture Overview](#architecture-overview)
- [Key Components](#key-components)
- [How to Build and Run](#how-to-build-and-run)
- [Project Folder Structure](#project-folder-structure)
- [Dependencies](#dependencies)
- [Limitations and Known Issues](#limitations-and-known-issues)
- [Future Improvement Ideas](#future-improvement-ideas)
- [Contributing](#contributing)
- [License](#license)

## Overview

This is a console-based cybersecurity awareness chatbot written in **C#** (.NET Framework).

The application provides basic educational explanations about common online security topics. 

It uses simple keyword matching, lightweight memory (persistent across sessions), typing animation, 
colored console output, and a small ML.NET sentiment classifier.

## Purpose and Goals

- Raise basic awareness of common cybersecurity risks and good practices
- Demonstrate clean object-oriented design in a small console application
- Show simple usage of ML.NET for sentiment detection in .NET Framework
- Provide an extensible base for adding more topics and interaction patterns
- Serve as an educational example for intermediate C# developers

## Main Features

- One-time ASCII art banner at startup
- Optional welcome voice greeting (welcome.wav)
- User name recognition and recall
- Persistent lightweight memory (keyword frequency tracking)
- Typing animation for chatbot responses
- Colored console output
- Basic command support (`help`, `exit`, name recall, frequent topic query)
- Rule-based + ML.NET hybrid sentiment detection
- Randomised high-quality answers from a small knowledge base

## Supported Cybersecurity Topics

The following topics currently have predefined responses:

- password
- 2fa
- phishing
- privacy
- vpn
- wifi
- email

Additional meta-topics:

- how are you
- purpose
- help

## Architecture Overview

```
ChatBot (main orchestrator)
├── UserInterface          → all console I/O + animations + audio
├── KnowledgeBase          → topic → response mapping (randomized)
├── MemoryManager          → persistent keyword frequency + name
├── ConversationManager    → input parsing, command detection, NLP flow
├── SentimentAnalyzerML    → ML.NET binary sentiment + rule-based refinements
└── SimpleKeywordExtractor → very basic stop-word filtered tokenization
```

## Key Components

| Class / File                | Responsibility                                      |
|-----------------------------|------------------------------------------------------|
| `ChatBot.cs`                | Application entry point & subsystem coordination     |
| `UserInterface.cs`          | All console output, input, typing effect, audio      |
| `KnowledgeBase.cs`          | Topic-response dictionary with randomization         |
| `MemoryManager.cs`          | Persistent keyword counts (text file)                |
| `ConversationManager.cs`    | Main chat loop, command detection, response logic    |
| `SentimentAnalyzerML.cs`    | ML.NET sentiment model + curious/worried rules       |
| `SimpleKeywordExtractor.cs` | Naive keyword extraction (used by conversation flow) |
| `ChatDisplayBuffer.cs`      | Circular buffer (currently not actively used)        |

## How to Build and Run

1. Open the solution in Visual Studio 2019 / 2022
2. Restore NuGet packages
3. Build the project (`cybersecurity-chatbot-cs`)
4. Run the executable

**Required NuGet packages**

- Microsoft.ML (≈ 1.7.x – 2.x should also work)

## Project Folder Structure

```
cybersecurity-chatbot-cs/
├── Properties/
├── References/
├── Documentation/
│   └── Documentation.md
├── Resources/
│   ├── Audio/
│   │   └── welcome.wav
│   └── Images/
│       └── cybersecurity.png
├── App.config
├── ChatBot.cs
├── ChatDisplayBuffer.cs
├── ConversationManager.cs
├── KnowledgeBase.cs
├── MemoryManager.cs
├── Program.cs
├── SentimentAnalyzerML.cs
├── SimpleKeywordExtractor.cs
├── SimpleSentimentAnalyzer.cs
├── UserInterface.cs
└── packages.config
```

## Dependencies

- .NET Framework 4.7.2 or higher
- Microsoft.ML (NuGet)

No external web services or cloud dependencies are used.

## Limitations and Known Issues

- Very basic keyword matching (no intent classification, no entity recognition)
- Small training set for ML.NET sentiment model → modest accuracy
- No real conversation context beyond keyword frequency
- No input validation against prompt injection / jailbreak attempts
- Console-only (no GUI, no web interface)
- Windows-only audio playback (SoundPlayer)
- No unit tests
- Circular buffer class exists but is not currently wired into the UI

## Future Improvement Ideas

- Replace keyword matching with small transformer / embeddings model
- Add more training examples to the sentiment model
- Implement proper conversation history display with scrolling buffer
- Add topic-specific follow-up questions
- Save/load full session transcript
- Support multiple languages
- Add threat model / adversary simulation mode
- Export memory statistics to JSON / CSV

## Contributing

Contributions are welcome.

Preferred workflow:

1. Fork the repository
2. Create a feature branch (`feature/xxx` or `fix/yyy`)
3. Submit a pull request with clear description

Please open an issue first for larger changes.

Repository:  
```
https://github.com/HChristopherNaoyuki/cybersecurity-chatbot-cs.git
```

## License

This project is provided as-is for educational and personal use.
See the *Disclaimer* section below.

**Disclaimer**

UNDER NO CIRCUMSTANCES SHOULD IMAGES OR EMOJIS BE INCLUDED DIRECTLY IN THE README FILE. 
ALL VISUAL MEDIA, INCLUDING SCREENSHOTS AND IMAGES OF THE APPLICATION, MUST BE STORED IN A 
DEDICATED FOLDER WITHIN THE PROJECT DIRECTORY. THIS FOLDER SHOULD BE CLEARLY STRUCTURED AND 
NAMED ACCORDINGLY TO INDICATE THAT IT CONTAINS ALL VISUAL CONTENT RELATED TO THE APPLICATION 
(FOR EXAMPLE, A FOLDER NAMED `images`, `screenshots`, OR `media`).

I AM NOT LIABLE OR RESPONSIBLE FOR ANY MALFUNCTIONS, DEFECTS, OR ISSUES THAT MAY OCCUR AS A 
RESULT OF COPYING, MODIFYING, OR USING THIS SOFTWARE. IF YOU ENCOUNTER ANY PROBLEMS OR ERRORS, 
PLEASE DO NOT ATTEMPT TO FIX THEM SILENTLY OR OUTSIDE THE PROJECT. INSTEAD, KINDLY SUBMIT A PULL 
REQUEST OR OPEN AN ISSUE ON THE CORRESPONDING GITHUB REPOSITORY, SO THAT IT CAN BE ADDRESSED 
APPROPRIATELY BY THE MAINTAINERS OR CONTRIBUTORS.

---