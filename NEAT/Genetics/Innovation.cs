using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEAT.Genetics
{
    public static class InnovationGenerator
    {
        private static ulong _current = 0;
        public static ulong Current
        {
            get
            {
                return _current;
            }
            set
            {
                _current = value;
            }
        }
        public static ulong Next
        {
            get
            { 
                return _current++;
            }
        }
        private static Dictionary<Tuple<int, int>, ulong> ConnectionMutations = new Dictionary<Tuple<int, int>, ulong>();

        public static ulong NextMutationNumber(int input, int output)
        {
            Tuple<int, int> tuple = new Tuple<int, int>(input, output);

            if (!ConnectionMutations.ContainsKey(tuple))
            {
                ConnectionMutations.Add(tuple, Next);
            }

            return ConnectionMutations[tuple];
        }

        public static void ClearRegisteredMutations()
        {
            ConnectionMutations.Clear();
        }
    }
}
