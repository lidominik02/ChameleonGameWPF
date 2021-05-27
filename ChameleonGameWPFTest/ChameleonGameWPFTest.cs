using ChameleonGameWPF.Model;
using ChameleonGameWPF.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace CameleonWPFTest
{
    [TestClass]
    public class ChameleonGameWPFTest
    {
        [TestClass]
        public class CameleonTest
        {
            private ChameleonModel _model;
            private ChameleonTable _mockedTable;
            private Mock<IDataAccess> _mock;

            [TestInitialize]
            public void Initialize()
            {
                _mockedTable = new ChameleonTable();
                _mockedTable.Step(0, 1, 1, 1);
                _mockedTable.Step(2, 1, 0, 1);
                _mockedTable.CurrentChameleon = 2;

                _mock = new Mock<IDataAccess>();
                _mock.Setup(mock => mock.LoadAsync(It.IsAny<String>()))
                    .Returns(() => Task.FromResult(_mockedTable));

                _model = new ChameleonModel(_mock.Object);

                _model.GameOver += Model_GameOver;
            }

            private void Model_GameOver(object sender, ChameleonEventArgs e)
            {
                int numOfRC = 0;
                int numOfGC = 0;
                for (int i = 0; i < _model.Size; i++)
                {
                    for (int j = 0; j < _model.Size; j++)
                    {
                        if (_model.Table[i, j] != 0)
                        {
                            if (_model.Table[i, j] == 1)
                                numOfRC++;
                            else if (_model.Table[i, j] == 2)
                                numOfGC++;
                        }
                    }
                }
                Assert.IsTrue((numOfGC == 0) || (numOfRC == 0));
                int winner = (numOfRC == 0) ? 2 : 1;
                Assert.AreEqual(winner, e.WinnerChameleon);
            }

            [TestMethod]
            public void CameleonStepTest()
            {
                _model.NewGame(3);

                //nem valid esetek
                Assert.IsFalse(_model.Step(0, 0, 0, 0));
                Assert.IsFalse(_model.Step(0, 0, 0, 1));
                Assert.IsFalse(_model.Step(0, 0, 0, 2));

                Assert.IsFalse(_model.Step(0, 0, 1, 1));
                Assert.AreEqual(_model.Table[0, 0], 1);
                Assert.AreEqual(_model.Table[1, 1], 0);

                Assert.IsFalse(_model.Step(0, 0, 1, 2));
                Assert.AreEqual(_model.Table[0, 0], 1);
                Assert.AreEqual(_model.Table[1, 2], 2);

                Assert.IsFalse(_model.Step(0, 0, 2, 2));
                Assert.AreEqual(_model.Table[0, 0], 1);
                Assert.AreEqual(_model.Table[2, 2], 2);

                Assert.IsFalse(_model.Step(2, 2, 0, 0));
                Assert.AreEqual(_model.Table[2, 2], 2);
                Assert.AreEqual(_model.Table[0, 0], 1);

                Assert.IsFalse(_model.Step(2, 2, 2, 1));

                //Valid lépések
                Assert.IsTrue(_model.Step(0, 1, 1, 1));
                Assert.AreEqual(_model.Table[0, 1], 0);
                Assert.AreEqual(_model.Table[1, 1], 1);

                Assert.IsFalse(_model.Step(1, 1, 0, 1));

                Assert.IsTrue(_model.Step(2, 1, 0, 1));
                Assert.AreEqual(_model.Table[2, 1], 0);
                Assert.AreEqual(_model.Table[0, 1], 2);
                Assert.AreEqual(_model.Table[1, 1], 0);

                Assert.IsTrue(_model.Step(1, 0, 1, 1));
                Assert.AreEqual(_model.Table[1, 0], 0);
                Assert.AreEqual(_model.Table[1, 1], 1);
                Assert.AreEqual(_model.Table[0, 1], 1);

                Assert.IsTrue(_model.Step(1, 2, 1, 0));
                Assert.IsTrue(_model.Step(0, 1, 1, 1));
                Assert.IsTrue(_model.Step(2, 0, 2, 1));
                Assert.IsTrue(_model.Step(0, 2, 1, 2));
                Assert.IsTrue(_model.Step(2, 1, 0, 1));
                Assert.IsTrue(_model.Step(1, 0, 1, 1));
                Assert.IsTrue(_model.Step(1, 2, 1, 0));
                Assert.IsTrue(_model.Step(0, 1, 0, 2));
                Assert.IsTrue(_model.Step(2, 2, 1, 2));
                Assert.IsTrue(_model.Step(0, 2, 2, 2));
            }

            [TestMethod]
            public void CameleonNewGameTest()
            {
                _model.NewGame(3);
                Assert.AreEqual(3, _model.Size);
                Assert.AreEqual("Piros kaméleon", _model.CurrentChameleon);
                _model.NewGame(5);
                Assert.AreEqual(5, _model.Size);
                Assert.AreEqual("Piros kaméleon", _model.CurrentChameleon);
                _model.NewGame(7);
                Assert.AreEqual(7, _model.Size);
                Assert.AreEqual("Piros kaméleon", _model.CurrentChameleon);
            }

            [TestMethod]
            public async Task CameleonLoadTest()
            {
                // kezdünk egy új játékot
                _model.NewGame(5);

                // majd betöltünk egy játékot
                await _model.LoadGameAsync(String.Empty);

                Assert.AreEqual(_mockedTable.CurrentChameleon, _model.Table.CurrentChameleon);
                for (Int32 i = 0; i < 3; i++)
                    for (Int32 j = 0; j < 3; j++)
                    {
                        Assert.AreEqual(_mockedTable.GetValue(i, j), _model.Table.GetValue(i, j));
                    }
                // ellenõrizzük, hogy meghívták-e a Load mûveletet a megadott paraméterrel
                _mock.Verify(dataAccess => dataAccess.LoadAsync(String.Empty), Times.Once());
            }
        }
    }
}
