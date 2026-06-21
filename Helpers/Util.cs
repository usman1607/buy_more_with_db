using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuyMore.Helpers
{
    public static class Util
    {
        public static string GenerateReference(string prefix)
        {
            return $"{prefix}-{DateTime.Now:yyyyMMddHHmmssfff}";
        }
    }
}