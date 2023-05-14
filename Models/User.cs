using System;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace COMPUTINGNEA.Models
{
    public class User
    {
        // defining User variables
        private int userid;
        public int UserID
        {
            get { return userid; }
            set { userid = value; }
        }

        private string firstname;
        public string FirstName
        {
            get { return firstname; }
            set { firstname = value; }
        }

        private string lastname;
        public string LastName
        {
            get { return lastname; }
            set { lastname = value; }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string userpassword;
        public string Userpassword
        {
            get { return userpassword; }
            set { userpassword = value; }
        }

        // Checks if email address or username entered is already in database
        // using a select query to the User table
        public int CheckUserExists()
        {
            using (SqlConnection con = new SqlConnection(Constring))
            {
                string checkUser = "SELECT COUNT(*) FROM [dbo].[User] WHERE Email = @Email OR Username = @Username";

                using (SqlCommand cmd = new SqlCommand(checkUser, con))
                {
                 //   cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Username", username);
                    con.Open();
                    // returns 0 if no users are found
                    int UserExists = (int)cmd.ExecuteScalar();
                    return UserExists;
                }
            }
        }

        // Saves user details into User table upon successful registration
        public virtual int SaveDetails()
        {
            // Calls method to hash password and stores in variable
            var hashcode = GetHashCode(Userpassword);

            using (SqlConnection con = new SqlConnection(Constring))
            {
                // Insert query to insert user details into User table
                string insertUserDetails = "INSERT INTO [dbo].[User](FirstName, LastName, Email, Username, Userpassword) VALUES (@FirstName,@LastName,@Email,@Username,@hashcode)";

                using (SqlCommand cmd = new SqlCommand(insertUserDetails, con))
                {
                    // parameterised variables
                    cmd.Parameters.AddWithValue("@FirstName", firstname);
                    cmd.Parameters.AddWithValue("@LastName", lastname);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@hashcode", hashcode);
                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    return result;
                }
            }
        }
       
        // Hashes the password upon registration
        public string GetHashCode(string password)
        {
            // creating the hash
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(Userpassword, salt, 100000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        // De-hases the hashed password when user attempts to login
        public int GetPassword()
        {
            using (SqlConnection con = new SqlConnection(Constring))
            {

                // SELECT query to retreive the stored hashed password 
                string savedPasswordHash = "SELECT Userpassword FROM [dbo].[User] WHERE Username = @Username";

                using (SqlCommand cmd = new SqlCommand(savedPasswordHash, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);

                    con.Open();
                    // stores the hashed password in a string variable
                    string passwordHash = (string)cmd.ExecuteScalar();

                    //  Extract the bytes 
                    byte[] hashBytes = Convert.FromBase64String(passwordHash);
                    // Get the salt 
                    byte[] salt = new byte[16];
                    Array.Copy(hashBytes, 0, salt, 0, 16);
                    // Compute the hash on the password the user entered 
                    var pbkdf2 = new Rfc2898DeriveBytes(Userpassword, salt, 100000);
                    byte[] hash = pbkdf2.GetBytes(20);
                    // Compare the results 
                    for (int i = 0; i < 20; i++)
                    {
                        if (hashBytes[i + 16] != hash[i])
                        {
                            return 0; // prevents unauthorised access
                        }
                    }
                    return 1; // access granted
                }
            }
        }

        // retrieves user id from the User table using SELECT query
        public int GetUserID()
        {
            using (SqlConnection con = new SqlConnection(Constring))
            {
                string userID = "SELECT UserID FROM [dbo].[User] WHERE Username = @Username";

                using (SqlCommand cmd = new SqlCommand(userID, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    con.Open();
                    int Userid = (int)cmd.ExecuteScalar();
                    return Userid;
                }
            }
        }
    }
}
