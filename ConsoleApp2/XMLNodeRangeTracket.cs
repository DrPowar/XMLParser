using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class XMLNodeTracker
    {
        public Stack<uint> NodeStack { get; private set; }
        public uint Start { get; private set; }
        public uint End { get; private set; }
        public bool IsValid { get; set; }

        public XMLNodeTracker()
        {
            NodeStack = new Stack<uint>();
            IsValid = true;
        }

        public void PushStart(uint index)
        {
            NodeStack.Push(index);
            Start = index;
        }

        public void PopEnd(uint index)
        {
            if (NodeStack.Count > 0)
            {
                Start = NodeStack.Pop();
                End = index;
            }
        }
    }

}
