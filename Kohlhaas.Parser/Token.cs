namespace Kohlhaas.Parser;

public sealed class Token
{
    public TokenEnum Type { get; private set; }

    public string Value { get; set; }

    public int Line { get; private set; }

    public int Column { get; private set; }

    public Token Childs { get; private set; }

    public Token Sibling { get; private set; }

    public void AppendChild(Token child)
    {
        if (Childs == null)
        {
            Childs = child;
        }
        else
        {
            var t = Childs;
            while (t.Sibling != null) t = t.Sibling;
            t.Sibling = child;
        }

        child.Sibling = null;
    }

    public string LineColumnText
    {
        get { return "[" + Line + "/" + Column + "]"; }
    }

    public Token(TokenEnum type, int line, int column)
    {
        Type = type;
        Value = "";
        Line = line;
        Column = column;
    }

    public Token(TokenEnum type, string value, int line, int column)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = column;
    }

    public override string ToString()
    {
        return "(" + Type.ToString() + " at " + Line + "/" + Column + ")";
    }
}