using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SpreadsheetUtilities;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SS
{

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
    /// File Contents:
    ///
    ///This library class build a spreadsheet
    ///this class implements the AbstractSpreadsheet and 
    /// also in this class I reuse or incorporate code I have already created. 
    /// and also this class is used to re-evaluate cells based on dependencies.
    /// and this class is "internal" of the spreadsheet program
    /// This class should inherit from AbstractSpreadsheet and implement its 
    /// abstract methods so that it satisfies their specifications.
    /// It should also provide a zero-argument constructor that creates an empty spreadsheet. 
    /// and also can not add any other public fields, constructors, methods, or properties to Spreadsheet
    /// </summary>
    /// 

    public class Spreadsheet : AbstractSpreadsheet
    {


        private Dictionary<string, Cell> cells;
        private DependencyGraph dependencyGraph;
        private bool changed;

        /// <summary>
        /// default Constructor
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
            changed = false;
        }
        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  
        /// </summary>
        /// 
        /// <remarks>
        ///   The variable validity test is used throughout to determine whether a string that consists of 
        ///   one or more letters followed by one or more digits is a valid cell name.  The variable
        ///   equality test should be used throughout to determine whether two variables are equal.
        /// </remarks>
        /// 
        /// <param name="isValid">   defines what valid variables look like for the application</param>
        /// <param name="normalize"> defines a normalization procedure to be applied to all valid variable strings</param>
        /// <param name="version">   defines the version of the spreadsheet (should it be saved)</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
        : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
            changed = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filepath">where to open the file</param>
        /// <param name="isValid">defines what valid variables look like for the application</param>
        /// <param name="normalize">defines a normalization procedure to be applied to all valid variable strings</param>
        /// <param name="version">defines the version of the spreadsheet (should it be saved)</param>
        /// <exception cref="SpreadsheetReadWriteException"></exception>
        public Spreadsheet(string filepath, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
            Changed = false;

            if (!(GetSavedVersion(filepath).Equals(version)))
                throw new SpreadsheetReadWriteException("Version does not match");

            ReadFile(filepath);
        }
        /// <summary>
        /// a helper method to help read the file
        /// </summary>
        /// <param name="filepath">where to find the file</param>
        private void ReadFile(string filepath)
        {
            XDocument doc = XDocument.Load(filepath);
            foreach (XElement cellElement in doc.Descendants("cell"))
            {
                string name = cellElement.Element("name").Value;
                string content = cellElement.Element("contents").Value;
                SetContentsOfCell(name, content);
            }
        }

        /// <summary>
        /// cell class
        /// </summary>
        private class Cell
        {
            public object Content { get; set; }
            public object Value { get; set; }

            public Cell(object content)
            {
                Content = content;
                Value = content;
            }
        }
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get { return changed; }
            protected set { changed = value; }
        }

        /// <summary>
        ///   Returns the names of all non-empty cells.
        /// </summary>
        /// 
        /// <returns>
        ///     Returns an Enumerable that can be used to enumerate
        ///     the names of all the non-empty cells in the spreadsheet.  If 
        ///     all cells are empty then an IEnumerable with zero values will be returned.
        /// </returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }
        /// <summary>
        ///   Returns the contents (as opposed to the value) of the named cell.
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   Thrown if the name is invalid: blank/empty/""
        /// </exception>
        /// 
        /// <param name="name">The name of the spreadsheet cell to query</param>
        /// 
        /// <returns>
        ///   The return value should be either a string, a double, or a Formula.
        ///   See the class header summary 
        /// </returns>
        public override object GetCellContents(string name)
        {
            name = Normalize(name);
            if (name == null || !IsValidName(name))
            {
                throw new InvalidNameException();
            }

            if (cells.TryGetValue(name, out Cell cell))
            {
                return cell.Content;
            }
            else
            {
                return ""; // empty
            }
        }

        /// <summary>
        ///   <para>Sets the contents of the named cell to the appropriate value. </para>
        ///   <para>
        ///       First, if the content parses as a double, the contents of the named
        ///       cell becomes that double.
        ///   </para>
        ///
        ///   <para>
        ///       Otherwise, if content begins with the character '=', an attempt is made
        ///       to parse the remainder of content into a Formula.  
        ///       There are then three possible outcomes:
        ///   </para>
        ///
        ///   <list type="number">
        ///       <item>
        ///           If the remainder of content cannot be parsed into a Formula, a 
        ///           SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       </item>
        /// 
        ///       <item>
        ///           If changing the contents of the named cell to be f
        ///           would cause a circular dependency, a CircularException is thrown,
        ///           and no change is made to the spreadsheet.
        ///       </item>
        ///
        ///       <item>
        ///           Otherwise, the contents of the named cell becomes f.
        ///       </item>
        ///   </list>
        ///
        ///   <para>
        ///       Finally, if the content is a string that is not a double and does not
        ///       begin with an "=" (equal sign), save the content as a string.
        ///   </para>
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name parameter is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="SpreadsheetUtilities.FormulaFormatException"> 
        ///   If the content is "=XYZ" where XYZ is an invalid formula, throw a FormulaFormatException.
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name that is being changed</param>
        /// <param name="content"> The new content of the cell</param>
        /// 
        /// <returns>
        ///       <para>
        ///           This method returns a list consisting of the passed in cell name,
        ///           followed by the names of all other cells whose value depends, directly
        ///           or indirectly, on the named cell. The order of the list MUST BE any
        ///           order such that if cells are re-evaluated in that order, their dependencies 
        ///           are satisfied by the time they are evaluated.
        ///       </para>
        ///
        ///       <para>
        ///           For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///           list {A1, B1, C1} is returned.  If the cells are then evaluate din the order:
        ///           A1, then B1, then C1, the integrity of the Spreadsheet is maintained.
        ///       </para>
        /// </returns>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            if (!IsValid(name) || name == null) throw new InvalidNameException();
            if (content is null) throw new ArgumentNullException("content");
            name = Normalize(name);

            if (double.TryParse(content, out double number))
            {
                return SetCellContents(name, number);
            }
            else if (content.StartsWith("="))
            {
                Formula formula;
                try
                {
                    formula = new Formula(content.Substring(1), Normalize, IsValid);
                }
                catch (FormulaFormatException)
                {
                    throw;
                }

                return SetCellContents(name, formula);
            }
            else
            {
                return SetCellContents(name, content);
            }
        }

        /// <summary>
        ///  Set the contents of the named cell to the given number.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid: non-empty/not ""
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="number"> The new contents/value </param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, double number)
        {
            if (name == null || !IsValidName(name))
            {
                throw new InvalidNameException();
            }
            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell(number));
            else
                cells[name].Content = number;
            changed = true;
            List<string> dependents = dependencyGraph.GetDependents(name).ToList();
            dependents.Insert(0, name);
            return dependents;
        }

        /// <summary>
        /// Helper method to check if the cell's name is valid
        /// </summary>
        /// <param name="name"> cell content's name</param>
        /// <returns></returns>
        private bool IsValidName(string name)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$");
        }

        /// <summary>
        /// The contents of the named cell becomes the text.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid/non-empty ""
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>       
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, string text)
        {
            if (name == null || !IsValidName(name))
            {
                throw new InvalidNameException();
            }
            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell(text));
            else
                cells[name].Content = text;
            changed = true;
            List<string> dependents = dependencyGraph.GetDependents(name).ToList();
            dependents.Insert(0, name);
            return dependents;
        }
        /// <summary>
        /// Set the contents of the named cell to the formula.  
        /// </summary>
        /// 
        /// <requires> 
        ///   The name parameter must be valid/non-empty
        /// </requires>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <exception cref="CircularException"> 
        ///   If changing the contents of the named cell to be the formula would 
        ///   cause a circular dependency, throw a CircularException.  
        ///   (NOTE: No change is made to the spreadsheet.)
        /// </exception>
        /// 
        /// <param name="name"> The cell name</param>
        /// <param name="formula"> The content of the cell</param>
        /// 
        /// <returns>
        ///   <para>
        ///       This method returns a LIST consisting of the passed in name followed by the names of all 
        ///       other cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///
        ///   <para>
        ///       The order must correspond to a valid dependency ordering for recomputing
        ///       all of the cells, i.e., if you re-evaluate each cell in the order of the list,
        ///       the overall spreadsheet will be consistently updated.
        ///   </para>
        ///
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned, i.e., A1 was changed, so then A1 must be 
        ///     evaluated, followed by B1 re-evaluated, followed by C1 re-evaluated.
        ///   </para>
        /// </returns>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            IEnumerable<string> oldDependees = dependencyGraph.GetDependees(name).ToList();

            dependencyGraph.ReplaceDependees(name, formula.GetVariables());

            try
            {
                GetCellsToRecalculate(name);
            }
            catch
            {
                dependencyGraph.ReplaceDependees(name, oldDependees);
                throw new CircularException();
            }
            if (!cells.ContainsKey(name))
                cells.Add(name, new Cell(formula));
            else
                cells[name].Content = formula;

            changed = true;
            return new List<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell. 
        /// </summary>
        /// 
        /// <required>
        ///    The name must be valid upon entry to the function.
        /// </required>
        /// 
        /// <param name="name"></param>
        /// <returns>
        ///   Returns an enumeration, without duplicates, of the names of all cells that contain
        ///   formulas containing name.
        /// 
        ///   <para>For example, suppose that: </para>
        ///   <list type="bullet">
        ///      <item>A1 contains 3</item>
        ///      <item>B1 contains the formula A1 * A1</item>
        ///      <item>C1 contains the formula B1 + A1</item>
        ///      <item>D1 contains the formula B1 - C1</item>
        ///   </list>
        /// 
        ///   <para>The direct dependents of A1 are B1 and C1</para>
        /// 
        /// </returns>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {


            return dependencyGraph.GetDependents(name);
        }

        /// <summary>
        /// If name is invalid, throws an InvalidNameException.
        /// </summary>
        ///
        /// <exception cref="InvalidNameException"> 
        ///   If the name is invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell that we want the value of (will be normalized)</param>
        /// 
        /// <returns>
        ///   Returns the value (as opposed to the contents) of the named cell.  The return
        ///   value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </returns>
        public override object GetCellValue(string name)
        {
            name = Normalize(name);
            if (!IsValidName(name))
                throw new InvalidNameException();

            if (!cells.TryGetValue(name, out Cell cell))
                return "";
            if (cell.Content is Formula formula)
            {
                // for this method i used the deligate lookup
                Func<string, double> lookup = s =>
                {
                    s = Normalize(s);
                    if (GetCellValue(s) is double value)
                        return value;
                    else
                        throw new ArgumentException("value is not a number.");
                };

                return formula.Evaluate(lookup);
            }
            else
            {
                return cell.Content;
            }
        }

        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>cell name goes here</name>
        /// <contents>cell contents goes here</contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            try
            {
                var root = new XElement("spreadsheet", new XAttribute("version", Version));

                foreach (var cell in cells)
                {
                    string content;
                    if (cell.Value.Content is Formula)
                    {
                        content = "=" + cell.Value.Content.ToString();
                    }
                    else
                    {
                        content = cell.Value.Content.ToString();
                    }

                    root.Add(new XElement("cell",
                        new XElement("name", cell.Key),
                        new XElement("contents", content)));
                }
                var doc = new XDocument(root);

                doc.Save(filename);
                Changed = false;
            }
            catch (Exception ex)
            {
                throw new SpreadsheetReadWriteException("Failed to save");
            }
        }

        /// <summary>
        ///   Look up the version information in the given file. If there are any problems opening, reading, 
        ///   or closing the file, the method should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// 
        /// <remarks>
        ///   In an ideal world, this method would be marked static as it does not rely on an existing SpreadSheet
        ///   object to work; indeed it should simply open a file, lookup the version, and return it.  Because
        ///   C# does not support this syntax, we abused the system and simply create a "regular" method to
        ///   be implemented by the base class.
        /// </remarks>
        /// 
        /// <exception cref="SpreadsheetReadWriteException"> 
        ///   1Thrown if any problem occurs while reading the file or looking up the version information.
        /// </exception>
        /// 
        /// <param name="filename"> The name of the file (including path, if necessary)</param>
        /// <returns>Returns the version information of the spreadsheet saved in the named file.</returns>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                var doc = XDocument.Load(filename);
                return doc.Root.Attribute("version").Value;
            }
            catch (Exception e)
            {
                throw new SpreadsheetReadWriteException("Failed to load ");
            }
        }
        /// <summary>
        ///   Return an XML representation of the spreadsheet's contents
        /// </summary>
        /// <returns> contents in XML form </returns>
        public override string GetXML()
        {
            var doc = new XDocument(new XElement("spreadsheet", new XAttribute("version", Version),
            cells.Select(c => new XElement("cell",
                new XElement("name", c.Key),
                new XElement("contents", c.Value.Content.ToString())))));

            return doc.ToString();
        }


    }
}
