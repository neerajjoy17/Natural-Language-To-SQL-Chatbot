AI Football Query Engine ⚽
====================

A full-stack MVC web application for querying historical football match data using natural language.
This project focuses on integrating AI into a traditional backend architecture to allow users to retrieve
football statistics without writing SQL queries.

I built this project mainly for learning purposes, with a strong focus on backend architecture,
AI integration, and understanding how large language models can be used inside real applications.
The UI is intentionally simple — correctness, workflow, and system design were the priority.

📦 Technologies
------------

- .NET (C#)
- ASP.NET MVC
- Azure OpenAI
- Azure AI SDK
- SQL Server
- HTML
- CSS

🦄 Features
--------

Here's what you can do with the AI Football Query Engine:

Natural Language Queries  
Ask football-related questions in plain English instead of writing SQL.

AI SQL Generation  
The system uses Azure OpenAI to automatically generate SQL queries based on user input.

Dataset Relevance Detection  
The AI checks whether the question matches the available football dataset before generating queries.

Automatic Query Validation  
Generated SQL queries are validated before execution to reduce errors.

Database Integration  
Football match data is stored in SQL Server and retrieved dynamically.

MVC Backend Architecture  
The application follows the MVC pattern to keep the frontend, logic, and data layers cleanly separated.

📐 Architecture
------------

The application follows a traditional MVC structure combined with an AI-powered query pipeline.

![Architecture Diagram](/ai-query-architecture.png)

How it works:

User Input  
A user enters a football-related question in plain English through the web interface.

Controller Processing  
The MVC controller receives the input and forwards it to the Azure OpenAI service.

AI Query Analysis  
The AI checks whether the question is relevant to the available football dataset.
If valid, it generates an SQL query that matches the user’s request.

Query Validation & Execution  
The generated SQL query is validated and then executed against the SQL Server database.

Formatted Response  
The retrieved data is sent back through the controller and displayed on the frontend in a structured format.

This architecture allowed me to combine a standard backend design pattern (MVC)
with an AI-driven processing layer, showing how language models can be integrated
into real production-style workflows rather than just chat interfaces.

👩🏽‍🍳 The Process
-----------

I started by preparing the football dataset and understanding its structure,
including how match data, teams, and seasons were organized.

Next, I built the MVC backend application in ASP.NET.
The goal was to create a clean workflow where user input could be processed safely
and forwarded to the AI service.

After that, I integrated Azure OpenAI using the Azure SDK.
This allowed the system to analyze user questions, determine relevance to the dataset,
and generate SQL queries dynamically.

Once the AI-generated queries were working, I added validation checks
before executing them against the database to ensure safe and reliable results.

Finally, I connected the backend to a simple frontend interface where users
can submit questions and view formatted results.

Throughout the project, I focused on understanding how AI fits into real backend systems
rather than just building a chat interface.

📚 What I Learned
--------------

During this project, I gained a stronger understanding of how AI services
can be integrated into production-style backend applications.

AI Integration  
I learned how to connect Azure OpenAI to a .NET MVC application and process responses programmatically.

Backend Workflow Design  
This project helped me understand how to build multi-step processing pipelines
including validation, AI processing, database execution, and formatting.

Prompt Engineering  
I gained experience designing prompts that help the model generate correct SQL queries.

Database Query Automation  
I learned how structured database queries can be generated dynamically from natural language.

Enterprise-style Architecture  
Building this project improved my understanding of how AI-powered features
fit into traditional MVC application design.

💭 How Can It Be Improved?
-----------------------

- Add graphical dashboards for statistics
- Improve prompt engineering for higher SQL accuracy
- Add authentication and user sessions
- Cache frequent queries for performance
- Support multiple football leagues
- Add Swagger documentation for backend APIs

🚦 Running the Project
-------------------

To run the project in your local environment, follow these steps:

Clone the repository

```bash
git clone <your-repo-url>
cd <project-folder>
dotnet restore
dotnet run
```

Make sure you configure:
-------------------
- Azure OpenAI API credentials
- Database connection string

Open the application in your browser using the URL shown in the console, usually:
```https://localhost:5001```

🍿 Video
-----------------------
Here is a short demo showing the main features of the application:

https://github.com/user-attachments/assets/20387b17-09fd-40c8-ab17-7f4a57309aa2
