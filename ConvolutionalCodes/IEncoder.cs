using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalCodes
{
    internal interface IEncoder
    {
        int FirstPolynom { get; set; }
        int SecondPolynom { get; set; }
        string Encode(string message);
        List<string> Decode(string message);
    }
}
