using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachineMetadata.Extensions
{
    public struct Coordinates
    {
        public int Left { get; private set; }
        public int Top { get; private set; }
        public int Right { get; private set; }
        public int Bottom { get; private set; }
        public Coordinates(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
        public bool Contains(Coordinates inner)
        {
            var result = Left <= inner.Left && Top <= inner.Top && Right >= inner.Right && Bottom >= inner.Bottom;
            return result;
        }

        public Int64 Area => (Bottom - Top) * (Right - Left);
    }
}
