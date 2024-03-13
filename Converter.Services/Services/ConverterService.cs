using Converter.Domain.Configuration;
using SautinSoft;
using System.Text;

namespace Converter.Services.Services;

public class ConverterService
{
    public List<string> GetMethodNames()
    {
        List<string> methodNames =
        [
            nameof(PdfToWordConverter),
            nameof(PdfToHTMLConverter),
            nameof(FileToByteConverter),
            nameof(FileToBinaryConverter),
            nameof(TextToBinaryConverter),
            nameof(BinaryToTextConverter),
            nameof(TextToAsciiConverter),
            nameof(DecimalToBinaryConverter),
            nameof(DecimalToOctalConverter),
            nameof(DecimalToHexadecimalConverter),
            nameof(DecimalToPercent),
            nameof(ConvertFromDecimal),
            nameof(ConvertToDecimal),
            nameof(GetCharValue),
            nameof(GetDigitValue),
            nameof(ConvertBase),
        ];

        return methodNames;
    }

    public string GenerateFileName(string extension)
    {
        // Generate a unique file name based on the current timestamp
        string fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");

        // Append the provided extension to the file name
        fileName += extension;

        return fileName;
    }

    public async Task<string> PdfToWordConverter(string pdfFilePath, string fileName)
    {
        string docxFilePath = Constants.CONVERTED_PATH + fileName;
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
            if (inputBinary.Contains(" "))
            {
                inputBinary = inputBinary.Replace(" ", "");
            }

            if (inputBinary.Length % 8 != 0)
            {
                return "Invalid binary input. The length of the binary string must be divisible by 8.";
            }

            int byteLength = 8;
            int byteCount = inputBinary.Length / byteLength;
            byte[] bytes = new byte[byteCount];

            for (int i = 0; i < byteCount; i++)
            {
                string byteString = inputBinary.Substring(i * byteLength, byteLength);

                if (!IsBinary(byteString))
                {
                    throw new ArgumentException("Invalid binary input. The binary string contains non-binary characters.");
                }

                byte byteValue = Convert.ToByte(byteString, 2);
                bytes[i] = byteValue;
            }

            string text = Encoding.ASCII.GetString(bytes);
            return text;
        }
        catch (Exception ex)
        {
            return $"An error occurred: {ex.Message}";
        }
    }

    private bool IsBinary(string input)
    {
        foreach (char c in input)
        {
            if (c != '0' && c != '1')
            {
                return false;
            }
        }

        return true;
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
    public async Task<string> ConvertFromDecimal(int decimalValue, int toBase = 10)
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
    public async Task<int> ConvertToDecimal(string number, int fromBase = 8)
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
