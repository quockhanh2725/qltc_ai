using System.Text.RegularExpressions;

namespace qltc_ai.Service.Base.AI
{
    public class ChatParserService
    {
        public (string note, decimal money) Parse(string message)
        {
            message = message.ToLower();

            decimal money = ExtractMoney(message);

            string note = CleanNote(message);

            return (note, money);
        }

        private decimal ExtractMoney(string text)
        {
            text = text.ToLower();

            
            var trMatch = Regex.Match(text, @"(\d+)(?:[.,](\d+))?\s*(tr|m)\b");
            if (trMatch.Success)
            {
                decimal main = decimal.Parse(trMatch.Groups[1].Value);
                decimal sub = 0;
                if (trMatch.Groups[2].Success)
                    sub = decimal.Parse("0." + trMatch.Groups[2].Value);
                return (main + sub) * 1_000_000;
            }

            var kMatch = Regex.Match(text, @"(\d+)\s*k\b");
            if (kMatch.Success)
                return decimal.Parse(kMatch.Groups[1].Value) * 1000;

            var normalMatch = Regex.Match(text, @"\d+");
            if (normalMatch.Success)
                return decimal.Parse(normalMatch.Value);

            return 0;
        }

        private string CleanNote(string text)
        {
            text = Regex.Replace(text,@"\d+(?:[.,]\d+)?\s*(k|tr|đ|vnd|m)?\b","");

            text = Regex.Replace(text,@"\s+"," ");

            return text.Trim();
        }
    }
}
