using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml;

namespace TestControlAPI.Services
{
    public class XmlProcessor
    {
        public void ProcessVariable(string targetValue, string variable, string targetFile)
        {
            XDocument doc = XDocument.Load(targetFile);
            var xmlAttributeVariables = doc.Descendants("Variable").ToList();

            foreach (var variableElement in xmlAttributeVariables)
            {
                if (variableElement.Attribute("Name")?.Value == variable)
                {
                    variableElement.Remove();
                }
            }

            doc.Root.Element("Variables").Add(new XElement("Variable",
                    new XAttribute("Name", variable),
                    targetValue));
            doc.Save(targetFile);
        }

        public void ProcessTag(string status, string tag, string targetFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(targetFile);
            XmlNode node = doc.SelectSingleNode("//CatapultTarget");

            if (node != null && node.Attributes != null)
            {
                string conditionTagExpression = node.Attributes["ConditionTagExpression"]?.Value ?? string.Empty;

                List<string> excludeValues = ExtractValues(conditionTagExpression, "exclude");
                List<string> includeValues = ExtractValues(conditionTagExpression, "include");

                switch (status)
                {
                    case "Exclude":
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "exclude", tag, excludeValues, includeValues, addTag: true);
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "include", tag, includeValues, excludeValues, addTag: false);
                        break;
                    case "Include":
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "exclude", tag, excludeValues, includeValues, addTag: false);
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "include", tag, includeValues, excludeValues, addTag: true);
                        break;
                    case "Not Set":
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "exclude", tag, excludeValues, includeValues, addTag: false);
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "include", tag, includeValues, excludeValues, addTag: false);
                        break;
                }

                node.Attributes["ConditionTagExpression"].Value = conditionTagExpression;
                doc.Save(targetFile);
            }
        }

        private List<string> ExtractValues(string conditionTagExpression, string section)
        {
            string pattern = $@"{section}\[(.*?)\]";
            Match match = Regex.Match(conditionTagExpression, pattern);
            if (match.Success)
            {
                string content = match.Groups[1].Value;
                return Regex.Matches(content, "\"(.*?)\"").Cast<Match>().Select(m => m.Groups[1].Value).ToList();
            }
            return new List<string>();
        }

        private string UpdateConditionTagExpression(string conditionTagExpression, string section, string tag, List<string> currentValues, List<string> oppositeValues, bool addTag)
        {
            if (addTag)
            {
                if (!currentValues.Contains(tag))
                {
                    if (conditionTagExpression.Contains($"{section}["))
                    {
                        int startIndex = conditionTagExpression.IndexOf($"{section}[") + $"{section}[".Length;
                        int endIndex = conditionTagExpression.IndexOf("]", startIndex);
                        string existingValues = conditionTagExpression.Substring(startIndex, endIndex - startIndex).Trim();
                        if (!string.IsNullOrEmpty(existingValues))
                        {
                            existingValues += " or ";
                        }
                        existingValues += $"\"{tag}\"";
                        conditionTagExpression = conditionTagExpression.Substring(0, startIndex) + existingValues + conditionTagExpression.Substring(endIndex);
                    }
                }
            }
            else
            {
                if (currentValues.Contains(tag))
                {
                    if (conditionTagExpression.Contains($"{section}["))
                    {
                        int startIndex = conditionTagExpression.IndexOf($"{section}[") + $"{section}[".Length;
                        int endIndex = conditionTagExpression.IndexOf("]", startIndex);
                        string sectionContent = conditionTagExpression.Substring(startIndex, endIndex - startIndex);
                        var items = sectionContent.Split(new[] { " or " }, StringSplitOptions.None).Where(x => x.Trim() != $"\"{tag}\"").ToList();
                        string newSectionContent = string.Join(" or ", items);
                        conditionTagExpression = conditionTagExpression.Substring(0, startIndex) + newSectionContent + conditionTagExpression.Substring(endIndex);
                    }
                }
            }
            return conditionTagExpression;
        }
    }
}
