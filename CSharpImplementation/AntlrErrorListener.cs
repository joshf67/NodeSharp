using Antlr4.Runtime;

namespace NodeSharp;

public class AntlrErrorListener : IAntlrErrorListener<IToken>
{
    public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
        RecognitionException e)
    {
        if (offendingSymbol is CommonToken token)
            throw new Exception($"Unknown input: '{token.Text}', at {line}:{charPositionInLine}");

        throw new NotImplementedException();
    }
}