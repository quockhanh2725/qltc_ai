namespace qltc_ai.Service.Base.AI
{
    public class ScanTokenService : IScanTokenService
    {
        private const int TOKEN_LIFE_MIN = 30;

        private readonly Dictionary<string, (DateTime Exp, int AccId)> _tokens = new();
        private readonly HashSet<string> _done = new();
        private readonly object _lock = new();

        public string GenerateToken(int accountId)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                               .Replace("+", "-").Replace("/", "_").TrimEnd('=');

            lock (_lock)
            {
                var expired = _tokens
                    .Where(kv => kv.Value.Exp < DateTime.UtcNow)
                    .Select(kv => kv.Key)
                    .ToList();
                foreach (var k in expired) _tokens.Remove(k);

                _tokens[token] = (DateTime.UtcNow.AddMinutes(TOKEN_LIFE_MIN), accountId);
            }

            return token;
        }

        public string GetOrCreateToken(int accountId)
        {
            lock (_lock)
            {
                // Trả token cũ còn hạn nếu có
                var existing = _tokens
                    .FirstOrDefault(kv => kv.Value.AccId == accountId && kv.Value.Exp > DateTime.UtcNow);
                if (existing.Key != null) return existing.Key;

                // Xóa token hết hạn
                var expired = _tokens
                    .Where(kv => kv.Value.Exp < DateTime.UtcNow)
                    .Select(kv => kv.Key).ToList();
                foreach (var k in expired) _tokens.Remove(k);

                // Tạo token mới
                var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                                   .Replace("+", "-").Replace("/", "_").TrimEnd('=');
                _tokens[token] = (DateTime.UtcNow.AddMinutes(TOKEN_LIFE_MIN), accountId);
                return token;
            }
        }

        public bool IsValid(string? token)
            => GetAccountId(token) != null;

        public int? GetAccountId(string? token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;
            lock (_lock)
                return _tokens.TryGetValue(token, out var v) && v.Exp > DateTime.UtcNow
                    ? v.AccId
                    : null;
        }

        public bool TryConsume(string token)
        {
            lock (_lock)
            {
                if (!_tokens.TryGetValue(token, out var v) || v.Exp <= DateTime.UtcNow)
                    return false;
                _tokens.Remove(token);
                _done.Add(token);
                return true;
            }
        }

        public void MarkDone(string token)
        {
            lock (_lock)
            {
                _done.Add(token);
                _tokens.Remove(token);
            }
        }

        public bool IsDone(string token)
        {
            lock (_lock)
            {
                if (!_done.Contains(token)) return false;
                _done.Remove(token);   
                return true;
            }
        }
    }
}