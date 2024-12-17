using System.Text;

namespace ConvolutionalCodes
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConvolutionalEncoder encoder = new(0b101, 0b111);

            while (true)
            {
                Console.WriteLine("Введите сообщение для отправки: ");
                string text = Console.ReadLine() ?? "";
                Console.WriteLine("Введите вероятность p: ");
                double p = double.Parse(Console.ReadLine() ?? "0");

                ProbalisticalChannel channel = new ProbalisticalChannel(p, encoder);
                channel.TransferMessage(text);
                //Console.WriteLine(encoder.Encode("00011"));
                //Console.WriteLine(encoder.Decode(encoder.Encode("000")) == "000");
            }
        }
    }
}
