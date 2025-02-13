using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sudoku_Solver_NEA;

namespace Sudoku_Solver_NEA.Tests
{
    [TestClass]
    public class TestBackend
    {
        [TestMethod]
        public void TestGraph9x9()
        {
            string[,] boardSketch = GenerateBoardSketch(9);
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            Assert.AreEqual(20, board.AdjacencyList[board.VariableNodes[0]].Count);
        }

        [TestMethod]
        public void TestGraph16x16()
        {
            string[,] boardSketch = GenerateBoardSketch(16);
            Board board = new Board("", boardSketch, 16);
            board.InitialiseGraph();
            Assert.AreEqual(39, board.AdjacencyList[board.VariableNodes[0]].Count);
        }

        [TestMethod]
        public void TestGraph25x25()
        {
            string[,] boardSketch = GenerateBoardSketch(25);
            Board board = new Board("", boardSketch, 25);
            board.InitialiseGraph();
            Assert.AreEqual(64, board.AdjacencyList[board.VariableNodes[0]].Count);
        }

        [TestMethod]
        public void TestCheckInvalidFullFalse()
        {
            string[,] boardSketch =
            { {"8","0v","0v","0v","2","0v","0v","6","0v"},
              {"2","0v","0v","0v","3","8","5","1","4" },
              {"0v","0v","0v","6","0v","0v","3","8","0v"},
              {"0v","7","0v","5","0v","2","1","0v","0v"},
              {"0v","0v","6","0v","4","1","0v","9","0v"},
              {"0v","0v","8","0v","0v","0v","2","5","6" },
              {"6","0v","2","4","5","0v","8","7","9" },
              {"0v","0v","0v","7","0v","0v","4","2","1" },
              {"7","8","0v","2","1","0v","6","0v","0v" } };
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            BacktrackingSolver solver = new BacktrackingSolver(board);
            Assert.IsFalse(solver.CheckInvalidFull());
        }

        [TestMethod]
        public void TestCheckInvalidFullTrue()
        {
            string[,] boardSketch =
            { {"8","8v","0v","0v","2","0v","0v","6","0v"},
              {"2","0v","0v","0v","3","8","5","1","4" },
              {"0v","0v","0v","6","0v","0v","3","8","0v"},
              {"0v","7","0v","5","0v","2","1","0v","0v"},
              {"0v","0v","6","0v","4","1","0v","9","0v"},
              {"0v","0v","8","0v","0v","0v","2","5","6" },
              {"6","0v","2","4","5","0v","8","7","9" },
              {"0v","0v","0v","7","0v","0v","4","2","1" },
              {"7","8","0v","2","1","0v","6","0v","0v" } };
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            BacktrackingSolver solver = new BacktrackingSolver(board);
            Assert.IsTrue(solver.CheckInvalidFull());
        }

        [TestMethod]
        public void TestCheckFinishedTrue()
        {
            string[,] boardSketch =
              { {"8","0v","0v","0v","2","0v","0v","6","0v"},
              {"2","0v","0v","0v","3","8","5","1","4" },
              {"0v","0v","0v","6","0v","0v","3","8","0v"},
              {"0v","7","0v","5","0v","2","1","0v","0v"},
              {"0v","0v","6","0v","4","1","0v","9","0v"},
              {"0v","0v","8","0v","0v","0v","2","5","6" },
              {"6","0v","2","4","5","0v","8","7","9" },
              {"0v","0v","0v","7","0v","0v","4","2","1" },
              {"7","8","0v","2","1","0v","6","0v","0v" } };
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            BacktrackingSolver solver = new BacktrackingSolver(board);
            Assert.IsFalse(solver.CheckFinished());
        }

        [TestMethod]
        public void TestCheckFinishedFalse()
        {
           string[,] boardSketch =
              { {"1","5","6","7","2","9","3","4","8"},
              {"4","3","2","8","5","6","9","1","7" },
              {"8","9","7","1","3","4","5","6","2"},
              {"5","6","9v","2","8","7","1","3","4"},
              {"2","4","3","5","6","1","8","7","9"},
              {"7","1","8","9","4","3","6","2","5" },
              {"3","8","1","4","7","5","2","9","6" },
              {"6","2","4","3","9","8","7","5","1" },
              {"9","7","5","6","1","2","4","8","3" } };
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            BacktrackingSolver solver = new BacktrackingSolver(board);
            Assert.IsTrue(solver.CheckFinished());
        }

        [TestMethod]
        public void TestBacktrackingSolver()
        {
            string[,] boardSketch =
            { {"7","6","0v","0v","1","0v","0v","0v","0v"},
              {"1","4","0v","6","0v","0v","0v","0v","0v" },
              {"5","0v","0v","4","0v","0v","7","1","0v"},
              {"4","0v","0v","0v","2","3","0v","7","5"},
              {"2","0v","7","0v","0v","0v","3","9","8"},
              {"0v","8","6","0v","5","0v","0v","4","2" },
              {"0v","2","0v","0v","0v","6","0v","0v","7" },
              {"8","3","0v","0v","0v","0v","2","6","0v" },
              {"6","7","4","0v","9","0v","0v","5","0v" } };
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            BacktrackingSolver solver = new BacktrackingSolver(board);
            solver.Solve();
            Assert.IsFalse(solver.CheckInvalidFull());
            Assert.IsTrue(solver.CheckFinished());
        }

        [TestMethod]
        public void TestForwardChecker()
        {
            string[,] boardSketch =
            { {"7","6","0v","0v","1","0v","0v","0v","0v"},
              {"1","4","0v","6","0v","0v","0v","0v","0v" },
              {"5","0v","0v","4","0v","0v","7","1","0v"},
              {"4","0v","0v","0v","2","3","0v","7","5"},
              {"2","0v","7","0v","0v","0v","3","9","8"},
              {"0v","8","6","0v","5","0v","0v","4","2" },
              {"0v","2","0v","0v","0v","6","0v","0v","7" },
              {"8","3","0v","0v","0v","0v","2","6","0v" },
              {"6","7","4","0v","9","0v","0v","5","0v" } };
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            board.InitialiseQueue();
            ForwardChecker solver = new ForwardChecker(board);
            solver.Solve();
            Assert.IsFalse(solver.CheckInvalidFull());
            Assert.IsTrue(solver.CheckFinished());
        }

        [TestMethod]
        public void TestAnnealer()
        {
            string[,] boardSketch = GenerateBoardSketch(25);
            Board board = new Board("", boardSketch, 25);
            board.InitialiseGraph();
            Annealer solver = new Annealer(board);
            solver.Solve();
            Assert.IsFalse(solver.CheckInvalidFull());
            Assert.IsTrue(solver.CheckFinished());
        }

        [TestMethod]
        public void TestAPIUniqueGenerator()
        {
            string[,] boardSketch =
            { {"1","0v","0v","0v","2","8","4","3","0v" },
              {"2","0v","0v","3","0v","0v","8","0v","0v" },
              {"0v","0v","0v","1","0v","0v","6","0v","0v" },
              {"0v","0v","0v","5","7","0v","0v","0v","0v" },
              {"8","0v","0v","0v","0v","0v","2","0v","9" },
              {"0v","7","0v","9","0v","2","0v","5","0v" },
              {"0v","0v","1","4","0v","0v","0v","0v","0v" },
              {"0v","6","0v","0v","0v","0v","0v","0v","0v" },
              {"0v","0v","0v","2","0v","0v","5","4","6" }
            };
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            BoardGeneratorAPI generator = new BoardGeneratorAPI();
            generator.GenerateUniqueSolution(9, board);
            Assert.AreEqual(1, board.SolutionCount);
        }

        [TestMethod]
        public void TestGeneralUniqueGenerator9x9()  // not strictly used in program but nice for completeness
        {
            string[,] boardSketch = GenerateBoardSketch(9);
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            UniqueBoardGenerator generator = new();
            generator.GenerateUniqueSolution(9, board);
            Assert.AreEqual(1, board.SolutionCount);
        }

        [TestMethod]
        public void TestGeneralUniqueGenerator16x16()
        {
            string[,] boardSketch = GenerateBoardSketch(16);
            Board board = new Board("", boardSketch, 16);
            board.InitialiseGraph();
            UniqueBoardGenerator generator = new();
            generator.GenerateUniqueSolution(16, board);
            Assert.AreEqual(1, board.SolutionCount);
        }

        [TestMethod]
        public void TestMoveStackPush()
        {
            MoveStack stack = new MoveStack(5);
            stack.Push(new Move(new Cell((0, 1), 3), 3));
            stack.Push(new Move(new Cell((0, 2), 5), 5));
            Assert.AreEqual(2, stack.Count);
            Assert.ReferenceEquals(new Move(new Cell((0,2),5),5), stack.StackArray[1]);
            stack.Pop();
            Assert.AreEqual(1, stack.Count);
            Move move = stack.Pop();
            Assert.ReferenceEquals(new Move(new Cell((0, 1), 3), 3), move);
        }

        [TestMethod]
        public void TestMoveStackPop()
        {
            MoveStack stack = new MoveStack(5);
            stack.Push(new Move(new Cell((0, 1), 3), 3));
            stack.Push(new Move(new Cell((0, 2), 5), 5));
            stack.Pop();
            Assert.AreEqual(1, stack.Count);
            Move move = stack.Pop();
            Assert.ReferenceEquals(new Move(new Cell((0, 1), 3), 3), move);
        }

        [TestMethod]
        public void TestMoveStackThrowsExceptionWhenPopEmpty()
        {
            MoveStack stack = new MoveStack(5);
            Assert.ThrowsException<InvalidOperationException>(() => stack.Pop());
        }

        [TestMethod]
        public void TestPriorityQueue()
        {
            Cell newCell1 = new Cell((0, 1), 1);
            newCell1.InitialiseDomain(5);   // sets domain of cells to a particular size
            Cell newCell2 = new Cell((0, 2), 2);
            newCell2.InitialiseDomain(4);
            Cell newCell3 = new Cell((0, 3), 3);
            newCell3.InitialiseDomain(2);

            HeapPriorityQueue queue = new HeapPriorityQueue(new List<Cell> { newCell1, newCell2, newCell3});
            queue.Enqueue(newCell1);
            queue.Enqueue(newCell2);
            queue.Enqueue(newCell3);
            Assert.ReferenceEquals(new Cell((0, 3), 3), queue.Dequeue());
            Assert.AreEqual(2, queue.Occupied);;
            Assert.ReferenceEquals(new Cell((0,2),2), queue.Dequeue());
        }

        [TestMethod]
        public void TestPriorityQueueThrowsExceptionWhenDequeueEmpty()
        {
            HeapPriorityQueue queue = new HeapPriorityQueue(new List<Cell>());
            Assert.ThrowsException<InvalidOperationException>(() => queue.Dequeue());
        }

        private string[,] GenerateBoardSketch(int dimensions)
        {
            string[,] sketch = new string[dimensions,dimensions];
            for (int i=0; i<dimensions; i++)
            {
                for (int j=0; j<dimensions; j++)
                {
                    sketch[i, j] = "0v";
                }
            }
            return sketch;
        }
    }
}
