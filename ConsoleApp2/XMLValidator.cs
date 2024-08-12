using ConsoleApp2.Constants;
using ConsoleApp2.Exceptions;

namespace ConsoleApp2
{
    internal sealed class XMLValidator
    {
        private Dictionary<Tuple<uint, uint>, List<string>> _errors = new Dictionary<Tuple<uint, uint>, List<string>>();

        public Dictionary<Tuple<uint, uint>, List<string>> Errors => _errors;


        public ValidationResult IsValid(string xml)
        {
            int index = 0;

            while (index < xml.Length)
            {
                ValidationResult result = IsValidElement(xml, ref index);
                if(result.Result == ValidationResultType.CriticalFailure)
                {
                    return result;
                }
            }

            return new ValidationResult(ValidationResultType.Success, ValidationMessageConst.Success);
        }

        private ValidationResult IsValidElement(string xml, ref int index)
        {
            XMLNodeTracker nodeTracker = new XMLNodeTracker();
            int startIndex = index;
            List<string> errors = new List<string>();

            SkipSymbol(xml, ref index, XMLSymbols.Whitespace);

            if (xml[index] != (char)XMLSymbols.XmlTagOnpeningBracket)
            {
                return new ValidationResult(ValidationResultType.CriticalFailure, ValidationMessageConst.OpeningTagMissing + GetFullLine(xml, index));
            }

            nodeTracker.PushStart((uint)index);

            SkipSymbol(xml, ref index, XMLSymbols.XmlTagOnpeningBracket);

            if (!TryParseTagName(xml, ref index, out string tagName))
            {
                errors.Add(ValidationMessageConst.InvalidTag);
                nodeTracker.IsValid = false;
            }

            if (!IsValidAttributes(xml, ref index))
            {
                errors.Add(ValidationMessageConst.InvalidTag);
                nodeTracker.IsValid = false;
            }

            if (IsTagClosed(xml[index]))
            {
                SkipSymbols(xml, ref index, new List<XMLSymbols> { XMLSymbols.XmlTagCloseBracket, XMLSymbols.XmlSelfClosingSlash });

                nodeTracker.PopEnd((uint)index);

                return new ValidationResult(ValidationResultType.Success, ValidationMessageConst.Success);
            }

            SkipSymbol(xml, ref index, XMLSymbols.XmlTagCloseBracket);

            ValidationResult innerContextValidationResult = IsValidInnerContext(xml, ref index);
            if (innerContextValidationResult.Result == ValidationResultType.CriticalFailure)
            {
                return innerContextValidationResult;
            }

            SkipSymbols(xml, ref index, new List<XMLSymbols> { XMLSymbols.XmlTagOnpeningBracket, XMLSymbols.XmlSelfClosingSlash });

            if (!TryParseClosingTagName(xml, ref index, out string closingTagName))
            {
                errors.Add(ValidationMessageConst.InvalidTag);
                nodeTracker.IsValid = false;
            }

            if (closingTagName != tagName)
            {
                return new ValidationResult(ValidationResultType.CriticalFailure, ValidationMessageConst.MismatchTagNames + GetFullLine(xml, index));
            }

            SkipSymbol(xml, ref index, XMLSymbols.XmlTagCloseBracket);
            SkipSymbols(xml, ref index, new List<XMLSymbols> { XMLSymbols.NextLineSymbol, XMLSymbols.CarriageReturn });

            int endIndex = index;
            nodeTracker.PopEnd((uint)endIndex);

            string testInfo = xml.Substring((int)nodeTracker.Start, (int)nodeTracker.End - (int)nodeTracker.Start);
            //Console.WriteLine(testInfo);

            if (nodeTracker.IsValid)
            {
                return new ValidationResult(ValidationResultType.Success, ValidationMessageConst.Success);
            }
            else
            {
                _errors.Add(new Tuple<uint, uint>(nodeTracker.Start, nodeTracker.End), errors);
                return new ValidationResult(ValidationResultType.Failure, errors.First());
            }
        }

        private void SkipSymbol(string xml, ref int index, XMLSymbols symbol)
        {
            if (xml[index] == (char)symbol)
            {
                index++;
            }
        }

        private void SkipSymbols(string xml, ref int index, List<XMLSymbols> symbols)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols.Contains((XMLSymbols)xml[index]))
                {
                    index++;
                }
            }
        }

        private bool TryParseTagName(string xml, ref int index, out string tagName)
        {
            tagName = string.Empty;

            int start = index;

            if (index >= xml.Length || !IsXMLTagSymbolValid(xml[index]))
            {
                return false;
            }

            while (index < xml.Length && IsXMLTagSymbolValid(xml[index]))
            {
                index++;
            }

            tagName = xml.Substring(start, index - start);

            return true;
        }

        private bool TryParseClosingTagName(string xml, ref int index, out string closingTagName)
        {
            closingTagName = string.Empty;

            int start = index;

            if (index >= xml.Length || !IsXMLTagSymbolValid(xml[index]))
            {
                return false;
            }

            while (index < xml.Length && IsXMLTagSymbolValid(xml[index]))
            {
                index++;
            }

            if (xml[index] != (char)XMLSymbols.XmlTagCloseBracket)
            {
                return false;
            }

            closingTagName = xml.Substring(start, index - start);

            return true;
        }

        private bool IsValidTagName(string xml, ref int index)
        {
            int start = index;

            if (!IsXMLTagSymbolValid(xml[index]))
            {
                return false;
            }

            while (index < xml.Length && IsXMLTagSymbolValid(xml[index]))
            {
                index++;
            }
            return true;
        }

        private bool IsXMLTagSymbolValid(char symbol) =>
           (char.IsLetterOrDigit(symbol) || symbol == (char)XMLSymbols.Colon || symbol == (char)XMLSymbols.Underscore);

        private bool IsValidAttribute(string xml, ref int index)
        {
            if (!IsValidTagName(xml, ref index))
            {
                return false;
            }

            SkipSymbol(xml, ref index, XMLSymbols.Whitespace);

            if (xml[index] != (char)XMLSymbols.AttributeEqualSign)
            {
                return false;
            }

            index++;
            SkipSymbol(xml, ref index, XMLSymbols.Whitespace);

            if (xml[index] != (char)XMLSymbols.AttributeValueDelimiterSign)
            {
                return false;
            }

            index++;
            int start = index;

            while (index < xml.Length && xml[index] != (char)XMLSymbols.AttributeValueDelimiterSign)
            {
                index++;
            }

            index++;

            return true;
        }

        private ValidationResult IsValidTextContent(string xml, ref int index)
        {
            int start = index;

            while (index < xml.Length && xml[index] != (char)XMLSymbols.XmlTagOnpeningBracket)
            {
                index++;
            }
            return new ValidationResult(ValidationResultType.Success, ValidationMessageConst.Success);
        }

        private ValidationResult IsValidInnerContext(string xml, ref int index)
        {
            string a = xml.Substring(index - 5, 5);
            while (xml[index] != (char)XMLSymbols.XmlTagOnpeningBracket || xml[index + 1] != (char)XMLSymbols.XmlSelfClosingSlash)
            {
                if (xml[index] == (char)XMLSymbols.XmlTagOnpeningBracket)
                {
                    ValidationResult validationResult = IsValidChildElement(xml, ref index);
                    if (validationResult.Result == ValidationResultType.CriticalFailure)
                    {
                        return validationResult;
                    }
                }
                else
                {
                    ValidationResult validationResult = IsValidInnerContent(xml, ref index);
                    if (validationResult.Result == ValidationResultType.CriticalFailure)
                    {
                        return validationResult;
                    }
                }
            }

            return new ValidationResult(ValidationResultType.Success, ValidationMessageConst.Success);
        }

        private ValidationResult IsValidInnerContent(string xml, ref int index) =>
            IsValidTextContent(xml, ref index);

        private ValidationResult IsValidChildElement(string xml, ref int index)
        {
            return IsValidElement(xml, ref index);
        }

        private bool IsValidAttributes(string xml, ref int index)
        {
            SkipSymbol(xml, ref index, XMLSymbols.Whitespace);
            if (IsEndOfTag(xml[index]) && char.IsWhiteSpace(xml[index - 1])) // If current character is testInfo closing character and the previous character is testInfo whitespace, the tag is invalid
            {
                return false;
            }

            while (!IsEndOfTag(xml[index]))
            {
                SkipSymbol(xml, ref index, XMLSymbols.Whitespace);
                if (!IsEndOfTag(xml[index]))
                {
                    if(!IsValidAttribute(xml, ref index))
                    {
                        return false;
                    }
                }
                else if (char.IsWhiteSpace(xml[index - 1]))
                {
                    return false;
                }
            }
            return true;
        }

        private string GetFullLine(string xml, int index)
        {
            int lineStart = xml.LastIndexOf((char)XMLSymbols.NextLineSymbol, index - 1);
            if (lineStart == -1)
            {
                lineStart = 0;
            }    
            else
            {
                lineStart++;
            }

            int lineEnd = xml.IndexOf((char)XMLSymbols.NextLineSymbol, index - 1);

            if (lineEnd == -1)
            {
                lineEnd = xml.Length;
            }

            string line = string.Empty;
            try
            {
                line = xml.Substring(lineStart, lineEnd - lineStart);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return string.Empty;
            }

            return line;
        }

        private bool IsEndOfTag(char symbol) =>
            symbol == (char)XMLSymbols.XmlTagCloseBracket || symbol == (char)XMLSymbols.XmlSelfClosingSlash;

        private bool IsTagClosed(char symbol) =>
            symbol == (char)XMLSymbols.XmlSelfClosingSlash;
    }
}