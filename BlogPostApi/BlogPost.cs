using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace BlogPostApi
{
    public class BlogPost
    {
        // Properties
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        // DataBase Property
        internal AppDb Db { get; set; }
        // Constructors
        public BlogPost() { 
        }
        internal BlogPost(AppDb db) {
            Db = db;
        }
        // Async function for inserting a Post
        public async Task InsertAsync() {
            // Create a database command, using var makes use of IDisposable which cleans up resources 
            // A Database connection is a resource which is aquired and then released.
            // What is a resource?: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/resource-management-the-use-keyword#resources
            // the `using` bindings calls Dispose() then the value goes our of scope.
            using var cmd = Db.Connection.CreateCommand();
            // Create query command
            cmd.CommandText = @"INSERT INTO `BlogPost` (`Title`, `Content`) VALUES (@title, @content)";
            // Invoke BindParams function and pass the db command
            BindParams(cmd);
            // Execute query
            await cmd.ExecuteNonQueryAsync();
            // Gets the id from the inserted value
            Id = (int)cmd.LastInsertedId;
            // cmd.Dispose() is called implicityly here
        }
        // Async function for updating a Post
        public async Task UpdateAsync()
        {
            // Create a database command, using var makes use of IDisposable which cleans up resources 
            // A Database connection is a resource which is aquired and then released.
            // What is a resource?: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/resource-management-the-use-keyword#resources
            // the `using` bindings calls Dispose() then the value goes our of scope.
            using var cmd = Db.Connection.CreateCommand();
             // Create query command
            cmd.CommandText = @"UPDATE `BlogPost` SET `Title` = @title , `Content` =  @content WHERE `Id` = @id";
            // Invoke BindParams function and pass the db command
            BindParams(cmd);
            // Invoke BindId function and pass the db command
            BindId(cmd);
            // Execute query
            await cmd.ExecuteNonQueryAsync();
            // Gets the id from the updated value
            Id = (int)cmd.LastInsertedId;
            // cmd.Dispose() is called implicityly here
        }
        // Async function for deleting a Post
        public async Task DeleteAsync()
        {
            // Create a database command, using var makes use of IDisposable which cleans up resources 
            // A Database connection is a resource which is aquired and then released.
            // What is a resource?: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/resource-management-the-use-keyword#resources
            // the `using` bindings calls Dispose() then the value goes our of scope.
            using var cmd = Db.Connection.CreateCommand();
            // Create query command
            cmd.CommandText = @"DELETE FROM `Blogpost` WHERE `Id` = @id";
             // Invoke BindId function and pass the db command
            BindId(cmd);
            // Execute query
            await cmd.ExecuteNonQueryAsync();
        }
        // Function for adding id parameter into the command
        private void BindId(MySqlCommand cmd)
        {
            // Add a id parameter
            cmd.Parameters.Add(new MySqlParameter
            { 
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = Id,
            });
        }
        // Function for adding parameters into the command
        private void BindParams(MySqlCommand cmd) 
        {
            // Adds a title parameter
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@title",
                DbType = DbType.String,
                Value = Title,
            });
            // Adds a content parameter
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@content",
                DbType = DbType.String,
                Value = Content,
            });
        }
    }
}
