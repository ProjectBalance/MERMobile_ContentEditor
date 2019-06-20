using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Contentful.Core.Models;
using Contentful.Importer.Library.Extensions;

namespace Contentful.Importer.App.Interfaces.EditTemplates
{
    public partial class TextAreaControl : UserControl, Template
    {
        public int numchar;
        public Field Field { get; set; }
        public void SetFieldValue(Field field)
        {
            this.Field = field;
            if (Field.Required)
            {
                SetLabel(field.Name + "*");
            }
            else
            {
                SetLabel(field.Name);
            }
        }
        public Field GetFieldValue()
        {
            return Field;
        }
        public TextAreaControl()
        {
            InitializeComponent();
        }

        public string GetValue()
        {
            return txtArea.Text;
        }

        public void SetLabel(string value)
        {
            lblHeading.Text = value;
        }

        public void SetValue(string value)
        {
            txtArea.Text = value.Replace("\r\r\n", "\r\n");
        }
        private string WriteFontStyleTag(string line, string tag, string styleData)
        {
            var output = line.ToString();
            while (output.Contains(tag))
            {
                output = output.ReplaceFirst(tag, "<font style = '"+styleData+"'>");
                output = output.ReplaceFirst(tag, "</font>");
            }
            return output;
        }
        public string GetHTMLEquivalent()
        {
            string code = "";
            foreach (var line in txtArea.Lines)
            {
                var codeline = line;

                codeline = WriteFontStyleTag(codeline, "<t-red>", "color:#FF1C1C");
                codeline = WriteFontStyleTag(codeline, "<t-yellow>", "color:#C4C413");
                codeline = WriteFontStyleTag(codeline, "<t-green>", "color:#038503");
                codeline = WriteFontStyleTag(codeline, "<t-blue>", "color:#0D8686");

                codeline = WriteFontStyleTag(codeline, "<red>", "background-color:#FF8080");
                codeline = WriteFontStyleTag(codeline, "<yellow>", "background-color:#FFFF80");
                codeline = WriteFontStyleTag(codeline, "<green>", "background-color:#80FF80");
                codeline = WriteFontStyleTag(codeline, "<blue>", "background-color:#00FFFF");


                codeline = BlockQuote(codeline);
                codeline = Bullet(codeline);
                codeline = Indent(codeline);

                codeline = Bold(codeline);
                codeline = Strikeout(codeline);
                codeline = Italics(codeline);
                codeline = Header3(codeline);
                codeline = Header2(codeline);
                codeline = Header1(codeline);
                codeline = HTMLLinks(codeline);



                if (codeline.Contains("</h") || codeline.Contains("</block") || codeline.Contains("</li") || codeline.Contains("</p") || codeline.Contains("<br><font"))
                {
                    code += codeline;
                }
                else
                {
                    code += "<br>" + codeline;
                }
            }
            StringBuilder builder = new StringBuilder(code);
            builder.Replace("</h1><br>", "</h1>");
            builder.Replace("</h2><br>", "</h2>");
            builder.Replace("</h3><br>", "</h3>");
            code = builder.ToString();
            code = "<div style='font-family:Arial;font-size:12px'>" + code + "</div>";
            return code;
        }
        #region Transformers
        private string Link(string lineInput)
        {

            string lineResult = lineInput.ToString();

            return lineResult;
        }

        public string HTMLLinks(string line)
        {
            var result = line;
            var startindex = 0;

            var stringSnippet = line;
            while (true)
            {
                var nextIndex = stringSnippet.IndexOf("[");
                if (nextIndex >= 0)
                {
                    //concat
                    //result += line.Substring(startindex, nextIndex + startindex);
                    var workingonSnippet = stringSnippet.Substring(nextIndex);

                    var endIndex = workingonSnippet.IndexOf(")");
                    if (endIndex > 0)
                    {
                        var subSnippet = workingonSnippet.Substring(0, endIndex+1);
                        var label = GetStringEnclosedBy(subSnippet, "[", "]");
                        var link = GetStringEnclosedBy(subSnippet, "(", ")");
                        var tooltip = GetStringEnclosedBy(subSnippet, "\"", "\"");
                        if (!string.IsNullOrEmpty(tooltip))
                        {
                            link = link.Replace("\""+tooltip+"\"", "");
                        }
                        if(!string.IsNullOrEmpty(label) && !string.IsNullOrEmpty(link))
                        {
                            var linkData = "<a href='Javascript:void(0)' title='"+(tooltip??"")+"'>"+label+"</a>";
                            result = result.Replace(subSnippet, linkData);
                        }                        

                    }
                    
                    //result += workingonSnippet;
                    stringSnippet = result;
                    
                }
                else
                {

                    //result += stringSnippet;
                    return result;
                }
            }
            return result;
        }

        public string GetStringEnclosedBy(string value, string open, string close)
        {
            var startindex = value.IndexOf(open);
            if (startindex > -1 && startindex+1 < value.Length)
            {
                var remainder = value.Substring(startindex+1, value.Length - (startindex+1));
                var endindex = remainder.IndexOf(close);
                if(endindex > -1)
                {
                    return remainder.Substring(0, endindex);
                }
            }
            return null;
        }
        private string Bold(string lineInput)
        {
            string starttag = "<b>";
            string endtag = "</b>";
            string lineResult = lineInput.ToString();
            while (IsMatchAndRewrite(false, lineResult, "__", starttag, endtag, out lineResult))
            {

            }
            return lineResult;
        }
        private string Strikeout(string lineInput)
        {
            string starttag = "<del>";
            string endtag = "</del>";
            string lineResult = lineInput.ToString();
            while (IsMatchAndRewrite(false, lineResult, "~~", starttag, endtag, out lineResult))
            {

            }
            return lineResult;
        }
        private string Italics(string lineInput)
        {
            string starttag = "<i>";
            string endtag = "</i>";
            string lineResult = lineInput.ToString();
            while (IsMatchAndRewrite(false, lineResult, "*", starttag, endtag, out lineResult))
            {

            }
            return lineResult;
        }
        private string Bullet(string lineInput)
        {

            string starttag = "<li>";
            string endtag = "</li>";
            string lineResult = lineInput.ToString();
            while (IsMatchAndRewrite(true, lineResult, "• ", starttag, endtag, out lineResult))
            {

            }
            return lineResult;
        }
        private string Indent(string lineInput)
        {

            string starttag = "<br><font style=\"margin - left: 10px; \">";
            string endtag = "</font>";
            string lineResult = lineInput.ToString();
            while (IsMatchAndRewrite(true, lineResult, "    ", starttag, endtag, out lineResult))
            {

            }
            return lineResult;
        }
        private string BlockQuote(string lineInput)
        {

            string starttag = "<blockquote class=\"markdown-quote markdown-block\"><i>";
            string endtag = "</i></blockquote>";
            string lineResult = lineInput.ToString();
            while (IsMatchAndRewrite(true, lineResult, "> ", starttag, endtag, out lineResult))
            {

            }
            return lineResult;
        }
        private string Header3(string lineInput)
        {

            string starttag = "<h3>";
            string endtag = "</h3>";
            string lineResult = lineInput.ToString();
            while (IsMatchAndRewrite(true, lineResult, "### ", starttag, endtag, out lineResult))
            {

            }
            return lineResult;
        }
        private string Header2(string lineInput)
        {

            string starttag = "<h2>";
            string endtag = "</h2>";
            string lineResult = lineInput.ToString();
            while (IsMatchAndRewrite(true, lineResult, "## ", starttag, endtag, out lineResult))
            {

            }
            return lineResult;
        }
        private string Header1(string lineInput)
        {

            string starttag = "<h1>";
            string endtag = "</h1>";
            string lineResult = lineInput.ToString();
            while (IsMatchAndRewrite(true, lineResult, "# ", starttag, endtag, out lineResult))
            {

            }
            return lineResult;
        }


        public bool IsMatchAndRewrite(bool isFullLineMarker, string input, string Pattern, string startTag, string endTag, out string result)
        {


            string buf = input.ToString();
            int firstindex = buf.IndexOf(Pattern);
            if (firstindex > -1)
            {
                if ((Pattern == "> " || Pattern == "    " || Pattern == "# " || Pattern == "## " || Pattern == "### ") && (firstindex != 0))
                {
                    //This is a heading, quote tag or indent that is not the first item on the line - ignore it
                    result = buf;
                    return false;
                }
                var tempBuf1 = buf.ReplaceFirst(Pattern, startTag);
                if (!isFullLineMarker)
                {//for anything that is not a heading, quote or indent, expect a closing tag 
                    firstindex = tempBuf1.IndexOf(Pattern);
                    if (firstindex > -1)
                    {
                        tempBuf1 = tempBuf1.ReplaceFirst(Pattern, endTag);
                        result = tempBuf1;
                    }
                    else
                    {
                        //closing pattern not found 
                        result = buf;
                        return false;
                    }
                }
                else //for full line markers - i.e. headings and quotes:
                {
                    tempBuf1 += endTag;
                    result = tempBuf1;
                    return true;
                }
            }
            else
            {
                result = buf;
                return false;
            }
            return true;


        }

        #endregion

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                browser.DocumentText = GetHTMLEquivalent();
            }
        }

        private void EncloseSelection(string startTags, string endTags = "")
        {
            if (txtArea.SelectionLength > 0)
            {
                string txtbox = txtArea.ToString();
                var start = txtArea.SelectionStart;
                var end = start + txtArea.SelectionLength;
                int startlen = startTags.Length;
                int endlen = endTags.Length;
                var scrollpos = txtArea.AutoScrollOffset;
                //first see if the tags already exist either side of the selected text
                if (start - startlen >= 0)
                {
                    string pre = txtArea.Text.Substring(start - startlen, startlen);
                    if (end + startlen < txtArea.TextLength +1)
                    {
                        //check if tags aren't already there
                        string post = txtArea.Text.Substring(start - startlen, startlen);
                        if (pre == startTags && post == endTags)
                        {
                            // This tag already exists, remove it (toggle)
                            txtArea.Text = txtArea.Text.Substring(0,start - startlen) + txtArea.Text.Substring(start,txtArea.SelectionLength) + txtArea.Text.Substring(end + endlen, txtArea.Text.Length - (end + endlen));
                            numchar = numchar - startlen - endlen;
                            return; //Ed
                        }
                    }
                }
                txtArea.Text = txtArea.Text.Insert(end, endTags).Insert(start, startTags);
                //txtArea.Select(start, 0);
                txtArea.AutoScrollOffset = scrollpos;
            }
        }
        private void btnBoldSelection_Click(object sender, EventArgs e)
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Multiple lines selected, please select only one");
            }
            else
            {
                Enclose("__", "__", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void Prepend (string strPrepend, int selstart, int sellen)
        {
            numchar = strPrepend.Length;
            PrependLines(strPrepend);
            if (selstart > 0)
            {
                this.ActiveControl = txtArea;
                txtArea.SelectionStart = selstart + numchar;
                txtArea.SelectionLength = sellen;
                txtArea.ScrollToCaret();
            }
        }

        private void Enclose(string strOpen, string strClose, int selstart, int sellen)
        {
            numchar = strOpen.Length;
            EncloseSelection(strOpen, strClose);
            if (selstart > 0)
            {
                this.ActiveControl = txtArea;
                txtArea.SelectionStart = selstart + numchar;
                txtArea.SelectionLength = sellen;
                txtArea.ScrollToCaret();                
            }
        }

        private void btnH1Select_Click(object sender, EventArgs e) //Heading 1
        {
            Prepend("# ", txtArea.SelectionStart, txtArea.SelectionLength);
        }

        private void btnH2Select_Click(object sender, EventArgs e) //Heading 2
        {
            Prepend("## ", txtArea.SelectionStart, txtArea.SelectionLength);
        }

        private void btnH3Select_Click(object sender, EventArgs e) //Heading 3
        {
            Prepend("### ", txtArea.SelectionStart, txtArea.SelectionLength);
        }

        private void btnItalics_Click(object sender, EventArgs e)
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Multiple lines selected, please select only one");
            }
            else
            {
                Enclose("*", "*", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void button4_Click(object sender, EventArgs e) //Strikethrough
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Multiple lines selected, please select only one");
            }
            else
            {
                Enclose("~~", "~~", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void button5_Click(object sender, EventArgs e) //Quote
        {
            Prepend("> ", txtArea.SelectionStart, txtArea.SelectionLength);
        }
      
        private void btnLink_Click(object sender, EventArgs e)
        {
            var linkdialog = new InsertLink(); //URL
            if (linkdialog.ShowDialog(this) == DialogResult.OK)
            {
                var start = txtArea.SelectionStart;

                txtArea.Text = txtArea.Text.Insert(start, linkdialog.ResultLink);
            }
        }

        private void btnTab_Click(object sender, EventArgs e) //indent
        {
            Prepend("    ", txtArea.SelectionStart, txtArea.SelectionLength);
        }
        private void PrependLines(string chars)
        {
            //
            int start = 0;
            int count = 0;

            int activeLine = this.txtArea.GetLineFromCharIndex(this.txtArea.SelectionStart) + 1;
            var selectectText = txtArea.SelectedText;
            if (string.IsNullOrEmpty(selectectText))
            {
                start = activeLine - 1;
                count = 0;
            }
            else
            {
                start = txtArea.SelectionStart;
                var end = txtArea.SelectionStart + txtArea.SelectionLength;
                int totalLinesStart = 0;
                int totalLinesSelected = 1;
                totalLinesSelected = CountNewLines(selectectText);

                if (start > 0)
                {
                    var startlineSelection = txtArea.Text.Substring(0, start);
                    totalLinesStart = CountNewLines(startlineSelection);
                }
                start = totalLinesStart;
                count = totalLinesSelected;
            }

            //
            List<string> lines = new List<string>();
            lines.AddRange(txtArea.Lines);
            for (int i = start; i <= (start + count); i++)
            {
                if (lines[i].StartsWith(chars))
                {
                    lines[i] = lines[i].ReplaceFirst(chars, "");
                    numchar = numchar * -1;
                }
                else
                {
                    lines[i] = chars + lines[i];
                }
            }

            txtArea.Lines = lines.ToArray();

        }
        private int CountNewLines(string input)
        {
            int count = 0;
            int a = 0;
            var pattern = Environment.NewLine;

            while ((a = input.IndexOf(pattern, a)) != -1)
            {
                a += pattern.Length;
                count++;
            }
            return count;
        }

        private void btnRedCol_Click(object sender, EventArgs e) //red text
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Please select text on one line at a time when applying colors");
            }
            else
            {
                Enclose("<t-red>", "<t-red>", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void btnYelCol_Click(object sender, EventArgs e) //yellow text
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Please select text on one line at a time when applying colors");
            }
            else
            {
                Enclose("<t-yellow>", "<t-yellow>", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void btnGreenCol_Click(object sender, EventArgs e) //Green text
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Please select text on one line at a time when applying colors");
            }
            else
            {
                Enclose("<t-green>", "<t-green>", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void btnBlueCol_Click(object sender, EventArgs e) //Blue highlight
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Please select text on one line at a time when applying colors");
            }
            else
            {
                Enclose("<t-blue>", "<t-blue>", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void btnRedBack_Click(object sender, EventArgs e) //Red highlight
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Please select text on one line at a time when applying colors");
            }
            else
            {
                Enclose("<red>", "<red>", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void btnYelBack_Click(object sender, EventArgs e) //Yellow highlight
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Please select text on one line at a time when applying colors");
            }
            else
            {
                Enclose("<yellow>", "<yellow>", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void btnGreenBack_Click(object sender, EventArgs e) //Green highlight
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Please select text on one line at a time when applying colors");
            }
            else
            {
                Enclose("<green>", "<green>", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void btnBlueBack_Click(object sender, EventArgs e) //Blue highlight
        {
            if (txtArea.SelectedText.Contains(Environment.NewLine))
            {
                MessageBox.Show("Please select text on one line at a time when applying colors");
            }
            else
            {
                Enclose("<blue>", "<blue>", txtArea.SelectionStart, txtArea.SelectionLength);
            }
        }

        private void button1_Click(object sender, EventArgs e) //bullet
        {
            Prepend("• ", txtArea.SelectionStart, txtArea.SelectionLength);
        }
    }
}
