using System;
using System.Collections.Generic;


class Token
{
    
}

class TokenIdentifier : Token
{
    public string value;

    public TokenIdentifier(string value)
    {
        this.value = value;
    }
}

class TokenString : Token
{
    public string value;

    public TokenString(string value)
    {
        this.value = value;
    }
}

class TokenNumber : Token
{
    public float value;

    public TokenNumber(float value) {
        this.value = value;
    }
}

class TokenAssign : Token
{
    
}

class TokenComma : Token
{
    
}

class TokenArrayStart : Token
{
    
}

class TokenArrayEnd : Token
{
    
}

class TokenDictStart : Token
{
    
}

class TokenDictEnd : Token
{
    
}

class TokenEnumStart : Token
{

}

class TokenEnumEnd : Token 
{

}

class Program {
    static void Main(string[] args) {
        string[] fileContentsArray = File.ReadAllLines("./examples/fullconfig.fcf");
        string fileContents = "";
        foreach(string line in fileContentsArray)
        {
            fileContents += line.TrimStart().TrimEnd();
        }

        string newFileContents = "";
        bool isString = false;
        foreach (char c in fileContents)
        {
            if (c == '"')
            {
                isString = !isString;
            }

            if (!isString && c == ' ')
            {
                continue;
            }

            newFileContents += c;
        }

        fileContents = newFileContents;

        List<Token> stack = new List<Token>();
        
        string currentWord = "";
        isString = false;
        
        foreach (char c in fileContents)
        {
            switch (c)
            {
                case '=':
                    stack.Add(new TokenIdentifier(currentWord));
                    currentWord = "";
                    stack.Add(new TokenAssign());
                    break;
                case ',':
                    if (currentWord != "")
                    {
                        float value = 0f;
                        if(float.TryParse(currentWord, out value)) {
                            stack.Add(new TokenNumber(value));
                            currentWord = "";
                        } else {
                            stack.Add(new TokenIdentifier(currentWord));
                            currentWord = "";
                        }
                    }

                    stack.Add(new TokenComma());
                    break;
                case '[':
                    stack.Add(new TokenArrayStart());
                    break;
                case ']':
                    if (currentWord != "")
                    {
                        stack.Add(new TokenIdentifier(currentWord));
                        currentWord = "";
                    }

                    stack.Add(new TokenArrayEnd());
                    break;
                case '{':
                    stack.Add(new TokenDictStart());
                    break;
                case '}':
                    if (currentWord != "")
                    {
                        stack.Add(new TokenIdentifier(currentWord));
                        currentWord = "";
                    }

                    stack.Add(new TokenDictEnd());
                    break;
                case '(':
                    stack.Add(new TokenEnumStart());
                    break;
                case ')':
                    if (currentWord != "")
                    {
                        stack.Add(new TokenIdentifier(currentWord));
                        currentWord = "";
                    }

                    stack.Add(new TokenEnumEnd());
                    break;

                default:
                    if (c != '"')
                    {
                        currentWord += c;
                    }
                    
                    if (c == '"')
                    {
                        if (isString)
                        {
                            stack.Add(new TokenString(currentWord));
                            currentWord = "";
                        }
                        
                        isString = !isString;
                    }

                    break;
            }
        }

        foreach (dynamic c in stack)
        {
            if (c.GetType() == typeof(TokenIdentifier) || c.GetType() == typeof(TokenString) || c.GetType() == typeof(TokenNumber))
            {
                Console.WriteLine(c + " " + c.value);
            }
            else
            {
                Console.WriteLine(c);
            }
        }
    }
}
