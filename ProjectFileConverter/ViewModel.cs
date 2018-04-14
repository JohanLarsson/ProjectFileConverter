namespace ProjectFileConverter
{
    public class ViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private string original;
        private string migrated;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

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
    }
}