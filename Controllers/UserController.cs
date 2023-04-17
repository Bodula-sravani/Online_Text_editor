using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Text_Editor.Models;

namespace Text_Editor.Controllers
{
    public class UserController : Controller
    {

		IConfiguration configuration;
		public SqlConnection connection;
		public UserController(IConfiguration configuration)
		{
			this.configuration = configuration;
			this.connection = new SqlConnection(configuration.GetConnectionString("DB"));

		}
		// GET: UserController
		public ActionResult Index()
        {
            return View();
        }
		public Users getUser(string userId)
		{
			Console.WriteLine("entered getUser method");
			Users user = new Users();
			try
			{
				connection.Open();
				SqlCommand command = new SqlCommand("getUser", connection);
				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@userid", userId);
				SqlDataReader reader = command.ExecuteReader();
				Console.WriteLine("reader excecuted");
				while (reader.Read())
				{
					user.Password = (string)reader["userPassword"];
					user.phoneNumber = (string)reader["userPhonenumber"];
					user.userId = (string)reader["userId"];
					user.Email = (string)reader["userEmail"];
					user.Name = (string)reader["userName"];
                    if (reader["documentId"] == null)
                    {
                        user.documentId = -1;
                    }
                    else
                    {
                        user.documentId = (int)reader["documentId"];
                    }

                }
				reader.Close();
				connection.Close();

			}
			catch (SqlException ex)
			{
				Console.WriteLine("error: " + ex.Message);
			}
			return user;
		}

        public Document getDocuments(int id)
        {
			Console.WriteLine("entered get document method");
			Document doc = new Document();
			try
			{
				connection.Open();
				SqlCommand command = new SqlCommand("getDocument", connection);
				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@id", id);
				SqlDataReader reader = command.ExecuteReader();
				Console.WriteLine("reader excecuted");
				while (reader.Read())
				{
					doc.id = (int)reader["Id"];
					doc.name = (string)reader["name"];
					doc.content = (string)reader["content"];
					doc.createdDate = (DateTime)reader["createdDate"];
					doc.updatedDate = (DateTime)reader["UpdatedDate"];
				}
				reader.Close();
				connection.Close();

			}
			catch (SqlException ex)
			{
				Console.WriteLine("error: " + ex.Message);
			}
			return doc;

		}
		public ActionResult userPage(string userId)
		{
			Console.WriteLine("user id in user page method: " + userId);
			Users currentUser = getUser(userId);
            Console.WriteLine("user doc id : " + currentUser.documentId);
            if (currentUser.documentId == -1)
            {
                Console.WriteLine("in if cond");
                ViewBag.documentList = null;
            }
            else
            { 
                ViewBag.documentList = getDocuments(currentUser.documentId); 
            }
			Console.WriteLine("user Name: " + currentUser.Name);

			return View(currentUser);
		}

		// GET: UserController/Details/5
		public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UserController/Create
        public ActionResult Create(string userId)
        {
			Console.WriteLine("in create method user id: " + userId);
			ViewBag.userId = userId;
            return View();
        }

		public void insertDocument(string userId,Document d)
		{
            Console.WriteLine("entered insert document  method");
            Console.WriteLine("user id" + userId);
            Console.WriteLine("doc id" + d.id);
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("insertDocument", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", d.id);
                command.Parameters.AddWithValue("@name", d.name);
                command.Parameters.AddWithValue("@content", d.content);
                command.Parameters.AddWithValue("@createdDate", d.createdDate);
                command.Parameters.AddWithValue("@updatedDate", d.updatedDate);
                SqlDataReader reader = command.ExecuteReader();
                connection.Close();

            }
            catch (SqlException ex)
            {
                Console.WriteLine("error: " + ex.Message);
            }

            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("updateDoc", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", userId);
                command.Parameters.AddWithValue("@docid", d.id);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch(SqlException ex)
            {
                Console.WriteLine("error: " + ex.Message);
            }
        }
        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string userId,Document d)
        {
            try
            {
                Console.WriteLine("in create post method");
                Console.WriteLine("user id" + userId);
                Console.WriteLine("doc id" + d.id);
                insertDocument(userId,d);
                return RedirectToAction("userPage",new {userId = userId});
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
