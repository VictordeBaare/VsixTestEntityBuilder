using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System.Collections.Generic;
using System.Windows.Controls;

namespace VSIXTestEntity
{
    /// <summary>
    /// Interaction logic for SelectDestination.xaml
    /// </summary>
    public partial class SelectDestination
    {
        private readonly ProjectItem projectItem;
        private readonly TestEntityBuilderGenerator _generator;
        private readonly FileWriter _fileWriter;

        public SelectDestination(List<Project> projects, ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            InitializeComponent();
            OkButton.IsEnabled = false;
            _generator = new TestEntityBuilderGenerator();
            _fileWriter = new FileWriter();
            Title = $"Generate TestEntityBuilder for {projectItem.Name}";
            ProjectsComboBox.ItemsSource = projects;
            ProjectsComboBox.SelectionChanged += ProjectsComboBox_SelectionChanged;
            this.projectItem = projectItem;
        }

        private void ProjectsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var selectedProject = (sender as ComboBox).SelectedItem as Project;
            var folders = GetFolders(selectedProject);

            FolderComboBox.ItemsSource = folders;
            OkButton.IsEnabled = true;
        }

        private List<ProjectItem> GetFolders(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            List<ProjectItem> folders = new List<ProjectItem>();
            for (var i = 1; i < project.ProjectItems.Count; i++)
            {
                var item = project.ProjectItems.Item(i);
                string itemType = item.Kind;
                if (itemType == Constants.vsProjectItemKindPhysicalFolder)
                {
                    folders.Add(project.ProjectItems.Item(i));
                }
            }

            return folders;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var selectedProject = ProjectsComboBox.SelectedItem as Project;
            var selectedFolder = FolderComboBox.SelectedItem as ProjectItem;

            if (projectItem.Name.EndsWith(".cs"))
            {
                var codeFile = _generator.GenerateCode(projectItem, selectedProject, selectedFolder);

                if (codeFile.Succesfull)
                {
                    var isNewlyAddedFile = _fileWriter.WriteOutputToFile(codeFile);

                    if (isNewlyAddedFile)
                    {
                        selectedProject.ProjectItems.AddFromFile(codeFile.Path);
                    }
                }
            }
            

            Close();
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }
    }
}
