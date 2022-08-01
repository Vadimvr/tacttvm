namespace tacttvm.Service.StyleOfWritingCompoundWords
{
    public static class StyleOfWritingCompoundWordsSetter
    {
        public static  IStyleOfWritingCompoundWords Get(StyleOfWritingCompoundWordsEnum style)
        {
            switch (style)
            {
                case StyleOfWritingCompoundWordsEnum.SnakeCase:
                    return new SnakeCase();
                default:
                    return new CamelCase();
            }
        }
    }
}
