using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml;

namespace TestControlAPI.Services
{
    public class XmlProcessor
    {
        // This method processes and updates a variable within an XML file
        public void ProcessVariable(string targetValue, string variable, string targetFile)
        {
            // Load the XML document from the specified file
            XDocument doc = XDocument.Load(targetFile);

            // Find all the elements named "Variable"
            var xmlAttributeVariables = doc.Descendants("Variable").ToList();

            // Loop through each variable element
            foreach (var variableElement in xmlAttributeVariables)
            {
                // Check if the attribute "Name" matches the given variable
                if (variableElement.Attribute("Name")?.Value == variable)
                {
                    // If it matches, remove the element
                    variableElement.Remove();
                }
            }

            // Add a new "Variable" element with the specified name and value to the "Variables" parent element
            doc.Root.Element("Variables").Add(new XElement("Variable",
                    new XAttribute("Name", variable),
                    targetValue));

            // Save the updated XML document back to the file
            doc.Save(targetFile);
        }

        // This method processes and updates a tag within an XML file
        public void ProcessTag(string status, string tag, string targetFile)
        {
            // Load the XML document from the specified file
            XmlDocument doc = new XmlDocument();
            doc.Load(targetFile);

            // Select the first "CatapultTarget" node
            XmlNode node = doc.SelectSingleNode("//CatapultTarget");

            // Check if the node and its attributes exist
            if (node != null && node.Attributes != null)
            {
                // Get the "ConditionTagExpression" attribute value or set it to an empty string if it does not exist
                string conditionTagExpression = node.Attributes["ConditionTagExpression"]?.Value ?? string.Empty;

                // Extract the values within the "exclude" and "include" sections
                List<string> excludeValues = ExtractValues(conditionTagExpression, "exclude");
                List<string> includeValues = ExtractValues(conditionTagExpression, "include");

                // Update the "ConditionTagExpression" based on the status and tag
                switch (status)
                {
                    case "Exclude":
                        // Add tag to exclude section and remove from include section
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "exclude", tag, excludeValues, includeValues, addTag: true);
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "include", tag, includeValues, excludeValues, addTag: false);
                        break;
                    case "Include":
                        // Add tag to include section and remove from exclude section
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "exclude", tag, excludeValues, includeValues, addTag: false);
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "include", tag, includeValues, excludeValues, addTag: true);
                        break;
                    case "Not Set":
                        // Remove tag from both exclude and include sections
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "exclude", tag, excludeValues, includeValues, addTag: false);
                        conditionTagExpression = UpdateConditionTagExpression(conditionTagExpression, "include", tag, includeValues, excludeValues, addTag: false);
                        break;
                }

                // Update the "ConditionTagExpression" attribute with the new value
                node.Attributes["ConditionTagExpression"].Value = conditionTagExpression;

                // Save the updated XML document back to the file
                doc.Save(targetFile);
            }
        }

        // This helper method extracts values from a section (either "exclude" or "include") in the condition tag expression
        private List<string> ExtractValues(string conditionTagExpression, string section)
        {
            // Create a regex pattern to find the specified section and extract its content
            string pattern = $@"{section}\[(.*?)\]";

            // Match the pattern within the condition tag expression
            Match match = Regex.Match(conditionTagExpression, pattern);

            // If a match is found, extract the values within quotes
            if (match.Success)
            {
                string content = match.Groups[1].Value;
                return Regex.Matches(content, "\"(.*?)\"").Cast<Match>().Select(m => m.Groups[1].Value).ToList();
            }

            // Return an empty list if no match is found
            return new List<string>();
        }

        // This helper method updates the condition tag expression by adding or removing a tag from a specified section
        private string UpdateConditionTagExpression(string conditionTagExpression, string section, string tag, List<string> currentValues, List<string> oppositeValues, bool addTag)
        {
            // If the tag should be added
            if (addTag)
            {
                // Only add if the tag is not already present in the current values
                if (!currentValues.Contains(tag))
                {
                    // Check if the section already exists in the expression
                    if (conditionTagExpression.Contains($"{section}["))
                    {
                        // Find the start and end indices of the section content
                        int startIndex = conditionTagExpression.IndexOf($"{section}[") + $"{section}[".Length;
                        int endIndex = conditionTagExpression.IndexOf("]", startIndex);

                        // Extract the existing values within the section
                        string existingValues = conditionTagExpression.Substring(startIndex, endIndex - startIndex).Trim();

                        // Append the new tag, handling the "or" logic
                        if (!string.IsNullOrEmpty(existingValues))
                        {
                            existingValues += " or ";
                        }
                        existingValues += $"\"{tag}\"";

                        // Replace the old section content with the new content
                        conditionTagExpression = conditionTagExpression.Substring(0, startIndex) + existingValues + conditionTagExpression.Substring(endIndex);
                    }
                }
            }
            else
            {
                // If the tag should be removed
                if (currentValues.Contains(tag))
                {
                    // Check if the section already exists in the expression
                    if (conditionTagExpression.Contains($"{section}["))
                    {
                        // Find the start and end indices of the section content
                        int startIndex = conditionTagExpression.IndexOf($"{section}[") + $"{section}[".Length;
                        int endIndex = conditionTagExpression.IndexOf("]", startIndex);

                        // Extract the section content
                        string sectionContent = conditionTagExpression.Substring(startIndex, endIndex - startIndex);

                        // Remove the specified tag and handle the "or" logic
                        var items = sectionContent.Split(new[] { " or " }, StringSplitOptions.None).Where(x => x.Trim() != $"\"{tag}\"").ToList();
                        string newSectionContent = string.Join(" or ", items);

                        // Replace the old section content with the new content
                        conditionTagExpression = conditionTagExpression.Substring(0, startIndex) + newSectionContent + conditionTagExpression.Substring(endIndex);
                    }
                }
            }

            // Return the updated condition tag expression
            return conditionTagExpression;
        }
    }
}
