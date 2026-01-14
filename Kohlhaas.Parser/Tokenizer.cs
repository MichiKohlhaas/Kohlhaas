namespace Kohlhaas.Parser;

/// <summary>
/// 
/// </summary>
public sealed class Tokenizer
{
    private const char CHAR_NUL = '\0';
    private const char CHAR_ALERT = '\a';
    private const char CHAR_BS = '\b';
    private const char CHAR_FF = '\f';
    private const char CHAR_LF = '\n';
    private const char CHAR_CR = '\r';
    private const char CHAR_HTAB = '\t';
    private const char CHAR_VTAB = '\v';
    private const char CHAR_SPC = ' ';

    private const byte MaxASCIIValue = 128;
    
    private string Text;
    private int TextPos;
    private int TextLength;
    private bool TextPosIsAtEndOfLine;
    private bool LastEndOfLineIsCRLF;
    private int TextLine;
    private int TextColumn;
    private StatementSeparator StatementSeparator;
    
    private Dictionary<string, TokenEnum> Keywords = new Dictionary<string, TokenEnum>()
    {
        {"create", TokenEnum.Create},
        {"label", TokenEnum.LabelArray},
        {"update", TokenEnum.Update},
        {"delete", TokenEnum.Delete},
    };
    
    /// <summary>
    /// Construct the tokenizer
    /// </summary>
    /// <param name="text">Text we like to tokenize</param>
    /// <param name="statementSeparator">Selects the separator used between statements</param>
    public Tokenizer(string text, StatementSeparator statementSeparator = StatementSeparator.EndOfLineOnly)
    {
        Text = text ?? throw new ArgumentException("Text must not be null!");
        TextLength = text.Length;
        TextPos = 0;
        TextLine = 1;
        TextColumn = 0;
        StatementSeparator = statementSeparator;
    }
    
    /// <summary>
    /// Get the next character from the text we like to tokenize.
    /// </summary>
    /// <returns>Next character</returns>
    private char GetChar()
    {
        // We assume that Text[TextLength] contains a virtual NUL character to mark the end of the text.
        // This makes it easier to handle the ungetchar case.

        if (TextPos <= TextLength)
        {
            if (TextPosIsAtEndOfLine)
            {
                TextPosIsAtEndOfLine = false;
                TextLine++;
                TextColumn = 1;
            }
            else
            {
                TextColumn++;
            }

            if (TextPos < TextLength)
            {
                var ch = Text[TextPos++];
                if (ch == CHAR_CR)
                {
                    TextPosIsAtEndOfLine = true;
                    if ((TextPos < TextLength) && (Text[TextPos] == CHAR_LF))
                    {
                        // we are mapping CR LF to LF
                        LastEndOfLineIsCRLF = true;
                        TextPos++;
                        return CHAR_LF; // CR LF
                    }
                    else
                    {
                        LastEndOfLineIsCRLF = false;
                        return CHAR_CR; // CR only
                    }
                }
                else if (ch == CHAR_LF)
                {
                    TextPosIsAtEndOfLine = true;
                    LastEndOfLineIsCRLF = false;
                    return CHAR_LF; // LF only
                }
                return ch;
            }
            else
            {
                TextPos++;
                return CHAR_NUL; // End of text character
            }
        }
        else
        {
            return CHAR_NUL; // End of text character
        }
    }
    
    private char PeekChar()
    {
        char ch = GetChar();
        UngetChar();
        return ch;
    }
    
    /// <summary>
    /// Ungets the last GetChar().
    /// Warning: Call this method only once after GetChar!
    /// </summary>
    private void UngetChar()
    {
        if (TextPos > 0)
        {
            if (TextPosIsAtEndOfLine)
            {
                if (LastEndOfLineIsCRLF) TextPos--;
                TextPosIsAtEndOfLine = false;
                TextColumn--;
            }
            else if (TextColumn == 1)
            {
                TextPosIsAtEndOfLine = true;
                TextLine--;
            }
            else
            {
                TextColumn--;
            }
            TextPos--;
        }
    }
    
    private bool IsASCIILetter(char ch)
    {
        return char.IsLetter(ch) && (ch < MaxASCIIValue);
    }
    
    private int HexDigitValue(char ch)
    {
        return char.ToUpper(ch) switch
        {
            '0' => 0,
            '1' => 1,
            '2' => 2,
            '3' => 3,
            '4' => 4,
            '5' => 5,
            '6' => 6,
            '7' => 7,
            '8' => 8,
            '9' => 9,
            'A' => 10,
            'B' => 11,
            'C' => 12,
            'D' => 13,
            'E' => 14,
            'F' => 15,
            _ => -1
        };
    }
    
    private Token GetNameOrKeywordToken(int line, int column, char ch)
    {
        string tokenText = "";
        do
        {
            tokenText += ch;
            ch = GetChar();
        } while (IsASCIILetter(ch) || char.IsDigit(ch) || (ch == '_'));
        UngetChar();
        
        return Keywords.TryGetValue(tokenText.ToLower(), out TokenEnum tokenType) ? new Token(tokenType, line, column) : 
            new Token(TokenEnum.Name, tokenText, line, column);
    }
    
    private Token GetNumberToken(int line, int column, char ch)
    {
        string tokenText = "";

        // Get the sign of the number
        //int sign;
        ch = ch switch
        {
            '-' => GetChar(), //sign = -1;
            '+' => GetChar(), //sign = +1;
            _ => ch //sign = 1;
        };

        // collect the digits of the number
        do
        {
            tokenText += ch;
            ch = GetChar();
        } while (char.IsDigit(ch));

        if (IsASCIILetter(ch))
        {
            throw new LineColumnException(line, column, "A letter must not follow a number!");
        }
        else
        {
            UngetChar();
            return new Token(TokenEnum.Number, tokenText, line, column);
        }
    }
    
    private Token GetStringToken(int line, int column, char ch)
    {
        string tokenText = "";
        while (true)
        {
            ch = GetChar();
            if (ch == '\\')
            {
                ch = GetChar();
                switch (ch)
                {
                    case '\'': break;
                    case '\"': break;
                    case '\\': break;
                    case '\0': ch = CHAR_NUL; break;
                    case 'a': ch = CHAR_ALERT; break;
                    case 'b': ch = CHAR_BS; break;
                    case 'f': ch = CHAR_FF; break;
                    case 'n': ch = CHAR_LF; break;
                    case 'r': ch = CHAR_CR; break;
                    case 't': ch = CHAR_HTAB; break;
                    case 'v': ch = CHAR_VTAB; break;
                    case 'x': // \xH[H[H[H]]]
                        ch = GetChar();
                        int value = HexDigitValue(ch);
                        if (value < 0) throw new LineColumnException(line, column, "Expected a hex digit after '\\x'!");
                        for (int i = 0; i < 3; i++)
                        {
                            ch = GetChar();
                            int v = HexDigitValue(ch);
                            if (v < 0)
                            {
                                UngetChar();
                                break;
                            }
                            value = value * 16 + v;
                        }
                        ch = (char)value;
                        break;
                    default:
                        throw new LineColumnException(line, column, "Unsupported character after escape character '\'!");
                }
            }
            else if (ch == '\"')
            {
                break;
            }
            else if ((ch == CHAR_CR) || (ch == CHAR_LF) || (ch == CHAR_NUL))
            {
                throw new LineColumnException(line, column, "Missing '\"' at the end of a string.");
            }
            else if (char.IsControl(ch))
            {
                throw new LineColumnException(line, column, "Control characters are not allowed in strings!");
            }
            tokenText += ch;
        }
        return new Token(TokenEnum.String, tokenText, line, column);
    }
    
    /// <summary>
        /// Skip the text up to the end of line.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns>true if end of line is also end of text</returns>
        private bool SkipSingleLineComment(int line, int column)
        {
            char ch;
            do
            {
                ch = GetChar();
                if (ch == CHAR_NUL)
                {
                    return true; // End of text
                }

            } while ((ch != CHAR_CR) && (ch != CHAR_LF));
            return false; // End of line
        }

        /// <summary>
        /// Skip the text up to the end of the multi-line comment.
        /// The multi-line comment may contain another multi-line comment
        /// </summary>
        /// <param name="line"></param>
        /// <param name="column"></param>
        private void SkipMultiLineComment(int line, int column)
        {
            char ch;
            int level = 1;
            while (level > 0)
            {
                while (true)
                {
                    ch = GetChar();
                    if (ch == CHAR_NUL) throw new LineColumnException(line, column, "Multi-line comment not closed with '*/'!");
                    if (ch == '*') break;
                    if (ch == '/')
                    {
                        ch = GetChar();
                        if (ch == '*')
                        {
                            level++;
                        }
                        else
                        {
                            UngetChar();
                        }
                    }
                };
                ch = GetChar();
                if (ch == '/')
                {
                    level--;
                }
                else
                {
                    UngetChar();
                }
            }
        }

        /// <summary>
        /// Gets the next token from the text.
        /// </summary>
        /// <returns>Detected token</returns>
        public Token GetNextToken()
        {
            char ch;
            int line;
            int column;

            while (true)
            {
                // skip space and tab characters in front of the token
                do
                {
                    ch = GetChar();
                } while (ch is CHAR_SPC or CHAR_HTAB);

                // remember the start of the token in the text
                line = TextLine;
                column = TextColumn;

                switch (ch)
                {
                    case CHAR_NUL: // End of text signaled
                        return new Token(TokenEnum.EndOfText, line, column);
                    case '/':
                        ch = GetChar();
                        if (ch == '/') // Single-line comment starting with '//'
                        {    
                            if (SkipSingleLineComment(line, column)) return new Token(TokenEnum.EndOfText, TextLine, TextColumn);
                            if (StatementSeparator != StatementSeparator.SeparatorOnly) return new Token(TokenEnum.EndOfStatement, TextLine, TextColumn);
                        }
                        else if (ch == '*') // Multi-line multi-level comment starting with '/*'
                        {
                            SkipMultiLineComment(line, column);
                        }
                        else // divider
                        {
                            UngetChar();
                            return new Token(TokenEnum.OperatorDivide, line, column);
                        }
                        break;
                    case '%':
                        return new Token(TokenEnum.OperatorModulo, line, column);
                    case '*':
                        return new Token(TokenEnum.OperatorMultiply, line, column);
                    case '+':
                        return char.IsDigit(PeekChar()) ? GetNumberToken(line, column, '+') : 
                            new Token(TokenEnum.OperatorAdd, line, column);
                    case '-':
                        return char.IsDigit(PeekChar()) ? GetNumberToken(line, column, '-') : 
                            new Token(TokenEnum.OperatorSubtract, line, column);
                    case ';':
                        if (StatementSeparator == StatementSeparator.EndOfLineOnly) goto labelUnexpectedCharacter;
                        return new Token(TokenEnum.EndOfStatement, line, column);
                    case ',':
                        return new Token(TokenEnum.ParameterSeparator, line, column);
                    case '(':
                        return new Token(TokenEnum.OpenParenthesis, line, column);
                    case ')':
                        return new Token(TokenEnum.CloseParenthesis, line, column);
                    case '[':
                        return new Token(TokenEnum.OpenSquareBracket, line, column);
                    case ']':
                        return new Token(TokenEnum.CloseSquareBracket, line, column);
                    case '{':
                        return new Token(TokenEnum.OpenCurlyBrace, line, column);
                    case '}':
                        return new Token(TokenEnum.CloseCurlyBrace, line, column);
                    case '=':
                        return new Token(TokenEnum.OperatorAssign, line, column);
                    case CHAR_CR:
                    case CHAR_LF:
                        if (StatementSeparator != StatementSeparator.SeparatorOnly) return new Token(TokenEnum.EndOfStatement, line, column);
                        break;
                    case '\"':
                        return GetStringToken(line, column, ch);
                    default:
                        // Handle names
                        if (IsASCIILetter(ch))
                        {
                            return GetNameOrKeywordToken(line, column, ch);
                        }
                        // Handle numbers
                        if (char.IsDigit(ch))
                        {
                            return GetNumberToken(line, column, ch);
                        }
                        // Unexpected character
                        goto labelUnexpectedCharacter;
                }
            }
        labelUnexpectedCharacter:
            throw new LineColumnException(line, column, "Unexpected character " + (char.IsControl(ch) ? ((int)ch).ToString("X") : "'" + ch + "'") + "!");
        }
}