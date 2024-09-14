using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace NameRegister
{
    public partial class MainWindow : Window
    {
        private List<Person> people = new List<Person>();
        private List<string> allFields = new List<string>();
        private Random random = new Random();
        private Person selectedPerson;


        public MainWindow()
        {
            InitializeComponent();
            AddDefaultFields();
            this.MinWidth = 600;
            this.MinHeight = 400;
        }

        private string GenerateUniqueId()
        {
            long ticks = DateTime.UtcNow.Ticks % 10000;
            int rand = random.Next(100, 999);
            return $"{ticks}-{rand}";
        }


        private void AddDefaultFields()
        {
            AddField("ID");
            AddField("First Name");
            AddField("Last Name");
            AddField("Phone Number");
            AddField("Address");
            AddField("Birth Date");

            UpdateFieldsComboBox();
        }

        private void UpdateFieldsComboBox()
        {
            FieldsComboBox.ItemsSource = null;
            FieldsComboBox.ItemsSource = allFields.Where(f => f != "ID" && f != "Name");
            FieldsComboBox.SelectedIndex = -1;
        }


        private void AddField(string fieldName)
        {
            if (!allFields.Contains(fieldName))
            {
                allFields.Add(fieldName);
                UpdateListViewColumns();
                if (fieldName != "ID") // Don't add ID field to the input panel
                {
                    var label = new Label { Content = fieldName + ":", Margin = new Thickness(0, 5, 0, 0) };
                    var textBox = new TextBox { Margin = new Thickness(0, 0, 0, 5), Tag = fieldName };
                    FieldsPanel.Children.Add(label);
                    FieldsPanel.Children.Add(textBox);
                }
                UpdateFieldsComboBox();
            }
        }

        private void RemoveSelectedField_Click(object sender, RoutedEventArgs e)
        {
            if (FieldsComboBox.SelectedItem is string fieldToRemove)
            {
                var result = MessageBox.Show($"Are you sure you want to remove the '{fieldToRemove}' field? This will remove the field from all entries.", "Confirm Field Removal", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    allFields.Remove(fieldToRemove);

                    // Remove field from all people
                    foreach (var person in people)
                    {
                        person.Fields.Remove(fieldToRemove);
                    }

                    // Remove field from UI
                    var itemsToRemove = FieldsPanel.Children.OfType<FrameworkElement>()
                        .Where(fe => fe.Tag?.ToString() == fieldToRemove || (fe is Label label && label.Content.ToString() == fieldToRemove + ":"))
                        .ToList();

                    foreach (var item in itemsToRemove)
                    {
                        FieldsPanel.Children.Remove(item);
                    }

                    UpdateListViewColumns();
                    UpdateListView();
                    UpdateFieldsComboBox();
                }
            }
            else
            {
                MessageBox.Show("Please select a field to remove.", "No Field Selected", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UpdateListViewColumns()
        {
            PeopleGridView.Columns.Clear();
            foreach (var field in allFields)
            {
                PeopleGridView.Columns.Add(new GridViewColumn
                {
                    Header = field,
                    DisplayMemberBinding = new Binding($"Fields[{field}]")
                });
            }
        }

        private void AddNewField_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomFieldDialog();
            if (dialog.ShowDialog() == true)
            {
                string newFieldName = dialog.FieldName;
                AddField(newFieldName);
            }
        }

        private void AddPerson_Click(object sender, RoutedEventArgs e)
        {
            var person = new Person();
            person.SetField("ID", GenerateUniqueId());
            foreach (var child in FieldsPanel.Children)
            {
                if (child is TextBox textBox)
                {
                    string fieldName = textBox.Tag.ToString();
                    string fieldValue = textBox.Text;
                    person.SetField(fieldName, fieldValue);
                    textBox.Clear();
                }
            }
            people.Add(person);
            UpdateListView();
        }

        private void UpdateListView()
        {
            PeopleListView.ItemsSource = null;
            PeopleListView.ItemsSource = people;
        }

        private void PeopleListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PeopleListView.SelectedItem is Person person)
            {
                selectedPerson = person;
                foreach (var child in FieldsPanel.Children)
                {
                    if (child is TextBox textBox)
                    {
                        string fieldName = textBox.Tag.ToString();
                        textBox.Text = selectedPerson.GetField(fieldName);
                    }
                }
            }
            else
            {
                selectedPerson = null;
                ClearInputFields();
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPerson != null)
            {
                foreach (var child in FieldsPanel.Children)
                {
                    if (child is TextBox textBox)
                    {
                        string fieldName = textBox.Tag.ToString();
                        string fieldValue = textBox.Text;
                        selectedPerson.SetField(fieldName, fieldValue);
                    }
                }
                UpdateListView();
                MessageBox.Show("Changes saved successfully.", "Save Changes", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a person to update.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LoadDemoData_Click(object sender, RoutedEventArgs e)
        {
            people.Clear();
            people.Add(new Person
            {
                Fields = new Dictionary<string, string>
                {
                { "ID", GenerateUniqueId() },
                { "First Name", "John" },
                { "Last Name", "Doe" },
                { "Phone Number", "123-456-7890" },
                { "Address", "123 Main St" },
                { "Birth Date", "1980-01-01" }
                }
            });
            people.Add(new Person
            {
                Fields = new Dictionary<string, string>
                {
                { "ID", GenerateUniqueId() },
                { "First Name", "Jane" },
                { "Last Name", "Doe" },
                { "Phone Number", "987-654-3210" },
                { "Address", "456 Elm St" },
                { "Birth Date", "1985-05-15" }
                }
            });
            people.Add(new Person
            {
                Fields = new Dictionary<string, string>
                {
                { "ID", GenerateUniqueId() },
                { "First Name", "John" },
                { "Last Name", "Smith" },
                { "Phone Number", "123-456-7890" },
                { "Address", "123 Main St" },
                { "Birth Date", "1990-03-20" }
                }
            });

            people.Add(new Person
            {
                Fields = new Dictionary<string, string>
                {
                { "ID", GenerateUniqueId() },
                { "First Name", "Alice" },
                { "Last Name", "Johnson" },
                { "Phone Number", "555-123-4567" },
                { "Address", "789 Oak St" },
                { "Birth Date", "1992-08-10" }
                }
            });

            people.Add(new Person
            {
                Fields = new Dictionary<string, string>
                {
                { "ID", GenerateUniqueId() },
                { "First Name", "Michael" },
                { "Last Name", "Williams" },
                { "Phone Number", "222-333-4444" },
                { "Address", "654 Maple Ave" },
                { "Birth Date", "1987-11-30" }
                }
            });

            people.Add(new Person
            {
                Fields = new Dictionary<string, string>
                {
                { "ID", GenerateUniqueId() },
                { "First Name", "Emma" },
                { "Last Name", "Brown" },
                { "Phone Number", "444-555-6666" },
                { "Address", "321 Pine St" },
                { "Birth Date", "1995-04-25" }
                }
            });

            people.Add(new Person
            {
                Fields = new Dictionary<string, string>
                {
                { "ID", GenerateUniqueId() },
                { "First Name", "David" },
                { "Last Name", "Taylor" },
                { "Phone Number", "777-888-9999" },
                { "Address", "987 Birch Ln" },
                { "Birth Date", "1983-12-05" }
                }
            });

            UpdateListView();
        }

        private void DeletePerson_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPerson != null)
            {
                people.Remove(selectedPerson);
                UpdateListView();
                ClearInputFields();
                selectedPerson = null;
            }
            else
            {
                MessageBox.Show("Please select a person to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearInputFields()
        {
            foreach (var child in FieldsPanel.Children)
            {
                if (child is TextBox textBox)
                {
                    textBox.Clear();
                }
            }
        }

    }

    public class Person
    {
        public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();

        public void SetField(string fieldName, string value)
        {
            Fields[fieldName] = value;
        }

        public string GetField(string fieldName)
        {
            return Fields.TryGetValue(fieldName, out string value) ? value : string.Empty;
        }
    }
}