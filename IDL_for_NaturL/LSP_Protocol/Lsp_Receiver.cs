using System.Collections.Generic;
using System.IO;

namespace IDL_for_NaturL
{
    public interface Lsp_Receiver
    {
        void ReceiveKeywords(List<string> keywords);
        void JumpToDefinition(Location location);
    }
}