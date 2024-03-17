namespace Converter.Services.Interfaces
{
    public interface IConverterService
    {
        Task<string> BinaryToTextConverter(string inputBinary);
        Task<string> ConvertBase(string number, int fromBase, int toBase);
        Task<string> ConvertFromDecimal(int decimalValue, int toBase = 10);
        Task<int> ConvertToDecimal(string number, int fromBase = 8);
        Task<string> DecimalToBinaryConverter(decimal decimalNumber);
        Task<string> DecimalToHexadecimalConverter(decimal decimalNumber);
        Task<string> DecimalToOctalConverter(decimal decimalNumber);
        Task<string> DecimalToPercent(decimal decimalNumber);
        Task<byte[]> FileToBinaryConverter(string filePath);
        Task<byte[]> FileToByteConverter(string filePath);
        string GenerateFileName(string extension);
        Task<char> GetCharValue(int value);
        Task<int> GetDigitValue(char c);
        List<string> GetMethodNames();
        Task<string> PdfToHTMLConverter(string pdfFilePath);
        Task<string> PdfToWordConverter(string pdfFilePath, string fileName);
        Task<string> TextToAsciiConverter(string inputText);
        Task<byte[]> TextToBinaryConverter(string inputText);
    }
}