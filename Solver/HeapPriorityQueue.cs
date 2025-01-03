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
            Cell temp = NodeArray[index1];  // creates temporary storage cell, then swaps the values at each index
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
                NodeArray[Occupied] = cell;  // inserts the cell at the bottom of the heap
                PushUp(Occupied);
                Occupied++;
            }
        }

        public Cell Dequeue()
        {
            Cell topCell = NodeArray[0];  // returns the cell at the top of the heap (front of the queue)
            NodeArray[0] = NodeArray[Occupied-1];  // highest priority cell replace by the cell at the bottom of the heap
            Occupied--;
            PushDown(0);
            return topCell;
        }

        private void PushUp(int childIndex)
        {
            if (childIndex != 0)  // if not already at the top of the heap
            {
                int parentIndex = Convert.ToInt32(Math.Ceiling((decimal)childIndex / 2) - 1);
                if (NodeArray[parentIndex].Domain.Count > NodeArray[childIndex].Domain.Count)  // if the domain of the child cell is smaller than the domain of the parent cell
                {
                    SwapIndexes(parentIndex, childIndex);  // swap the parent and child cell elements
                    PushUp(parentIndex);  // continue the process with the child cell (which is now at the parent index)
                }
            }
        }

        private void PushDown(int parentIndex)
        {
            int leftChildIndex = parentIndex * 2 + 1;
            int rightChildIndex = parentIndex * 2 + 2;
            if (!(leftChildIndex > Occupied && rightChildIndex > Occupied))  // if both child indexes are out of range
            {
                if (rightChildIndex > Occupied)  // if left child index in range, right child index out of range
                {
                    if (NodeArray[parentIndex].Domain.Count > NodeArray[leftChildIndex].Domain.Count)
                    {
                        SwapIndexes(parentIndex, leftChildIndex);
                    }
                }
                else
                {
                    if (!(NodeArray[parentIndex].Domain.Count < NodeArray[leftChildIndex].Domain.Count && NodeArray[parentIndex].Domain.Count < NodeArray[rightChildIndex].Domain.Count))  // if the domain of the parent cell is bigger than at least one of the child cell domains
                    {
                        if (NodeArray[leftChildIndex].Domain.Count < NodeArray[rightChildIndex].Domain.Count)
                        {
                            SwapIndexes(parentIndex, leftChildIndex);  // if left child domain is smaller than right child domain, swap with left child
                            PushDown(leftChildIndex);  // continue process with paret cell (now at left child index)
                        }
                        else
                        {
                            SwapIndexes(parentIndex, rightChildIndex); // if right child domain is smaller than left child domain, swap with right child
                            PushDown(rightChildIndex);
                        }
                    }
                }
            }
        }
    }
}
