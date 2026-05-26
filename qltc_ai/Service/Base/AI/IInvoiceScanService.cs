namespace qltc_ai.Service.Base.AI
{
    public interface IInvoiceScanService
    {
        Task<ScanResult> ScanImageAsync(IFormFile image);
        Task<ScanResult> ScanQrTextAsync(string qrText);
        string? GetNgrokUrl();
        void SetNgrokUrl(string url);
    }
}
