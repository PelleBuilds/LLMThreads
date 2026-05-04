using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ThreadMapLLM.Models;
using Microsoft.AspNetCore.Authorization;

namespace ThreadMapLLM.Controllers
{
    public class LoginController : Controller
    {
        //private readonly UserManager<User> _userManager;
        //private readonly SignInManager<User> _signInManager;
        public LoginViewModel? _loginViewModel { get; set; }
        public LoginController() {
            //_userManager = new UserManager<User>(1);
            //_signInManager = new SignInManager<User>(1);
        }
        
        public IActionResult LoginPage()
        {
            _loginViewModel = new LoginViewModel();
            return View("Login", _loginViewModel);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model != null && model.Action.Equals("Login"))
                    {
                        //_signInManager.PasswordSignInAsync(model.Username, model.Password, false, false).Wait();

                        return View("Index");
                    }
                    else if (model != null && model.Action.Equals("ForgotPassword"))
                    {
                        return View("Login", model);
                        // Handle forgot password logic here
                    }
                }
                catch (Exception ex) 
                {
                    Console.WriteLine($"Error during login: {ex.Message}");
                }
            }
            return View("Login", model);
        }
    }
}
