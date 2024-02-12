using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;


namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {


        private Dictionary<string, Cell> cells;
        private DependencyGraph dependencyGraph;

        /// <summary>
        /// Constructor
        /// </summary>
        public Spreadsheet()
        {
            cells = new Dictionary<string, Cell>();
            dependencyGraph = new DependencyGraph();
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
        /// helper method for test
        /// </summary>
        /// <param name="name"> the name of the cell</param>
        /// <returns></returns>
        private object GetCellValue(string name)
        {
            if (cells.TryGetValue(name, out Cell cell))
            {
                return cell.Value;
            }
            return null;
        }

        /// <summary>
        /// Returns an Enumerable that can be used to enumerates 
        /// the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        /// <summary>
        ///   Returns the contents (as opposed to the value) of the named cell.
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   Thrown if the name is null or invalid
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
        ///  Set the contents of the named cell to the given number.  
        /// </summary>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="number"> The new contents/value </param>
        /// 
        /// <returns>
        ///   <para>
        ///      The method returns a set consisting of name plus the names of all other cells whose value depends, 
        ///      directly or indirectly, on the named cell.
        ///   </para>
        /// 
        ///   <para>
        ///      For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///      set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        public override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null || !IsValidName(name))
                throw new InvalidNameException();

            if (!cells.ContainsKey(name))
            {
                cells.Add(name, new Cell(number));
            }
            else
            {
                cells[name].Content = number;
                //cells[name].Value = number;
            }

            dependencyGraph.ReplaceDependees(name, new HashSet<string>());
            return new HashSet<string>(GetCellsToRecalculate(name));
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
        /// <exception cref="ArgumentNullException"> 
        ///   If text is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
        /// </exception>
        /// 
        /// <param name="name"> The name of the cell </param>
        /// <param name="text"> The new content/value of the cell</param>
        /// 
        /// <returns>
        ///   The method returns a set consisting of name plus the names of all 
        ///   other cells whose value depends, directly or indirectly, on the 
        ///   named cell.
        /// 
        ///   <para>
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// </returns>
        public override ISet<string> SetCellContents(string name, string text)
        {
            if (name == null || !IsValidName(name))
                throw new InvalidNameException();
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (text == "")
            {
                if (cells.ContainsKey(name))
                {
                    cells.Remove(name);
                }
            }
            else
            {
                if (cells.ContainsKey(name))
                {
                    cells[name].Content = text;
                    //cells[name].Value = text;
                }
                else
                {
                    cells.Add(name, new Cell(text));
                }
            }

            dependencyGraph.ReplaceDependees(name, new HashSet<string>());
            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// Set the contents of the named cell to the formula.  
        /// </summary>
        /// 
        /// <exception cref="ArgumentNullException"> 
        ///   If formula parameter is null, throw an ArgumentNullException.
        /// </exception>
        /// 
        /// <exception cref="InvalidNameException"> 
        ///   If the name is null or invalid, throw an InvalidNameException
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
        ///     The method returns a Set consisting of name plus the names of all other 
        ///     cells whose value depends, directly or indirectly, on the named cell.
        ///   </para>
        ///   <para> 
        ///     For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        ///     set {A1, B1, C1} is returned.
        ///   </para>
        /// 
        /// </returns>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (name == null || !IsValidName(name))
                throw new InvalidNameException();
            if (formula == null)
                throw new ArgumentNullException(nameof(formula));

            var previous = new HashSet<string>(dependencyGraph.GetDependees(name));
            // update the dependency graph
            dependencyGraph.ReplaceDependees(name, formula.GetVariables());

            // check if there is a circular exception
            try
            {
                GetCellsToRecalculate(name);
            }
            catch (CircularException)
            {
                dependencyGraph.ReplaceDependees(name, previous); // Restore previous dependees
                throw; 
            }

            cells[name] = new Cell(formula);
            //cells[name].Value = formula;

            // return the cells
            return new HashSet<string>(GetCellsToRecalculate(name));
        }

        /// <summary>
        /// Returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell. 
        /// </summary>
        /// 
        /// <requires>
        /// the name that is passed in must be valid
        /// </requires>
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

    }
}
