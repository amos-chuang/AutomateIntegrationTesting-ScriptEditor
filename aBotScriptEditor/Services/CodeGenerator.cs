using aBotScriptEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aBotScriptEditor.Services
{
    class CodeGenerator
    {
        public string Describe(string title)
        {
            string result = String.Format("describe('{0}', function () {{\n\n}});", title);
            return result;
        }

        public string It(string title)
        {
            string result = String.Format("it('{0}', function () {{\n\n}});", title);
            return result;
        }

        public string CssSelector(string tagName, Dictionary<string, string> attributes,int nthOfType=0,int nthChild=0)
        {
            if (tagName != null)
            {
                tagName = tagName.Trim();
            }
            else { tagName = ""; }
            string condition = "";
            if (attributes != null && attributes.Count > 0)
            {
                List<string> conditionList = new List<string>();
                foreach (KeyValuePair<string, string> kv in attributes)
                {
                    string temp = String.Format("[{0}='{1}']", kv.Key, kv.Value);
                    conditionList.Add(temp);
                }
                condition = String.Join("", conditionList);
            }
            string result = tagName + condition;
            if (nthOfType > 0)
            {
                result += String.Format(":nth-of-type({0})", nthOfType);
            }
            if (nthChild > 0)
            {
                result += String.Format(":nth-child({0})", nthChild);
            }
            return result;
        }

        public string Url(string url)
        {
            string result = String.Format("browser.url(\"{0}\");", url);
            return result;
        }

        public string Click(ElementInfo ei)
        {
            string selector = ei.CssSelector;
            string result = String.Format("browser.click(\"{0}\");", selector);
            return result;
        }

        public string SetValue(ElementInfo ei, string value)
        {
            string selector = ei.CssSelector;
            string result = String.Format("browser.setValue(\"{0}\",\"{1}\");", selector, value);
            return result;
        }

        public string WaitForExist(ElementInfo ei)
        {
            string selector = ei.CssSelector;
            string result = String.Format("browser.waitForExist(\"{0}\");", selector);
            return result;
        }

        public string WaitMS(int ms)
        {
            string result = String.Format("waitMS(\"{0}\");", ms);
            return result;
        }

        public string GetAttribute(ElementInfo ei, string varName, string attrName)
        {
            string selector = ei.CssSelector;
            string result = "";
            switch (attrName)
            {
                case "innerText":
                    result = String.Format("var {0} = browser.$(\"{1}\").getText();", varName, selector);
                    break;
                default:
                    result = String.Format("var {0} = browser.$(\"{1}\").getAttribute(\"{2}\");", varName, selector, attrName);
                    break;
            }
            return result;
        }

        public string ShouldEqual(string varName, string value)
        {
            string result = String.Format("{0}.should.equal(\"{1}\");",varName,value);
            return result;
        }

        public string TakeScreenshot(string fileName)
        {
            string result = String.Format("takeScreenshot(\"{0}\");",fileName);
            return result;
        }
    }
}
