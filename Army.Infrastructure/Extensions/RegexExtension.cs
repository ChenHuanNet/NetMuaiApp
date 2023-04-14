using Army.Infrastructure.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Army.Infrastructure.Extensions
{
    public static class RegexExtension
    {


        public static string RemoveHtmlLabel(this string html)
        {
            string strText = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
            return strText;
        }

        /// <summary>
        /// 获取所有标签的href
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<string> GetHtmlAHref(this string html)
        {
            string pattern = @"(?is)<[^>]*?href=(['""\s]?)(?<href>[^'""\s]*)\1[^>]*?></a>";

            Regex regex = new Regex(pattern);
            var list = regex.Matches(html);

            List<string> res = new List<string>();
            foreach (Match item in list)
            {
                res.Add(item.Value);
            }

            return res;
        }


        /// <summary>
        /// 获取html中所有input标签的 name 和 value
        /// </summary>
        /// <param name="html"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetHtmlInput(this string html, string[] keys)
        {

            var ary = Regex.Matches(html, @"(?is)<input(?=[^>]*?name=[""'](?<name>[^""'\s]+)[""'])(?=[^>]*?value=[""'](?<value>[^""'\s]+)[""'])[^>]+>").OfType<Match>().Select(t => new { name = t.Groups["name"].Value, value = t.Groups["value"].Value }).ToArray().ToList();

            var dic = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                dic.Add(key, ary.Find(x => x.name == key).value);
            }
            return dic;
        }


        /// <summary>  
        /// 获取第一个匹配到的标签中的值： <a href="www.csdn.net" class="main" >CSDN</a> 结果：CSDN  
        /// </summary>  
        /// <param name="str">字符串</param>  
        /// <param name="title">标签</param>  
        /// <returns>值</returns>  
        public static string GetHtmlTextFirst(this string str, string title)
        {
            string tmpStr = string.Format("<{0}[^>]*?>(?<Text>[^<]*)</{0}>", title); //获取<title>之间内容  

            Match TitleMatch = Regex.Match(str, tmpStr, RegexOptions.IgnoreCase);

            string result = TitleMatch.Groups["Text"].Value;
            return result;
        }



        /// <summary>  
        /// 获取第一个标签的 内容
        /// </summary>  
        /// <param name="label">字符串</param>  
        /// <param name="title">标签</param>  
        /// <returns>值</returns>  
        public static string GetHtmlLabel(this string html, string label)
        {
            string tmpStr = string.Format(@"<{0}[^>]*?>((?>(?<o><{0}[^>]*>)|(?<-o></{0}>)|(?:(?!</?{0})[\s\S]))*)(?(o)(?!))</{0}>", label);
            var ary = Regex.Match(html, tmpStr, RegexOptions.IgnoreCase);
            return ary.Value;
        }


        /// <summary>  
        /// 获取第一个匹配到的标签中的属性： <a href="www.csdn.net" class="main">CSDN</a>  获取 “href” 的结果：www.csdn.net  
        /// </summary>  
        /// <param name="str">字符串</param>  
        /// <param name="title">标签</param>  
        /// <param name="attrib">属性名</param>  
        /// <returns>属性</returns>  
        /// <summary>  
        public static string GetHtmlAttrFirst(this string str, string title, string attrib)
        {
            string tmpStr = string.Format("<{0}[^>]*?{1}=(['\"\"]?)(?<url>[^'\"\"\\s>]+)\\1[^>]*>", title, attrib); //获取<title>之间内容  

            Match TitleMatch = Regex.Match(str, tmpStr, RegexOptions.IgnoreCase);

            string result = TitleMatch.Groups["url"].Value;
            return result;
        }


        /// <summary>  
        /// 匹配第一条属性名attrid=attrv 的html内容
        /// </summary>  
        /// <param name="str">字符串</param>  
        /// <param name="title">标签</param>  
        /// <param name="attrid">属性名</param>  
        /// <param name="attrv">属性值</param>  
        /// <returns>html</returns>  
        /// <summary>  
        public static string GetFirstHtmlWithAttr(this string str, string title, string attrId, string attrVal)
        {
            string tmpStr = string.Format(@"<{0}[^>]*?{1}=""{2}""[^>]*>((?>(?<o><{0}[^>]*>)|(?<-o></{0}>)|(?:(?!</?{0})[\s\S]))*)(?(o)(?!))</{0}>", title, attrId, attrVal);
            Match m = Regex.Match(str, tmpStr, RegexOptions.IgnoreCase);

            return m.Value;
        }



        /// <summary>
        /// 匹配所有标签属性=attrib的属性值
        /// </summary>
        /// <param name="html"></param>
        /// <param name="title"></param>
        /// <param name="attrib"></param>
        /// <returns></returns>
        public static List<string> GetHtmlAttrs(this string html, string title, string attrib)
        {
            string tmpStr = string.Format("<{0}[^>]*?{1}=(['\"\"]?)(?<url>[^'\"\"\\s>]+)\\1[^>]*>", title, attrib); //获取<title>之间内容  

            var ary = Regex.Matches(html, tmpStr).OfType<Match>().Select(t => new { attr = t.Groups["url"].Value }).ToArray().ToList();

            List<string> result = new List<string>();
            foreach (var item in ary)
            {
                result.Add(item.attr);
            }
            return result;
        }


        /// <summary>  
        /// 获取第一个匹配到的标签含有属性attrib的标签内容： <a href="www.csdn.net" class="main" >CSDN</a> 结果：CSDN  
        /// </summary>  
        /// <param name="html">字符串</param>  
        /// <param name="title">标签</param>  
        /// <param name="attrib">属性名称</param>  
        /// <returns>值</returns>  
        public static string GetHtmlTextFirst(this string html, string title, string attrib, string attrvstartwith)
        {
            string tmpStr = string.Format(@"<{0}[^>]*?{1}=""{2}[^>]*>((?>(?<o><{0}[^>]*>)|(?<-o></{0}>)|(?:(?!</?{0})[\s\S]))*)(?(o)(?!))</{0}>", title, attrib, attrvstartwith);
            var ary = Regex.Match(html, tmpStr, RegexOptions.IgnoreCase);
            return ary.Value;
        }



        /// <summary>  
        /// 获取所有匹配到的标签含有属性attrib的标签内容： <a href="www.csdn.net" class="main" >CSDN</a> 结果：CSDN  
        /// </summary>  
        /// <param name="html">字符串</param>  
        /// <param name="title">标签</param>  
        /// <param name="attrib">属性名称</param>  
        /// <returns>值</returns>  
        public static List<string> GetHtmlTexts(this string html, string title, string attrib, string attrvstartwith)
        {
            string tmpStr = string.Format(@"<{0}[^>]*?{1}=""{2}[^>]*>((?>(?<o><{0}[^>]*>)|(?<-o></{0}>)|(?:(?!</?{0})[\s\S]))*)(?(o)(?!))</{0}>", title, attrib, attrvstartwith);
            var ary = Regex.Matches(html, tmpStr, RegexOptions.IgnoreCase);
            List<string> result = new List<string>();
            for (int i = 0; i < ary.Count; i++)
            {
                result.Add(ary[i].Value);
            }
            return result;
        }




        /// <summary>
        /// 获取a标签的href 和 内容
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<HtmlALabel> GetAHrefAndText(this string html)
        {
            string pattern = @"<a[^>]*href=(""(?<href>[^""]*)""|'(?<href>[^']*)'|(?<href>[^\s>]*))[^>]*>(?<text>[\s\S]*?)</a>";

            MatchCollection mcs = Regex.Matches(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            List<HtmlALabel> res = new List<HtmlALabel>();

            HtmlALabel a;
            foreach (Match item in mcs)
            {
                a = new HtmlALabel()
                {
                    Href = item.Groups["href"].Value,
                    Text = item.Groups["text"].Value,
                };
                res.Add(a);
            }

            return res;
        }

        /// <summary>
        /// 获取 属性ID=属性值 的 标签的 html内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="title"></param>
        /// <param name="attrId"></param>
        /// <param name="attrVal"></param>
        /// <returns></returns>
        public static List<string> GetHtmlWithAttr(this string html, string title, string attrId, string attrVal)
        {
            string pattern = string.Format(@"<{0}[^>]*?{1}=""{2}""[^>]*>((?>(?<o><{0}[^>]*>)|(?<-o></{0}>)|(?:(?!</?{0})[\s\S]))*)(?(o)(?!))</{0}>", title, attrId, attrVal);

            MatchCollection mcs = Regex.Matches(html, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            List<string> res = new List<string>();
            foreach (Match item in mcs)
            {
                res.Add(item.Value);
            }
            return res;
        }

    }
}
