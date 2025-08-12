using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace LabTestApi.Services
{
    public static class DocumentContentHelper
    {
        public static string? ExtractText(byte[]? bytes, string? documentType)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return null;
            }

            var type = (documentType ?? string.Empty).Trim().ToUpperInvariant();

            switch (type)
            {
                // Plain-text encodings
                case "TXT":
                case "CSV":
                case "XML":
                case "HTML":
                case "HTM":
                case "SVG":
                case "MHT":
                    return DecodeText(bytes);

                case "RTF":
                    return ExtractTextFromRtf(bytes);

                case "PDF":
                    return ExtractTextFromPdf(bytes);

                // Images and other binaries not supported for text extraction without OCR
                case "PNG":
                case "JPG":
                case "JPEG":
                case "GIF":
                case "BMP":
                case "TIFF":
                case "TIF":
                case "SVGZ":
                default:
                    return null;
            }
        }

        private static string DecodeText(byte[] bytes)
        {
            try
            {
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                try
                {
                    return Encoding.Unicode.GetString(bytes);
                }
                catch
                {
                    return null;
                }
            }
        }

        private static string? ExtractTextFromRtf(byte[] bytes)
        {
            var rtf = DecodeText(bytes);
            if (string.IsNullOrEmpty(rtf)) return null;

            var text = rtf;
            text = Regex.Replace(text, @"\\'[0-9a-fA-F]{2}", string.Empty);
            text = Regex.Replace(text, @"\\par[d]?", Environment.NewLine, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\\[a-zA-Z]+-?\d*\s?", string.Empty);
            text = text.Replace("{", string.Empty).Replace("}", string.Empty);
            text = text.Replace("\\\\", "\\");
            return text.Trim();
        }

        private static string? ExtractTextFromPdf(byte[] bytes)
        {
            try
            {
                using var ms = new MemoryStream(bytes);
                using var pdf = new PdfDocument(new PdfReader(ms));
                var sb = new StringBuilder();
                int total = pdf.GetNumberOfPages();
                for (int i = 1; i <= total; i++)
                {
                    var page = pdf.GetPage(i);
                    var strategy = new LocationTextExtractionStrategy();
                    var text = PdfTextExtractor.GetTextFromPage(page, strategy);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        sb.AppendLine(text);
                    }
                }

                var combined = sb.ToString();
                if (string.IsNullOrWhiteSpace(combined)) return null;

                // Remove lines that are only underscores (form lines/artifacts)
                var cleaned = Regex.Replace(combined, @"^_+$", string.Empty, RegexOptions.Multiline);
                cleaned = Regex.Replace(cleaned, @"\n{3,}", "\n\n");
                cleaned = cleaned.Trim();
                return string.IsNullOrWhiteSpace(cleaned) ? null : cleaned;
            }
            catch
            {
                return null;
            }
        }
    }
}
