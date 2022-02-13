using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopIt.Data;
using PopIt.Exception;

namespace PopIt.Test
{
    [TestClass]
    public class BoardUtilsTests
    {
        [TestMethod]
        public void AreComponentsBroken_WhenBroken_Returns_True()
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
        public void AreComponentsBroken_WhenNotBroken_Returns_False()
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
        public void IsBoardBroken_WhenBroken_Returns_True()
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
        public void IsBoardBroken_WhenNotBroken_Returns_False()
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
    }
}