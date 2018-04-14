namespace ProjectFileConverter
{
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Microsoft.Win32;

    public partial class MainWindow : Window
    {
        private const string Filter = "*.csproj|*.csproj|All files (*.*)|*.*";
        private string fileName;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Filter
            };
            if (dialog.ShowDialog(this) == true)
            {
                var vm = (ViewModel)this.DataContext;
                this.fileName = dialog.FileName;
                vm.Original = File.ReadAllText(this.fileName);
                vm.Migrated = Migrate.ProjectFile(vm.Original, this.fileName);
            }
            else
            {
                this.fileName = null;
            }

            e.Handled = true;
        }


        private void OnCanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.fileName != null &&
                           ((ViewModel)this.DataContext).Migrated != null;
            e.Handled = true;
        }

        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            BindingOperations.GetBindingExpression(this.Migrated, TextBox.TextProperty)?.UpdateSource();
            var vm = (ViewModel)this.DataContext;
            File.WriteAllText(this.fileName, vm.Migrated);
            e.Handled = true;
        }
    }
}
