namespace Kohlhaas.Parser;

public enum TokenEnum
{
    EndOfText,
    
    // Terminals
    EndOfStatement,
    ParameterSeparator,
    OpenParenthesis,
    CloseParenthesis,
    OpenSquareBracket,
    CloseSquareBracket,
    OpenCurlyBrace,
    CloseCurlyBrace,
    OperatorAdd,
    OperatorSubtract,
    OperatorMultiply,
    OperatorDivide,
    OperatorModulo,
    OperatorAssign,
    
    // Non-terminals
    Script,
    CreateNodeCommand,
    NodeDefinition,
    LabelArray,
    PropertyList,
    Property,
    Name,
    Number,
    String,
    
    // Keywords
    Create,
    Update,
    Delete,
}