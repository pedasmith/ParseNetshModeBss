using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal static class HtmlStringUtilities
    {
        public static string td(this string text)
        {
            // See http://sunriseprogrammer.blogspot.com/2022/09/clipboard-data-for-excel.html for why!

            var escaped = System.Net.WebUtility.HtmlEncode(text);
            return $"<td>{escaped}</td>";
        }
        public static string th(this string text)
        {
            var escaped = System.Net.WebUtility.HtmlEncode(text);
            return $"<th>{escaped}</th>";
        }

        /// <summary>
        /// text is expected to be a string like <td>data</td><td>value</td>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string tr(this string text)
        {
            return $"<tr>{text}</tr>\n";
        }
        /// <summary>
        /// trs is expected to be a series of tr lines
        /// </summary>
        /// <param name="trs"></param>
        /// <returns></returns>
        public static string html(this string trs)
        {
            return $"<html>\r\n<body>\r\n<table>{trs}</table>\r\n</body>\r\n</html>";
        }
    }
}
