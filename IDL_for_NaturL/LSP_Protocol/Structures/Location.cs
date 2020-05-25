using System;

namespace IDL_for_NaturL
{
    public struct Location
    {
        public string uri;
        public Range range;

        public override string ToString()
        {
            return $"Uri is {uri}.";
        }
    }

}