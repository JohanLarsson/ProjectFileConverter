namespace ProjectFileConverter
{
    public class ViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private string old;
        private string migrated;

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        public string Old
        {
            get => old;
            set
            {
                if (value == old)
                {
                    return;
                }

                old = value;
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

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}