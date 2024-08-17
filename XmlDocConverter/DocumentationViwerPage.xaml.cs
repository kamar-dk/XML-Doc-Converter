using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentationViwerPage));

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
                try
                {
                    PopulateTableOfContents(classDocs);
                    DisplayDocumentation(classDocs);
                } catch (Exception ex)
                {
                    log.Error($"An error occurred while loading the documentation: {ex.Message}");
                    MessageBox.Show($"An error occurred while loading the documentation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select an XML file first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateTableOfContents(List<ClassDocumentation> classDocs)
        {
            var namespaceTree = new Dictionary<string, TreeViewItem>();

            // Sort the classDocs to ensure that sub-namespaces are handled before classes
            classDocs.Sort((x, y) => string.Compare(x.Namespace, y.Namespace));

            foreach (var classDoc in classDocs)
            {
                // Split the namespace into two parts: main namespace and remainder
                var lastDotIndex = classDoc.Namespace.LastIndexOf('.');
                string mainNamespace;
                string subNamespace;

                if (lastDotIndex != -1)
                {
                    mainNamespace = classDoc.Namespace.Substring(0, lastDotIndex); // e.g., "program.utilities"
                    subNamespace = classDoc.Namespace.Substring(lastDotIndex + 1); // e.g., "models"
                }
                else
                {
                    mainNamespace = classDoc.Namespace; // If no dot found, use the entire namespace
                    subNamespace = string.Empty;
                }

                // Handle the main namespace
                if (!namespaceTree.ContainsKey(mainNamespace))
                {
                    var nsNode = new TreeViewItem { Header = mainNamespace };
                    namespaceTree[mainNamespace] = nsNode;
                    tocTreeView.Items.Add(nsNode);
                }

                var parentNode = namespaceTree[mainNamespace];

                // If there's a subnamespace, handle it
                if (!string.IsNullOrEmpty(subNamespace))
                {
                    if (!namespaceTree.ContainsKey(classDoc.Namespace))
                    {
                        var subNode = new TreeViewItem { Header = subNamespace };
                        namespaceTree[classDoc.Namespace] = subNode;

                        // Insert the subnamespace node at the top of the parent node's items
                        parentNode.Items.Insert(0, subNode);
                    }

                    parentNode = namespaceTree[classDoc.Namespace];
                }

                // Now add the class under the correct node
                var classLink = new TextBlock();
                var hyperlink = new Hyperlink(new Run(classDoc.ClassName))
                {
                    NavigateUri = new Uri(classDoc.ClassName, UriKind.Relative)
                };
                hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
                classLink.Inlines.Add(hyperlink);

                var classNode = new TreeViewItem { Header = classLink };

                // Add class nodes to the parent node (which could be either the main namespace or a subnamespace)
                parentNode.Items.Add(classNode);
            }
        }

        private void DisplayDocumentation(List<ClassDocumentation> classDocs)
        {
            foreach (var classDoc in classDocs)
            {
                var classTitle = ClassTitle(classDoc);
                contentPanel.Children.Add(classTitle);
                RegisterName(GenerateValidName(classDoc.ClassName), classTitle);

                // Namespace
                if (!string.IsNullOrEmpty(classDoc.Namespace))
                {
                    var classNamespace = ClassNamespace(classDoc);
                    contentPanel.Children.Add(classNamespace);
                }

                // Summary
                if (!string.IsNullOrEmpty(classDoc.Summary))
                {
                    var classSummary = ClassSummary(classDoc);
                    contentPanel.Children.Add(classSummary);
                }

                // Remarks
                if (!string.IsNullOrEmpty(classDoc.Remarks))
                {
                    var classRemarks = ClassRemarks(classDoc);
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

        private TextBlock ClassRemarks(ClassDocumentation classDoc)
        {
            return new TextBlock
            {
                Text = $"Remarks: {classDoc.Remarks}",
                Margin = new Thickness(0, 0, 0, 5)
            };
        }

        private TextBlock ClassSummary(ClassDocumentation classDoc)
        {
            return new TextBlock
            {
                Text = $"Summary: {classDoc.Summary}",
                Margin = new Thickness(0, 0, 0, 5)
            };
        }

        private TextBlock ClassTitle(ClassDocumentation classDoc)
        {
            // Class Title
            return new TextBlock
            {
                Text = classDoc.ClassName,
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 10, 0, 5)
            };
        }

        private TextBlock ClassNamespace(ClassDocumentation classDoc)
        {
            return new TextBlock
            {
                Text = $"Namespace: {classDoc.Namespace}",
                FontStyle = FontStyles.Italic,
                Margin = new Thickness(0, 0, 0, 5)
            };
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
            // Replace invalid characters with underscores
            return new string(name.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
        }
    }
}
