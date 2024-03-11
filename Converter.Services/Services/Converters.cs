using SautinSoft;
using System.Text;

namespace Converter.Services.Services;

public class Converters
{
    public async Task<string> PdfToWordConverter(string pdfFilePath)
    {
        string docxFilePath = string.Empty;
        try
        {
            PdfFocus pdfFocus = new();

            pdfFocus.OpenPdf(pdfFilePath);

            if (pdfFocus.PageCount > 0)
            {
                if (pdfFocus.ToWord(docxFilePath) == 0)
                    Console.WriteLine("PDF converted to DOCX successfully!");
                else
                    Console.WriteLine("Error converting PDF to DOCX");
            }
            else
                Console.WriteLine("No pages found in the PDF file.");

            pdfFocus.ClosePdf();
            return docxFilePath;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred: " + ex.Message);
        }
    }
    public async Task<string> PdfToHTMLConverter(string pdfFilePath)
    {
        string htmlFilePath = string.Empty;

        try
        {
            PdfFocus pdfFocus = new PdfFocus();

            pdfFocus.OpenPdf(pdfFilePath);

            if (pdfFocus.PageCount > 0)
            {
                if (pdfFocus.ToHtml(htmlFilePath) == 0)
                {
                    Console.WriteLine("PDF converted to HTML successfully!");
                }
                else
                {
                    Console.WriteLine("Error converting PDF to HTML");
                }
            }
            else
            {
                Console.WriteLine("No pages found in the PDF file.");
            }

            pdfFocus.ClosePdf();
            return htmlFilePath;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred: " + ex.Message);
        }
    }
    public async Task<byte[]> FileToByteConverter(string filePath)
    {
        try
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            return fileBytes;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred: " + ex.Message);
        }
    }
    public async Task<byte[]> FileToBinaryConverter(string filePath)
    {
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] fileBinary = new byte[fs.Length];

                fs.Read(fileBinary, 0, (int)fs.Length);

                return fileBinary;
            }
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred: " + ex.Message);
        }

    }
    public async Task<byte[]> TextToBinaryConverter(string inputText)
    {
        try
        {
            byte[] binaryData = Encoding.UTF8.GetBytes(inputText);
            return binaryData;

        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred: " + ex.Message);
        }
    }
    public async Task<string> BinaryToTextConverter(string inputBinary)
    {
        try
        {
            inputBinary = inputBinary.Replace(" ", "");

            byte[] bytes = new byte[inputBinary.Length / 8];
            for (int i = 0; i < inputBinary.Length; i += 8)
            {
                bytes[i / 8] = Convert.ToByte(inputBinary.Substring(i, 8), 2);
            }

            string text = Encoding.ASCII.GetString(bytes);
            return text;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred: " + ex.Message);
        }
    }
    public async Task<string> TextToAsciiConverter(string inputText)
    {
        StringBuilder asciiBuilder = new StringBuilder();

        foreach (char c in inputText)
        {
            int asciiValue = (int)c;
            asciiBuilder.Append(asciiValue).Append(" ");
        }

        return asciiBuilder.ToString();
    }
    public async Task<string> DecimalToBinaryConverter(decimal decimalNumber)
    {
        string binaryString = Convert.ToString((long)decimalNumber, 2);
        return binaryString;
    }
    public async Task<string> DecimalToOctalConverter(decimal decimalNumber)
    {
        string octalString = Convert.ToString((long)decimalNumber, 8);
        return octalString;
    }
    public async Task<string> DecimalToHexadecimalConverter(decimal decimalNumber)
    {
        string hexadecimalString = Convert.ToString((long)decimalNumber, 16);
        return hexadecimalString;
    }
    public async Task<string> DecimalToPercent(decimal decimalNumber)
    {
        string percentString = (decimalNumber * 100).ToString("0.##") + "%";
        return percentString;
    }
    public async Task<string> ConvertFromDecimal(int decimalValue, int toBase)
    {
        string result = "";

        while (decimalValue > 0)
        {
            int remainder = decimalValue % toBase;
            char digit = await GetCharValue(remainder);

            result = digit + result;
            decimalValue /= toBase;
        }

        return result;
    }
    public async Task<int> ConvertToDecimal(string number, int fromBase)
    {
        int result = 0;
        int multiplier = 1;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            char c = number[i];
            int digit = await GetDigitValue(c);

            result += digit * multiplier;
            multiplier *= fromBase;
        }

        return result;
    }
    public async Task<char> GetCharValue(int value)
    {
        if (value < 10)
            return (char)(value + '0');
        else
            return (char)(value - 10 + 'A');
    }
    public async Task<int> GetDigitValue(char c)
    {
        if (char.IsDigit(c))
            return c - '0';
        else
            return char.ToUpper(c) - 'A' + 10;
    }
    public async Task<string> ConvertBase(string number, int fromBase, int toBase)
    {
        int decimalValue = await ConvertToDecimal(number, fromBase);
        string result = await ConvertFromDecimal(decimalValue, toBase);

        return result;
    }
}
