using aBotScriptEditor.Models;
using aBotScriptEditor.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aBotScriptEditor
{
    public partial class Form1 : Form
    {
        private string filePath;
        private ElementInfo elementInfo;

        public Form1()
        {
            InitializeComponent();
            this.filePath = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadEditor();
        }

        private void LoadEditor()
        {
            string curDir = Directory.GetCurrentDirectory();
            string url = String.Format("file:///{0}/editor/index.html", curDir);
            this.webBrowser1.Navigate(url);
        }

        private void LoadFile()
        {
            try
            {
                using (FileStream fs = new FileStream(this.filePath, FileMode.Open, FileAccess.Read))
                {
                    StreamReader sr = new StreamReader(fs, Encoding.UTF8);
                    string content = sr.ReadToEnd();
                    SetEditorValue(content);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtHtml_TextChanged(object sender, EventArgs e)
        {
            this.dgvAttributes.Tag = "busy";
            string html = this.txtHtml.Text;
            ElementParser parser = new ElementParser();
            this.elementInfo = parser.Parse(html);
            this.txtTagName.Text = this.elementInfo.TagName;
            this.dgvAttributes.DataSource = null;
            this.dgvAttributes.Rows.Clear();
            foreach (KeyValuePair<string, string> kv in this.elementInfo.Attributes)
            {
                this.dgvAttributes.Rows.Add(kv.Key, kv.Value);
            }
            this.dgvAttributes.Tag = "";
            this.txtSelector.Text = new CodeGenerator().CssSelector(this.elementInfo.TagName, this.elementInfo.Attributes);
        }

        private void RefreshElementInfoAttributes()
        {
            if (this.dgvAttributes.Tag.ToString().Equals(""))
            {
                this.elementInfo.Attributes = new Dictionary<string, string>();
                foreach (DataGridViewRow row in this.dgvAttributes.Rows)
                {
                    object key = row.Cells[0].Value;
                    object value = row.Cells[1].Value;
                    if (key != null && value != null)
                    {
                        this.elementInfo.Attributes.Add(key.ToString(), value.ToString());
                    }
                }
                this.txtSelector.Text = new CodeGenerator().CssSelector(this.elementInfo.TagName, this.elementInfo.Attributes);
            }
        }

        private void dgvAttributes_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            RefreshElementInfoAttributes();
        }

        private void dgvAttributes_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            RefreshElementInfoAttributes();
        }

        private void FormatCode()
        {
            this.webBrowser1.Document.Focus();
            SendKeys.Send("%+f");
        }

        private void SetEditorValue(string content)
        {
            object[] args = { content };
            this.webBrowser1.Document.InvokeScript("setEditorValue", args);
        }

        private void InsertCode(string code)
        {
            dynamic position = this.webBrowser1.Document.InvokeScript("getCursorPosition");
            var data = new
            {
                LineNumber = position.lineNumber,
                Column = position.column
            };
            string content = this.webBrowser1.Document.InvokeScript("getEditorValue").ToString();
            string[] lines = content.Split('\n');
            lines[data.LineNumber - 1] = lines[data.LineNumber - 1].Insert(data.Column - 1, code);
            content = String.Join("\n", lines);
            SetEditorValue(content);
            FormatCode();
        }

        private string GetEditorValue()
        {
            return this.webBrowser1.Document.InvokeScript("getEditorValue").ToString();
        }

        private bool SaveAs()
        {
            bool result = false;
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                result = true;
                this.filePath = sfd.FileName;
            }
            return result;
        }

        private void Save()
        {
            try
            {
                if (this.filePath != "")
                {
                    using (var fs = new FileStream(this.filePath, FileMode.Create, FileAccess.Write))
                    {
                        var sr = new StreamWriter(fs);
                        sr.Write(GetEditorValue());
                        sr.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            CodeGenerator cg = new CodeGenerator();
            string code = cg.Describe(this.txtDescribe.Text);
            InsertCode(code);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            CodeGenerator cg = new CodeGenerator();
            string code = cg.It(this.txtIt.Text);
            InsertCode(code);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CodeGenerator cg = new CodeGenerator();
            string code = cg.Click(this.elementInfo);
            InsertCode(code);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CodeGenerator cg = new CodeGenerator();
            string code = cg.SetValue(this.elementInfo, this.txtValue.Text);
            InsertCode(code);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CodeGenerator cg = new CodeGenerator();
            int ms = Int32.Parse(this.numMS.Value.ToString());
            string code = cg.WaitMS(ms);
            InsertCode(code);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CodeGenerator cg = new CodeGenerator();
            string code = cg.WaitForExist(this.elementInfo);
            InsertCode(code);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            CodeGenerator cg = new CodeGenerator();
            string code = cg.Url(this.txtUrl.Text);
            InsertCode(code);
        }

        private void 開啟ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.filePath = ofd.FileName;
                LoadFile();
            }
        }

        private void 儲存ctrlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isOK = true;
            if (this.filePath == "")
            {
                isOK = SaveAs();
            }
            if (isOK)
            {
                Save();
            }
        }

        private void 另存新檔ctrlshiftsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SaveAs())
            {
                Save();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CodeGenerator cg = new CodeGenerator();
            string attrName = "";
            if (this.cbAttributeName.Text != null)
            {
                string temp = this.cbAttributeName.Text.Trim();
                if (temp.Length > 0)
                {
                    attrName = temp;
                }
            }
            if(this.txtAttributeName.Text!=null)
            {
                string temp = this.txtAttributeName.Text.Trim();
                if (temp.Length > 0)
                {
                    attrName = temp;
                }
            }
            string code = cg.GetAttribute(this.elementInfo,this.txtVarName.Text,attrName);
            InsertCode(code);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            CodeGenerator cg = new CodeGenerator();
            string code = cg.ShouldEqual(this.txtConditionVarName.Text,this.txtExpectValue.Text);
            InsertCode(code);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            CodeGenerator cg = new CodeGenerator();
            string code = cg.TakeScreenshot(this.txtScreenshotName.Text);
            InsertCode(code);
        }
    }
}
