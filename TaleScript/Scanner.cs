using System;
using System.Collections.Generic;
using System.Text;
using TaleScript;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaleScript
{
    internal class Scanner
    {
        private string source_ { get; set; }
        private List<Token> tokens_ { get; set; } = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        private static readonly Dictionary<string, TokenType> Keywords = new() {
            { "SAY", TokenType.KeywordSay },
            { "CHOICE", TokenType.KeywordChoice },
            { "SET", TokenType.KeywordSet },
            { "GOTO", TokenType.KeywordGoto },
            { "IF", TokenType.KeywordIf },
            { "ELSE", TokenType.KeywordElse },
            { "HINT", TokenType.KeywordHint },
            { "BACK", TokenType.KeywordBack }
        };

        public Scanner(string source)
        {
            source_ = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens_.Add(new Token(TokenType.EOF, "", null, line));
            return tokens_;
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private char Peek() => IsAtEnd() ? '\0' : source_[current];
        private bool IsAlpha(char c) => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';
        private bool IsAlphaNumeric(char c) => IsAlpha(c) || (c >= '0' && c <= '9');
        private char Advance() => source_[current++];
        private bool IsAtEnd() => current >= source_.Length;
        private void AddToken(TokenType type, object literal = null) =>
            tokens_.Add(new Token(type, source_.Substring(start, current - start), literal,line));

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek())) Advance();
            string text = source_.Substring(start, current - start);
            string upper = text.ToUpper();
            if (Keywords.TryGetValue(upper, out TokenType type))
            {
                AddToken(type, text);
            }
            else
            {
                AddToken(TokenType.Identifier, text);
            }
        }

        private void StringLiteral()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }
            if (IsAtEnd())
            {
                Console.WriteLine($"[Line {line}] Greška: Nezavršen string.");
                return;
            }
            Advance();
            string value = source_.Substring(start + 1, current - start - 2);
            AddToken(TokenType.StringLiteral, value);
        }

        private bool Match(char expected)
        {
            if (IsAtEnd() || source_[current] != expected) return false;
            current++;
            return true;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case ':': AddToken(TokenType.Colon); break;
                case '-':
                    if (Match('>')) AddToken(TokenType.Assign);
                    break;
                case '@':
                    while (IsAlphaNumeric(Peek())) Advance();
                    AddToken(TokenType.NodeId, source_.Substring(start + 1, current - start - 1));
                    break;
                case ' ':
                case '\r':
                case '\t': break;
                case '\n': line++; break;
                case '"': StringLiteral(); break;
                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    break;
                default:
                    if (IsAlpha(c)) Identifier();
                    else Console.WriteLine($"[Line {line}] Greška: Nepoznat karakter '{c}'.");
                    break;
            }
        }
    }
}