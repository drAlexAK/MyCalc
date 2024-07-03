﻿using System;
using System.Text;
using Microsoft.VisualBasic;

static class Error
{
    public static void Throw(string s)
    {
        Console.WriteLine(s);
    }
}

enum TokenType : long
{
    Blank = 0,
    Operation = 1 << 7,
    Number = 1 << 8,
    
    BracketOpen = 1,
    BracketClose = 2,
    Addition = 4,
    Subtraction = 8,
    Multiplication = 16,
    Division = 32,
    Exponentiation = 64
}

class Number
{
    public bool IsInteger
    {
        get;
        private set;
    }

    public double dVal
    {
        get;
        private set;
    }

    public long iVal
    {
        get;
        private set;
    }

    public Number(double v)
    {
        iVal = (long)v;
        dVal = v;
        IsInteger = false;
    }

    public Number(long v)
    {
        iVal = v;
        dVal = v;
        IsInteger = true;
    }
    public Number(string s)
    {
        if (s.Contains('.') || s.Contains(','))
        {
            IsInteger = false;
            double val;
            if (double.TryParse(s, out val))
            {
                iVal = (long)val;
                dVal = val;
            }
            else
            {
                // TODO error of incorrect number
            }
        }
        else
        {
            IsInteger = true;
            long val;
            if (Int64.TryParse(s, out val))
            {
                iVal = val;
                dVal = val;
            }
            else
            {
                // TODO error of incorrect number
            }
        }
    }

    public override string ToString()
    {
        if (IsInteger) return iVal.ToString();
        return dVal.ToString();
    }
}


static class Operations
{
    private static long MyPower(long a, long b)
    {
        long res = 1;
        long val = a;
        while (b > 0)
        {
            if ((b & 1) > 0) res = res * val;
            val = val * val;
            b >>= 1;
        }
        return res;
    }
    private static double EPS = 0.000000001;
    static public Number Sum(Number a, Number b)
    {
        if (a.IsInteger && b.IsInteger)
        {
            return new Number(a.iVal + b.iVal);
        }
        return new Number(a.dVal + b.dVal);
    }
    static public Number Sub(Number a, Number b)
    {
        if (a.IsInteger && b.IsInteger)
        {
            return new Number(a.iVal - b.iVal);
        }
        return new Number(a.dVal - b.dVal);
    }
    static public Number Mul(Number a, Number b)
    {
        if (a.IsInteger && b.IsInteger)
        {
            return new Number(a.iVal * b.iVal);
        }
        return new Number(a.dVal * b.dVal);
    }
    static public Number Div(Number a, Number b)
    {
        if (b.dVal < EPS)
        {
            // TODO Division by Zero
        }
        if (a.IsInteger && b.IsInteger && (a.iVal % b.iVal == 0))
        {
            return new Number(a.iVal / b.iVal);
        }
        return new Number(a.dVal / b.dVal);
    }
    static public Number Exp(Number a, Number b)
    {
        if (a.IsInteger && b.IsInteger)
        {
            return new Number(MyPower(a.iVal, b.iVal));
        }
        return new Number(Math.Pow(a.dVal, b.dVal));
    }
    public delegate Number MyOperations(Number a, Number b);

    public static MyOperations[] MapOfOperations =
        new MyOperations [2 * Math.Max((long)TokenType.Number, (long)TokenType.Operation) + 1];

    static Operations()
    {
        MapOfOperations[(long)TokenType.Operation | (long)TokenType.Addition] = Sum;
        MapOfOperations[(long)TokenType.Operation | (long)TokenType.Subtraction] = Sub;
        MapOfOperations[(long)TokenType.Operation | (long)TokenType.Multiplication] = Mul;
        MapOfOperations[(long)TokenType.Operation | (long)TokenType.Division] = Div;
        MapOfOperations[(long)TokenType.Operation | (long)TokenType.Exponentiation] = Exp;
    }
}

class Token
{
    static public TokenType GetType(string s)
    {
        switch (s)
        {
            case null:
                return TokenType.Blank;
            case "":
                return TokenType.Blank;
            case "+":
                return TokenType.Operation | TokenType.Addition;
            case "-":
                return TokenType.Operation | TokenType.Subtraction;
            case "*":
                return TokenType.Operation | TokenType.Multiplication;
            case "/":
                return TokenType.Operation | TokenType.Division;
            case "^":
                return TokenType.Operation | TokenType.Exponentiation;
            case "(":
                return TokenType.Operation | TokenType.BracketOpen;
            case ")":
                return TokenType.Operation | TokenType.BracketClose;
            default:
                return TokenType.Number;
        }
    }
    public TokenType Type
    {
        private set;
        get;
    }
    public string RawString
    {
        private set;
        get;
    }
    
    public Token(string s)
    {
        RawString = s;
        Type = Token.GetType(s);
    }
}

class MyClass
{
    static List<Token> Parser(string s)
    {
        List<StringBuilder> strings = new List<StringBuilder>();
        foreach (var ch in s)
        {
            if ((int)ch <= 32) continue; // ignoring all blank chars
            var type = Token.GetType(ch.ToString());
            TokenType typeOfLastElem = TokenType.Blank;
            if (strings.Count() > 0)
            {
                typeOfLastElem = Token.GetType(strings.Last().ToString());
            }
            if ((type & TokenType.Operation) > 0)
            {
                strings.Add(new StringBuilder(ch.ToString()));
                if ((type & TokenType.Subtraction) > 0 && (typeOfLastElem & TokenType.Number) > 0)
                {
                    strings.Add(new StringBuilder());
                }
            }
            else
            {
                if ((typeOfLastElem & TokenType.Number) > 0 || typeOfLastElem == TokenType.Blank || (typeOfLastElem & TokenType.Subtraction) > 0)
                {
                    strings.Last().Append(ch);
                }
                else
                {
                    strings.Add(new StringBuilder(ch.ToString()));
                }
            }
        }
        
        List<Token> res = new List<Token>();
        foreach (var x in strings) res.Add(new Token(x.ToString()));
        return res;
    }
    static void Main()
    {
        string s = Console.ReadLine();
        var ListOfTokens = Parser(s);
        foreach (var val in ListOfTokens)
        {
            Console.WriteLine(val.RawString);
        }
        Console.WriteLine("=====================");
        Console.WriteLine(Operations.MapOfOperations[(long)ListOfTokens[1].Type](new Number(ListOfTokens[0].RawString), new Number(ListOfTokens[2].RawString)).ToString());
    }
}