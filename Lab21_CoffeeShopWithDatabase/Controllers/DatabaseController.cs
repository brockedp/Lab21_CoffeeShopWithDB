using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lab21_CoffeeShopWithDatabase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Lab21_CoffeeShopWithDatabase.Controllers
{
    public class DatabaseController : Controller
    {
        Member loginUser = new Member();

        private readonly CoffeeshopDbContext _context;

        public DatabaseController(CoffeeshopDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Register()
        {
           
            return View();
        }
        public IActionResult MakeNewMember(Member newMember)
        {
            if (ModelState.IsValid)
            {
                _context.Member.Add(newMember);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Register", newMember);
            }
            
            
        }
        public IActionResult Shop()
        {
            string LoginUserJson = HttpContext.Session.GetString("LoginUserSession");
            if(LoginUserJson != null)
            {
                loginUser = JsonConvert.DeserializeObject<Member>(LoginUserJson);
                ViewBag.Login = loginUser.FirstName;


            }

            List<Item> itemList = _context.Item.ToList();
            return View(itemList);
        }

        public IActionResult LoginUser()
        {

            return View();
        }

        [HttpPost]
        public IActionResult CheckUser(string UserName, string Password)
        {
            List<Member> memberList = _context.Member.ToList();

            foreach (var item in memberList)
            {
                if ((item.UserName == UserName) && (item.Password == Password))
                {

                    string LoginUserJson = HttpContext.Session.GetString("LoginUserSession");
                    loginUser = item;

                    HttpContext.Session.SetString("LoginUserSession", JsonConvert.SerializeObject(loginUser));
                    

                }

            }
            return RedirectToAction("Index");

        }


        public IActionResult Buy(int Id)
        {
            int i = 0;
            if (ModelState.IsValid)
            {
                Item buyItem = _context.Item.Find(Id);
                if(buyItem != null)
                {
                    string LoginUserJson = HttpContext.Session.GetString("LoginUserSession");
                    loginUser = JsonConvert.DeserializeObject<Member>(LoginUserJson);
                    if (buyItem.Price < loginUser.Funds)
                    {
                        i = 1;
                    }
                    
                }
            }
            if (i == 1)
            {
                return View("Yay");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}