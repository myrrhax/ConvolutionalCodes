using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalCodes;

internal class ProbalisticalChannel
{
    private Random rnd = new Random();
    private double probability;
    private IEncoder encoder;
    public double Probability { 
        set
        {
            if (value < 0 || value > 1) throw new ArgumentOutOfRangeException("Probability");
            probability = value;
        } 
    }
    public ProbalisticalChannel(double probability, IEncoder encoder)
    {
        Probability = probability;
        this.encoder = encoder;
    }

    private (string, int) AddNoise(string message)
    {
        int changed = 0;
        StringBuilder sb = new StringBuilder();
        
        foreach (char c in message)
        {
            double p = rnd.NextDouble();
            if (p < probability)
            {
                sb.Append(c);
            } 
            else
            {
                changed++;
                sb.Append(c == '1' ? '0' : '1');
            }
        }
        return (sb.ToString(), changed);
    }

    public void TransferMessage(string message)
    {
        Console.WriteLine("Отправляется сообщение: " + message);

        bool isText = message.Any(x => x != '0' && x != '1');
        Console.WriteLine(isText);
        string msg = isText ? StringToBinary(message) : message;
        string encoded = encoder.Encode(msg);
        Console.WriteLine("Закодированное сообщение: " + encoded);
        var noiseMessage = AddNoise(encoded);
        Console.WriteLine("Пришедшее сообщение: " + noiseMessage.Item1);
        var decoded = encoder.Decode(noiseMessage.Item1);
        Console.WriteLine("Было повреждено: " + noiseMessage.Item2 + " символов");
        foreach (var e in decoded)
        {
            string s = isText ? BinaryToString(e) : e;
            Console.WriteLine("Декодированное сообщение: " + s);
            if (s == message)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Сообщения совпадают");
                Console.ForegroundColor = ConsoleColor.White;
                break;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Сообщения не совпадают");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }

    static string StringToBinary(string text)
    {
        // Преобразуем текст в байты с помощью UTF-8
        byte[] bytes = Encoding.UTF8.GetBytes(text);

        // Переводим каждый байт в двоичное представление
        StringBuilder binaryBuilder = new StringBuilder();
        foreach (byte b in bytes)
        {
            binaryBuilder.Append(Convert.ToString(b, 2).PadLeft(8, '0')); // 8-битное представление
        }

        return binaryBuilder.ToString();
    }

    static string BinaryToString(string binaryText)
    {
        // Разбиваем двоичное представление на байты (по 8 бит)
        int numOfBytes = binaryText.Length / 8;
        byte[] bytes = new byte[numOfBytes];

        for (int i = 0; i < numOfBytes; i++)
        {
            // Берём 8 символов, преобразуем в байт
            string byteString = binaryText.Substring(i * 8, 8);
            bytes[i] = Convert.ToByte(byteString, 2);
        }

        // Преобразуем байты обратно в строку с помощью UTF-8
        return Encoding.UTF8.GetString(bytes);
    }
}
