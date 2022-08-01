using System.Text;

namespace tacttvm.Service.StyleOfWritingCompoundWords
{
    internal class CamelCase : IStyleOfWritingCompoundWords
    {
        StringBuilder stringBuilder;
        public CamelCase()
        {
            stringBuilder = new StringBuilder();
        }
        public string GetTetx(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            bool previousSymbol = true;
            stringBuilder.Clear();
            stringBuilder.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsSeparator(text[i]))
                {
                    previousSymbol = false;
                }
                else if (!char.IsLetterOrDigit(text[i]))
                {
                    continue;
                }
                else if (!previousSymbol)
                {
                    previousSymbol = true;
                    stringBuilder.Append(char.ToUpper(text[i]));
                }
                else
                {
                    stringBuilder.Append(text[i]);
                }
            }
            return stringBuilder.ToString();
        }
    }
    internal class SnakeCase : IStyleOfWritingCompoundWords
    {
        StringBuilder stringBuilder;
        public SnakeCase()
        {
            stringBuilder = new StringBuilder();
        }
        public string GetTetx(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }

            bool previousSymbol = true;
            stringBuilder.Clear();
            stringBuilder.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsSeparator(text[i]))
                {
                    previousSymbol = false;
                }
                else if (!char.IsLetterOrDigit(text[i]))
                {
                    continue;
                }
                else if (!previousSymbol)
                {
                    previousSymbol = true;
                    stringBuilder.Append('_');
                    stringBuilder.Append(char.ToUpper(text[i]));
                }
                else
                {
                    stringBuilder.Append(text[i]);
                }
            }
            return stringBuilder.ToString();
        }
    }
}
