namespace Kohlhaas.Parser;

/// <summary>
/// statement        ::= create-statement | empty-statement
/// create-statement ::= 'CREATE' '{' node-content '}' ';'
/// node-content     ::= STRING '=' label-array [ ',' property-list ]
/// label-array      ::= '[' [STRING {',' STRING}* '}'
/// property-list    ::= property {',' property}*
/// property         ::= NAME '=' value
/// value            ::= STRING | NUMBER
/// empty-statement  ::= ';'
/// </summary>
public class Parser
{
    private readonly Tokenizer _parserTokenizer;

    private Token HandleNextStatement(Tokenizer tokenizer, Token parent, Token token)
    {
        var statementToken = token;
        switch (statementToken.Type)
        {
            case TokenEnum.EndOfStatement: // Empty statement
                // Console.WriteLine(statementToken.LineColumnText + " empty-statement ignored");
                return token;
            case TokenEnum.Create: // create-statement ::= 'create' '{' string '=' '[' label-list ']' '}' ';'
                return HandleCreateStatement(tokenizer, parent, token);
            default:
                throw new LineColumnException(statementToken.Line, statementToken.Column,
                    "Unexpected token! " + token.Type);
                ;
        }
    }

    private Token HandleCreateStatement(Tokenizer tokenizer, Token parent, Token token)
    {
        var createNodeCommand = new Token(TokenEnum.CreateNodeCommand, token.Line, token.Column);
        parent.AppendChild(createNodeCommand);

        // expecting '{'
        token = tokenizer.GetNextToken();
        if (token.Type != TokenEnum.OpenCurlyBrace)
            throw new LineColumnException(token.Line, token.Column, "Expected '{' after CREATE");
        createNodeCommand.Value = token.Value;

        // node name
        token = tokenizer.GetNextToken();
        if (token.Type != TokenEnum.String)
            throw new LineColumnException(token.Line, token.Column, "Expected node name (string) after '{'!");

        var nodeDefinition = new Token(TokenEnum.NodeDefinition, token.Line, token.Column) { Value = token.Value };
        createNodeCommand.AppendChild(nodeDefinition);

        // Expect '='
        token = tokenizer.GetNextToken();
        if (token.Type != TokenEnum.OperatorAssign)
            throw new LineColumnException(token.Line, token.Column, "Expected '=' after node name!");

        // start label array '['
        token = tokenizer.GetNextToken();
        if (token.Type != TokenEnum.OpenSquareBracket)
            throw new LineColumnException(token.Line, token.Column, "Expected '[' after '='!");

        var labelArray = new Token(TokenEnum.LabelArray, token.Line, token.Column);
        nodeDefinition.AppendChild(labelArray);

        token = tokenizer.GetNextToken();
        while (token.Type != TokenEnum.CloseSquareBracket)
        {
            if (token.Type != TokenEnum.String)
                throw new LineColumnException(token.Line, token.Column, "Expected label (string) in label array!");

            labelArray.AppendChild(token);
            token = tokenizer.GetNextToken();
            if (token.Type == TokenEnum.CloseSquareBracket) break;
            if (token.Type != TokenEnum.ParameterSeparator)
                throw new LineColumnException(token.Line, token.Column, "Expected ',' or ']' in label array!");

            token = tokenizer.GetNextToken();
        }

        // Check for properties
        token = tokenizer.GetNextToken();
        if (token.Type == TokenEnum.ParameterSeparator)
        {
            var propertyList = new Token(TokenEnum.PropertyList, token.Line, token.Column);
            nodeDefinition.AppendChild(propertyList);
            token = ParsePropertyList(tokenizer, propertyList);
        }

        if (token.Type != TokenEnum.CloseCurlyBrace)
            throw new LineColumnException(token.Line, token.Column, "Expected '}' after properties or label array!");
        token = tokenizer.GetNextToken();
        if (token.Type != TokenEnum.EndOfStatement)
            throw new LineColumnException(token.Line, token.Column, "Expected ';' after '}'!");
        return token;
    }

    private Token ParsePropertyList(Tokenizer tokenizer, Token propertyList)
    {
        Token token;

        while (true)
        {
            token = tokenizer.GetNextToken();
            if (token.Type != TokenEnum.Name) throw new LineColumnException(token.Line, token.Column, "Expected property name!");
            
            var property = new Token(TokenEnum.Property, token.Line, token.Column)
            {
                Value = token.Value
            };
            propertyList.AppendChild(property);
            
            // Expect '='
            token = tokenizer.GetNextToken();
            if(token.Type != TokenEnum.OperatorAssign) throw new LineColumnException(token.Line, token.Column, "Expected '=' after property name!");
            
            token = tokenizer.GetNextToken();
            if (token.Type != TokenEnum.String && token.Type != TokenEnum.Number) throw new LineColumnException(token.Line, token.Column, "Expected string or number for property value!");
            
            property.AppendChild(token);
            
            token = tokenizer.GetNextToken();
            if (token.Type != TokenEnum.ParameterSeparator) break;
        }

        return token;
    }

    public Token GetTokenTree()
    {
        var scriptToken = new Token(TokenEnum.Script, 1, 1);
        var token = _parserTokenizer.GetNextToken();
        while (token.Type != TokenEnum.EndOfText)
        {
            token = HandleNextStatement(_parserTokenizer, scriptToken, token);
            if (token.Type == TokenEnum.EndOfText) break;
            if (token.Type != TokenEnum.EndOfStatement)
                throw new LineColumnException(token.Line, token.Column, "End of statement expected!");
            token = _parserTokenizer.GetNextToken();
        }

        return scriptToken;
    }

    public Parser(string text)
    {
        _parserTokenizer = new Tokenizer(text, StatementSeparator.SeparatorOnly);
    }
}