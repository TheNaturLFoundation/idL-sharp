using System.Collections.Generic;
using System.IO;

namespace IDL_for_NaturL
{
    public interface LspReceiver
    {
        void JumpToDefinition(Location location);
        void Completion(IList<CompletionItem> completionList);
        void Diagnostic(Range range, DiagnosticSeverity warningSeverity, string message, string uri);
    }
}