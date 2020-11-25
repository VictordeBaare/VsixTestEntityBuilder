using EnvDTE;
using System.Collections.Generic;

namespace VSIXTestEntity
{
    public class TestEntityBuilderGenerator
    {
        private static readonly HashSet<string> AddRangeList = new HashSet<string>
        {
            ".List<"
        };

        private static readonly HashSet<string> AddableLists = new HashSet<string>
        {
            ".List<",
            ".Collection<",
            ".HashSet<"
        };

        public CodeFile GenerateCode(ProjectItem projectItem, Project project, ProjectItem folder)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var stringBuilder = new IndentedStringBuilder();
            var fileCodeModel = projectItem.FileCodeModel;
            if (fileCodeModel == null)
            {
                return new CodeFile
                {
                    Succesfull = false,
                };
            }

            var codeMetaData = GetCodeMetaData(fileCodeModel);

            var ns = GetNamespace(project, folder);

            GetCodeMetaData(fileCodeModel);

            stringBuilder.AppendLine($"namespace {ns}")
                .AppendLine("{");

            using (stringBuilder.Indent())
            {
                stringBuilder.AppendLine($"using {codeMetaData.Namespace};")
                    .AppendLine($"using System;")
                    .AppendLine($"using System.Linq;")
                    .AppendLine($"using System.CodeDom.Compiler;")
                    .AppendLine()
                    .AppendLine("[GeneratedCode(\"VsixTestBuilder\", \"0.1\")]")
                    .AppendLine($"public partial class {codeMetaData.Name}Builder")
                    .AppendLine("{");

                using (stringBuilder.Indent())
                {
                    stringBuilder.AppendLine($"private {codeMetaData.Name} _entity;");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine($"private {codeMetaData.Name}Builder()")
                        .AppendLine("{");

                    using (stringBuilder.Indent())
                    {
                        stringBuilder.AppendLine($"_entity = new {codeMetaData.Name}();");
                        stringBuilder.AppendLine($"InitializeEntity(_entity);");
                    }

                    stringBuilder.AppendLine("}");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine($"partial void InitializeEntity({codeMetaData.Name} entity);")
                        .AppendLine();

                    stringBuilder.AppendLine($"public static {codeMetaData.Name}Builder Valid()")
                        .AppendLine("{");

                    using (stringBuilder.Indent())
                    {
                        stringBuilder.AppendLine($"return new {codeMetaData.Name}Builder();");
                    }

                    stringBuilder.AppendLine("}")
                        .AppendLine();

                    stringBuilder.AppendLine($"public {codeMetaData.Name} Build()")
                        .AppendLine("{");

                    using (stringBuilder.Indent())
                    {
                        stringBuilder.AppendLine($"return _entity;");
                    }

                    stringBuilder.AppendLine("}")
                        .AppendLine();

                    codeMetaData.Properties.ForEach(x =>
                    {
                        Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                        stringBuilder.AppendLine($"public {codeMetaData.Name}Builder With{x.Name}({x.Type.AsString} obj)")
                            .AppendLine("{");

                        using (stringBuilder.Indent())
                        {
                            if (x.IsCollection && !x.IsSetterPublic)
                            {
                                if (AddRangeList.Contains(x.Type.AsString))
                                {
                                    stringBuilder.AppendLine($"_entity.{x.Name}.AddRange(obj);");
                                }
                                else
                                {
                                    stringBuilder.AppendLine($"foreach(var element in obj)")
                                        .AppendLine("{");
                                    using (stringBuilder.Indent())
                                    {
                                        stringBuilder.AppendLine($"_entity.{x.Name}.Add(element);");
                                    }
                                    stringBuilder.AppendLine("}");

                                }
                            }
                            else
                            {
                                stringBuilder.AppendLine($"_entity.{x.Name} = obj;");
                            }
                            stringBuilder.AppendLine($"return this;");
                        }

                        stringBuilder.AppendLine("}")
                            .AppendLine();
                    });
                }

                stringBuilder.AppendLine("}");
            }
            stringBuilder.AppendLine("}");

            return new CodeFile
            {
                GeneratedCode = stringBuilder.ToString(),
                Path = $"{GetFilePath(project, folder)}{codeMetaData.Name}Builder_Generated.cs",
                Succesfull = true,
            };
        }

        private static string GetFilePath(Project project, ProjectItem folder)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            return folder != null ? folder.Properties.Item("FullPath").Value.ToString() : project.Properties.Item("FullPath").Value.ToString();
        }

        private static CodeMetaData GetCodeMetaData(FileCodeModel fileCodeModel)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            var codeMetaData = new CodeMetaData();

            foreach (CodeElement element in fileCodeModel.CodeElements)
            {
                if (element is CodeNamespace codeNamespace)
                {
                    codeMetaData.Namespace = codeNamespace.Name;

                    foreach (CodeElement subElement in codeNamespace.Children)
                    {
                        if (subElement is CodeClass codeClass)
                        {
                            codeMetaData.Name = codeClass.Name;

                            foreach (CodeElement member in codeClass.Members)
                            {
                                if (member is CodeProperty codeProperty)
                                {
                                    var isList = AddableLists.Contains(codeProperty.Type.AsString);
                                    if (codeProperty.Setter != null || AddableLists.Contains(codeProperty.Type.AsString))
                                    {
                                        codeMetaData.Properties.Add(new CodeMetaData.CodeMetaDataProperty
                                        {
                                            Name = codeProperty.Name,
                                            Type = codeProperty.Type,
                                            IsCollection = isList,                                            
                                            IsSetterPublic = codeProperty.Setter != null
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return codeMetaData;
        }

        private string GetNamespace(Project project, ProjectItem folder)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            if (folder != null)
            {
                var projectFullPath = project.Properties.Item("FullPath").Value.ToString();
                var folderFullPath = folder.Properties.Item("FullPath").Value.ToString();

                var folderWithoutProjectPath = folderFullPath.Replace(projectFullPath, "").TrimEnd('\\');
                var folders = folderWithoutProjectPath.Split('\\');

                return $"{project.Name}.{string.Join(".", folders)}";
            }
            else
            {
                return project.Name;
            }            
        }

        private class CodeMetaData
        {
            public CodeMetaData()
            {
                Properties = new List<CodeMetaDataProperty>();
            }

            public string Namespace { get; set; }

            public string Name { get; set; }

            public List<CodeMetaDataProperty> Properties { get; set; }

            public class CodeMetaDataProperty
            {
                public string Name { get; set; }

                public CodeTypeRef Type { get; set; }

                public bool IsCollection { get; internal set; }

                public bool IsSetterPublic { get; internal set; }
            }
        }
    }

    

}
