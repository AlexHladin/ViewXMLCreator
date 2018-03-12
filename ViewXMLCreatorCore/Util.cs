using System;
using System.Linq;

namespace ViewXMLCreatorCore
{
    public class Util
    {
        public static bool IsArrayWithNumbers(string[] array)
        {
            int n;
            return !array
                .Select(element => int.TryParse(element, out n))
                .Contains(false);
        }
    }
}
