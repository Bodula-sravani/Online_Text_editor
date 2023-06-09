﻿using Microsoft.AspNetCore.Http;
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
		public Users getUser(string userId)
		{
            // Get user details by using userid 
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
                    Console.WriteLine("result: " + reader["documentId"] == null);
					Console.WriteLine("result2: " + reader["documentId"]);
                    user.documentId = (int)reader["documentId"];

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
            // Get the document using document id
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
            // Gets the user details to display their name on the page and some features
			Console.WriteLine("user id in user page method: " + userId);
			Users currentUser = getUser(userId);
            Console.WriteLine("user doc id : " + currentUser.documentId);
            if (currentUser.documentId == -1)
            {
                // By default doucumentId of user is -1 if he is not assifned to any doc
                Console.WriteLine("in if cond");
                ViewBag.documentList = null;
            }
            else
            { 
                // View bag to store that user doc details and dislay on html page
                ViewBag.documentList = getDocuments(currentUser.documentId); 
            }
			Console.WriteLine("user Name: " + currentUser.Name);
     
			return View(currentUser);
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
            // Inserts a document into document table
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
                // Associates that user to that document
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
            // Createas a doc and redirects to userPage
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

        public ActionResult Exist(string userId)
        {
            // user adds already existing doc and modify it in future
            
            Console.WriteLine("exist method: " + userId);
            ViewBag.userId = userId;
            return View();
        }

        public void addExistingDoc(int id,string userId)
        {
			try
			{
				connection.Open();
				SqlCommand command = new SqlCommand("updateDoc", connection);
				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.Parameters.AddWithValue("@id", userId);
				command.Parameters.AddWithValue("@docid", id);
				command.ExecuteNonQuery();
				connection.Close();
			}
			catch (SqlException ex)
			{
				Console.WriteLine("error: " + ex.Message);
			}
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Exist(int id, string userId)
        {
            // Adds a already exixtsing doc to that user using doc id
			Console.WriteLine("in Exist post method");
			Console.WriteLine("user id" + userId);
			Console.WriteLine("doc id" + id);
			try
            {
                addExistingDoc(id, userId);
                return RedirectToAction("userPage", new { userId = userId });
            }
            catch
            {
                return View();
            }
        }
		// GET: UserController/Edit/5
		public ActionResult Edit(int id,string userid)
        {
            Console.WriteLine("in edit method: ");
            Console.WriteLine("user id: "+userid);
            Console.WriteLine("doc id: " + id);
            ViewBag.userId = userid;
            return View(getDocuments(id));
        }
        public void updateDocument(Document d)
        {
            // Updates the document and sets upated date to today 
            Console.WriteLine("entered update document method");
            try
            {
                connection.Open();
                SqlCommand command = new SqlCommand("updateDocument", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id", d.id);
                command.Parameters.AddWithValue("@content", d.content);
                command.Parameters.AddWithValue("@updatedDate", DateTime.Now);
                command.Parameters.AddWithValue("@name", d.name);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("error: " + ex.Message);
            }

        }
        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,string userId,Document d)
        {
            try
            {
                Console.WriteLine("inside edit ost method");
                Console.WriteLine("doc id: " + id);
                Console.WriteLine("doc id using doc object: " + d.id);
                Console.WriteLine("user id from viee bag" + userId);
                updateDocument(d);
                return RedirectToAction("userPage", new { userId = userId });
            }
            catch
            {
                return View();
            }
        }


        // GET: UserController/Details/5
        public ActionResult Details(int id,string userId)
        {
            // To view the details of the document
            ViewBag.userId = userId;    
            return View(getDocuments(id));
        }


        // GET: UserController/Delete/5
        public ActionResult Delete(int id,string userId)
        {
            
            ViewBag.userId = userId;
            return View();
        }

        public void deleteDocument(int id)
        {
            // Delete docs and unlinks it with all users
            try
            {
                connection.Open();
                string query = $"delete from documents where Id= @{id}";
                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch(SqlException e)
            {
                throw;
            }
            try
            {
                connection.Open();
                string query = $"update users set documentId= {-1} where documentId = {id}";
                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (SqlException e)
            {
                throw;
            }
  
        }
        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, string userId,Document d)
        {
            try
            {
                deleteDocument(id);
                return RedirectToAction("userPage", new { userId = userId });
            }
            catch
            {
                return View();
            }
        }
    }
}
