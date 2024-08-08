﻿using ConsoleApp2.Constants;
using ConsoleApp2.Exceptions;

namespace ConsoleApp2
{
    internal sealed class XMLValidator
    {
        public ValidationResult IsValid(string xml)
        {
            int index = 0;

            ValidationResult result = IsValidElement(xml, ref index);

            return result;
        }

        private ValidationResult IsValidElement(string xml, ref int index)
        {
            SkipWhitespace(xml, ref index);

            if (xml[index] != XMLSymbolsConst.XmlOpeningTag)
                return new ValidationResult(false, ValidationMessageConst.OpeningTagMissing);

            index++;

            if(!TryParseTagName(xml, ref index, out string tagName))
            {
                return new ValidationResult(false, ValidationMessageConst.InvalidTag);
            }

            if (!IsValidAttributes(xml, ref index))
            {
                return new ValidationResult(false, ValidationMessageConst.InvalidTag);
            }

            if (IsTagClosed(xml[index]))
            {
                index += 2; //Skip />
                return new ValidationResult(true, ValidationMessageConst.Success);
            }

            index++;

            ValidationResult innerContextValidationResult = IsValidInnerContext(xml, ref index);
            if(!innerContextValidationResult.Result)
            {
                return innerContextValidationResult;
            }

            index += 2;
            SkipWhitespace(xml, ref index);

            if (!TryParseTagName(xml, ref index, out string closingTagName))
            {
                return new ValidationResult(false, ValidationMessageConst.InvalidTag);
            }


            if (closingTagName != tagName)
                return new ValidationResult(false, ValidationMessageConst.MismatchTagNames);

            index++;

            return new ValidationResult(true, ValidationMessageConst.MismatchTagNames);
        }

        private void SkipWhitespace(string xml, ref int index)
        {
            while (index < xml.Length && char.IsWhiteSpace(xml[index]))
                index++;
        }

        private bool TryParseTagName(string xml, ref int index, out string tagName)
        {
            tagName = string.Empty;

            int start = index;

            if (index >= xml.Length || !IsXMLSymbolValid(xml[index]))
                return false;

            while (index < xml.Length && IsXMLSymbolValid(xml[index]))
            {
                index++;
            }

            tagName = xml.Substring(start, index - start);

            return true;
        }


        private bool IsValidTagName(string xml, ref int index)
        {
            int start = index;

            if (!IsXMLSymbolValid(xml[index]))
                return false;

            while (index < xml.Length && IsXMLSymbolValid(xml[index]))
            {
                index++;
            }
            return true;
        }

        private bool IsXMLSymbolValid(char symbol) =>
           (char.IsLetterOrDigit(symbol) || symbol == XMLSymbolsConst.Colon || symbol == XMLSymbolsConst.Underscore);

        private bool IsValidAttribute(string xml, ref int index)
        {
            if (!IsValidTagName(xml, ref index))
            {
                return false;
            }
            SkipWhitespace(xml, ref index);

            if (xml[index] != XMLSymbolsConst.AttributeEqualSign)
                return false;

            index++;
            SkipWhitespace(xml, ref index);

            if (xml[index] != XMLSymbolsConst.AttributeValueDelimiterSign)
                return false;

            index++;
            int start = index;

            while (index < xml.Length && xml[index] != '"')
                index++;

            index++;

            return true;
        }

        private ValidationResult IsValidTextContent(string xml, ref int index)
        {
            int start = index;

            while (index < xml.Length && xml[index] != XMLSymbolsConst.XmlOpeningTag)
                index++;

            return new ValidationResult(true, ValidationMessageConst.Success);
        }

        private ValidationResult IsValidInnerContext(string xml, ref int index)
        {
            while (xml[index] != XMLSymbolsConst.XmlOpeningTag || xml[index + 1] != XMLSymbolsConst.XmlSelfClosingSlash)
            {
                if (xml[index] == XMLSymbolsConst.XmlOpeningTag)
                {
                    ValidationResult validationResult = IsValidChildElement(xml, ref index);
                    if (!validationResult.Result)
                        return validationResult;
                }
                else
                {
                    ValidationResult validationResult = IsValidInnerContent(xml, ref index);
                    if (!validationResult.Result)
                        return validationResult;
                }
            }

            return new ValidationResult(true, ValidationMessageConst.Success);
        }

        private ValidationResult IsValidInnerContent(string xml, ref int index) =>
            IsValidTextContent(xml, ref index);

        private ValidationResult IsValidChildElement(string xml, ref int index)
        {
            return IsValidElement(xml, ref index);
        }

        private bool IsValidAttributes(string xml, ref int index)
        {
            SkipWhitespace(xml, ref index);
            if (IsEndOfTag(xml[index]) && char.IsWhiteSpace(xml[index - 1])) // If current character is a closing character and the previous character is a whitespace, the tag is invalid
            {
                return false;
            }

            while (!IsEndOfTag(xml[index]))
            {
                SkipWhitespace(xml, ref index);
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
            int lineStart = xml.LastIndexOf('\n', index - 1);
            if (lineStart == -1)
                lineStart = 0;
            else
                lineStart++;

            int lineEnd = xml.IndexOf('\n', index - 1);

            if (lineEnd == -1)
                lineEnd = xml.Length;

            return xml.Substring(lineStart, lineEnd - lineStart);
        }


        private bool IsEndOfTag(char symbol) =>
            symbol == XMLSymbolsConst.XmlTagCloseBracket || symbol == XMLSymbolsConst.XmlSelfClosingSlash ? true : false;

        private bool IsTagClosed(char symbol) =>
            symbol == XMLSymbolsConst.XmlSelfClosingSlash ? true : false;
    }
}
