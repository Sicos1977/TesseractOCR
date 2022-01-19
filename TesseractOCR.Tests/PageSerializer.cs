using System.Globalization;
using System.Text;
using TesseractOCR;
using TesseractOCR.Enums;

namespace Tesseract.Tests
{
    /// <summary>
    ///     Serialise the OCR results to a string using the iterator api
    /// </summary>
    internal class PageSerializer
    {
        public static string Serialize(Page page, bool outputChoices)
        {
            var output = new StringBuilder();
            using (var iterator = page.GetIterator())
            {
                iterator.Begin();
                do
                {
                    do
                    {
                        do
                        {
                            do
                            {
                                do
                                {
                                    if (iterator.IsAtBeginningOf(PageIteratorLevel.Block))
                                    {
                                        var confidence = iterator.GetConfidence(PageIteratorLevel.Block) / 100;
                                        if (iterator.TryGetBoundingBox(PageIteratorLevel.Block, out var bounds))
                                            output.AppendFormat(CultureInfo.InvariantCulture,
                                                "<block confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">",
                                                confidence, bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        else
                                            output.AppendFormat(CultureInfo.InvariantCulture,
                                                "<block confidence=\"{0:P}\">", confidence);
                                        output.AppendLine();
                                    }

                                    if (iterator.IsAtBeginningOf(PageIteratorLevel.Para))
                                    {
                                        var confidence = iterator.GetConfidence(PageIteratorLevel.Para) / 100;
                                        if (iterator.TryGetBoundingBox(PageIteratorLevel.Para, out var bounds))
                                            output.AppendFormat(CultureInfo.InvariantCulture,
                                                "<para confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence,
                                                bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        else
                                            output.AppendFormat(CultureInfo.InvariantCulture,
                                                "<para confidence=\"{0:P}\">", confidence);
                                        output.AppendLine();
                                    }

                                    if (iterator.IsAtBeginningOf(PageIteratorLevel.TextLine))
                                    {
                                        var confidence = iterator.GetConfidence(PageIteratorLevel.TextLine) / 100;
                                        if (iterator.TryGetBoundingBox(PageIteratorLevel.TextLine, out var bounds))
                                            output.AppendFormat(CultureInfo.InvariantCulture,
                                                "<line confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence,
                                                bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        else
                                            output.AppendFormat(CultureInfo.InvariantCulture,
                                                "<line confidence=\"{0:P}\">", confidence);
                                    }

                                    if (iterator.IsAtBeginningOf(PageIteratorLevel.Word))
                                    {
                                        var confidence = iterator.GetConfidence(PageIteratorLevel.Word) / 100;
                                        if (iterator.TryGetBoundingBox(PageIteratorLevel.Word, out var bounds))
                                            output.AppendFormat(CultureInfo.InvariantCulture,
                                                "<word confidence=\"{0:P}\" bounds=\"{1}, {2}, {3}, {4}\">", confidence,
                                                bounds.X1, bounds.Y1, bounds.X2, bounds.Y2);
                                        else
                                            output.AppendFormat(CultureInfo.InvariantCulture,
                                                "<word confidence=\"{0:P}\">", confidence);
                                    }

                                    // symbol and choices
                                    if (outputChoices)
                                        using (var choiceIterator = iterator.GetChoiceIterator())
                                        {
                                            var symbolConfidence = iterator.GetConfidence(PageIteratorLevel.Symbol) / 100;
                                            if (choiceIterator != null)
                                            {
                                                output.AppendFormat(CultureInfo.InvariantCulture,
                                                    "<symbol text=\"{0}\" confidence=\"{1:P}\">",
                                                    iterator.GetText(PageIteratorLevel.Symbol), symbolConfidence);
                                                output.Append("<choices>");
                                                do
                                                {
                                                    var choiceConfidence = choiceIterator.GetConfidence() / 100;
                                                    output.AppendFormat(CultureInfo.InvariantCulture,
                                                        "<choice text=\"{0}\" confidence\"{1:P}\"/>",
                                                        choiceIterator.GetText(), choiceConfidence);
                                                } while (choiceIterator.Next());

                                                output.Append("</choices>");
                                                output.Append("</symbol>");
                                            }
                                            else
                                            {
                                                output.AppendFormat(CultureInfo.InvariantCulture,
                                                    "<symbol text=\"{0}\" confidence=\"{1:P}\"/>",
                                                    iterator.GetText(PageIteratorLevel.Symbol), symbolConfidence);
                                            }
                                        }
                                    else
                                        output.Append(iterator.GetText(PageIteratorLevel.Symbol));

                                    if (iterator.IsAtFinalOf(PageIteratorLevel.Word, PageIteratorLevel.Symbol))
                                        output.Append("</word>");
                                } while (iterator.Next(PageIteratorLevel.Word, PageIteratorLevel.Symbol));

                                if (iterator.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                    output.AppendLine("</line>");
                            } while (iterator.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                            if (iterator.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                                output.AppendLine("</para>");
                        } while (iterator.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                    } while (iterator.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));

                    output.AppendLine("</block>");
                } while (iterator.Next(PageIteratorLevel.Block));
            }

            return TestUtils.NormalizeNewLine(output.ToString());
        }
    }
}