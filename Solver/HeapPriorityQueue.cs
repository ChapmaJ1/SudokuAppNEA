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
        // stores the queue in the form of an array, with parent and child indices for each entry
        public Cell[] NodeArray { get; private set; }
        public int Occupied { get; private set; }
        public HeapPriorityQueue(List<Cell> variableNodes) 
        {
            NodeArray = new Cell[Convert.ToInt32(variableNodes.Count)];
            Occupied = 0;
        }

        public void Enqueue(Cell cell)
        {
            // indicates that the queue is full
            if (Occupied == NodeArray.Length)
            {
                throw new IndexOutOfRangeException("Queue is full");
            }
            else
            {
                // inserts the cell at the bottom of the heap and executes push up operation
                NodeArray[Occupied] = cell; 
                PushUp(Occupied);
                Occupied++;
            }
        }

        public Cell Dequeue()
        {
            if (Occupied == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }
            // returns the cell at the top of the heap (front of the queue)
            Cell topCell = NodeArray[0];
            // the highest priority cell is replaced by the cell at the bottom of the heap
            NodeArray[0] = NodeArray[Occupied-1];
            Occupied--;
            // push down operation executed
            PushDown(0);
            return topCell;
        }

        private void PushUp(int childIndex)
        {
            // if the cell is not already at the top of the heap
            if (childIndex != 0)
            {
                int parentIndex = Convert.ToInt32(Math.Ceiling((decimal)childIndex / 2) - 1);
                // if the domain of the child cell is smaller than the domain of the parent cell
                if (NodeArray[parentIndex].Domain.Count > NodeArray[childIndex].Domain.Count)
                {
                    // swap the parent and child cell elements
                    SwapIndexes(parentIndex, childIndex);
                    // recursive loop - continue the process with the child cell (which is now at the parent index)
                    PushUp(parentIndex);
                }
            }
        }

        private void PushDown(int parentIndex)
        {
            int leftChildIndex = parentIndex * 2 + 1;
            int rightChildIndex = parentIndex * 2 + 2;
            // if at least one child index is in range (the parent has at least one child)
            if (leftChildIndex <= Occupied) 
            {
                // if left child index in range, right child index out of range
                if (rightChildIndex > Occupied)
                {
                    if (NodeArray[parentIndex].Domain.Count > NodeArray[leftChildIndex].Domain.Count)
                    {
                        SwapIndexes(parentIndex, leftChildIndex);
                    }
                }
                // otherwise, the parent has 2 children
                // parent cannot have a right child without a left child due to the push down operation
                else
                {
                    if (!(NodeArray[parentIndex].Domain.Count < NodeArray[leftChildIndex].Domain.Count && NodeArray[parentIndex].Domain.Count < NodeArray[rightChildIndex].Domain.Count))  // if the domain of the parent cell is bigger than at least one of the child cell domains
                    {
                        if (NodeArray[leftChildIndex].Domain.Count < NodeArray[rightChildIndex].Domain.Count)
                        {
                            // if left child domain is smaller than right child domain, swap with left child
                            SwapIndexes(parentIndex, leftChildIndex);
                            // recursive loop - continue process with parent cell (now at the index where the left child previously was)
                            PushDown(leftChildIndex);
                        }
                        else
                        {
                            // if right child domain is smaller than left child domain, swap with right child
                            SwapIndexes(parentIndex, rightChildIndex);
                            PushDown(rightChildIndex);
                        }
                    }
                }
            }
        }

        private void SwapIndexes(int index1, int index2)
        {
            // creates temporary storage cell, then swaps the values at each index
            Cell temp = NodeArray[index1];
            NodeArray[index1] = NodeArray[index2];
            NodeArray[index2] = temp;
        }
    }
}
