using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using COMPUTINGNEA.Models;
using System.Web;
using System.Net;
using System.Net.Http;

namespace COMPUTINGNEA.Controllers
{
    public class HomeController : Controller
    {
        // creating objects of each class in order to store data from the forms
        User user = new User();
        Investment investment = new Investment();
        NeuralNetWork nn = new NeuralNetWork();

        // declaring the Home Controller constructor
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
         // returns the html for the login page
        public IActionResult Index()
        {
            return View();
        }

        // returns the html for the registration page
        public IActionResult Registration()
        {
            return View();
        }

        // returns the html for the home page
        public IActionResult Home()
        {
            return View();
        }

        // returns the html for the add investment page
        public IActionResult AddInvestment()
        {
            return View();
        }

        // returns the html for the view investments page
        public IActionResult ViewInvestments()
        {
            return View();
        }

        // returns the html for the investment not found error message
        public IActionResult InvestmentNotFoundError()
        {
            return View();
        }

        // returns the html for the AddCustomMG page
        public IActionResult AddCustomMG()
        {
            return View();
        }

        // returns the html for the UpdateMG page
        public IActionResult UpdateMG()
        {
            return View();
        }

        // returns the html for the update investment page
        public IActionResult UpdateInvestment()
        {
            return View();
        }

        // returns the html for the Account page  
        public IActionResult Account()
        {
            return View();
        }

        // returns the html for the About page    
        public IActionResult About()
        {
            return View();
        }

        // returns the html for the login page
        public IActionResult Logout()
        {
            return View("Home");
        }

        // retrieves username and password from login page input form
        // validates logins
        [HttpPost]
        public IActionResult Login()
        {
            user.Username = HttpContext.Request.Form["Username"];
            user.Userpassword = HttpContext.Request.Form["Userpassword"];

                // de-hashes password
                int result = user.GetPassword();
                // checks the user already exists in the database
                int result2 = user.CheckUserExists();

                // successful login
                if (result == 1 && result2 > 0)
                {
                    return View("Home");
                } // user exists but password is incorrect
                else if (result <= 0 && result2 > 0)
                {
                    return View("IncorrectPasswordError");
                } // user doesn't exist
                else
                {
                    return View("LoginError");
                }
            } // any other error redirects user to the login page again
            
        // retrieves the search input from the search bar
        [HttpPost]
        public IActionResult Search()
        {
            // creates new instance of Search class
            Search s = new Search();
            s.SearchInput = HttpContext.Request.Form["SearchInput"];
            // stores result in an array
            string[] matches = (s.SearchInvestment(s.SearchInput)).ToArray();
            try
            {
                // checks if investment exists in database
                int result = s.CheckInvestmentExists(s.SearchInput);
                // returns 0 if no investments are found
                if (result == 0)
                {
                    return View("InvestmentNotFoundError");
                }
                // redirects to display page if investment(s) are found
                if (result > 0)
                {
                    return View("Search");
                }
                return View("ViewInvestments");
            } // any other error redirects the user back to the search page
            catch
            {
                return View("ViewInvestments");
            }
        }

        // retrieves updated investment details from input form
        [HttpPost]
        IActionResult UpdateInvestmentDetails()
        { 
            // method acts the same as adding an investment, a new return on the investment is calculated
            investment.InvestmentName = HttpContext.Request.Form["InvestmentName"];
            int result = investment.UpdateInvestmentDetails();
          
            investment.InvestmentDate = HttpContext.Request.Form["InvestmentDate"];
            investment.Industry = HttpContext.Request.Form["Industry"];
            investment.AmountInvested = float.Parse(HttpContext.Request.Form["AmountInvested"]);


            nn.InvestmentName = HttpContext.Request.Form["InvestmentName"];
            nn.AmountInvested = float.Parse(HttpContext.Request.Form["AmountInvested"]);
            nn.Revenue = float.Parse(HttpContext.Request.Form["Revenue"]);
            nn.Profit = float.Parse(HttpContext.Request.Form["Profit"]);
            nn.Industry = HttpContext.Request.Form["Industry"];
           
            nn.Calculation();
            int result3 = nn.UpdateInvestmentDetails();
            int result2 = nn.SaveDetails();

            // redirects back to UpdateInvestment page if an error occurs
            return View("Home");
        }

        // retrieves user details from input form and validates for registration
        [HttpPost]
        public IActionResult GetUserDetails()
        {
            // Retrieve user input from forms and assign to variables
            user.FirstName = HttpContext.Request.Form["FirstName"];
            user.LastName = HttpContext.Request.Form["LastName"];
            user.Email = HttpContext.Request.Form["Email"];
            user.Username = HttpContext.Request.Form["Username"];
            user.Userpassword = HttpContext.Request.Form["Userpassword"];
            string confirmationPassword = HttpContext.Request.Form["Confirmation"];

            // checks if user already exists in database
            try
            {
                int result = user.CheckUserExists();
                // returns 0 if no users are found
                if (result > 0)
                {
                    return View("RegistrationError");
                }
            }
            catch
            {
                return View("Index");
            }

            // check if password entered matches confirmed password entered
            if (user.Userpassword != confirmationPassword)
            {
                return View("ConfirmationError");
            }

            // attempts to save user details into database
            try
            {
                int result2 = user.SaveDetails();
                return View("Home");
            }
            catch
            {
                return View("Index");
            }
        }

        // retrieves investment details from form to be stored and used in calculation
        [HttpPost]
        public IActionResult GetInvestmentDetails()
        {
            // variables retreived from page
            investment.InvestmentName = HttpContext.Request.Form["InvestmentName"];
            investment.InvestmentDate = HttpContext.Request.Form["InvestmentDate"];
            investment.Industry = HttpContext.Request.Form["Industry"];
            investment.AmountInvested = float.Parse(HttpContext.Request.Form["AmountInvested"]);

            nn.InvestmentName = HttpContext.Request.Form["InvestmentName"];
            nn.AmountInvested = float.Parse(HttpContext.Request.Form["AmountInvested"]);
            nn.Revenue = float.Parse(HttpContext.Request.Form["Revenue"]);
            nn.Profit = float.Parse(HttpContext.Request.Form["Profit"]);
            nn.Industry = HttpContext.Request.Form["Industry"];
            int result = investment.SaveDetails();
            nn.Calculation();
            int result3 = nn.UpdateInvestmentDetails();
            int result2 = nn.SaveDetails();
            
            // redirects to home page upon successful adding of an investment
            return View("Home");
        }

        // retrieves the new industry and industry market growth from the input form
        [HttpPost]
        public IActionResult AddCustomMarketGrowth()
        {
            investment.Industry = HttpContext.Request.Form["CustomIndustry"];
            try
            {
                // validates input is of correct type and between 0 and 100
                investment.IndustryMarketGrowth = float.Parse(HttpContext.Request.Form["CustomMarketGrowth"]);
                if (investment.IndustryMarketGrowth < 0 || investment.IndustryMarketGrowth > 100)
                {
                    return View("AddCustomMG");
                }
                int result = investment.AddCustomGrowth();
                return View("AddInvestment"); // redirects back to AddInvestment page if successful
            }
            catch
            {
                return View("AddCustomMG"); // redirects back to AddCustomMG page if an error occurs
            }
        }

        // retrieves the industry market growth for a selected industry from the input form
        [HttpPost]
        public IActionResult UpdateMarketGrowth()
        {
            investment.Industry = HttpContext.Request.Form["Industry"];
            try
            {
                // validates input is of correct type and between 0 and 100
                investment.IndustryMarketGrowth = float.Parse(HttpContext.Request.Form["CustomMarketGrowth"]);
                if (investment.IndustryMarketGrowth < 0 || investment.IndustryMarketGrowth > 100)
                {
                    return View("UpdateMG");
                }
                int result = investment.UpdateGrowth();
                return View("AddInvestment"); // redirects back to AddInvestment page if successful
            }
                catch
                {
                    return View("UpdateMG"); // redirects back to UpdateMG page if an error occurs
                }
            }

            // checks for errors by creating an instance of the ErrorViewModel class
            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
