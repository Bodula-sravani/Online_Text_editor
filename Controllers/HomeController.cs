using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using Text_Editor.Models;

namespace Text_Editor.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        IConfiguration configuration;
        public SqlConnection connection;
        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connection = new SqlConnection(configuration.GetConnectionString("DB"));

        }

        public IActionResult Index()
        {
            Console.WriteLine("entered index method");
            return View();
        }

        public bool validateUser(string userId, string password)
        {
            Console.WriteLine("entered validateUser method");
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("getUser", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userid", userId);
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("reader excecuted");
                string getPassword = "";
                while (reader.Read())
                {
                    Console.WriteLine("inside whike");
                    getPassword = (string)reader["userPassword"];
                }
                //Console.WriteLine("reader value: " + (string)reader["userPassword"]);
                Console.WriteLine("passsowrds:  " + getPassword + " " + password);
                if (password.Equals(getPassword))
                {
                    Console.WriteLine("paswoord are equal");
                    return true;
                }
                reader.Close();
                connection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("error: " + ex.Message);
            }

            Console.WriteLine("passwords not equal exit");
            return false;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Index(string userid, string password, Users user)
        {
            try
            {
                Console.WriteLine("in post index: ");
                Console.WriteLine("userid: from string " + userid);
                Console.WriteLine("password: " + password);
                Console.WriteLine("user.id: " + user.userId);
                if (validateUser(userid, password))
                {
                    //Console.WriteLine("validaed user");
                    //Console.WriteLine("userid in index page "+user.userId);
                    //               Console.WriteLine("userid: from string " + userid);
                    return RedirectToAction("userPage", "User", new { userId = userid });
                }
                else
                {
                    Console.WriteLine("paswword mismatch");
                    TempData["ErrorMessage"] = "Invalid userid or password";
                    return View();
                }

            }
            catch
            {
                return View();
            }
        }


        // GET: UserController/Register new user
        public ActionResult Register()
        {
            return View();
        }
        public void insertUser(Users user)
        {
            Console.WriteLine("inside insert user");
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("insertUser", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@userid", user.userId);
                command.Parameters.AddWithValue("@username", user.Name);
                command.Parameters.AddWithValue("@useremail", user.Email);
                command.Parameters.AddWithValue("@userpassword", user.Password);
                command.Parameters.AddWithValue("@userphonenumber", user.phoneNumber);

                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException e)
            {
                Console.WriteLine("error: " + e.Message);
                throw;
            }
            Console.WriteLine(user.phoneNumber);
            Console.WriteLine("exiting insert user");
        }
        // POST: UserController/Register new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Users user)
        {
            try
            {
                //Console.WriteLine("phone nmber: "+user.phoneNumber);
                insertUser(user);
                //Console.WriteLine("compeleted insert");
                return RedirectToAction("Index", "Home");
            }
            catch (SqlException e)
            {
                TempData["ErrorMessage"] = "userid exists";
                return View();
            }
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
    }
}