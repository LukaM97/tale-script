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

        private bool IsAtEnd()
        {
            return current >= source_.Length;
        }


        private string Advance()
        {
            while (!IsAtEnd() && char.IsWhiteSpace(source_[current]))
            {
                if (source_[current] == '\n') line++;
                current++;
            }

            // Update start to the beginning of the actual word/token
            start = current;

            if (IsAtEnd()) return "";

            // 2. If it's a standalone symbol like '=', return it immediately
            if (source_[current] == '=')
            {
                current++;
                return "=";
            }

            // 3. Otherwise, read characters until we hit whitespace or '='
            while (!IsAtEnd() && !char.IsWhiteSpace(source_[current]) && source_[current] != '=')
            {
                current++;
            }

            return source_.Substring(start, current - start);
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return source_[current];
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source_[current] != expected) return false;

            current++;
            return true;
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object literal)
        {
            string text = source_.Substring(start, current - start);
            tokens_.Add(new Token(type, text, literal, line));
        }

        private void ScanToken()
        {
            string c = Advance();
            if (string.IsNullOrEmpty(c)) return;

            switch (c)
            {
                case "SAY": AddToken(TokenType.KeywordSay); break;
                case "CHOICE": AddToken(TokenType.KeywordChoice); break;
                case "SET": AddToken(TokenType.KeywordSet); break;
                case "GOTO": AddToken(TokenType.KeywordGoto); break;
                case "->": AddToken(TokenType.Arrow); break;
                case "=": AddToken(TokenType.Assign); break;
                default: AddToken(TokenType.Identifier); break;
            }
        }
    }
}