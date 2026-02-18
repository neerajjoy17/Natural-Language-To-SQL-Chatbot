using System.Diagnostics;
using Bot.Models;
using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using Npgsql;
using System;
namespace Bot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var previousResults = HttpContext.Session.GetString("Results");
            ViewData["PreviousResults"] = previousResults;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> TalktoAI(string question)
        {
            var previousResults = HttpContext.Session.GetString("Results");
            var answers = await GetAnswer(question);
            var allResults = "";
            var endresult = new System.Text.StringBuilder();
            if (answers.queryResponse != "YES")
            {
                ViewData["ResultData"] = answers.queryResponse;
                if (string.IsNullOrEmpty(previousResults))
                {
                    endresult.AppendLine(question);
                    endresult.AppendLine(answers.queryResponse);
                }
                else
                {
                    endresult.AppendLine(previousResults);
                    endresult.AppendLine(question);
                    endresult.AppendLine(answers.queryResponse);
                }
            }
            else
            {
                string header = string.Format("{0,-10} {1,-8} {2,-6} {3,-12} {4,-25} {5,-11} {6,-11} {7,-25} {8,-8}", "ID", "Season", "Week", "Date", "Home", "HomeGoals", "AwayGoals", "Away", "Result");
                string seperator = new string('-', header.Length);
                var table = new System.Text.StringBuilder();
                table.AppendLine(header);
                table.AppendLine(seperator);
                foreach(var match in answers.matches)
                {
                    string row = string.Format("{0,-10} {1,-8} {2,-6} {3,-12} {4,-25} {5,-11} {6,-11} {7,-25} {8,-8}", match.id, match.season, match.week, match.date.ToShortDateString(), match.home, match.homegoals, match.awaygoals, match.away, match.result);
                    table = table.AppendLine(row);
                }
                Debug.WriteLine(table);
                if (string.IsNullOrEmpty(previousResults))
                {
                    endresult.AppendLine(question);
                    endresult.AppendLine(table.ToString());
                }
                else
                {
                    endresult.AppendLine(previousResults);
                    endresult.AppendLine(question);
                    endresult.AppendLine(table.ToString());
                }
            }
            allResults = endresult.ToString();
            HttpContext.Session.SetString("Results", allResults);
            ViewData["PreviousResults"] = allResults;
            return View("Index");
        }

        private async Task<BotAnswer> GetAnswer(string question)
        {
            var pretext = "A query should be generated for the following scenario based on a table called Matches. Only return the query without any other text if the scenario contains a valid query based on the schema given. else just return 'I don't think this is relevant to the table. can you try again?'";
            var posttext = "\n The Schema Of the Table is as Follows \n Matches(id, season, week, date, home, homegoals, awaygoals, away, result";
            var systemmsg = pretext + question + posttext;
            var apiKey = _configuration.GetValue<string>("Azure:APIKEY");
            var apiEndpoint = _configuration.GetValue<string>("Azure:APIEndpoint");
            var modelName = _configuration.GetValue<string>("Azure:model");
            var deploymentName = "gpt-35-turbo-16k";
            var endpointUrl = apiEndpoint;
            var key = apiKey;

            var client = new AzureOpenAIClient(new Uri(endpointUrl), new ApiKeyCredential(key));
            var chatClient = client.GetChatClient(deploymentName);
            var messages = new List<ChatMessage>();
            messages.Add(systemmsg);

            var query = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions()
            {
                Temperature = (float)0.7,
                FrequencyPenalty = (float)0,
                PresencePenalty = (float)0,
            });
            var chatResponse = query.Value.Content.Last().Text;

            var queryCheck = "Check if this is a valid PostgreSQL query or not. If Yes, return just the string 'YES' else return the string 'NO' \n" + chatResponse;
            var queryCheckResult = await chatClient.CompleteChatAsync(new List<ChatMessage>() { queryCheck }, new ChatCompletionOptions()
            {
                Temperature = (float)0.7,
                FrequencyPenalty = (float)0,
                PresencePenalty = (float)0,
            });
            if(queryCheckResult.Value.Content.Last().Text != "YES")
            {
                return new BotAnswer()
                {
                    queryResponse = chatResponse,
                    matches = new List<Match>()
                };
            }
            else
            {
                List<Match> matches = new List<Match>();
                using (var conn = new NpgsqlConnection(_configuration.GetValue<string>("Azure:ConnectionString")))
                {
                    conn.Open();
                    using (var command = new NpgsqlCommand(chatResponse, conn))
                    {
                        var reader = await command.ExecuteReaderAsync();
                        while (reader.Read())
                        {
                            Match match = new Match();
                            Type type = match.GetType();
                            // Iterate through each column of the current row
                            foreach (var item in type.GetProperties())
                            {
                                switch (item.Name)
                                {
                                    case "id":
                                        match.id = (int)reader[item.Name];
                                        break;
                                    case "season":
                                        match.season = (int)reader[item.Name];
                                        break;
                                    case "week":
                                        match.week = (int)reader[item.Name];
                                        break;
                                    case "date":
                                        match.date = (DateTime)reader[item.Name];
                                        break;
                                    case "home":
                                        match.home = (string)reader[item.Name];
                                        break;
                                    case "homegoals":
                                        match.homegoals = (int)reader[item.Name];
                                        break;
                                    case "awaygoals":
                                        match.awaygoals = (int)reader[item.Name];
                                        break;
                                    case "away":
                                        match.away = (string)reader[item.Name];
                                        break;
                                    case "result":
                                        match.result = (string)reader[item.Name];
                                        break;
                                }
                            }

                            // Add the row to the result list
                            matches.Add(match);
                        }
                        reader.Close();
                    }
                    conn.Close();
                }
                return new BotAnswer()
                {
                    queryResponse = queryCheckResult.Value.Content.Last().Text,
                    matches = matches
                };
            }
            
        }
    }
}
