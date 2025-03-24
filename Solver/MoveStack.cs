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
            // when filled as much as possible, the "capacity" most recent moves are stored
            StackArray = new Move[capacity];
            FrontPointer = -1;
            Count = 0;
            Capacity = capacity;
        }

        public void Push(Cell cell, int entry)
        {
            // increments front pointer, bringing it back to the start of the array if index gets too large
            FrontPointer = (FrontPointer + 1) % Capacity;
            StackArray[FrontPointer] = new Move(cell, entry);
            // if stack is not full, record the increase in moves being stored
            if (Count < Capacity) 
            {
                Count++;
            }
        }

        public (Cell, int) Pop()
        {
            // if stack is not empty - there is a move that can be popped
            if (Count > 0)
            {
                // returns the properties of the move at the top of the stack, indicated by the pointer
                Move move = StackArray[FrontPointer];
                // moves the pointer back by 1 position, or to the back of the array if it is currently at the front
                FrontPointer = (FrontPointer + (Capacity - 1)) % Capacity;
                Count--;
                return (move.Cell, move.OldEntry);
            }
            throw new InvalidOperationException();
        }
    }
}
