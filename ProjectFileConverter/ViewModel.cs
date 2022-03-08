namespace ProjectFileConverter
{
    using System.IO;
    using System.Linq;
    using System.Windows.Input;

    public class ViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private string original;
        private string migrated;
        private string fileName;

        public ViewModel()
        {
            this.RemoveAssemblyInfoCommand = new RelayCommand(
                this.RemoveAssemblyInfo,
                () => !string.IsNullOrEmpty(this.migrated) &&
                      !this.migrated.Contains("<GenerateAssemblyInfo>false</GenerateAssemblyInfo>") &&
                      this.TryGetAssemblyInfo(out _));
            this.AddAutoGenerateBindingRedirectsCommand = new RelayCommand(
                () => this.Migrated = Migrate.WithAutoGenerateBindingRedirects(this.migrated),
                () => !string.IsNullOrEmpty(this.migrated) &&
                      !this.migrated.Contains("<AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>"));
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public ICommand RemoveAssemblyInfoCommand { get; }

        public ICommand AddAutoGenerateBindingRedirectsCommand { get; }

        public string FileName
        {
            get => this.fileName;
            set
            {
                if (value == this.fileName)
                {
                    return;
                }

                this.fileName = value;
                this.OnPropertyChanged();
            }
        }

        public string Original
        {
            get => this.original;
            set
            {
                if (value == this.original)
                {
                    return;
                }

                this.original = value;
                this.OnPropertyChanged();
            }
        }

        public string Migrated
        {
            get => this.migrated;
            set
            {
                if (value == this.migrated)
                {
                    return;
                }

                this.migrated = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        private bool TryGetAssemblyInfo(out FileInfo assemblyInfo)
        {
            assemblyInfo = null;
            if (string.IsNullOrEmpty(this.fileName))
            {
                return false;
            }

            assemblyInfo = new FileInfo(Path.Combine(Path.GetDirectoryName(this.fileName), "Properties", "AssemblyInfo.cs"));
            return assemblyInfo.Exists;
        }

        private void RemoveAssemblyInfo()
        {
            if (this.TryGetAssemblyInfo(out var assemblyInfo))
            {
                assemblyInfo.Delete();
                if (!assemblyInfo.Directory.EnumerateFileSystemInfos().Any())
                {
                    assemblyInfo.Directory.Delete();
                }
            }
        }
    }
}
