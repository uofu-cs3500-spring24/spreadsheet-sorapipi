using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using SpreadsheetUtilities;
using System;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Xml;
using System.Xml.Linq;

namespace SpreadsheetTests
{

    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>

    /// <summary>
    /// Author:    YINGHAO CHEN
    /// Partner:   -NONE-
    /// Date:      11/02/2024
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and YINGHAO CHEN - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, YINGHAO CHEN, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents
    ///
    ///    This MSTest is used to test the spreadsheet
    /// </summary>


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
            ss.SetContentsOfCell(null, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsInvalidNameException()
        {
            ss.SetContentsOfCell("1A", "test");
        }

        [TestMethod]
        public void TestSetCellContentsString()
        {
            ss.SetContentsOfCell("A1", "test");
            Assert.AreEqual("test", ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestSetCellContentsDouble()
        {
            ss.SetContentsOfCell("A1", "1.23");
            Assert.AreEqual(1.23, ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestSetCellContentsFormula()
        {
            Formula formula = new Formula("1+1");
            ss.SetContentsOfCell("A1", "=1+1");
            Assert.AreEqual(formula, ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestSetCellContentsEmptyString()
        {
            ss.SetContentsOfCell("A1", "test");
            ss.SetContentsOfCell("A1", "");
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
            ss.SetContentsOfCell("A1", "test");
            ss.SetContentsOfCell("B1", "1.23");
            ss.SetContentsOfCell("C1", "=1+1");
            var nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
            CollectionAssert.AreEquivalent(new[] { "A1", "B1", "C1" }, (System.Collections.ICollection?)nonEmptyCells);
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsCirculaException()
        {
            ss.SetContentsOfCell("A1", "=B1");
            ss.SetContentsOfCell("B1", "=A1");
        }

        [TestMethod]
        public void TestSetCellContentsFormula1()
        {
            ss.SetContentsOfCell("A1", "=B1 + 1");
            Assert.IsTrue(ss.GetCellContents("A1") is Formula);
        }


        [TestMethod]
        public void TestSetCellContentsUpdate()
        {
            ss.SetContentsOfCell("B1", "1.0");
            ss.SetContentsOfCell("A1", "=B1 + 3");
            ss.SetContentsOfCell("B1", "2.0");
            var toRecalculate = ss.SetContentsOfCell("B1", "5.0");
            Assert.IsTrue(toRecalculate.Contains("A1"));
        }


        [TestMethod]
        public void TestGetCellContents()
        {
            ss.SetContentsOfCell("A1", "Hello");
            ss.SetContentsOfCell("A1", "1.0");
            ss.SetContentsOfCell("A1", "=2 + 2");
            Assert.IsTrue(ss.GetCellContents("A1") is Formula);
        }

        [TestMethod]
        public void TestSetCellContents()
        {
            ss.SetContentsOfCell("A1", "B1");
        }

        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsCircularException()
        {
            ss.SetContentsOfCell("B1", "=A1 + 1");
            ss.SetContentsOfCell("A1", "=B1 + 1");
        }


        [TestMethod]
        public void TestSetCellContents1()
        {
            ss.SetContentsOfCell("A1", "123s");
            Assert.AreEqual("123s", ss.GetCellContents("A1"));
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
            ss.SetContentsOfCell("A1", "Original");
            ss.SetContentsOfCell("A1", "Changed");
            Assert.AreEqual("Changed", ss.GetCellContents("A1"));
        }

        [TestMethod]
        public void TestGetNamesOfAllNonemptyCells1()
        {
            ss.SetContentsOfCell("A1", "Non-Empty cells");
            ss.SetContentsOfCell("A1", "");
            var nonEmptyCells = ss.GetNamesOfAllNonemptyCells();
            CollectionAssert.Contains(nonEmptyCells.ToList(), "A1");
        }

        [TestMethod]
        public void TestSetCellContentsCircularException1()
        {
            Assert.ThrowsException<CircularException>(() => ss.SetContentsOfCell("A1", "=A1"));
        }

        [TestMethod]
        public void TestSetCellContents3()
        {
            ss.SetContentsOfCell("B1", "5.0");
            ss.SetContentsOfCell("A1", "=B1 * 2");
            Assert.AreEqual(new Formula("B1 * 2"), ss.GetCellContents("A1"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsInvalidNameException1()
        {
            var ss = new Spreadsheet();
            ss.SetContentsOfCell(null, "=A1 + A2");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsInvalidNameException2()
        {
            var ss = new Spreadsheet();
            ss.SetContentsOfCell("123", "2.0");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsNullNameException1()
        {
            var ss = new Spreadsheet();
            ss.SetContentsOfCell(null, "1.0");
        }

        [TestMethod]
        public void TestCellClass()
        {
            var spreadsheet = new Spreadsheet();
            spreadsheet.SetContentsOfCell("A1", "123.45");

            var value = spreadsheet.GetCellValue("A1");
            Assert.AreEqual(123.45, value);
        }

        [TestMethod]
        public void TestConstructor2()
        {
            Func<string, bool> isValid = s => true; 
            Func<string, string> normalize = s => s.ToUpper(); 
            string version = "1.0";

            Spreadsheet ss = new Spreadsheet(isValid, normalize, version);


            Assert.AreEqual(version, ss.Version);

        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestConstructorException()
        {
            string filename = "versionMismatchSpreadsheet.xml";

            using (XmlWriter writer = XmlWriter.Create(filename)) 
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "1.0"); 

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            try
            {
                Spreadsheet ss = new Spreadsheet(filename, s => true, s => s, "2.0");
            }
            finally
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }
        }

        [TestMethod]
        public void TestConstructor()
        {
            string filename = "save.txt";
            using (XmlWriter writer = XmlWriter.Create(filename))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");
                writer.WriteAttributeString("version", "1.0");

                writer.WriteStartElement("cell");
                writer.WriteElementString("name", "A1");
                writer.WriteElementString("contents", "hello");
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            try
            {
                Spreadsheet ss = new Spreadsheet(filename, s => true, s => s, "1.0");

                Assert.AreEqual("hello", ss.GetCellContents("A1"));
            }
            finally
            {
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestGetCellValueInvalidNameException()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.GetCellValue("1Invalid"); 
        }

        [TestMethod]
        public void TestGetCellValueEmptyString()
        {
            Spreadsheet ss = new Spreadsheet();
            var result = ss.GetCellValue("A10"); 
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void TestGetCellValue()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "2");
            ss.SetContentsOfCell("A2", "=A1*2");
            var result = ss.GetCellValue("A2");
            Assert.AreEqual(4.0, result);
        }
        [TestMethod]
        public void TestGetCellValueFormulaError()
        {
            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "hello"); 
            ss.SetContentsOfCell("A2", "=A1*2"); 

            var result = ss.GetCellValue("A2");
            Assert.IsTrue(result is FormulaError);
        }

        [TestMethod]
        public void TestSave()
        {

            var ss = new Spreadsheet();
            string tempFile = Path.GetTempFileName();
            ss.SetContentsOfCell("A1", "hello");


            ss.Save(tempFile);


            Assert.IsFalse(ss.Changed);


            var doc = XDocument.Load(tempFile);
            var cell = doc.Descendants("cell").FirstOrDefault();
            Assert.IsNotNull(cell);
            Assert.AreEqual("A1", cell.Element("name")?.Value);
            Assert.AreEqual("hello", cell.Element("contents")?.Value);


            File.Delete(tempFile);
        }

        [TestMethod]
        public void TestGetXML()
        {
            Spreadsheet ss = new Spreadsheet(s => true, s => s, "1.0");
            ss.SetContentsOfCell("A1", "1");
            ss.SetContentsOfCell("B1", "2"); 
            ss.SetContentsOfCell("C1", "=A1+B1"); 

            string xml = ss.GetXML();


            Assert.IsFalse(string.IsNullOrEmpty(xml));

            XDocument doc = XDocument.Parse(xml);
            Assert.AreEqual("1.0", (string)doc.Root.Attribute("version"));

            var cells = doc.Descendants("cell").ToList();
            Assert.AreEqual(3, cells.Count);

            var cellA1 = cells.FirstOrDefault(c => c.Element("name")?.Value == "A1");
            Assert.IsNotNull(cellA1);
            Assert.AreEqual("1", cellA1.Element("contents")?.Value);

            var cellB1 = cells.FirstOrDefault(c => c.Element("name")?.Value == "B1");
            Assert.IsNotNull(cellB1);
            Assert.AreEqual("2", cellB1.Element("contents")?.Value);

            var cellC1 = cells.FirstOrDefault(c => c.Element("name")?.Value == "C1");
            Assert.IsNotNull(cellC1);
            Assert.AreEqual("A1+B1", cellC1.Element("contents")?.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestGetSavedVersionSpreadsheetReadWriteException()
        {
            Spreadsheet ss = new Spreadsheet(s => true, s => s, "1.0");
            string nonexistentFilePath = "nonexistent.xml";

            ss.GetSavedVersion(nonexistentFilePath);
        }


        [TestMethod]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void TestSaveSpreadsheetReadWriteException()
        {
            Spreadsheet ss = new Spreadsheet(s => true, s => s, "1.0");
            ss.SetContentsOfCell("A1", "Test");

            string filename = "invalid/save.xml";


            ss.Save(filename);


        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestSetContentsOfCellFormulaFormatException()
        {
 
            var ss = new Spreadsheet();
            string name = "A1";
            string invalidFormula = "=2 * * 3"; 

     
            ss.SetContentsOfCell(name, invalidFormula); 


        }

        [TestMethod]
        public void TestGetCellValueFormulaError1()
        {

            Spreadsheet ss = new Spreadsheet();
            ss.SetContentsOfCell("A1", "a number"); 
            ss.SetContentsOfCell("B1", "=A1 * 2"); 


            var result = ss.GetCellValue("B1");

            Assert.IsInstanceOfType(result, typeof(FormulaError));
        }

    }
}