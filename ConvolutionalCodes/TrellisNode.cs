using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalCodes
{
    internal class TrellisNode
    {
        public string State { get; set; }
        public int PathMetric { get; set; }
        public TrellisNode? From { get; set; }
        public string Code { get; set; }
        public TrellisNode(string state, int pathMetric, TrellisNode? from, string code)
        {
            State = state;
            PathMetric = pathMetric;
            From = from;
            Code = code;
        }
    }
}
