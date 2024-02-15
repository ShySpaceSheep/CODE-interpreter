# Language Specification of CODE Programming Language
CODE is a strongly – typed programming language developed to teach Junior High School students basics of programming. It was developed by a group of students enrolled in the Programming Languages course. CODE is a pure interpreter.

## Sample Program
A sample source code written in CODE looks like this:
```
# this is a sample program in CODE
BEGIN CODE
  INT x, y, z=5
  CHAR a_1=’n’
  BOOL t=”TRUE”
  x=y=4
  a_1=’c’
  # this is a comment
  DISPLAY: x & t & z & $ & a_1 & [#] & “last”
END CODE
```
This code will then return an output:
```
4TRUE5
n#last
```

## Language Grammar
### Program Structure:
  - all codes are placed inside BEGIN CODE and END CODE
  - all variable declaration is found after BEGIN CODE
  - all variable names are case sensitive and starts with letter or an underscore (_) and followed by a letter, underscore or digits.
  - every line contains a single statement
  - comments starts with sharp sign(#) and it can be placed anywhere in the program
  - executable codes are placed after variable declaration
  - all reserved words are in capital letters and cannot be used as variable names
  - dollar sign($) signifies next line or carriage return
  - ampersand(&) serves as a concatenator
  - the square braces([]) are as escape code

### Data Types:
  1. INT – an ordinary number with no decimal part. It occupies 4 bytes in the memory.
  2. CHAR – a single symbol.
  3. BOOL – represents the literals true or false.
  4. FLOAT – a number with decimal part. It occupies 4 bytes in the memory.

### Operators:
**Arithmetic operators:**
  - `( )` - parenthesis
  - `*, /, %` - multiplication, division, modulo
  - `+, -` - addition, subtraction
  - `>, <` - greater than, lesser than
  - `>=, <=` - greater than or equal to, lesser than or equal to
  - `==, <>` - equal, not equal
  - 
**Logical operators (<BOOL expression><LogicalOperator><BOOL expression>):**
  - `AND` - needs the two BOOL expression to be true to result to true, else false
  - `OR` - if one of the BOOL expressions evaluates to true, returns true, else false
  - `NOT` - the reverse value of the BOOL value
    
**Unary operators:**
  - `+` - positive
  - `-` - negative
