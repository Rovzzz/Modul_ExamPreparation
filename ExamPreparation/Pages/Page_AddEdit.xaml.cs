using ExamPreparation.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExamPreparation.Pages
{
    /// <summary>
    /// Логика взаимодействия для Page_AddEdit.xaml
    /// </summary>
    public partial class Page_AddEdit : Page
    {
        private Employee _currentEmployee = new Employee();
        public Page_AddEdit(Employee selectedEmployee)
        {
            InitializeComponent();

            if (selectedEmployee != null)
            {
                _currentEmployee = selectedEmployee;
            }


            DataContext = _currentEmployee;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentEmployee.LastName))
                errors.AppendLine("Укажите фамилию клиента");

            if (string.IsNullOrWhiteSpace(_currentEmployee.FirstName))
                errors.AppendLine("Укажите имя клиента");

            if (!_currentEmployee.BirthDate.HasValue)
                errors.AppendLine("Укажите дату рождения клиента");

            if (string.IsNullOrWhiteSpace(_currentEmployee.City))
                errors.AppendLine("Укажите город клиента");

            if (string.IsNullOrWhiteSpace(_currentEmployee.Address))
                errors.AppendLine("Укажите адресс клиента");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentEmployee.EmployeeID == 0)
                NorthwindEntities.GetContext().Employees.Add(_currentEmployee);

            try
            {
                NorthwindEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentEmployee.BirthDate = datePicker1.SelectedDate;
        }
    }
}
