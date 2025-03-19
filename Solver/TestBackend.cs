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
            string[,] boardSketch = GenerateEmptyBoardSketch(9);
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            // checks whether a node has the correct number of connected nodes for 9x9 boards
            foreach (Cell cell in board.AdjacencyList.Keys)
            {
                Assert.AreEqual(20, board.AdjacencyList[cell].Count);
            }
        }

        [TestMethod]
        public void TestGraph16x16()
        {
            string[,] boardSketch = GenerateEmptyBoardSketch(16);
            Board board = new Board("", boardSketch, 16);
            board.InitialiseGraph();
            // checks whether a node has the correct number of connected nodes for 16x16 boards
            foreach (Cell cell in board.AdjacencyList.Keys)
            {
                Assert.AreEqual(39, board.AdjacencyList[cell].Count);
            }
        }

        [TestMethod]
        public void TestGraph25x25()
        {
            string[,] boardSketch = GenerateEmptyBoardSketch(25);
            Board board = new Board("", boardSketch, 25);
            board.InitialiseGraph();
            // checks whether a node has the correct number of connected nodes for 25x25 boards
            foreach (Cell cell in board.AdjacencyList.Keys)
            {
                Assert.AreEqual(64, board.AdjacencyList[cell].Count);
            }
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
            // checks correctness of the CheckInvalidFull() function
            // correctness of CheckInvalidFull() also validates the CheckInvalid() function for a particular MostRecentlyChangedCell object
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
            // checks correctness of the CheckInvalidFull() function
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
            // checks correctness of the CheckFinished() function
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
            // checks correctness of the CheckFinished() function
            Assert.IsTrue(solver.CheckFinished());
        }

        [TestMethod]
        public void TestMoveStackPushPop()
        {
            MoveStack stack = new MoveStack(5);
            stack.Push(new Move(new Cell((0, 1), 3), 3));
            stack.Push(new Move(new Cell((0, 2), 5), 5));
            // checks if Push() method operates as expected
            Assert.AreEqual(2, stack.Count);
            Assert.AreEqual((0,2), stack.StackArray[1].Cell.Position);
            Assert.AreEqual(5, stack.StackArray[1].OldEntry);
            stack.Pop();
            // check if Pop() method operates as expected
            Assert.AreEqual(1, stack.Count);
            Move move = stack.Pop();
            Assert.AreEqual((0, 1), move.Cell.Position);
            Assert.AreEqual(3, move.OldEntry);
        }

        [TestMethod]
        public void TestMoveStackThrowsExceptionWhenPopEmpty()
        {
            // checks if Pop() method throws an exception when trying to pop from an empty stack
            MoveStack stack = new MoveStack(5);
            Assert.ThrowsException<InvalidOperationException>(() => stack.Pop());
        }
        [TestMethod]
        public void TestPriorityQueueEnqueueDequeue()
        {
            // initialises cells with different numbers of elements in their domains
            Cell cell1 = new Cell((0, 0), 3);
            cell1.InitialiseDomain(3);
            Cell cell2 = new Cell((0, 1), 4);
            cell2.InitialiseDomain(4);
            Cell cell3 = new Cell((0, 2), 1);
            cell3.InitialiseDomain(1);
            List<Cell> cellList = new() { cell1, cell2, cell3 };
            HeapPriorityQueue queue = new(cellList);
            // enqueues all cells
            queue.Enqueue(cell1);
            queue.Enqueue(cell2);
            // checks whether the element at the top of the queue is the one with the smallest domain
            // the domain count can be used as each cell was initialised with a unique domain size
            Assert.AreEqual(3, queue.NodeArray[0].Domain.Count);
            queue.Enqueue(cell3);
            // tests if occupied property is working properly
            Assert.AreEqual(3, queue.Occupied);
            // tests dequeue operation
            Cell dequeuedCell = queue.Dequeue();
            Assert.AreEqual(1, dequeuedCell.Domain.Count);
            dequeuedCell = queue.Dequeue();
            Assert.AreEqual(3, dequeuedCell.Domain.Count);
            Assert.AreEqual(1, queue.Occupied);
        }

        [TestMethod]
        public void TestPriorityQueueThrowsExceptionWhenDequeueEmptyQueue()
        {
            HeapPriorityQueue queue = new(new List<Cell>());
            Assert.ThrowsException<InvalidOperationException>(() => queue.Dequeue());
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
            // validates functionality of the base backtracking solver
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
            // validates functionality of the forward checking solver
            ForwardChecker solver = new ForwardChecker(board);
            solver.Solve();
            Assert.IsFalse(solver.CheckInvalidFull());
            Assert.IsTrue(solver.CheckFinished());
        }

        [TestMethod]
        public void TestAnnealer()
        {
            string[,] boardSketch = GenerateEmptyBoardSketch(25);
            Board board = new Board("", boardSketch, 25);
            board.InitialiseGraph();
            // validates functionality of the simulated annealing solver
            Annealer solver = new Annealer(board);
            solver.Solve();
            Assert.IsFalse(solver.CheckInvalidFull());
            Assert.IsTrue(solver.CheckFinished());
        }

        [TestMethod]
        public async Task TestAPIUniqueGenerator()
        {
            BoardGeneratorAPI generator = new();
            // generates a board from the API
            List<Board> boards = await generator.GenerateBoard();
            string[,] boardSketch = boards[0].BoardSketch;
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            // validates functionality of the first unique solution generator (comparing solutions) for 9x9 boards
            generator.GenerateUniqueSolution(9, board);
            // checks for uniqueness of solution
            Assert.AreEqual(1, board.SolutionCount);
        }

        [TestMethod]
        public void TestGeneralUniqueGenerator9x9()  // not strictly used in program but nice for completeness
        {
            string[,] boardSketch = GenerateEmptyBoardSketch(9);
            Board board = new Board("", boardSketch, 9);
            board.InitialiseGraph();
            // validates functionality of the second unique solution generator (backtracking) for 9x9 boards
            UniqueBoardGenerator generator = new();
            generator.GenerateUniqueSolution(9, board);
            Assert.AreEqual(1, board.SolutionCount);
        }

        [TestMethod]
        public void TestGeneralUniqueGenerator16x16()
        {
            string[,] boardSketch = GenerateEmptyBoardSketch(16);
            Board board = new Board("", boardSketch, 16);
            board.InitialiseGraph();
            // validates functionality of the second unique solution generator (backtracking) for 16x16 boards
            UniqueBoardGenerator generator = new();
            generator.GenerateUniqueSolution(16, board);
            Assert.AreEqual(1, board.SolutionCount);
        }

        private string[,] GenerateEmptyBoardSketch(int dimensions)
        {
            // generates an board sketch of specified size, with all cells empty and variable
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
