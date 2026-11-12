using System;
using System.Collections.Generic;
using System.Text;

namespace TaleScript
{
    internal enum TokenType
    {
        NodeId,

        //Commands
        KeywordSay,
        KeywordChoice,
        KeywordGoto,
        KeywordSet,

        //Literals
        StringLiteral,
        Identifier,
        BooleanLiteral,

        //Assigners
        Assign,
        Arrow,

        EOF
    }
}
