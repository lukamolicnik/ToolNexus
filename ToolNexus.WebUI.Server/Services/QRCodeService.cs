using QRCoder;

namespace ToolNexus.WebUI.Server.Services;

public class QRCodeService : IQRCodeService
{
    public string GenerateQRCodeBase64(string text)
    {
        try
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeBytes = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeBytes)}";
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Napaka pri generiranju QR kode: {ex.Message}", ex);
        }
    }

    public byte[] GenerateQRCodePdf(string toolCode, string toolName, string? toolDescription = null)
    {
        // Za PDF generiranje bi bilo potrebno dodatno knjižnico kot je iTextSharp ali QuestPDF
        // Za sedaj vrnemo prazen array
        // V produkciji bi implementirali pravo PDF generiranje
        return new byte[0];
    }
}