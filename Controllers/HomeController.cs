using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ActivityCenter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace ActivityCenter.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // "Inject" our context service into the constructor here
        public HomeController(MyContext context){
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index(){
            if (HttpContext.Session.GetString("Session") == null){
                return View("Index");
            }else{
                return RedirectToAction("Dashboard");
            }
        }
// ---------------------------------------------------------------------------------------------------------------------

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
// ---------------------------------------------------------------------------------------------------------------------
        [HttpPost("register")]
        public IActionResult register(User user){
            // Check initial ModelState
            if (ModelState.IsValid){
                // If a User exists with provided email
                if (dbContext.Users.Any(u => u.Email == user.Email)){
                    ModelState.AddModelError("Email", "This email already has an account!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserID", user.UserId);
                HttpContext.Session.SetString("Session", "True");
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }
// ---------------------------------------------------------------------------------------------------------------------

        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission){
            if (ModelState.IsValid){
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                // If no user has this email....
                if (userInDb == null){
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("Index");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // compare password with hashed database
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                
                if (result == 0){
                    ModelState.AddModelError("LoginPassword", "Invalid Password");
                    return View("Index");
                    
                }
                HttpContext.Session.SetInt32("UserID", userInDb.UserId);
                HttpContext.Session.SetString("Session", "True");
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }
// ---------------------------------------------------------------------------------------------------------------------

        [HttpGet("Dashboard")]
        public IActionResult Dashboard(){
            if (HttpContext.Session.GetString("Session") == null){
                return RedirectToAction("Index");
            }else{
                User newu = dbContext.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("UserID"));
                ViewBag.User = newu;

                ViewBag.Userid = HttpContext.Session.GetInt32("UserID");

                List<NewActivity> allAct = dbContext.NewActivitys
                .Include(c => c.ActtoUser)
                .ThenInclude(x => x.User)
                .OrderBy(f => f.ActivityDate)
                .ToList();
                ViewBag.AllAct = allAct;
                return View();
            }
        }
// ---------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        [Route("addactivity")]
        public IActionResult Newactivity(){
            if (HttpContext.Session.GetInt32("UserID") == null){
                return RedirectToAction("Index");
            }

            String clock = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.datetime = clock;

            User newu = dbContext.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("UserID"));
            ViewBag.User = newu;
            ViewBag.Firstname = newu.FirstName;
            ViewBag.Userid = HttpContext.Session.GetInt32("UserID");

            return View("NewActivity");
        }
// ---------------------------------------------------------------------------------------------------------------------

        [Route("btAddAct")]
        [HttpPost]
        public IActionResult btAddAct(NewActivity newAct){
            Console.WriteLine("<============== Activity Submitted (Start) ============>");
            Console.WriteLine(newAct.Title);
            Console.WriteLine(newAct.Time);
            Console.WriteLine(newAct.ActivityDate);
            Console.WriteLine(newAct.Duration);
            Console.WriteLine(newAct.Description);
            Console.WriteLine(newAct.UserId);
            Console.WriteLine("<============== Activity Submitted (End) ============>");


            if (ModelState.IsValid){
                if (Request.Form["dura"] == "minutes"){
                    newAct.Duration = newAct.Duration;
                }
                if (Request.Form["dura"] == "hours"){
                    newAct.Duration = newAct.Duration * 60;
                }
                if (Request.Form["dura"] == "days"){
                    newAct.Duration = newAct.Duration * 1440;
                }

                newAct.Created_at = DateTime.Now;
                newAct.Updated_at = DateTime.Now;
                Console.WriteLine("<--------- BUTTON MASH 2 ----------->");
                dbContext.Add(newAct);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }else{
                Console.WriteLine("<--------- BUTTON MASH 3 -----------> ");
                return View("NewActivity");
            }
        }
// ---------------------------------------------------------------------------------------------------------------------

        [Route("detail/{id}")]
        [HttpGet]
        public IActionResult DetailAct(int id){
            NewActivity a = dbContext.NewActivitys
            .FirstOrDefault(pro => pro.ActivityId == id);
            ViewBag.ActDetail = a;
            NewActivity getnewact = dbContext.NewActivitys.Include(c => c.ActtoUser).ThenInclude(b => b.User).FirstOrDefault(c => c.ActivityId == id);
            ViewBag.act = getnewact;
            ViewBag.Userid = HttpContext.Session.GetInt32("UserID");
            int num = getnewact.UserId;
            ViewBag.theuser = dbContext.Users.SingleOrDefault(u => u.UserId == num);
            return View("ActivityDetail");
        }
// ---------------------------------------------------------------------------------------------------------------------

        [Route("joinact/{actid}/{userid}")]
        [HttpGet]
        public IActionResult joinact(int actid, int userid){
            NewActivity newAct = dbContext.NewActivitys.Include(c => c.ActtoUser).ThenInclude(b => b.User).FirstOrDefault(wed => wed.ActivityId == actid);
            User newUser = dbContext.Users.Include(c => c.UsertoAct).ThenInclude(b => b.Activity).FirstOrDefault(us => us.UserId == userid);
            foreach (var currentactivity in newUser.UsertoAct){
                if (currentactivity.Activity.ActivityDate.Date == newAct.ActivityDate.Date){
                    ModelState.AddModelError("ActivityDate", "You gotta pick 1 activity at a time bro.");
                    ViewBag.samedayrs = "You only can attend 1 event";
                    return RedirectToAction("Dashboard");
                }
            }
            ActConnect acjoin = new ActConnect();
            acjoin.ActivityId = actid;
            acjoin.UserId = userid;
            dbContext.Add(acjoin);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
// ---------------------------------------------------------------------------------------------------------------------

        [Route("leaveact/{id}")]
        [HttpGet]
        public IActionResult leaveact(int id){
            ActConnect a = dbContext.ActConnects.FirstOrDefault(web => web.ActConnectId == id);
            dbContext.Remove(a);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
// ---------------------------------------------------------------------------------------------------------------------

        [Route("delete/{actid}")]
        [HttpGet]
        public IActionResult delete(int actid){
            NewActivity a = dbContext.NewActivitys.FirstOrDefault(web => web.ActivityId == actid);
            dbContext.Remove(a);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}