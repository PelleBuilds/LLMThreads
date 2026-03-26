using HuggingFaceApiClient;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using ThreadMapLLM.Models;

namespace ThreadMapLLM.Controllers
{
    public class HomeController : Controller
    {
        private IHuggingFaceClient huggingFace;
        private ChatViewModel Model { get; set; } 

        public HomeController(IHuggingFaceClient aiClient)
        {
            huggingFace = aiClient;
            
        }
        public IActionResult Index()
        {
            Model = new ChatViewModel();
            return View(Model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> UserMessage(string userInput)
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
            try
            {
                var modelresponse = await Chat(userInput);
            }
            catch (Exception? ex) 
            { 
                return PartialView("ChatMessage", ex);
            }
            
            return PartialView("ChatMessage", chatmessage);

        }
        [HttpPost]
        public async Task<ChatMessageViewModel> Chat(string userInput)
        {
            
            if (string.IsNullOrEmpty(userInput))
            {
                return null;
            }


            //var response = await huggingFace.Query(userInput);
            var response = "hej";// for testing

            var chatmessage = new ChatMessageViewModel
            {
                Content = response,
                Role = "Assistant",
                TimeStamp = DateTime.Now
            };
            //myModel.Response = response;


            return chatmessage;/*PartialView("ChatMessage", chatmessage);*/

        }
        [HttpPost]
        public async Task<IActionResult> GenerateCode(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                return View("Index");
            }
            
            var prompt = $"You are a tool used for generating react code based on user prompts," +
                $" your task is generating the code that is fed into an Sandpack Preview, " +
                $"therefore you should only respond in code that will run straight away in sandpack," +
                $"generate code that matches the following criteria: {userInput}," +
                $" respond in code and nothing else, no explanation or any text that isnt code";

            try
            {
                var response = await huggingFace.Query(prompt);
                //var myModel = new ChatViewModel();
                //myModel.Response = response;
                response = response.Replace("```jsx\n", "").Replace("```javascript\n", "").Replace("```", "");
                return Json(new { generatedCode = response });
            }
            catch (HttpRequestException e) 
            { 
             return PartialView("ChatMessage",e);
            }
        }
        
    }
}
