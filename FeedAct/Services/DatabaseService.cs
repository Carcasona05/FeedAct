using SQLite;
using FeedAct.Entities;
using System.Diagnostics;

namespace FeedAct.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _db;

        public DatabaseService()
        {
            var dbPath = ResolveDbPath();
            Debug.WriteLine($"DB Path: {dbPath}");

            _db = new SQLiteAsyncConnection(dbPath);

            Task.Run(async () =>
            {
                await _db.CreateTableAsync<Post>();
                await _db.CreateTableAsync<Comment>();
            });
        }
        private static string ResolveDbPath()
        {
            try
            {
                var dir = new DirectoryInfo(AppContext.BaseDirectory);

                while (dir != null)
                {
                    if (dir.GetFiles("*.csproj").Any())
                    {
                        var dbFolder = Path.Combine(dir.FullName, "Database");
                        Directory.CreateDirectory(dbFolder);
                        return Path.Combine(dbFolder, "FeedAct.db3");
                    }
                    dir = dir.Parent;
                }
            }
            catch
            {
                // fallback ignored
            }

            return Path.Combine(FileSystem.AppDataDirectory, "FeedAct.db3");
        }

        // ---------------- POSTS ----------------
        public Task<List<Post>> GetPostsAsync()
            => _db.Table<Post>()
                  .OrderByDescending(p => p.CreatedAt)
                  .ToListAsync();

        public Task<int> SavePostAsync(Post post)
     => post.Id == 0
         ? _db.InsertAsync(post)  
         : _db.UpdateAsync(post); 

        public Task<int> DeletePostAsync(Post post)
            => _db.DeleteAsync(post);

        // ---------------- COMMENTS ----------------

        
        public Task<List<Comment>> GetCommentsAsync(int postId)
            => _db.Table<Comment>()
                  .Where(c => c.PostId == postId)
                  .ToListAsync();

        public Task<int> SaveCommentAsync(Comment comment)
            => _db.InsertAsync(comment);
    }
}