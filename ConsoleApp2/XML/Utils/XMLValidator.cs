using ConsoleApp2.Constants;
using ConsoleApp2.XMLUtils.Models;
using System.Collections.Generic;
using static ConsoleApp2.XML.Utils.XMLStringHelper;

namespace ConsoleApp2.XMLUtils
{
    internal sealed class XMLValidator
    {
        private Dictionary<NodeRange, List<string>> _errors = new Dictionary<NodeRange, List<string>>();

        public Dictionary<NodeRange, List<string>> Errors => _errors;

        public ValidationResult IsValid(string xml)
        {
            int index = 0;
            while (index < xml.Length)
            {
                ValidationResult result = IsValidElement(xml, ref index);
                if (result.Result == ValidationResultType.CriticalFailure)
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

            SkipWhitespaceAndControlCharacters(xml, ref index);

            string openingTag;
            if (!ValidateOpeningTag(xml, ref index, out openingTag, nodeTracker, errors))
            {
                return new ValidationResult(ValidationResultType.CriticalFailure, ValidationMessageConst.InvalidTag + GetFullLine(xml, index));
            }

            if (IsTagClosed(xml[index]))
            {
                HandleSelfClosingTag(xml, ref index, nodeTracker);
                return new ValidationResult(ValidationResultType.Success, ValidationMessageConst.Success);
            }

            SkipSymbol(xml, ref index, XMLSymbols.XmlTagCloseBracket);

            ValidationResult innerContextValidationResult = IsValidInnerContext(xml, ref index);
            if (innerContextValidationResult.Result == ValidationResultType.CriticalFailure)
            {
                return innerContextValidationResult;
            }

            if (!ValidateClosingTag(xml, ref index, openingTag, nodeTracker, errors))
            {
                return new ValidationResult(ValidationResultType.CriticalFailure, ValidationMessageConst.MismatchTagNames + GetFullLine(xml, index));
            }

            FinalizeNodeTracking(xml, ref index, nodeTracker, errors);

            return nodeTracker.IsValid
                ? new ValidationResult(ValidationResultType.Success, ValidationMessageConst.Success)
                : new ValidationResult(ValidationResultType.Failure, errors.First());
        }

        private bool ValidateOpeningTag(string xml, ref int index, out string tagName, XMLNodeTracker nodeTracker, List<string> errors)
        {
            tagName = string.Empty;
            if (!IsSymbol(xml, index, XMLSymbols.XmlTagOnpeningBracket))
            {
                return false;
            }

            nodeTracker.PushStart((uint)index);
            SkipSymbol(xml, ref index, XMLSymbols.XmlTagOnpeningBracket);

            if (!TryParseTagName(xml, ref index, out tagName))
            {
                return false;
            }

            if (!IsValidAttributes(xml, ref index))
            {
                errors.Add(ValidationMessageConst.InvalidAttribute + GetFullLine(xml, index));
                nodeTracker.IsValid = false;
            }

            return true;
        }

        private void HandleSelfClosingTag(string xml, ref int index, XMLNodeTracker nodeTracker)
        {
            SkipSymbols(xml, ref index, [XMLSymbols.XmlTagCloseBracket, XMLSymbols.XmlSelfClosingSlash]);
            nodeTracker.PopEnd((uint)index);
        }

        private bool ValidateClosingTag(string xml, ref int index, string tagName, XMLNodeTracker nodeTracker, List<string> errors)
        {
            SkipSymbols(xml, ref index, [XMLSymbols.XmlTagOnpeningBracket, XMLSymbols.XmlSelfClosingSlash]);

            if (!TryParseClosingTagName(xml, ref index, out string closingTagName))
            {
                errors.Add(ValidationMessageConst.InvalidTag + GetFullLine(xml, index));
                nodeTracker.IsValid = false;
                return false;
            }

            if (closingTagName != tagName)
            {
                return false;
            }

            SkipSymbol(xml, ref index, XMLSymbols.XmlTagCloseBracket);
            SkipSymbols(xml, ref index, [XMLSymbols.NextLineSymbol, XMLSymbols.CarriageReturn]);

            return true;
        }

        private void FinalizeNodeTracking(string xml, ref int index, XMLNodeTracker nodeTracker, List<string> errors)
        {
            int endIndex = index;
            nodeTracker.PopEnd((uint)endIndex);

            if (!nodeTracker.IsValid)
            {
                AddOrReplaceNode(new NodeRange(nodeTracker.Start, nodeTracker.End), errors);
            }
        }

        private bool IsNodeContained(NodeRange nodeRange)
        {
            return _errors.Keys.Any(NodeRange => NodeRange.Start <= nodeRange.Start && NodeRange.End >= nodeRange.End);
        }

        private void RemoveContainedNodes(NodeRange nodeRange, List<string> errors)
        {
            List<KeyValuePair<NodeRange, List<string>>> containedNodes = _errors
                .Where(pair => pair.Key.Start >= nodeRange.Start && pair.Key.End <= nodeRange.End)
                .ToList();

            foreach (KeyValuePair<NodeRange, List<string>> pair in containedNodes)
            {
                errors.AddRange(pair.Value);
                _errors.Remove(pair.Key);
            }
        }

        private void AddOrReplaceNode(NodeRange nodeRange, List<string> errors)
        {
            RemoveContainedNodes(nodeRange, errors);

            if (!IsNodeContained(nodeRange))
            {
                _errors.Add(nodeRange, errors);
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

            if (!IsSymbol(xml, index, XMLSymbols.XmlTagCloseBracket))
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

        private bool IsValidAttribute(string xml, ref int index)
        {
            if (!IsValidTagName(xml, ref index))
            {
                return false;
            }

            SkipWhiteSpaces(xml, ref index);

            if (!IsSymbol(xml, index, XMLSymbols.AttributeEqualSign))
            {
                return false;
            }

            index++;
            SkipWhiteSpaces(xml, ref index);

            if (!IsSymbol(xml, index, XMLSymbols.AttributeValueDelimiterSign))
            {
                return false;
            }

            index++;
            int start = index;

            while (index < xml.Length && !IsSymbol(xml, index, XMLSymbols.AttributeValueDelimiterSign))
            {
                index++;
            }

            index++;

            return true;
        }

        private static ValidationResult IsValidTextContent(string xml, ref int index)
        {
            int start = index;

            while (index < xml.Length && !IsSymbol(xml, index, XMLSymbols.XmlTagOnpeningBracket))
            {
                index++;
            }
            return new ValidationResult(ValidationResultType.Success, ValidationMessageConst.Success);
        }

        private ValidationResult IsValidInnerContext(string xml, ref int index)
        {
            while (!IsSymbol(xml, index, XMLSymbols.XmlTagOnpeningBracket) || !IsSymbol(xml, index + 1, XMLSymbols.XmlSelfClosingSlash))
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
            SkipWhiteSpaces(xml, ref index);
            if (IsEndOfTag(xml[index]) && char.IsWhiteSpace(xml[index - 1]))
            {
                return false;
            }

            while (!IsEndOfTag(xml[index]))
            {
                SkipWhiteSpaces(xml, ref index);
                if (!IsEndOfTag(xml[index]))
                {
                    if (!IsValidAttribute(xml, ref index))
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
    }
}