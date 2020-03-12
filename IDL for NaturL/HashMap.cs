using System.Collections.Generic;

namespace IDL_for_NaturL
{
    public class HashMap : MainWindow
    {
        private List<List<string>> hashMap;
        private int tabNumber;

        public HashMap(List<List<string>> hashMap)
        {
            this.hashMap = hashMap;
            this.tabNumber = 0;
            List<string> firstElements = new List<string>()
            {
                "CodeBox0","Python0","STD0"
            };
        }
    }
}