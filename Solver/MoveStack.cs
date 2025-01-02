using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class MoveStack
    {
        public Move[] StackArray { get; private set; }
        public int FrontPointer { get; private set; }
        public int Count { get; private set; }
        public MoveStack()
        {
            StackArray = new Move[5];
            FrontPointer = -1;
            Count = 0;
        }

        public void Push(Move move)
        {
            FrontPointer = (FrontPointer + 1) % 5;
            StackArray[FrontPointer] = move;
            if (Count < 5)
            {
                Count++;
            }
        }

        public Move Pop()
        {
            if (Count > 0)
            {
                Move move = StackArray[FrontPointer];
                FrontPointer = (FrontPointer - 1) % 5;
                Count--;
                return move;
            }
            throw new IndexOutOfRangeException();
        }
    }
}
