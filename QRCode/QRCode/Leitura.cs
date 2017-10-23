using iTextSharp.text.pdf;
using MessagingToolkit.QRCode.Codec;
using MessagingToolkit.QRCode.Codec.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace QRCode
{
    public class Leitura : IDisposable
    {
        public string imagem { get; set; }
        public List<string> caminhos { get; set; }

        public string LerUmQRCode(string caminhoPDF, string diretorio)
        {
            this.imagem = ImageConvertor.ConvertPDFToImage(new PdfReader(caminhoPDF), diretorio);
            Bitmap bitmap = new Bitmap(System.Drawing.Image.FromFile(imagem));
            QRCodeDecoder dec = new QRCodeDecoder();
            try
            {
                var x = dec.decode(new QRCodeBitmapImage(bitmap));
                //return (dec.decode(new QRCodeBitmapImage(bitmap))).Replace("tem sim", "NÂO tem");
                return x.ToString();
            }
            finally
            {
                bitmap.Dispose();
                GC.SuppressFinalize(bitmap);
                GC.SuppressFinalize(dec);
                GC.SuppressFinalize(bitmap);
                File.Delete(caminhoPDF);
            }

        }
        public Dictionary<string, string> LerVariosQRCode(string caminhoPDF, string diretorio)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            caminhos = ImageConvertor.ConvertPDFToImage(new PdfReader(caminhoPDF), diretorio, new PdfReader(caminhoPDF).NumberOfPages).ToList();
            MessagingToolkit.QRCode.Codec.QRCodeDecoder dec = new MessagingToolkit.QRCode.Codec.QRCodeDecoder();
            try
            {
                foreach (var caminho in caminhos)
                {

                    try
                    {
                        Bitmap bitmap = new Bitmap(System.Drawing.Image.FromFile(caminho));
                        output.Add(caminho, dec.decode(new MessagingToolkit.QRCode.Codec.Data.QRCodeBitmapImage(bitmap)));
                        bitmap.Dispose();
                        GC.SuppressFinalize(bitmap);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    

                }
                return output;

            }
            finally
            {
                GC.SuppressFinalize(dec);
                GC.SuppressFinalize(output);
                
                File.Delete(caminhoPDF);

            }



        }
        public void Dispose()
        {


            if (imagem != null)
            {

                GC.SuppressFinalize(imagem);
                File.Delete(imagem);

            }

            if (caminhos != null)
            {
                caminhos.ForEach(item => File.Delete(item));
                GC.SuppressFinalize(caminhos);
            }


            GC.SuppressFinalize(this);
        }
    }

}
