using aBotScriptEditor.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aBotScriptEditor.Services
{
    class ElementParser
    {
        public ElementInfo Parse(string html)
        {
            ElementInfo result = new ElementInfo();
            try
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                HtmlNode node = htmlDoc.DocumentNode.ChildNodes.FirstOrDefault();
                result.TagName = node.Name;
                result.InnerText = node.InnerText;
                if (node.Attributes.Contains("value"))
                {
                    result.Value = node.Attributes["value"].Value;
                }
                result.Attributes = new Dictionary<string, string>();
                foreach (HtmlAttribute attr in node.Attributes)
                {
                    result.Attributes.Add(attr.Name, attr.Value);
                }
            }
            catch (Exception ex) { }
            return result;
        }
    }
}
