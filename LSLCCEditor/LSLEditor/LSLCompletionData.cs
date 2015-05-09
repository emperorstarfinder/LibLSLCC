#region

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using LibLSLCC.Extensions;

#endregion

namespace LSLCCEditor.LSLEditor
{
    public class LSLCompletionData : ICompletionData
    {
        private readonly string _description;
        private readonly string _label;
        private readonly double _priority;
        private readonly string _text;
        private Brush _colorBrush = new SolidColorBrush(Color.FromRgb(50, 52, 138));

        public LSLCompletionData(string label, string text, string description, double priority)
        {
            _description = description;
            _priority = priority;
            _text = text;
            _label = label;
            TextSubStringStart = 1;
        }

        public bool ForceIndent { get; set; }
        public int IndentLevel { get; set; }
        public int TextSubStringStart { get; set; }

        public int CaretOffsetAfterInsert { get; set; }


        public bool OffsetCaretAfterInsert { get; set; }


        public bool InsertTextAtCaretAfterOffset { get; set; }

        public string CaretOffsetInsertionText { get; set; }

        public Brush ColorBrush
        {
            get { return _colorBrush; }
            set { _colorBrush = value; }
        }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            string text = Text;

            if (ForceIndent)
            {
                string indent = StringTools.CreateTabsString(IndentLevel);

                string result = "";

                var lines = text.Split('\n').ToList();

                for (int i = 0; i < lines.Count; i++)
                {
                    string line;
                    if (i == 0)
                    {
                        var j = completionSegment.Offset;
                        var start = j;
                        var end = 0;
                        while (j > 0)
                        {
                            char c = textArea.Document.Text[j];
                            if (c == '\n')
                            {
                                end = j + 1;
                                break;
                            }
                            j--;
                        }

                        textArea.Document.Replace(end, start - end, "");

                        line = indent + lines[i].Trim();
                    }
                    else
                    {
                        line = indent + lines[i].Trim();
                    }


                    if (i != lines.Count - 1)
                    {
                        line += '\n';
                    }
                    result += line;
                }

                text = result;
            }

            textArea.Document.Replace(completionSegment, text);

            if (OffsetCaretAfterInsert)
            {
                textArea.Caret.Offset = textArea.Caret.Offset+CaretOffsetAfterInsert;

                if (InsertTextAtCaretAfterOffset)
                {
                    textArea.Document.Insert(textArea.Caret.Offset,CaretOffsetInsertionText);
                }
            }
        }



        public ImageSource Image
        {
            get { return null; }
        }

        public string Text
        {
            get
            {
                if (TextSubStringStart == 0) return _text;

                return _text.Substring(TextSubStringStart);
            }
        }

        public object Content
        {
            get
            {
                var x = new TextBlock
                {
                    Text = _label,
                    TextAlignment = TextAlignment.Left,
                    TextTrimming = TextTrimming.CharacterEllipsis,
                    Foreground = ColorBrush,
                    FontWeight = FontWeights.Bold
                };
                return x;
            }
        }

        public object Description
        {
            get { return _description; }
        }

        public double Priority
        {
            get { return _priority; }
        }
    }
}