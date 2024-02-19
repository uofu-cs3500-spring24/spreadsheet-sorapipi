```
Author:     YINGHAO CHEN
Partner:    None
Start Date: 15/01/2024
Course:     CS 3505, University of Utah, School of Computing
GitHub ID:  sorapipi
Repo:       https://github.com/uofu-cs3500-spring24/spreadsheet-sorapipi
Commit Date: 18-01-2024 Time (of when submission is ready to be evaluated)
Solution:   Spreadsheet
Copyright:  CS 3500 and YINGHAO CHEN - This work may not be copied for use in Academic Coursework.
```

# Overview of the Spreadsheet functionality

The Spreadsheet program is currently capable of evaluating a input expression  Future extension is dependency graph

# Examples of Good Software Practice (GSP)
Well named, commented, short methods that do a specific job and return a specific result: for example my IsValidName method, it is clear that it's used to check if a name is valid.
Testing strategies: In my test codes, i always test the method first, then the exceptions. after that, i will check the Fine Code Coverage to find out all the red code(which means it's not covered), then add more tests to cover them.
DRY: I think in my codes, there are some methods need use the logic of GetCellsToRecalculate, i didn't write the same logic in every needed methods, i just call the GetCellsToRecalculate method to avoid repeat myself.


# Time Expenditures:

    1. Assignment One:   Predicted Hours:          5        Actual Hours:   7
    2. Assignment Two:   Predicted Hours:          7        Actual Hours:   10   spent 2 hours on debugging
    3. Assignment Three: Predicted Hours:          10       Actual Hours:   16   spent too much time on modifying original code ( trying to catch more exceptions but found too many repeated) and the tests
    4. Assignment Four:  Predicted Hours:          10       Actual Hours:   11   spent 2 hours on writing tests to make sure the Fine Code Coverage is near 100%, and one hour on debugging
    5. Assignment Five:  Predicted Hours:          8        Actual Hours:   10   spent 2.5 hours on modifying the old tests and codes