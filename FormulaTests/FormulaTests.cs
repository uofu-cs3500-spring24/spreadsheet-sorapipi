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

        // Test for all operators
        [TestMethod()]
        public void TestAllOperators()
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

        // Test for arithmetic with variable
        [TestMethod]
        public void TestArithmeticWithVariable()
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



        // Test for parenthesis impact
        [TestMethod()]
        public void TestParenthesesImpact()
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
        public void TestVariableNormalization()
        {
            Formula f = new Formula("x1 + Y1", s => s.ToUpper(), s => true);
            Assert.AreEqual(2.0, f.Evaluate(s => 1));
        }

        // Test for variable validation
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestVariableValidation()
        {
            new Formula("x1 + Y1", s => s, s => !s.Contains("Y"));
        }

        // Test for expression starting with operator
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestStartWithOperator()
        {
            new Formula("* 2 + 3");
        }

        // Test for expression ending with operator
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEndWithOperator()
        {
            new Formula("2 + 3 *");
        }

        // Test for empty expression
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestEmptyExpression()
        {
            new Formula("");
        }

        // Test for extra parenthesis
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestExtraParentheses()
        {
            new Formula("((2 + 3)");
        }

        // Test for incorrect order of parenthesis
        [TestMethod()]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestIncorrectOrderOfParentheses()
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


        // Test for equality of different formulae
        [TestMethod()]
        public void TestEqualityDifferentFormulas()
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

        [TestMethod]
        public void TestScientificNotation()
        {
            Formula f = new Formula("5e-5 + 1");
            double expected = 1.00005;
            object result = f.Evaluate(s => 0);
            double delta = 1e-6;

            // Check if the result is a double and not a FormulaError
            Assert.IsNotInstanceOfType(result, typeof(FormulaError), "Evaluation resulted in an error.");

            // Cast the result to double before comparing
            double actual = (double)result;

            Assert.IsTrue(Math.Abs(expected - actual) <= delta, "The evaluated result is not as expected.");
        }

        [TestMethod]
        public void TestIdenticalFormulas()
        {
            Formula f1 = new Formula("x1+y2");
            Formula f2 = new Formula("x1+y2");
            Assert.AreEqual(f1, f2);
        }

        [TestMethod]
        public void TestNumericTokenFormatting()
        {
            Formula f1 = new Formula("2.0 + x7");
            Formula f2 = new Formula("2.000 + x7");
            Assert.AreEqual(f1, f2);
            Assert.IsTrue(f1 == f2);
        }

        [TestMethod]
        public void TestDifferentOrder()
        {
            Formula f1 = new Formula("x1+y2");
            Formula f2 = new Formula("y2+x1");
            Assert.AreNotEqual(f1, f2);
        }

        [TestMethod]
        public void TestComparisonWithNonFormulaObject()
        {
            Formula f1 = new Formula("x1+y2");
            object obj = "x1+y2";
            Assert.AreNotEqual(f1, obj);
        }

        [TestMethod]
        public void TestComparisonWithNull()
        {
            Formula f1 = new Formula("x1+y2");
            Formula f2 = null;
            Assert.AreNotEqual(f1, f2);
        }

        [TestMethod]
        public void TestDifferentTokenCount()
        {
            Formula f1 = new Formula("x1+y2");
            Formula f2 = new Formula("x1+y2+z3");
            Assert.AreNotEqual(f1, f2);
        }

        [TestMethod]
        public void TestDifferentVariables()
        {
            Formula f1 = new Formula("x1+y2", s => s.ToUpper(), s => true);
            Formula f2 = new Formula("X1+Y2", s => s.ToLower(), s => true);
            Assert.AreNotEqual(f1, f2);
        }

        [TestMethod]
        public void TestNormalize()
        {
            Formula f1 = new Formula("x1+y2");
            Formula f2 = new Formula("X1+Y2");
            Assert.AreNotEqual(f1, f2);
            Assert.IsTrue(f1 != f2);
        }


        [TestMethod]
        public void TestDivisionByZero()
        {
            Formula f = new Formula("1 / 0");
            try
            {
                var result = f.Evaluate(s => 0);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                // Check if the exception is the expected one
                Assert.IsTrue(ex.Message.Contains("SpreadsheetUtilities.FormulaError: Divided by zero"));
            }
        }

        [TestMethod]
        public void TestPrecisionIssue()
        {
            Formula f = new Formula("2.0000000000000001");
            double expected = 2.0;
            object result = f.Evaluate(s => 0);
            double delta = 1e-15;
            // Check if the result is a double and not a FormulaError
            Assert.IsNotInstanceOfType(result, typeof(FormulaError), "Evaluation resulted in an error.");

            // Cast the result to double before comparing
            double actual = (double)result;

            Assert.IsTrue(Math.Abs(expected - actual) <= delta, "The evaluated result is not as expected.");
        }


        [TestMethod]
        public void TestDefaultNormalizationAndValidation()
        {
            Formula f = new Formula("a1 + B2");
            // Expecting that 'a1' and 'B2' are treated as different variables
            var variables = f.GetVariables();
            Assert.IsTrue(variables.Contains("a1") && variables.Contains("B2"));
        }


        [TestMethod]
        public void TestCustomNormalization()
        {
            Func<string, string> normalizeToLower = s => s.ToLower();
            Formula f = new Formula("A1 + B2", normalizeToLower, null);
            var variables = f.GetVariables();
            // Expecting that 'A1' and 'B2' are converted to 'a1' and 'b2'
            Assert.IsTrue(variables.Contains("a1") && variables.Contains("b2"));
        }

        [TestMethod]
        public void TestCustomValidation()
        {
            Func<string, bool> validateStartsWithLetter = s => char.IsLetter(s[0]);
            bool exceptionThrown = false;
            try
            {
                // '1A' should cause validation to fail because it does not start with a letter
                Formula f = new Formula("A1 + 1A", s => s, validateStartsWithLetter);
                f.Evaluate(s => 0);
            }
            catch (FormulaFormatException)
            {
                exceptionThrown = true;
            }
            Assert.IsTrue(exceptionThrown, "Expected a FormulaFormatException due to invalid variable name.");
        }

        [TestMethod]
        public void TestCustomNormalizationAndValidation()
        {
            Func<string, string> normalizeToUpper = s => s.ToUpper();
            Func<string, bool> validateLength = s => s.Length == 2;

            bool exceptionThrown = false;
            try
            {
                // The exception is expected to be thrown here, during the construction of the Formula object
                Formula f = new Formula("a1 + b2 + ccc3", normalizeToUpper, validateLength);
            }
            catch (FormulaFormatException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Expected a FormulaFormatException to be thrown due to invalid variable name.");
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidTokenSequence()
        {
            // This should fail due to an invalid sequence like "(*"
            new Formula("( * 2 + 3");
        }

        [TestMethod]
        public void TestUndefinedVariable()
        {
            Formula f = new Formula("a1 + b2");
            var result = f.Evaluate(s => throw new ArgumentException("Undefined variable"));
            Assert.IsInstanceOfType(result, typeof(FormulaError));
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInsufficientValues()
        {
            // This should fail due to insufficient values for the "+" operation
            new Formula("+").Evaluate(s => 0);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestUnbalancedParentheses()
        {
            // This should fail due to unbalanced parentheses
            new Formula("(1 + 2").Evaluate(s => 0);
        }

        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidToken()
        {
            // This should fail due to an invalid token like "@"
            new Formula("1 + @").Evaluate(s => 0);
        }


        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void TestInvalidExpressionAfterOperators()
        {
            // This should fail due to an invalid expression after processing all operators
            new Formula("1 +").Evaluate(s => 0);
        }

        [TestMethod]
        public void TestRethrowingExceptionInVariableMethod()
        {
            Formula f = new Formula("5 / 0"); // Division by zero
            try
            {
                f.Evaluate(s => 0);
                Assert.Fail("Expected exception was not thrown.");
            }
            catch (Exception ex)
            {
                // Check if the correct exception is rethrown
                Assert.IsTrue(ex.Message.Contains("SpreadsheetUtilities.FormulaError"));
            }
        }

        [TestMethod]
        public void TestUnexpectedExceptionInVariable()
        {
            // Arrange: Create a formula where the Calculate method would throw an unexpected exception.
            // This requires a setup where Calculate would behave unexpectedly, which might not be straightforward
            // to achieve with the current implementation.
            // As an example, let's assume a scenario where Calculate throws an InvalidOperationException
            // for a specific operation, like division by a specific number.

            Formula f = new Formula("10 / specialCase"); // Assuming 'specialCase' triggers the unexpected exception

            bool exceptionThrown = false;
            try
            {
                // Act: Evaluate the formula
                f.Evaluate(s =>
                {
                    if (s == "specialCase") throw new InvalidOperationException("Unexpected error");
                    return 5; // Return some default value for other cases
                });
            }
            catch (InvalidOperationException)
            {
                // Assert: Check if the InvalidOperationException is thrown
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Expected an InvalidOperationException to be thrown.");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRethrowUnexpectedException()
        {
            // Arrange: Create a formula where Calculate throws an unexpected exception
            Formula f = new Formula("someSpecialCase"); // someSpecialCase triggers an InvalidOperationException

            // Act: Evaluate the formula
            f.Evaluate(s =>
            {
                if (s == "someSpecialCase") throw new InvalidOperationException("Unexpected error");
                return 5; // Default return value for other cases
            });

            // Assert: Expected an InvalidOperationException to be rethrown
        }


    }
}