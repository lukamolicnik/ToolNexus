namespace ToolNexus.WebUI.Server.Services;

public interface IQRCodeService
{
    string GenerateQRCodeBase64(string text);
    byte[] GenerateQRCodePdf(string toolCode, string toolName, string? toolDescription = null);
}