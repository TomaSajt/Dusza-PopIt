using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopIt.Data;
using PopIt.Exception;
using System.Collections.Generic;
using System.Linq;

namespace PopIt.Test
{
    [TestClass]
    public class BoardUtilsTests
    {
        [TestMethod]
        public void AreComponentsBroken_WhenBroken_True()
        {
            var board = new Board(4, 4);
            for (int i = 0; i < 4; i++)
            {
                board[i, 0].Char = 'a';
                board[i, 1].Char = 'b';
                board[i, 2].Char = 'c';
                board[i, 3].Char = 'a';
            }
            var result = BoardUtils.AreComponentsBroken(board);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void AreComponentsBroken_WhenNotBroken_False()
        {
            var board = new Board(4, 4);
            for (int i = 0; i < 4; i++)
            {
                board[i, 0].Char = 'a';
                board[i, 1].Char = 'b';
                board[i, 2].Char = 'c';
                board[i, 3].Char = 'd';
            }
            var result = BoardUtils.AreComponentsBroken(board);
            Assert.IsFalse(result);
        }


        [TestMethod]
        public void IsBoardBroken_WhenBroken_True()
        {
            var board = new Board(4, 4);
            for (int i = 0; i < 4; i++)
            {
                board[i, 0].Char = 'a';
                board[i, 1].Char = '.';
                board[i, 2].Char = 'b';
                board[i, 3].Char = '.';
            }
            var result = BoardUtils.IsBoardBroken(board);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsBoardBroken_WhenNotBroken_False()
        {
            var board = new Board(4, 4);
            for (int i = 0; i < 4; i++)
            {
                board[i, 0].Char = '.';
                board[i, 1].Char = 'a';
                board[i, 2].Char = 'b';
                board[i, 3].Char = '.';
            }
            var result = BoardUtils.IsBoardBroken(board);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void FindFirstValidPos_WhenEmpty_Throws()
        {
            var board = new Board(4, 4);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    board[i, j].Char = '.';
                }
            }
            Assert.ThrowsException<InvalidBoardFormatException>(() => BoardUtils.FindFirstValidPos(board));
        }
        [TestMethod]
        public void FindFirstValidPos_WhenOneNotEmpty_Returns_Same()
        {
            var board = new Board(4, 4);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    board[i, j].Char = '.';
                }
            }
            board[2, 3].Char = 'a';
            var res = BoardUtils.FindFirstValidPos(board);
            Assert.AreEqual(res, new(2, 3));
        }
        [TestMethod]
        public void IsInBounds_WhenInBounds_True()
        {
            var board = new Board(4, 4);
            var res1 = BoardUtils.IsInBounds(board, 0, 0);
            var res2 = BoardUtils.IsInBounds(board, 2, 2);
            var res3 = BoardUtils.IsInBounds(board, 0, 3);
            var res4 = BoardUtils.IsInBounds(board, 3, 3);
            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
            Assert.IsTrue(res3);
            Assert.IsTrue(res4);
        }
        [TestMethod]
        public void IsInBounds_WhenOutOfBounds_False()
        {
            var board = new Board(4, 4);
            var res1 = BoardUtils.IsInBounds(board, -100, 100);
            var res2 = BoardUtils.IsInBounds(board, 2, -1);
            var res3 = BoardUtils.IsInBounds(board, -10, 3);
            var res4 = BoardUtils.IsInBounds(board, 3, 4);
            var res5 = BoardUtils.IsInBounds(board, 10, 5);
            Assert.IsFalse(res1);
            Assert.IsFalse(res2);
            Assert.IsFalse(res3);
            Assert.IsFalse(res4);
            Assert.IsFalse(res5);
        }
        [TestMethod]
        public void GetNeighbourPositions_Test()
        {
            static bool Eq(IEnumerable<Point> a, params Point[] b) => a.ToHashSet().SetEquals(b);
            var board = new Board(6, 4);
            var n1 = BoardUtils.GetNeighboursPositions(board, 0, 0);
            var n2 = BoardUtils.GetNeighboursPositions(board, 3, 3);
            var n3 = BoardUtils.GetNeighboursPositions(board, 5, 0);
            var n4 = BoardUtils.GetNeighboursPositions(board, 5, 3);
            var n5 = BoardUtils.GetNeighboursPositions(board, 2, 1);
            var res1 = Eq(n1, new(0, 1), new(1, 0));
            var res2 = Eq(n2, new(2, 3), new(4, 3), new(3, 2));
            var res3 = Eq(n3, new(4, 0), new(5, 1));
            var res4 = Eq(n4, new(5, 2), new(4, 3));
            var res5 = Eq(n5, new(2, 2), new(2, 0), new(1, 1), new(3, 1));
            Assert.IsTrue(res1);
            Assert.IsTrue(res2);
            Assert.IsTrue(res3);
            Assert.IsTrue(res4);
            Assert.IsTrue(res5);
        }

    }
}