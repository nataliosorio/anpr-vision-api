using QRCoder;

namespace Utilities.Qr
{
    public static class QrHelper
    {
        public static byte[] GenerateQr(string content)
        {
            QRCodeGenerator qrGen = new QRCodeGenerator();
            QRCodeData qrData = qrGen.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            // GENERADOR PNG NATIVO SIN Bitmap
            PngByteQRCode pngQrCode = new PngByteQRCode(qrData);

            // Puedes ajustar el pixelSize si quieres que sea más grande
            return pngQrCode.GetGraphic(pixelsPerModule: 10);
        }
    }
}
