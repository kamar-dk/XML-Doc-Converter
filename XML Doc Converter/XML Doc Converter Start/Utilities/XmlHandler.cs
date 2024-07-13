using System.Xml.Linq;

namespace XML_Doc_Converter_Start.Utilities
{
    public static class XmlHandler
    {
        public static List<ClassDocumentation> ParseDocumentation(XDocument xmlDoc)
        {
            var classDocs = new List<ClassDocumentation>();

            var members = xmlDoc.Descendants("member");
            var classes = members.Where(m => m.Attribute("name").Value.StartsWith("T:"));

            foreach (var classElement in classes)
            {
                var fullClassName = classElement.Attribute("name").Value.Substring(2);
                var classDoc = new ClassDocumentation
                {
                    ClassName = fullClassName.Split('.').Last(),
                    Namespace = string.Join(".", fullClassName.Split('.').SkipLast(1)),
                    Summary = classElement.Element("summary")?.Value.Trim(),
                    Remarks = classElement.Element("remarks")?.Value.Trim()
                };

                var memberElements = members.Where(m => m.Attribute("name").Value.StartsWith($"M:{fullClassName}."));

                foreach (var memberElement in memberElements)
                {
                    var memberDoc = new MemberDocumentation
                    {
                        MemberName = memberElement.Attribute("name").Value,
                        Summary = memberElement.Element("summary")?.Value.Trim(),
                        Remarks = memberElement.Element("remarks")?.Value.Trim()
                    };

                    var paramsElements = memberElement.Elements("param");
                    foreach (var paramElement in paramsElements)
                    {
                        memberDoc.Parameters[paramElement.Attribute("name").Value] = paramElement.Value.Trim();
                    }

                    var seeAlsoElements = memberElement.Elements("seealso");
                    foreach (var seeAlsoElement in seeAlsoElements)
                    {
                        memberDoc.SeeAlso.Add(seeAlsoElement.Attribute("cref").Value);
                    }

                    var exampleElements = memberElement.Elements("example");
                    foreach (var exampleElement in exampleElements)
                    {
                        memberDoc.Examples.Add(exampleElement.Value.Trim());
                    }

                    classDoc.Members.Add(memberDoc);
                }

                classDocs.Add(classDoc);
            }

            return classDocs;
        }
    }
}
