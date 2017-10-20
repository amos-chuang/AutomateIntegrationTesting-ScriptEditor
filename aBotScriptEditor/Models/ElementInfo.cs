using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aBotScriptEditor.Models
{
    class ElementInfo
    {
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public string Value { get; set; }
        public string InnerText { get; set; }
    }
}
