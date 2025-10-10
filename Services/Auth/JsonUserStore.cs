using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace AuctionPoc.Services.Auth
{
    public class JsonUserStore : IUserStore
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private List<UserRecord> _users = new();

        public JsonUserStore(IWebHostEnvironment env)
        {
            var dataDir = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(dataDir);
            _filePath = Path.Combine(dataDir, "users.json");
            if (File.Exists(_filePath))
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    var list = JsonSerializer.Deserialize<List<UserRecord>>(json);
                    if (list != null) _users = list;
                }
                catch
                {
                }
            }
        }

        public Task<UserRecord?> GetByEmailAsync(string email) =>
            Task.FromResult(_users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));

        public Task<IEnumerable<UserRecord>> AllAsync() => Task.FromResult<IEnumerable<UserRecord>>(_users);

        public Task<bool> AddAsync(UserRecord user)
        {
            lock (_lock)
            {
                if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                    return Task.FromResult(false);
                _users.Add(user);
                Save();
                return Task.FromResult(true);
            }
        }

        public Task<bool> ValidateCredentialsAsync(string email, string password)
        {
            var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (user is null) return Task.FromResult(false);
            return Task.FromResult(user.PasswordHash == Hash(password));
        }

        private void Save()
        {
            try
            {
                var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            catch
            {
            }
        }

        public static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }
    }
}
