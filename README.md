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

This is a console-based cybersecurity awareness chatbot developed in C# using  
.NET Framework. The application provides short, educational explanations about  
common online security topics using keyword-based matching, simple memory  
persistence, typing animation, colored output, and a lightweight ML.NET  
sentiment classifier. The project structure supports easy extension and  
demonstrates core software design principles.

Repository clone command:
```
git clone https://github.com/HChristopherNaoyuki/cybersecurity-chatbot-cs.git
```

## Purpose

The main goals of this project are to offer basic cybersecurity education  
in an interactive format, to demonstrate clean object-oriented design in a  
small console application, to show practical usage of ML.NET in .NET Framework  
for basic sentiment analysis, and to serve as an extensible educational  
example for intermediate C# developers.

## Main Features

- ASCII art banner displayed at startup for visual engagement.
- Optional audio greeting (welcome.wav) for user experience.
- User name collection and recall across sessions.
- Persistent memory of discussed topics via keyword frequency saved to file.
- Typing animation effect for realistic chatbot replies.
- Colored console output for improved readability.
- Built-in commands: help, exit, name recall, and most frequent topic query.
- Hybrid sentiment detection combining ML.NET with rule-based refinements.
- Randomized high-quality responses from a small knowledge base.

## Supported Topics

Currently implemented topics with dedicated responses include password, 2fa,  
phishing, privacy, vpn, wifi, and email. Meta or special commands include  
how are you, purpose, and help. Each recognized keyword triggers a relevant,  
educational response designed to raise basic security awareness.

## Architecture

High-level component structure: ChatBot orchestrates the application flow.  
UserInterface handles console I/O, typing effect, and audio. KnowledgeBase  
maps topics to randomized responses. MemoryManager manages persistent  
keyword frequency and name storage. ConversationManager oversees input  
parsing, command detection, and response logic. SentimentAnalyzerML provides  
ML.NET binary sentiment with rule-based extensions. SimpleKeywordExtractor  
performs basic stop-word filtered tokenization for keyword detection.

## Key Components

| File                        | Main responsibility                                          |
|-----------------------------|--------------------------------------------------------------|
| ChatBot.cs                  | Application startup and subsystem coordination               |
| UserInterface.cs            | Console rendering, input, typing animation, audio playback   |
| KnowledgeBase.cs            | Topic-to-response dictionary with randomization              |
| MemoryManager.cs            | Persistent storage (user_keywords.txt) of keyword counts     |
| ConversationManager.cs      | Conversation loop, command detection, and NLP flow           |
| SentimentAnalyzerML.cs      | ML.NET sentiment classification with rules                   |
| SimpleKeywordExtractor.cs   | Naive keyword extraction logic                               |
| ChatDisplayBuffer.cs        | Circular buffer (currently not connected to display)         |

## Prerequisites

- .NET Framework 4.7.2 or newer
- Visual Studio 2019 or 2022 (Community edition is sufficient)
- NuGet package: Microsoft.ML (~1.7.x – 2.x compatible)

## How to Build and Run

1. Clone the repository:
   git clone https://github.com/HChristopherNaoyuki/cybersecurity-chatbot-cs.git
2. Open the solution file (cybersecurity-chatbot-cs.sln) in Visual Studio.
3. Restore NuGet packages via right-click on solution or build.
4. Build the solution (Build -> Build Solution).
5. Run the project by pressing F5 or selecting Debug -> Start Debugging.

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
No external APIs, cloud services, or databases are used.

## Current Limitations

- Very basic keyword matching without real intent detection.
- Small ML.NET training dataset leading to limited sentiment accuracy.
- No multi-turn conversation context beyond keyword frequency.
- No protection against prompt injection-style inputs.
- Console-only interface without GUI or web version.
- Windows-specific audio playback using SoundPlayer class.
- No automated tests included.
- ChatDisplayBuffer class exists but is currently unused.

## Possible Future Improvements

- Replace keyword matching with lightweight embeddings or small transformer.
- Expand ML.NET sentiment training data significantly.
- Implement proper scrolling chat history display.
- Add follow-up questions per topic for deeper learning.
- Export or import memory and session logs.
- Multi-language support for broader accessibility.
- Threat or adversary simulation mode for practical scenarios.
- JSON or CSV export of usage statistics.

## Contributing

Pull requests are welcome. Preferred workflow: Fork the repository, create a  
branch (feature/short-description or fix/issue-description), make changes,  
and submit a pull request with a clear description. For larger changes,  
please open an issue first to discuss the direction.

## License

This project is provided as-is for educational and personal use. See the  
Disclaimer section below for more details.

## Disclaimer

UNDER NO CIRCUMSTANCES SHOULD IMAGES OR EMOJIS BE INCLUDED DIRECTLY IN THE  
README FILE. ALL VISUAL MEDIA, INCLUDING SCREENSHOTS AND IMAGES OF THE  
APPLICATION, MUST BE STORED IN A DEDICATED FOLDER WITHIN THE PROJECT DIRECTORY.  
THIS FOLDER SHOULD BE CLEARLY STRUCTURED AND NAMED ACCORDINGLY TO INDICATE THAT  
IT CONTAINS ALL VISUAL CONTENT RELATED TO THE APPLICATION (FOR EXAMPLE, A  
FOLDER NAMED images, screenshots, OR media). 

I AM NOT LIABLE OR RESPONSIBLE FOR ANY MALFUNCTIONS, DEFECTS, OR ISSUES THAT 
MAY OCCUR AS A RESULT OF COPYING, MODIFYING, OR USING THIS SOFTWARE. IF YOU 
ENCOUNTER ANY PROBLEMS OR ERRORS, PLEASE DO NOT ATTEMPT TO FIX THEM SILENTLY 
OR OUTSIDE THE PROJECT. INSTEAD, KINDLY SUBMIT A PULL REQUEST OR OPEN AN ISSUE 
ON THE CORRESPONDING GITHUB REPOSITORY, SO THAT IT CAN BE ADDRESSED APPROPRIATELY 
BY THE MAINTAINERS OR CONTRIBUTORS.

---

End of Document

---
