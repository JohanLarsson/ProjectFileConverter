namespace ProjectFileConverter
{
    public class ViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private string original;
        private string migrated;
        private string error;

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
                OnPropertyChanged();
            }
        }

        public string Migrated
        {
            get => migrated;
            set
            {
                if (value == migrated)
                {
                    return;
                }

                migrated = value;
                OnPropertyChanged();
            }
        }

        public string Error
        {
            get => this.error;
            set
            {
                if (value == this.error)
                {
                    return;
                }

                this.error = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}