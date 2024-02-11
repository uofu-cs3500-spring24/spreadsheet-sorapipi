using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System;
using System.Reflection;

namespace SpreadsheetTests
{


    [TestClass]
    public class SpreadsheetTests
    {



        Spreadsheet ss;

        [TestInitialize]
        public void TestInitialize()
        {
            ss = new Spreadsheet();
        }

        [TestMethod]
        public void TestEmptySpreadsheet()
        {
            int nonEmptyCount = ss.GetNamesOfAllNonemptyCells().Count();
            Assert.AreEqual(0, nonEmptyCount);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNullNameException()
        {
            ss.SetCellContents(null, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsInvalidNameException()
        {
            ss.SetCellContents("1A", "test");
        }

        [TestMethod]
        public void TestSetCellContentsString()
        {
            ss.SetCellContents("A1", "test");
            Assert.AreEqual("test", ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestSetCellContentsDouble()
        {
            ss.SetCellContents("A1", 1.23);
            Assert.AreEqual(1.23, ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestSetCellContentsFormula()
        {
            Formula formula = new Formula("1+1");
            ss.SetCellContents("A1", formula);
            Assert.AreEqual(formula, ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestSetCellContentsEmptyString()
        {
            ss.SetCellContents("A1", "test");
            ss.SetCellContents("A1", "");
            Assert.AreEqual("", ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestGetCellContentsForEmpty()
        {
            Assert.AreEqual("", ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells()
        {
            ss.SetCellContents("A1", "test");
            ss.SetCellContents("B1", 1.23);
            ss.SetCellContents("C1", new Formula("1+1"));
            var nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
            CollectionAssert.AreEquivalent(new[] { "A1", "B1", "C1" }, (System.Collections.ICollection?)nonEmptyCells);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsCirculaException()
        {
            ss.SetCellContents("A1", new Formula("B1"));
            ss.SetCellContents("B1", new Formula("A1"));
        }

        [TestMethod]
        public void TestSetCellContentsFormula1()
        {
            ss.SetCellContents("A1", new Formula("B1 + 1"));
            Assert.IsTrue(ss.GetCellContents("A1") is Formula);
        }


        [TestMethod]
        public void TestSetCellContentsUpdate()
        {
            ss.SetCellContents("B1", 1.0);
            ss.SetCellContents("A1", new Formula("B1 + 3"));
            ss.SetCellContents("B1", 2.0);
            var toRecalculate = ss.SetCellContents("B1", 5.0);
            Assert.IsTrue(toRecalculate.Contains("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentsNullFormulaException()
        {
            ss.SetCellContents("A1", null as Formula);
        }

        [TestMethod]
        public void TestGetCellContents()
        {
            ss.SetCellContents("A1", "Hello");
            ss.SetCellContents("A1", 1.0);
            ss.SetCellContents("A1", new Formula("2 + 2"));
            Assert.IsTrue(ss.GetCellContents("A1") is Formula);
        }

        [TestMethod]
        public void TestSetCellContents()
        {
            ss.SetCellContents("A1", new Formula("B1"));
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsCircularException()
        {
            ss.SetCellContents("B1", new Formula("A1 + 1"));
            ss.SetCellContents("A1", new Formula("B1 + 1"));
        }


        [TestMethod]
        public void TestSetCellContents1()
        {
            ss.SetCellContents("A1", "123");
            Assert.AreEqual("123", ss.GetCellContents("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsNullNameException()
        {
            ss.GetCellContents(null);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellContentsInvalidNameException()
        {
            ss.GetCellContents("Invalid!");
        }

        [TestMethod]
        public void TestSetCellContents2()
        {
            ss.SetCellContents("A1", "Original");
            ss.SetCellContents("A1", "Changed");
            Assert.AreEqual("Changed", ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells1()
        {
            ss.SetCellContents("A1", "Non-Empty cells");
            ss.SetCellContents("A1", "");
            var nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
            CollectionAssert.DoesNotContain(nonEmptyCells.ToList(), "A1");
        }

        [TestMethod]
        public void TestSetCellContentsCircularException1()
        {
            Assert.ThrowsException<CircularException>(() => ss.SetCellContents("A1", new Formula("A1")));
        }

        [TestMethod]
        public void TestSetCellContents3()
        {
            ss.SetCellContents("B1", 5.0);
            ss.SetCellContents("A1", new Formula("B1 * 2"));
            Assert.AreEqual(new Formula("B1 * 2"), ss.GetCellContents("A1"));
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetDirectDependentsNullNameException()
        {
            var ssTest = new SpreadsheetTestWrapper();
            ssTest.GetDirectDependentsWrapper(null); 
        }

        public class SpreadsheetTestWrapper : Spreadsheet
        {
            public IEnumerable<string> GetDirectDependentsWrapper(string name)
            {
                return this.GetDirectDependents(name);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetDirectDependentsInvalidNameException()
        {
            var ssTest = new SpreadsheetTestWrapper();
            ssTest.GetDirectDependentsWrapper("123Invalid");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsInvalidNameException1()
        {
            var ss = new Spreadsheet();
            ss.SetCellContents(null, new Formula("A1 + A2"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentsNullTextException()
        {

            var ss = new Spreadsheet();
            ss.SetCellContents("A1", null as string);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsInvalidNameException2()
        {
            var ss = new Spreadsheet();
            ss.SetCellContents("123", 2.0);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNullNameException1()
        {
            var ss = new Spreadsheet();
            ss.SetCellContents(null, 1.0);
        }

        [TestMethod]
        public void TestCellClass()
        {
            var cell = new Spreadsheet.Cell("Test content"); 
            cell.Value = "Test value";
            var actualValue = cell.Value;
            Assert.AreEqual("Test value", actualValue);
        }







    }
}