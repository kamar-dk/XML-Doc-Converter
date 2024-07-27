using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using XmlDocConverter.Models;

namespace XmlDocConverter
{
    public partial class DocumentationViewerPage : Page
    {
        public DocumentationViewerPage(List<ClassDocumentation> classDocs)
        {
            InitializeComponent();
            DisplayDocumentation(classDocs);
        }

        private void DisplayDocumentation(List<ClassDocumentation> classDocs)
        {
            foreach (var classDoc in classDocs)
            {
                // Class Title
                var classTitle = new TextBlock
                {
                    Text = classDoc.ClassName,
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 5)
                };
                ContentPanel.Children.Add(classTitle);

                // Namespace
                if (!string.IsNullOrEmpty(classDoc.Namespace))
                {
                    var classNamespace = new TextBlock
                    {
                        Text = $"Namespace: {classDoc.Namespace}",
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    ContentPanel.Children.Add(classNamespace);
                }

                // Summary
                if (!string.IsNullOrEmpty(classDoc.Summary))
                {
                    var classSummary = new TextBlock
                    {
                        Text = $"Summary: {classDoc.Summary}",
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    ContentPanel.Children.Add(classSummary);
                }

                // Remarks
                if (!string.IsNullOrEmpty(classDoc.Remarks))
                {
                    var classRemarks = new TextBlock
                    {
                        Text = $"Remarks: {classDoc.Remarks}",
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    ContentPanel.Children.Add(classRemarks);
                }

                // Members
                if (classDoc.Members.Count > 0)
                {
                    var membersTitle = new TextBlock
                    {
                        Text = "Members",
                        FontSize = 16,
                        FontWeight = FontWeights.Bold,
                        Margin = new Thickness(0, 10, 0, 5)
                    };
                    ContentPanel.Children.Add(membersTitle);

                    foreach (var member in classDoc.Members)
                    {
                        var memberTitle = new TextBlock
                        {
                            Text = member.MemberName,
                            FontSize = 14,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 5, 0, 0)
                        };
                        ContentPanel.Children.Add(memberTitle);

                        if (!string.IsNullOrEmpty(member.Summary))
                        {
                            var memberSummary = new TextBlock
                            {
                                Text = $"Summary: {member.Summary}",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            ContentPanel.Children.Add(memberSummary);
                        }

                        if (!string.IsNullOrEmpty(member.Remarks))
                        {
                            var memberRemarks = new TextBlock
                            {
                                Text = $"Remarks: {member.Remarks}",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            ContentPanel.Children.Add(memberRemarks);
                        }

                        if (member.Parameters.Count > 0)
                        {
                            var parametersTitle = new TextBlock
                            {
                                Text = "Parameters:",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            ContentPanel.Children.Add(parametersTitle);

                            var parametersList = new ListView
                            {
                                Margin = new Thickness(20, 0, 0, 5)
                            };
                            foreach (var param in member.Parameters)
                            {
                                parametersList.Items.Add($"{param.Key}: {param.Value}");
                            }
                            ContentPanel.Children.Add(parametersList);
                        }

                        if (member.TypeParameters.Count > 0)
                        {
                            var typeParametersTitle = new TextBlock
                            {
                                Text = "Type Parameters:",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            ContentPanel.Children.Add(typeParametersTitle);

                            var typeParametersList = new ListView
                            {
                                Margin = new Thickness(20, 0, 0, 5)
                            };
                            foreach (var typeParam in member.TypeParameters)
                            {
                                typeParametersList.Items.Add($"{typeParam.Key}: {typeParam.Value}");
                            }
                            ContentPanel.Children.Add(typeParametersList);
                        }

                        if (member.Exceptions.Count > 0)
                        {
                            var exceptionsTitle = new TextBlock
                            {
                                Text = "Exceptions:",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            ContentPanel.Children.Add(exceptionsTitle);

                            var exceptionsList = new ListView
                            {
                                Margin = new Thickness(20, 0, 0, 5)
                            };
                            foreach (var exception in member.Exceptions)
                            {
                                exceptionsList.Items.Add($"{exception.Key}: {exception.Value}");
                            }
                            ContentPanel.Children.Add(exceptionsList);
                        }

                        if (!string.IsNullOrEmpty(member.Returns))
                        {
                            var returnsText = new TextBlock
                            {
                                Text = $"Returns: {member.Returns}",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            ContentPanel.Children.Add(returnsText);
                        }

                        if (member.SeeAlso.Count > 0)
                        {
                            var seeAlsoTitle = new TextBlock
                            {
                                Text = "See Also:",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            ContentPanel.Children.Add(seeAlsoTitle);

                            var seeAlsoList = new ListView
                            {
                                Margin = new Thickness(20, 0, 0, 5)
                            };
                            foreach (var seeAlso in member.SeeAlso)
                            {
                                seeAlsoList.Items.Add(seeAlso);
                            }
                            ContentPanel.Children.Add(seeAlsoList);
                        }

                        if (member.Examples.Count > 0)
                        {
                            var examplesTitle = new TextBlock
                            {
                                Text = "Examples:",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            ContentPanel.Children.Add(examplesTitle);

                            foreach (var example in member.Examples)
                            {
                                var exampleText = new TextBlock
                                {
                                    Text = example,
                                    TextWrapping = TextWrapping.Wrap,
                                    Margin = new Thickness(20, 0, 0, 5)
                                };
                                ContentPanel.Children.Add(exampleText);
                            }
                        }

                        ContentPanel.Children.Add(new Separator());
                    }
                }

                ContentPanel.Children.Add(new Separator());
            }
        }
    }
}
