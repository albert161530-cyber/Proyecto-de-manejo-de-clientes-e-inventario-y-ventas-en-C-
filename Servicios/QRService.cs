using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QRCoder;  // INTALA QRCODER

namespace PLUNES
{
    public class QRService
    {
        public static Bitmap GenerarQR(string textoQR)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(textoQR, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrImage = qrCode.GetGraphic(4);

            return qrImage;
        }
    }
}
