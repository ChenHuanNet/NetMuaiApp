using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Army.Infrastructure.Extensions
{
    public static class StringExtension
    {

        public static string DecodeEncodedNonAsciiCharacters(this string value)
        {
            return Regex.Replace(
             value,
             @"\\u(?<Value>[a-zA-Z0-9]{4})",
             m =>
             {
                 return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
             });
        }
    }
}
