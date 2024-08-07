using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Xml.Linq;
using XmlDocConverterLibary.Models;
using XmlDocConverterLibary.Utilities.DocumentationParser;

namespace XmlDocConverter
{
    public partial class DocumentationViwerPage : Page
    {
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
                PopulateTableOfContents(classDocs);
                DisplayDocumentation(classDocs);
            }
            else
            {
                MessageBox.Show("Please select an XML file first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateTableOfContents(List<ClassDocumentation> classDocs)
        {
            var namespaces = new Dictionary<string, List<ClassDocumentation>>();
            foreach (var classDoc in classDocs)
            {
                if (!namespaces.ContainsKey(classDoc.Namespace))
                {
                    namespaces[classDoc.Namespace] = new List<ClassDocumentation>();
                }
                namespaces[classDoc.Namespace].Add(classDoc);
            }

            foreach (var ns in namespaces.Keys)
            {
                var nsNode = new TreeViewItem { Header = ns };
                foreach (var classDoc in namespaces[ns])
                {
                    var classLink = new TextBlock();
                    var hyperlink = new Hyperlink(new Run(classDoc.ClassName))
                    {
                        NavigateUri = new Uri(classDoc.ClassName, UriKind.Relative)
                    };
                    hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
                    classLink.Inlines.Add(hyperlink);

                    var classNode = new TreeViewItem { Header = classLink };
                    nsNode.Items.Add(classNode);
                }
                tocTreeView.Items.Add(nsNode);
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
                    Margin = new Thickness(0, 10, 0, 5)
                };
                contentPanel.Children.Add(classTitle);
                RegisterName(GenerateValidName(classDoc.ClassName), classTitle);

                // Namespace
                if (!string.IsNullOrEmpty(classDoc.Namespace))
                {
                    var classNamespace = new TextBlock
                    {
                        Text = $"Namespace: {classDoc.Namespace}",
                        FontStyle = FontStyles.Italic,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    contentPanel.Children.Add(classNamespace);
                }

                // Summary
                if (!string.IsNullOrEmpty(classDoc.Summary))
                {
                    var classSummary = new TextBlock
                    {
                        Text = $"Summary: {classDoc.Summary}",
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    contentPanel.Children.Add(classSummary);
                }

                // Remarks
                if (!string.IsNullOrEmpty(classDoc.Remarks))
                {
                    var classRemarks = new TextBlock
                    {
                        Text = $"Remarks: {classDoc.Remarks}",
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    contentPanel.Children.Add(classRemarks);
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
                    contentPanel.Children.Add(membersTitle);

                    foreach (var member in classDoc.Members)
                    {
                        var memberTitle = new TextBlock
                        {
                            Text = member.MemberName,
                            FontSize = 14,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 5, 0, 0)
                        };
                        contentPanel.Children.Add(memberTitle);
                        RegisterName(GenerateValidName(member.MemberName), memberTitle);

                        if (!string.IsNullOrEmpty(member.Summary))
                        {
                            var memberSummary = new TextBlock
                            {
                                Text = $"Summary: {member.Summary}",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            contentPanel.Children.Add(memberSummary);
                        }

                        if (!string.IsNullOrEmpty(member.Remarks))
                        {
                            var memberRemarks = new TextBlock
                            {
                                Text = $"Remarks: {member.Remarks}",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            contentPanel.Children.Add(memberRemarks);
                        }

                        if (member.Parameters.Count > 0)
                        {
                            var parametersTitle = new TextBlock
                            {
                                Text = "Parameters:",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            contentPanel.Children.Add(parametersTitle);

                            var parametersList = new StackPanel
                            {
                                Margin = new Thickness(20, 0, 0, 5)
                            };
                            foreach (var param in member.Parameters)
                            {
                                parametersList.Children.Add(new TextBlock { Text = $"{param.Key}: {param.Value}" });
                            }
                            contentPanel.Children.Add(parametersList);
                        }

                        if (member.TypeParameters.Count > 0)
                        {
                            var typeParametersTitle = new TextBlock
                            {
                                Text = "Type Parameters:",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            contentPanel.Children.Add(typeParametersTitle);

                            var typeParametersList = new StackPanel
                            {
                                Margin = new Thickness(20, 0, 0, 5)
                            };
                            foreach (var typeParam in member.TypeParameters)
                            {
                                typeParametersList.Children.Add(new TextBlock { Text = $"{typeParam.Key}: {typeParam.Value}" });
                            }
                            contentPanel.Children.Add(typeParametersList);
                        }

                        if (member.Exceptions.Count > 0)
                        {
                            var exceptionsTitle = new TextBlock
                            {
                                Text = "Exceptions:",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            contentPanel.Children.Add(exceptionsTitle);

                            var exceptionsList = new StackPanel
                            {
                                Margin = new Thickness(20, 0, 0, 5)
                            };
                            foreach (var exception in member.Exceptions)
                            {
                                exceptionsList.Children.Add(new TextBlock { Text = $"{exception.Key}: {exception.Value}" });
                            }
                            contentPanel.Children.Add(exceptionsList);
                        }

                        if (!string.IsNullOrEmpty(member.Returns))
                        {
                            var returnsText = new TextBlock
                            {
                                Text = $"Returns: {member.Returns}",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            contentPanel.Children.Add(returnsText);
                        }

                        if (member.SeeAlso.Count > 0)
                        {
                            var seeAlsoTitle = new TextBlock
                            {
                                Text = "See Also:",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            contentPanel.Children.Add(seeAlsoTitle);

                            var seeAlsoList = new StackPanel
                            {
                                Margin = new Thickness(20, 0, 0, 5)
                            };
                            foreach (var seeAlso in member.SeeAlso)
                            {
                                seeAlsoList.Children.Add(new TextBlock { Text = seeAlso });
                            }
                            contentPanel.Children.Add(seeAlsoList);
                        }

                        if (member.Examples.Count > 0)
                        {
                            var examplesTitle = new TextBlock
                            {
                                Text = "Examples:",
                                Margin = new Thickness(10, 0, 0, 5)
                            };
                            contentPanel.Children.Add(examplesTitle);

                            foreach (var example in member.Examples)
                            {
                                var exampleText = new TextBlock
                                {
                                    Text = example,
                                    TextWrapping = TextWrapping.Wrap,
                                    Margin = new Thickness(20, 0, 0, 5)
                                };
                                contentPanel.Children.Add(exampleText);
                            }
                        }

                        contentPanel.Children.Add(new Separator());
                    }
                }

                contentPanel.Children.Add(new Separator());
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ScrollToSection(e.Uri.OriginalString);
            e.Handled = true;
        }

        private void ScrollToSection(string sectionName)
        {
            var target = contentPanel.FindName(GenerateValidName(sectionName)) as FrameworkElement;
            target?.BringIntoView();
        }

        private string GenerateValidName(string name)
        {
            return name.Replace(".", "_").Replace(":", "_");
        }
    }
}