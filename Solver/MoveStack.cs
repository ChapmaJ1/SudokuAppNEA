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
        public int Capacity { get; private set; }
        public MoveStack(int capacity)
        {
            StackArray = new Move[capacity];  // maximum "capacity" most recent moves are stored
            FrontPointer = -1;
            Count = 0;
            Capacity = capacity;
        }

        public void Push(Move move)
        {
            FrontPointer = (FrontPointer + 1) % Capacity; // increments front pointer, bringing it back to the start of the array if index gets too large
            StackArray[FrontPointer] = move;
            if (Count < Capacity)  // if stack is not full, record the increase in moves being stored
            {
                Count++;
            }
        }

        public Move Pop()
        {
            if (Count > 0)  // if stack is not empty - there is a move that can be popped
            {
                Move move = StackArray[FrontPointer];  // returns the move at the top of the stack, indicated by the pointer
                FrontPointer = (FrontPointer + (Capacity - 1)) % Capacity;  
                Count--;  // records the decrease in moves being stored
                return move;
            }
            throw new InvalidOperationException();
        }
    }
}
