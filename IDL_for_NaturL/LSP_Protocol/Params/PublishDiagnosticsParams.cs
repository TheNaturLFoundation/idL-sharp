namespace IDL_for_NaturL
{
    public class PublishDiagnosticsParams
    {
        public string uri;
        public Diagnostic[] diagnostics;
    }

    public class Diagnostic
    {
        public Range range;
        public DiagnosticSeverity? severity;
        public string message;
    }
}