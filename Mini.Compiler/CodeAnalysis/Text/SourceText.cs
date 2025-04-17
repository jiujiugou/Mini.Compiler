using System;
using System.Collections.Immutable;

namespace Mini.Compiler.CodeAnalysis.Text
{
    public class SourceText
    {
        public readonly string Text;
        public ImmutableArray<TextLine> Lines { get; }
        public char this[int index] => Text[index];
        public int Length => Text.Length;

        public SourceText(string text)
        {
            Text = text ?? string.Empty;
            Lines = ParseLines(this, Text);
        }

        /// <summary>
        /// 根据字符位置查找对应的行索引，使用二分查找。
        /// </summary>
        public int GetLineIndex(int position)
        {
            if (position < 0 || position > Text.Length)
                throw new ArgumentOutOfRangeException(nameof(position));

            int lower = 0;
            int upper = Lines.Length - 1;

            while (lower <= upper)
            {
                int index = lower + (upper - lower) / 2;
                int start = Lines[index].Start;

                if (start == position)
                    return index;
                if (start < position)
                    lower = index + 1;
                else
                    upper = index - 1;
            }

            // 如果 position 在文本末尾，或未精确匹配，则返回最近的上一行
            return Math.Max(0, lower - 1);
        }

        private static ImmutableArray<TextLine> ParseLines(SourceText sourceText, string text)
        {
            var result = ImmutableArray.CreateBuilder<TextLine>();
            int lineStart = 0;
            int position = 0;
            int length = text.Length;

            while (position < length)
            {
                int lineBreakWidth = GetLineBreakWidth(text, position);
                if (lineBreakWidth == 0)
                {
                    position++;
                }
                else
                {
                    AddLine(result, sourceText, lineStart, position, lineBreakWidth);
                    position += lineBreakWidth;
                    lineStart = position;
                }
            }

            // 最后一行（若文档不以换行结尾）
            if (position > lineStart)
            {
                AddLine(result, sourceText, lineStart, position, 0);
            }

            return result.ToImmutable();
        }

        private static void AddLine(ImmutableArray<TextLine>.Builder result, SourceText sourceText, int lineStart, int position, int lineBreakWidth)
        {
            int lineLength = position - lineStart;
            int lineLengthIncludingBreak = lineLength + lineBreakWidth;
            var line = new TextLine(sourceText, lineStart, lineLength, lineLengthIncludingBreak);
            result.Add(line);
        }

        private static int GetLineBreakWidth(string text, int index)
        {
            char c = text[index];
            char next = (index + 1 < text.Length) ? text[index + 1] : '\0';

            if (c == '\r' && next == '\n')
                return 2;
            if (c == '\r' || c == '\n')
                return 1;

            return 0;
        }

        public static SourceText From(string text)
            => new(text);

        public override string ToString() => Text;

        public string ToString(int start, int length)
            => Text.Substring(start, length);

        public string ToString(TextSpan span)
            => Text.Substring(span.Start, span.Length);
    }
}
