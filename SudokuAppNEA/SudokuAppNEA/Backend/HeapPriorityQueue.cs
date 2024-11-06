using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class HeapPriorityQueue
    {
        public Cell[] NodeArray { get; private set; }
        public int Occupied { get; private set; }
        public HeapPriorityQueue(List<Cell> variableNodes, int boardDimensions) 
        {
            NodeArray = new Cell[Convert.ToInt32(variableNodes.Count)];
            Occupied = 0;
        }

        private void SwapIndexes(int index1, int index2)
        {
            Cell temp = NodeArray[index1];
            NodeArray[index1] = NodeArray[index2];
            NodeArray[index2] = temp;
        }

        public void Enqueue(Cell cell)
        {
            if (Occupied == NodeArray.Length)
            {
                throw new IndexOutOfRangeException("Queue is full");
            }
            else
            {
                NodeArray[Occupied] = cell;
                PushUp(Occupied);
                Occupied++;
            }
        }

        public Cell Dequeue()
        {
            Cell topCell = NodeArray[0];
            NodeArray[0] = NodeArray[Occupied-1];
            Occupied--;
            PushDown(0);
            return topCell;
        }

        private void PushUp(int childIndex)
        {
            if (childIndex != 0)
            {
                int parentIndex = Convert.ToInt32(Math.Ceiling((decimal)childIndex / 2) - 1);
                if (NodeArray[parentIndex].Domain.Count > NodeArray[childIndex].Domain.Count)
                {
                    SwapIndexes(parentIndex, childIndex);
                    PushUp(parentIndex);
                }
            }
        }

        private void PushDown(int parentIndex)
        {
            int leftChildIndex = parentIndex * 2 + 1;
            int rightChildIndex = parentIndex * 2 + 2;
            if (!(leftChildIndex > Occupied && rightChildIndex > Occupied))
            {
                if (rightChildIndex > Occupied)
                {
                    if (NodeArray[parentIndex].Domain.Count > NodeArray[leftChildIndex].Domain.Count)
                    {
                        SwapIndexes(parentIndex, leftChildIndex);
                    }
                }
                else
                {
                    if (NodeArray[leftChildIndex].Domain.Count < NodeArray[rightChildIndex].Domain.Count)
                    {
                        SwapIndexes(parentIndex, leftChildIndex);
                        PushDown(leftChildIndex);
                    }
                    else
                    {
                        SwapIndexes(parentIndex, rightChildIndex);
                        PushDown(rightChildIndex);
                    }
                }
            }
        }
    }
}
