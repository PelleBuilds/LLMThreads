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
        public IHuggingFaceClient huggingFace;
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
        public async Task<IActionResult> Chat(string userInput)
        {
            
            if (string.IsNullOrEmpty(userInput))
            {
                return View("Index");
            }


            //var response = await huggingFace.Query(userInput);
            var response = "hej";// for testing
            var myModel = new ChatViewModel();
            myModel.Response = response;

           
            return PartialView("ChatMessage", myModel);

        }

        public IActionResult NewThread()
        {
            var myModel = new ChatViewModel();
            myModel.newthread = true;
            return PartialView("StartNewThread", myModel);
        }

        public async Task<IActionResult> nestedMessage(string userInput)
        {
            //var response = await huggingFace.Query(userInput);
            var response = "hej";// for testing
            var myModel = new ChatViewModel();
            myModel.Response = response;
            return PartialView("Chatmessage", myModel);
        }
    }
}
