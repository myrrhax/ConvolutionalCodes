using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvolutionalCodes
{
    internal class ConvolutionalEncoder : IEncoder
    {
        public int FirstPolynom { get; set; }
        public int SecondPolynom { get; set; }
        
        const int REGISTER_SIZE = 3;
        const int INPUT_POWER = 1;
        const int OUTPUT_POWER = 2;

        public ConvolutionalEncoder(int firstPolynom, int secondPolynom)
        {
            FirstPolynom = firstPolynom;
            SecondPolynom = secondPolynom;
        }

        //public string Decode()
        //{
        //    return string.Empty;
        //}

        public List<string> Decode(string message)
        {
            var register = new LinkedList<char>();
            for (int i = 0; i < REGISTER_SIZE - 1; i++) register.AddFirst('0');
            return DecodeRecursive(message, register, 0, null);
        }

        public List<string> DecodeRecursive(string message, LinkedList<char> register, int stateNumber, Dictionary<string, TrellisNode>? from)
        {
            register.AddFirst(message[stateNumber]);
            string stateMessage = message.Substring(stateNumber * 2, 2);
            Console.WriteLine("Текущее сообщение: " + stateMessage);
            Console.WriteLine("Текущая таблица решетки");
            Console.WriteLine("N\tОткуда\tКуда\tZZ\tКод\tHD\tПуть");
            List<TrellisNode> currentLevelNodes = new List<TrellisNode>();
            // Формируем общую решетку
            for (int i = 0; i < Math.Pow(2, REGISTER_SIZE - 1); i++)
            {
                string nodeName = Convert.ToString(i, 2).PadLeft(REGISTER_SIZE - 1, '0');
                TrellisNode currentNode = stateNumber == 0 ? new TrellisNode(nodeName, 0, null, string.Empty) : from[nodeName];
                string sub = nodeName.Substring(0, REGISTER_SIZE - 2);
                string firstChildTransition = $"{GetQ('0' + nodeName, FirstPolynom)}{GetQ('0' + nodeName, SecondPolynom)}";
                int firstHammingDistance = CalculateHammingDistance(firstChildTransition, stateMessage);
                string firstChildName = '0' + sub;
                TrellisNode firstChild = new TrellisNode(firstChildName, firstHammingDistance + currentNode.PathMetric, currentNode, firstChildTransition);
                
                string secondChildTransition = $"{GetQ('1' + nodeName, FirstPolynom)}{GetQ('1' + nodeName, SecondPolynom)}";
                string secondChildName = '1' + sub;
                int secondHammingDistance = CalculateHammingDistance(secondChildTransition, stateMessage);
                TrellisNode secondChild = new TrellisNode(secondChildName, secondHammingDistance + currentNode.PathMetric, currentNode, secondChildTransition);

                currentLevelNodes.Add(firstChild);
                currentLevelNodes.Add(secondChild);
                Console.WriteLine($"{i}\t{currentNode.State}\t{firstChild.State}\t{firstChild.Code}\t{stateMessage}\t{firstHammingDistance}\t{firstChild.PathMetric}");
                Console.WriteLine($"{i}\t{currentNode.State}\t{secondChild.State}\t{secondChild.Code}\t{stateMessage}\t{secondHammingDistance}\t{secondChild.PathMetric}");
            }
            Dictionary<string, TrellisNode> newLevelTransitionMap = new();

            // Отсекаем выжившие пути
            Console.WriteLine("Отсекаем выжившие пути");
            foreach (var node in currentLevelNodes)
            {
                if (!newLevelTransitionMap.ContainsKey(node.State)) newLevelTransitionMap[node.State] = node;
                else
                {
                    if (newLevelTransitionMap[node.State].PathMetric > node.PathMetric)
                    {
                        newLevelTransitionMap[node.State] = node;
                    }
                    var metricalNode = newLevelTransitionMap[node.State];
                    Console.WriteLine($"{metricalNode.From!.State} -> {node.State} ({metricalNode.PathMetric})");
                }
            }
            register.RemoveLast();

            if (stateNumber + 1 >= message.Length / 2) // Финальное состояние
            {
                Console.WriteLine("Финальное состояние");
                List<TrellisNode> minNodes = newLevelTransitionMap.Select(x => x.Value).OrderBy(x => x.PathMetric)
                    .ToList();

                List<string> encodes = new List<string>();
                foreach (var e in minNodes)
                {
                    var path = GetMinPath(e);
                    string states = string.Join("", path);
                    Console.WriteLine(states);
                    StringBuilder decoded = new StringBuilder();
                    for (int i = 1; i < states.Length / 2; i++)
                    {
                        decoded.Append(states[2 * i]);
                    }
                    encodes.Add(decoded.ToString());
                }                
                
                return encodes;
            }

            return DecodeRecursive(message, register, stateNumber+1, newLevelTransitionMap);
        }

        public string Encode(string message)
        {
            StringBuilder sb = new StringBuilder();
            LinkedList<char> register = new();
            for (int i = 0; i < REGISTER_SIZE - 1; i++)
            {
                register.AddFirst('0');
            }

            int k = 0;
            while (k < message.Length)
            {
                register.AddFirst(message[k]); // добавляем новый
                char q1 = GetQ(string.Join("", register), FirstPolynom);
                char q2 = GetQ(string.Join("", register), SecondPolynom);
                sb.Append(q1);
                sb.Append(q2);
                k++;
                register.RemoveLast(); // удаляем последний символ регистра
            }

            //// промывка
            while (register.Any(el => el == '1'))
            {
                register.AddFirst('0');
                char q1 = GetQ(string.Join("", register), FirstPolynom);
                char q2 = GetQ(string.Join("", register), SecondPolynom);
                sb.Append(q1);
                sb.Append(q2);
                register.RemoveLast();
            }

            return sb.ToString();
        }

        private static int CalculateHammingDistance(string s1, string s2)
        {
            int res = 0;
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i] != s2[i]) res++;
            }
            return res;
        }

        private static char GetQ(string registerValue, int polynom)
        {
            string s = string.Empty;
            for (int i = 0; i < REGISTER_SIZE; i++)
            {
                s += registerValue[i];
            }
            string p = Convert.ToString(polynom, 2);
            int a = 0;
            for (int i = 0; i < REGISTER_SIZE; i++)
            {
                a ^= (s[i] - 48) * (p[i] - 48);
            }

            return (char) (a + 48);
        }

        private LinkedList<string> GetMinPath(TrellisNode minNode)
        {
            LinkedList<string> path = new LinkedList<string>();
            TrellisNode current = minNode;
            while (current != null)
            {
                path.AddFirst(current.State);
                current = current.From;
            }
            return path;
        }
    }
}
