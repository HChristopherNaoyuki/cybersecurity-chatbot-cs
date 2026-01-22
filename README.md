# Cybersecurity Awareness Chatbot

## Table of Contents

- [Project Overview](#project-overview)
- [Purpose](#purpose)
- [Main Features](#main-features)
- [Supported Topics](#supported-topics)
- [Architecture](#architecture)
- [Key Components](#key-components)
- [Prerequisites](#prerequisites)
- [How to Build and Run](#how-to-build-and-run)
- [Folder Structure](#folder-structure)
- [Dependencies](#dependencies)
- [Current Limitations](#current-limitations)
- [Possible Future Improvements](#possible-future-improvements)
- [Contributing](#contributing)
- [License](#license)
- [Disclaimer](#disclaimer)

## Project Overview

This is a console-based cybersecurity awareness chatbot developed in C# using .NET Framework.  
The application provides short, educational explanations about common online security topics 
using keyword-based matching, simple memory persistence, typing animation, colored output, and 
a lightweight ML.NET sentiment classifier.

Repository:  
```
https://github.com/HChristopherNaoyuki/cybersecurity-chatbot-cs.git
```

## Purpose

The main goals of this project are:

- To offer basic cybersecurity education in an interactive format
- To demonstrate clean object-oriented design in a small console application
- To show practical usage of ML.NET in .NET Framework for basic sentiment analysis
- To serve as an extensible educational example for intermediate C# developers

## Main Features

- ASCII art banner displayed at startup
- Optional audio greeting (welcome.wav)
- User name collection and recall
- Persistent memory of discussed topics (keyword frequency saved to file)
- Typing animation effect for chatbot replies
- Colored console output
- Built-in commands: `help`, `exit`, name recall, most frequent topic query
- Hybrid sentiment detection (ML.NET + rule-based refinements)
- Randomized high-quality responses from a small knowledge base

## Supported Topics

Currently implemented topics with dedicated responses:

- password  
- 2fa  
- phishing  
- privacy  
- vpn  
- wifi  
- email  

Meta / special commands:

- how are you  
- purpose  
- help  

## Architecture

High-level component structure:

```
ChatBot (orchestrator)
├─ UserInterface          → console I/O, typing effect, audio
├─ KnowledgeBase          → topic → randomized response mapping
├─ MemoryManager          → persistent keyword frequency & name storage
├─ ConversationManager    → input parsing, command handling, response logic
├─ SentimentAnalyzerML    → ML.NET binary sentiment + rule-based extensions
└─ SimpleKeywordExtractor → basic stop-word filtered tokenization
```

## Key Components

| File                        | Main responsibility                                          |
|-----------------------------|----------------------------------------------------------------|
| ChatBot.cs                  | Application startup and subsystem coordination                 |
| UserInterface.cs            | All console rendering, input, typing animation, audio playback |
| KnowledgeBase.cs            | Topic-to-response dictionary with randomization                |
| MemoryManager.cs            | Persistent storage (user_keywords.txt) of keyword counts       |
| ConversationManager.cs      | Main conversation loop, command detection, NLP flow            |
| SentimentAnalyzerML.cs      | ML.NET sentiment classification + curious/worried rules        |
| SimpleKeywordExtractor.cs   | Naive keyword extraction logic                                 |
| ChatDisplayBuffer.cs        | Circular buffer (currently not connected to display logic)     |

## Prerequisites

- .NET Framework 4.7.2 or newer  
- Visual Studio 2019 or 2022 (Community edition is sufficient)  
- NuGet package: Microsoft.ML (~1.7.x – 2.x compatible)

## How to Build and Run

1. Clone the repository  
   ```bash
   git clone https://github.com/HChristopherNaoyuki/cybersecurity-chatbot-cs.git
   ```

2. Open the solution file (`cybersecurity-chatbot-cs.sln`) in Visual Studio

3. Restore NuGet packages

4. Build the solution

5. Run the project (F5 or Debug → Start Debugging)

## Folder Structure

```
cybersecurity-chatbot-cs/
├── Documentation/
│   └── Documentation.md
├── Resources/
│   ├── Audio/
│   │   └── welcome.wav
│   └── Images/
│       └── cybersecurity.png
├── Properties/
├── References/
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

- Microsoft.ML (NuGet package)  
- .NET Framework 4.7.2+ (no .NET Core / .NET 5+ version yet)

No external APIs, cloud services or databases are used.

## Current Limitations

- Very basic keyword matching (no real intent detection or entity extraction)
- Small ML.NET training dataset → limited sentiment accuracy
- No multi-turn context beyond keyword frequency
- No protection against prompt injection-style inputs
- Console-only interface (no GUI or web version)
- Windows-specific audio playback (SoundPlayer class)
- No automated tests
- ChatDisplayBuffer class exists but is not currently used

## Possible Future Improvements

- Replace keyword matching with lightweight embeddings or small transformer model
- Expand ML.NET sentiment training data significantly
- Implement proper scrolling chat history display
- Add follow-up questions per topic
- Export/import memory or session logs
- Multi-language support
- Threat / adversary simulation mode
- JSON/CSV export of usage statistics

## Contributing

Pull requests are welcome.

Preferred workflow:

1. Fork the repository
2. Create a branch (`feature/short-description` or `fix/issue-description`)
3. Make changes
4. Submit a pull request with clear description

For larger changes, please open an issue first to discuss the direction.

## License

MIT License

This project is provided as-is for educational and personal use.
See the Disclaimer section below.

## Disclaimer

UNDER NO CIRCUMSTANCES SHOULD IMAGES OR EMOJIS BE INCLUDED DIRECTLY IN THE README FILE. 
ALL VISUAL MEDIA, INCLUDING SCREENSHOTS AND IMAGES OF THE APPLICATION, MUST BE STORED IN 
A DEDICATED FOLDER WITHIN THE PROJECT DIRECTORY. THIS FOLDER SHOULD BE CLEARLY STRUCTURED 
AND NAMED ACCORDINGLY TO INDICATE THAT IT CONTAINS ALL VISUAL CONTENT RELATED TO THE 
APPLICATION (FOR EXAMPLE, A FOLDER NAMED `images`, `screenshots`, OR `media`).

I AM NOT LIABLE OR RESPONSIBLE FOR ANY MALFUNCTIONS, DEFECTS, OR ISSUES THAT MAY OCCUR AS A 
RESULT OF COPYING, MODIFYING, OR USING THIS SOFTWARE. IF YOU ENCOUNTER ANY PROBLEMS OR ERRORS, 
PLEASE DO NOT ATTEMPT TO FIX THEM SILENTLY OR OUTSIDE THE PROJECT. INSTEAD, KINDLY SUBMIT A 
PULL REQUEST OR OPEN AN ISSUE ON THE CORRESPONDING GITHUB REPOSITORY, SO THAT IT CAN BE ADDRESSED 
APPROPRIATELY BY THE MAINTAINERS OR CONTRIBUTORS.

---
