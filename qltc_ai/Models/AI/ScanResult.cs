using qltc_ai.Models.AI;
using qltc_ai.Service.Base.AI;

public class ScanResult
{
    public bool Success { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public ScanResultDto? Data { get; private set; }

    public static ScanResult Ok(string note, decimal money) => new()
    { Success = true, Data = new ScanResultDto { Note = note, Money = money } };

    public static ScanResult Fail(string msg) => new()
    { Success = false, Message = msg };
}