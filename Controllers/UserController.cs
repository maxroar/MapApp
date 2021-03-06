using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mapapp.Models;
using mapapp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace mapapp.Controllers
{
    public class UserController : Controller
    {
        private MyContext _context;
        public UserController(MyContext context)
        {
            _context = context;
        }


        //get: send home route to login
        [HttpGetAttribute]
        [RouteAttribute("")]
        public IActionResult Index(){
            return RedirectToAction("ShowLogin");
        }
        // GET: /users/login
        [HttpGet]
        [Route("users/login")]
        public IActionResult ShowLogin(){
            return View("ShowLogin");
        }

        // GET: /users/login
        [HttpGet]
        [Route("logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("ShowLogin");
        }

        // GET: /users/login
        [HttpGet]
        [Route("users/register")]
        public IActionResult ShowRegister(){
            return View("ShowRegister");
        }
        
        // POST: /login
        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginViewModel loginModel){
            
            int? isInSession = HttpContext.Session.GetInt32("user");
            if(isInSession == null)
            {
                if (ModelState.IsValid)
                {
                    User isInDB = _context.Users.Where(u => u.Username == loginModel.Username).SingleOrDefault();

                    if(isInDB == null){
                        ViewBag.error = "This Username isn't registered. Please register.";
                        return View("ShowRegister");
                    }
                    var Hasher = new PasswordHasher<User>();
                    
                    if(0 != Hasher.VerifyHashedPassword(isInDB, isInDB.Password, loginModel.Password))
                    {
                        HttpContext.Session.SetInt32("user", isInDB.UserId);
                        return RedirectToAction("Success");
                    }
                    ViewBag.error = "Your password is incorrect.";
                    return View("ShowLogin");
                    
                }
                return View("ShowLogin", loginModel);
            }
            return RedirectToAction("Success");
            
        }

        // POST: /register
        [HttpPost]
        [Route("register")]
        public IActionResult Register(RegisterViewModel regModel){
            int? isInSession = HttpContext.Session.GetInt32("user");
            if(isInSession == null)
            {
                if (ModelState.IsValid)
                {
                    var isInDB = _context.Users.Where(u => u.Username == regModel.Username).SingleOrDefault();

                    if(isInDB != null){
                        ViewBag.error = "This email is already registered. Please log in.";
                        return View("ShowLogin");
                    }
                    else{
                        PasswordHasher<RegisterViewModel> Hasher = new PasswordHasher<RegisterViewModel>();
                        User user = new User{
                            Name = regModel.Name,
                            Username = regModel.Username,
                            Email = regModel.Email,
                            Password = Hasher.HashPassword(regModel, regModel.Password),
                            ProfilePic = regModel.ProfilePic,
                            Bio = regModel.Bio,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };
                        _context.Users.Add(user);
                        _context.SaveChanges();
                        User newUser = _context.Users.Last();
                    
                        HttpContext.Session.SetInt32("user", newUser.UserId);
                        return RedirectToAction("Success");
                    }
                    
                }
                return View("ShowRegister", regModel);
            }
            return RedirectToAction("Success");
        }

        [HttpGetAttribute]
        [RouteAttribute("success")]
        public IActionResult Success()
        {
            int? isInSession = HttpContext.Session.GetInt32("user");
            System.Console.WriteLine(isInSession);
            if(isInSession == null){
                return RedirectToAction("ShowLogin");
            }
            return RedirectToAction("ShowUserGroups", "Group");
        }
    }
}
