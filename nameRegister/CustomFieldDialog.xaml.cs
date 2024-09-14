using System.Windows;

namespace NameRegister
{
    public partial class CustomFieldDialog : Window
    {
        public string FieldName { get; private set; }

        public CustomFieldDialog()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FieldNameTextBox.Text))
            {
                FieldName = FieldNameTextBox.Text;
                DialogResult = true;
            }
        }
    }
}