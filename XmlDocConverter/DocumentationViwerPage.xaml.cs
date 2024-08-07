using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using XmlDocConverterLibary.Models;
using XmlDocConverterLibary.Utilities.DocumentationParser;

namespace XmlDocConverter
{
    /// <summary>
    /// Interaction logic for DocumentationViwerPage.xaml
    /// </summary>
    public partial class DocumentationViwerPage : Page
    {
        public DocumentationViwerPage()
        {
            InitializeComponent();
        }

        public DocumentationViwerPage(string filePath)
        {
            InitializeComponent();
            LoadDocumentation(filePath);
        }

        private void LoadDocumentation(string selectedFilePath)
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                XDocument xmlDoc = XDocument.Load(selectedFilePath);
                var classDocs = XmlParser.ParseDocumentation(xmlDoc);
                DisplayDocumentation(classDocs);
            }
            else
            {
                MessageBox.Show("Please select an XML file first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                    Margin = new Thickness(0, 10, 0, 5),
                    IsHitTestVisible = false
                };
                ContentPanel.Children.Add(classTitle);

                // Namespace
                if (!string.IsNullOrEmpty(classDoc.Namespace))
                {
                    var classNamespace = new TextBlock
                    {
                        Text = $"Namespace: {classDoc.Namespace}",
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 0, 0, 5),
                        IsHitTestVisible = false
                    };
                    ContentPanel.Children.Add(classNamespace);
                }

                // Summary
                if (!string.IsNullOrEmpty(classDoc.Summary))
                {
                    var classSummary = new TextBlock
                    {
                        Text = $"Summary: {classDoc.Summary}",
                        Margin = new Thickness(0, 0, 0, 5),
                        IsHitTestVisible = false
                    };
                    ContentPanel.Children.Add(classSummary);
                }

                // Remarks
                if (!string.IsNullOrEmpty(classDoc.Remarks))
                {
                    var classRemarks = new TextBlock
                    {
                        Text = $"Remarks: {classDoc.Remarks}",
                        Margin = new Thickness(0, 0, 0, 5),
                        IsHitTestVisible = false
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
                        Margin = new Thickness(0, 10, 0, 5),
                        IsHitTestVisible = false
                    };
                    ContentPanel.Children.Add(membersTitle);

                    foreach (var member in classDoc.Members)
                    {
                        var memberTitle = new TextBlock
                        {
                            Text = member.MemberName,
                            FontSize = 14,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 5, 0, 0),
                            IsHitTestVisible = false
                        };
                        ContentPanel.Children.Add(memberTitle);

                        if (!string.IsNullOrEmpty(member.Summary))
                        {
                            var memberSummary = new TextBlock
                            {
                                Text = $"Summary: {member.Summary}",
                                Margin = new Thickness(10, 0, 0, 5),
                                IsHitTestVisible = false
                            };
                            ContentPanel.Children.Add(memberSummary);
                        }

                        if (!string.IsNullOrEmpty(member.Remarks))
                        {
                            var memberRemarks = new TextBlock
                            {
                                Text = $"Remarks: {member.Remarks}",
                                Margin = new Thickness(10, 0, 0, 5),
                                IsHitTestVisible = false
                            };
                            ContentPanel.Children.Add(memberRemarks);
                        }

                        if (member.Parameters.Count > 0)
                        {
                            var parametersTitle = new TextBlock
                            {
                                Text = "Parameters:",
                                Margin = new Thickness(10, 0, 0, 5),
                                IsHitTestVisible = false
                            };
                            ContentPanel.Children.Add(parametersTitle);

                            var parametersList = new ListView
                            {
                                Margin = new Thickness(20, 0, 0, 5),
                                IsHitTestVisible = false
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
                                Margin = new Thickness(10, 0, 0, 5),
                                IsHitTestVisible = false
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
                                Margin = new Thickness(10, 0, 0, 5),
                                IsHitTestVisible = false
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
                                Margin = new Thickness(10, 0, 0, 5),
                                IsHitTestVisible = false
                            };
                            ContentPanel.Children.Add(returnsText);
                        }

                        if (member.SeeAlso.Count > 0)
                        {
                            var seeAlsoTitle = new TextBlock
                            {
                                Text = "See Also:",
                                Margin = new Thickness(10, 0, 0, 5),
                                IsHitTestVisible = false
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
                                Margin = new Thickness(10, 0, 0, 5),
                                IsHitTestVisible = false
                            };
                            ContentPanel.Children.Add(examplesTitle);

                            foreach (var example in member.Examples)
                            {
                                var exampleText = new TextBlock
                                {
                                    Text = example,
                                    TextWrapping = TextWrapping.Wrap,
                                    Margin = new Thickness(20, 0, 0, 5),
                                    IsHitTestVisible = false
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
