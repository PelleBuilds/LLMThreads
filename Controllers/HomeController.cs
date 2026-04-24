using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ThreadMapLLM.Models;
using ThreadMapLLM.Services;
using Microsoft.AspNetCore.Identity;

namespace ThreadMapLLM.Controllers
{
    public class HomeController: Controller
    {
        //private User _user = user;
        private readonly OllamaApiService ollama = new();
        public required ChatViewModel model { get; set; }

        public HomeController()
        {
            model = new ChatViewModel
            {
                UserId = "hej",
                ConversationId = "hej"
            };
        }
        public IActionResult Index()
        {
            
            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> UserMessage(string userInput, bool generateCode)
        {

            if (string.IsNullOrEmpty(userInput))
            {
                return View("Index");
            }

            var chatmessage = new ChatMessageViewModel
            {
                Content = userInput,
                Role = "User",
                TimeStamp = DateTime.Now
            };
           
            model.ChatMessages.Add(chatmessage);

            if (!generateCode)
            {
                var modelresponse = await Chat(userInput);
                return PartialView("ChatMessage", modelresponse);
            }
            else
            {
                var modelresponse = await GenerateCode(userInput);
                return Json(new { generatedCode = modelresponse });
                //return PartialView("ChatMessage", modelresponse);
            }
            
        }

        [HttpPost]
        public async Task<ChatMessageViewModel?> Chat(string Input)
        {
            var response = "";
            
            if (string.IsNullOrEmpty(Input))
            {
                return null;
            }

            try
            {

                response = await ollama.Query(Input);
                //response = "hej";// for testing

                var chatmessage = new ChatMessageViewModel
                {
                    Content = response,
                    Role = "Assistant",
                    TimeStamp = DateTime.Now
                };

                model.ChatMessages.Add(chatmessage);

                return chatmessage;

            }
            catch (HttpRequestException? ex) 
            {

                return new ChatMessageViewModel 
                {
                    Content = "The agent is not avalible right now" + ex,// for easier debugging create logging service later
                    Role = "System",
                    TimeStamp = DateTime.Now
                };

            }
            
        }

        [HttpPost]
        public async Task<string> GenerateCode(string userInput)
        {
            var response = "";
            if (string.IsNullOrEmpty(userInput))
            {
                //return View("Index");
                return "";
            }
            
            var prompt = $"You are a tool used for generating react code based on user prompts," +
                $" your task is generating the code that is fed into an Sandpack Preview, " +
                $"therefore you should only respond in code that will run straight away in sandpack," +
                $"generate code that matches the following criteria: {userInput}," +
                $" respond in code and nothing else, no explanation or any text that isnt code";

            try
            {
                response = await ollama.Query(prompt);
                response = response.Replace("```jsx\n", "").Replace("```javascript\n", "").Replace("```", "");
                return response;
            }
            catch (HttpRequestException e) 
            {
                //return PartialView("ChatMessage",e);
                return "";
            }
                

        }
        
    }
}
