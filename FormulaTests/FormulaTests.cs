using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        // Test for evaluating a single number
        public void TestSingleNumber()
        {
            Formula f = new Formula("5");
            Assert.AreEqual(5.0, f.Evaluate(s => 0));
        }

        // Test for evaluating a single variable
        [TestMethod]
        public void TestSingleVariable()
        {
            Formula f = new Formula("X5");
            Assert.AreEqual(13.0, f.Evaluate(s => 13));
        }

        // Test for addition
        [TestMethod]
        public void TestAddition()
        {
            Formula f = new Formula("2+3");
            Assert.AreEqual(5.0, f.Evaluate(s => 0));

        }

        // Test for four operators
        [TestMethod()]
        public void TestFourOperators()
        {
            Formula f = new Formula("2 + 3 - 4 * 5 / 2");
            Assert.AreEqual(-5.0, f.Evaluate(s => 0));
        }

        // Test for complex expression
        [TestMethod()]
        public void TestComplexExpression()
        {
            Formula f = new Formula("(5 + 10) * 2");
            Assert.AreEqual(30.0, f.Evaluate(s => 0));
        }




        // Test for subtraction
        [TestMethod]
        public void TestSubtraction()
        {
            Formula f = new Formula("18-10");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        // Test for multiplication
        [TestMethod]
        public void TestMultiplication()
        {
            Formula f = new Formula("2*4");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        // Test for division
        [TestMethod]
        public void TestDivision()
        {
            Formula f = new Formula("16/2");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }

        // Test for expression with variable
        [TestMethod]
        public void TestExpressionwithVariable()
        {
            Formula f = new Formula("2+X1");
            Assert.AreEqual(6.0, f.Evaluate(s => 4));
        }



        // Test for negative numbers
        [TestMethod]
        public void TestNegativeNumber()
        {
            Formula f = new Formula("0-5 + 10");
            Assert.AreEqual(5.0, f.Evaluate(s => 0));
        }

        // Test for floating-point numbers
        [TestMethod()]
        public void TestFloatingPoint()
        {
            Formula f = new Formula("5.5 + 2.5");
            Assert.AreEqual(8.0, f.Evaluate(s => 0));
        }



        // Test for parenthesis 
        [TestMethod()]
        public void TestParentheses()
        {
            Formula f = new Formula("2 * (3 + 5)");
            Assert.AreEqual(16.0, f.Evaluate(s => 0));
        }

        // Test for missing operand
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestMissingOperand()
        {
            new Formula("+ 2");
        }

        // Test for missing operator
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestMissingOperator()
        {
            new Formula("2 2");
        }

        // Test for invalid variable
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidVariable()
        {
            new Formula("2 + x$");
        }

        // Test for variable normalization
        [TestMethod()]
        public void TestNormalization()
        {
            Formula f = new Formula("x1 + Y1", s => s.ToUpper(), s => true);
            Assert.AreEqual(2.0, f.Evaluate(s => 1));
        }

        // Test for variable validation
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestValidation()
        {
            new Formula("x1 + Y1", s => s, s => !s.Contains("Y"));
        }

        // Test for expression starting with operator
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidStarting()
        {
            new Formula("* 2 + 3");
        }

        // Test for expression ending with operator
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidEnding()
        {
            new Formula("2 + 3 *");
        }

        // Test for empty expression
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmpty()
        {
            new Formula("");
        }

        // Test for extra parenthesis
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUnbalancedParentheses()
        {
            new Formula("((2 + 3)");
        }

        // Test for incorrect order of parenthesis
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidParentheses()
        {
            new Formula("2 + (3 - 5)) + (4");
        }

        // Test for variables and numbers
        [TestMethod()]
        public void TestVariableAndNumber()
        {
            Formula f = new Formula("x1 + 5");
            Assert.AreEqual(10.0, f.Evaluate(s => 5));
        }


        // Test for equality of different formula
        [TestMethod()]
        public void TestEquality()
        {
            Formula f1 = new Formula("2 + 2");
            Formula f2 = new Formula("4");
            Assert.IsFalse(f1 == f2);
        }

        // Test for ToString method
        [TestMethod()]
        public void TestToString()
        {
            Formula f = new Formula("2 + 3");
            Assert.AreEqual("2+3", f.ToString());
        }

        // Test for GetHashCode method
        [TestMethod()]
        public void TestGetHashCode()
        {
            Formula f1 = new Formula("2 + 3");
            Formula f2 = new Formula("2 + 3");
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
        }

        // Test for scientific notation
        [TestMethod]
        public void TestScientificNotation()
        {
            Formula f = new Formula("5e-5 + 1");
            double expected = 1.00005;
            object result = f.Evaluate(s => 0);
            double delta = 1e-6;

            Assert.IsNotInstanceOfType(result, typeof(FormulaError), "Evaluation resulted in an error.");

            double actual = (double)result;

            Assert.IsTrue(Math.Abs(expected - actual) <= delta, "The evaluated result is not as expected.");
        }

        // Test equality of formula
        [TestMethod]
        public void TestEqualityOfFormulas()
        {
            Formula f1 = new Formula("x1+y2");
            Formula f2 = new Formula("x1+y2");
            Assert.AreEqual(f1, f2);
        }

        // Test for formatting
        [TestMethod]
        public void TestNumericFormatting()
        {
            Formula f1 = new Formula("2.0 + x7");
            Formula f2 = new Formula("2.000 + x7");
            Assert.AreEqual(f1, f2);
            Assert.IsTrue(f1 == f2);
        }

        // Test equality of different order
        [TestMethod]
        public void TestDifferentOrder()
        {
            Formula f1 = new Formula("x1+y2");
            Formula f2 = new Formula("y2+x1");
            Assert.AreNotEqual(f1, f2);
        }

        // Test equality with non formula object
        [TestMethod]
        public void TestEqualitywithNonFormulaObject()
        {
            Formula f1 = new Formula("x1+y2");
            object obj = "x1+y2";
            Assert.AreNotEqual(f1, obj);
        }

        // Test comparison with null
        [TestMethod]
        public void TestEqualityWithNull()
        {
            Formula f1 = new Formula("x1+y2");
            Formula f2 = null;
            Assert.AreNotEqual(f1, f2);
        }

        // Test different token count
        [TestMethod]
        public void TestDifferentTokenCount()
        {
            Formula f1 = new Formula("x1+y2");
            Formula f2 = new Formula("x1+y2+z3");
            Assert.AreNotEqual(f1, f2);
        }

        // Test different normalization
        [TestMethod]
        public void TestDifferentNormalization()
        {
            Formula f1 = new Formula("x1+y2", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("X1+Y2", s => s.ToLower(), s => true);
            Assert.AreNotEqual(f1, f2);
        }

        // Test for normalizing
        [TestMethod]
        public void TestNormalize()
        {
            Formula f1 = new Formula("x1+y2");
            Formula f2 = new Formula("X1+Y2");
            Assert.AreNotEqual(f1, f2);
            Assert.IsTrue(f1 != f2);
        }


        // Test for precision
        [TestMethod]
        public void TestPrecision()
        {
            Formula f = new Formula("2.0000000000000001");
            double expected = 2.0;
            object result = f.Evaluate(s => 0);
            double delta = 1e-15;
            Assert.IsNotInstanceOfType(result, typeof(FormulaError), "Evaluation resulted in an error.");
            double actual = (double)result;

            Assert.IsTrue(Math.Abs(expected - actual) <= delta, "The evaluated result is not as expected.");
        }

        // Test default normalization
        [TestMethod]
        public void TestDefault()
        {
            Formula f = new Formula("a1 + B2");
            var variables = f.GetVariables();
            Assert.IsTrue(variables.Contains("a1") && variables.Contains("B2"));
        }

        // Test custom normalization
        [TestMethod]
        public void TestCustomNormalization()
        {
            Func<string, string> normalizeToLower = s => s.ToLower();
            Formula f = new Formula("A1 + B2", normalizeToLower, null);
            var variables = f.GetVariables();
            Assert.IsTrue(variables.Contains("a1") && variables.Contains("b2"));
        }

        // Test custom validation
        [TestMethod]
        public void TestCustomValidation()
        {
            Func<string, bool> validateStartsWithLetter = s => char.IsLetter(s[0]);
            bool exceptionThrown = false;
            try
            {
                Formula f = new Formula("A1 + 1A", s => s, validateStartsWithLetter);
                f.Evaluate(s => 0);
            }
            catch (FormulaFormatException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected a FormulaFormatException due to invalid variable name.");
        }

        // Test Custom
        [TestMethod]
        public void TestCustom()
        {
            Func<string, string> normalizeToUpper = s => s.ToUpper();
            Func<string, bool> validateLength = s => s.Length == 2;

            bool exceptionThrown = false;
            try
            {
                Formula f = new Formula("a1 + b2 + ccc3", normalizeToUpper, validateLength);
            }
            catch (FormulaFormatException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Expected a FormulaFormatException to be thrown due to invalid variable name.");
        }

        // Test Invalid sequence
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidTokenSequence()
        {
            new Formula("( * 2 + 3");
        }

        // Test undefined variable
        [TestMethod]
        public void TestUndefinedVariable()
        {
            Formula f = new Formula("a1 + b2");
            var result = f.Evaluate(s => throw new ArgumentException("Undefined variable"));
            Assert.IsInstanceOfType(result, typeof(FormulaError));
        }

        // Test insufficient values
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInsufficientValues()
        {

            new Formula("+").Evaluate(s => 0);
        }

        // Test unbalanced parentheses
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUnbalancedParentheses2()
        {

            new Formula("(1 + 2").Evaluate(s => 0);
        }

        // Test invalid token
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidToken()
        {

            new Formula("1 + @").Evaluate(s => 0);
        }

        // Test Exception
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidExpressionAfterOperators()
        {
            new Formula("1 +").Evaluate(s => 0);
        }


        // Test Exception
        [TestMethod]
        public void TestUnexpectedExceptionInVariable()
        {

            Formula f = new Formula("10 / specialCase"); 

            bool exceptionThrown = false;
            try
            {

                f.Evaluate(s =>
                {
                    if (s == "specialCase") throw new InvalidOperationException("Unexpected error");
                    return 5; 
                });
            }
            catch (InvalidOperationException)
            {

                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Expected an InvalidOperationException to be thrown.");
        }

        // Test Exception
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRethrowUnexpectedException()
        {

            Formula f = new Formula("someSpecialCase"); 


            f.Evaluate(s =>
            {
                if (s == "someSpecialCase") throw new InvalidOperationException("Unexpected error");
                return 5; 
            });


        }




    }
}