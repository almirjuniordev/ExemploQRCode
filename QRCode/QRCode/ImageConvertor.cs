using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace QRCode
{
    class ImageConvertor :  IDisposable
    {



        public static string ConvertPDFToImage(PdfReader reader, string diretorioSaida)
        {
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            ImageRenderListener listener = new ImageRenderListener();
            try
            {
                parser.ProcessContent(1, listener);
                return ConvertJpgToPng(0, listener, diretorioSaida);
            }
            finally
            {

                GC.SuppressFinalize(listener);
                GC.SuppressFinalize(parser);
                GC.SuppressFinalize(reader);
                reader.Dispose();
            }
        }


        public static IEnumerable<string> ConvertPDFToImage(PdfReader reader, string diretorioSaida, int quantidadePaginas)
        {
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            ImageRenderListener listener = new ImageRenderListener();
            List<string> listaDeCaminhos = new List<string>();
            try
            {
                for (int i = 1; i <= quantidadePaginas; i++)
                {
                    parser.ProcessContent(i, listener);
                }

                for (int i = 0; i < listener.Images.Count; ++i)
                {
                    listaDeCaminhos.Add(ConvertJpgToPng(i, listener, diretorioSaida));

                }
                return listaDeCaminhos;
            }
            finally
            {

                GC.SuppressFinalize(listener);
                GC.SuppressFinalize(parser);
                GC.SuppressFinalize(reader);
                GC.SuppressFinalize(listaDeCaminhos);
                reader.Dispose();
            }

        }




        private static string ConvertPngToJpg(int i, ImageRenderListener listener, string diretorio)
        {
            System.Drawing.Imaging.EncoderParameters parms = new System.Drawing.Imaging.EncoderParameters(1);
            parms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 0);

            System.Drawing.Imaging.ImageCodecInfo jpegEncoder = ImageCodecInfo.GetImageEncoders().Single(p => p.CodecName.Contains("JPEG"));
            System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(listener.Images[i]));
            string path = diretorio + "\\" + listener.ImageNames[i].ToLower().Replace(".png", ".jpg");
            img.Save(path, jpegEncoder, parms);
            return path;
        }

        private static string ConvertJpgToPng(int i, ImageRenderListener listener, string diretorio)
        {
            System.Drawing.Imaging.EncoderParameters parms = new System.Drawing.Imaging.EncoderParameters(1);
            parms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 0);

            System.Drawing.Imaging.ImageCodecInfo jpegEncoder = ImageCodecInfo.GetImageEncoders().Single(p => p.CodecName.Contains("PNG"));
            System.Drawing.Image img = System.Drawing.Image.FromStream(new MemoryStream(listener.Images[i]));
            string path = diretorio + "\\" + i +listener.ImageNames[i].ToLower().Replace(".jpg", ".png");
            img.Save(path, jpegEncoder, parms);
            return path;
        }

        public void Dispose()
        {

            GC.SuppressFinalize(this);
        }
    }

    class ImageRenderListener : IRenderListener
    {
        public void RenderText(TextRenderInfo renderInfo) { }
        public void BeginTextBlock() { }
        public void EndTextBlock() { }

        public List<byte[]> Images = new List<byte[]>();
        public List<string> ImageNames = new List<string>();
        public void RenderImage(ImageRenderInfo renderInfo)
        {
            PdfImageObject image = null;
            try
            {
                image = renderInfo.GetImage();
                if (image == null) return;

                ImageNames.Add(string.Format(
                  "qrcode.{0}", image.GetFileType()
                ));
                using (MemoryStream ms = new MemoryStream(image.GetImageAsBytes()))
                {
                    Images.Add(ms.ToArray());
                }
            }
            catch (IOException ie)
            {
                throw ie;
            }
        }
    }
}
