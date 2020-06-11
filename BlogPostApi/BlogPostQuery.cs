using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace BlogPostApi
{
    public class BlogPostQuery
    {
        // Db property
        public AppDb Db { get; }
        // Class constructor, it receibes a database instance as a param
        public BlogPostQuery(AppDb db)
        {
            Db = db;
        }
        // Find post function
        public async Task<BlogPost> FindOneAsync(int id) 
        {
             // Create a database command, using var makes use of IDisposable which cleans up resources 
            // A Database connection is a resource which is aquired and then released.
            // What is a resource?: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/resource-management-the-use-keyword#resources
            // the `using` bindings calls Dispose() then the value goes our of scope.
            using var cmd = Db.Connection.CreateCommand();
             // Create query command
            cmd.CommandText = @"SELECT `id`, `Title`, `Content` FROM `BlogPost` WHERE `Id` = @id";
            // Add parameter to cmd
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            // Execute the async function ReadAllAsync and pass the Reader 
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            // Ternary Operator
            // If the count is greater than 0, return the first item in the result array(list)
            // else return null
            return result.Count > 0 ? result[0] : null;
        }
        // Async function for selecting the lastes 10 posts
        public async Task<List<BlogPost>> LatestPostAsync()
        {
             // Create a database command, using var makes use of IDisposable which cleans up resources 
            // A Database connection is a resource which is aquired and then released.
            // What is a resource?: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/resource-management-the-use-keyword#resources
            // the `using` bindings calls Dispose() then the value goes our of scope.
            using var cmd = Db.Connection.CreateCommand();
            // Create query command
            cmd.CommandText = @"SELECT `Id`, `Title`, `Content` FROM `BlogPost` ORDER BY `Id` DESC LIMIT 10";
            // Return the list of posts
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }
        public async Task DeleteAllAsync()
        {

            using var txn = await Db.Connection.BeginTransactionAsync();
             // Create a database command, using var makes use of IDisposable which cleans up resources 
            // A Database connection is a resource which is aquired and then released.
            // What is a resource?: https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/resource-management-the-use-keyword#resources
            // the `using` bindings calls Dispose() then the value goes our of scope.
            using var cmd = Db.Connection.CreateCommand();
            // Create query command
            cmd.CommandText = @"DELETE FROM `BlogPost`";
            // MySql transaction workaround: https://mysqlconnector.net/troubleshooting/transaction-usage/
            cmd.Transaction = txn;
            // Execute transaction and query
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }
        // Async function wich returns a List of Blogpost Objects, takes a datareader as a param
        private async Task<List<BlogPost>> ReadAllAsync(DbDataReader reader)
        {
            // List of Posts
            var posts = new List<BlogPost>();
            // Defines a disposable funcion block with the reader as a disposable object
            // https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/resource-management-the-use-keyword#using-function
            using (reader)
            {
                // While there are items to read
                while (await reader.ReadAsync())
                {
                    // Create a post object and read items from each column in the row
                    var post = new BlogPost(Db)
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Content = reader.GetString(2),
                    };
                    // Add a new post to the List
                    posts.Add(post);
                }
            }
            // Return the list of posts
            return posts;
        }
    }
}
