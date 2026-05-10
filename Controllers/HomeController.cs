using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ThreadMapLLM.Models;
using ThreadMapLLM.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ThreadMapLLM.Controllers
{
    public class HomeController: Controller
    {
        private MongoDBClient _mongoDBClient;
        private User _user;
        private readonly OllamaApiService ollama = new();
        public ChatViewModel _model { get; set; }
        public Chat chat { get; set; }

        public HomeController(MongoDBClient mongoDBClient/*, User user*/)
        {

            User testuser = new User
            {
                UserId = "1",
                Username = "test",
                Password = "test"
            };

            this._user = testuser;

            chat = new Chat
            {
                UserId = _user.UserId,
                ConversationId = "hej1",
                ChatMessages = new List<ChatMessageViewModel?>()
            };

            this.chat = chat;

            _model = new ChatViewModel
            {
                Chats = new List<Chat>(),

            };
            //GetChat(testuser.UserId);
            _mongoDBClient = mongoDBClient;

        }
        
        public async Task<IActionResult> Index()
        {
            
                var allChats = await _mongoDBClient.GetAllChats(_user.UserId);
                _model.Chats = allChats ?? new List<Chat>();
            
            
            
            

            return View(_model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> UserMessage(string userInput, bool generateCode, string conversationId)
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

            if(chat.ConversationId != conversationId)
            {
                _model.Chats.Add(chat);
            }

            chat.ChatMessages.Add(chatmessage);

            if (!generateCode)
            {
                var modelresponse = ConvertToMessage(await GenerateText(userInput));
                
                await _mongoDBClient.SaveChatAsync(chat);

                return PartialView("ChatMessage", modelresponse);
               
            }
            else
            {
                var modelresponse = await GenerateCode(userInput);
                var modelResponseMessage =  ConvertToMessage(modelresponse);

                await _mongoDBClient.SaveChatAsync(chat);
                
                return Json(new { generatedCode = modelresponse });
                //return PartialView("ChatMessage", modelresponse);
            }
            


        }
        public ChatMessageViewModel ConvertToMessage(string message)
        {
            var chatmessage = new ChatMessageViewModel
            {
                Content = message,
                Role = "Assistant",
                TimeStamp = DateTime.Now
            };

            chat.ChatMessages.Add(chatmessage);

            return chatmessage;
        }

        [HttpPost]
        public async Task<string> GenerateText(string Input)
        {
            var response = "";

            try
            {
                response = await ollama.Query(Input);
               // response = "hej "+_user.Username;// for testing
                return response;
            }
            catch (HttpRequestException ex) 
            {
                return "The agent is not avalible right now" + ex;// for easier debugging create logging service later
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

        //public async void GetChat(string userID)
        //{

        //    var allChats= await _mongoDBClient.GetAllChats(userID);
        //    foreach (Chat chat in allChats) 
        //    {
        //    model.Chats.Add(chat);
        //    }
            
        //}

    }
}
