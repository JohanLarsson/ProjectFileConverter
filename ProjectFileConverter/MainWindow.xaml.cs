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

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = Filter,
            };
            if (dialog.ShowDialog(this) == true)
            {
                var vm = (ViewModel)this.DataContext;
                vm.FileName = dialog.FileName;
                vm.Original = File.ReadAllText(dialog.FileName);
                vm.Migrated = Migrate.ProjectFile(vm.Original, dialog.FileName);
            }
            else
            {
                var vm = (ViewModel)this.DataContext;
                vm.FileName = null;
                vm.Original = string.Empty;
                vm.Migrated = string.Empty;
            }

            e.Handled = true;
        }

        private void OnCanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            var vm = (ViewModel)this.DataContext;
            e.CanExecute = vm.FileName != null &&
                           vm.Migrated != null;
            e.Handled = true;
        }

        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            BindingOperations.GetBindingExpression(this.Migrated, TextBox.TextProperty)?.UpdateSource();
            var vm = (ViewModel)this.DataContext;
            File.WriteAllText(vm.FileName, vm.Migrated);
            e.Handled = true;
        }
    }
}
