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
using System.IO;
using Microsoft.Win32;
using System.Globalization;


namespace ExamPreparation.Pages
{
    /// <summary>
    /// Логика взаимодействия для Page_Client.xaml
    /// </summary>
    /// 
    public partial class Page_Client : Page
    {
        public Page_Client()
        {
            InitializeComponent();
            //DGridClient.ItemsSource = NorthwindEntities.GetContext().Employees.ToList();
            //var allCity = NorthwindEntities.GetContext().Employees.ToList();
            //allCity.Insert(0, new Employee
            //{
            //    City = "Все типы"
            //});
            //ComboType.ItemsSource = allCity;
            //ComboType.SelectedIndex = 0;

            updateEmployee();
        }

        private void updateEmployee()
        {

            var currentEmployee = NorthwindEntities.GetContext().Employees.ToList();

            //if (ComboType.SelectedIndex > 0)
            //    currentEmployee = currentEmployee.Where(p => p.City.Contains(ComboType.SelectedItem as Type)).ToList();

            if (DpBirthDate.SelectedDate != null)
                currentEmployee = currentEmployee.Where(p => p.BirthDate == DpBirthDate.SelectedDate).ToList();

            currentEmployee = currentEmployee.Where(p => p.LastName.ToLower().Contains(TbSearch.Text.ToLower()) || p.FirstName.ToLower().Contains(TbSearch.Text.ToLower())).ToList();

            DGridClient.ItemsSource = currentEmployee;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Pages.Page_AddEdit(null));
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Pages.Page_AddEdit((sender as Button).DataContext as Employee));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                NorthwindEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                DGridClient.ItemsSource = NorthwindEntities.GetContext().Employees.ToList();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var usersForRemoving = DGridClient.SelectedItems.Cast<Employee>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить следующие {usersForRemoving.Count()} элементов?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    NorthwindEntities.GetContext().Employees.RemoveRange(usersForRemoving);
                    NorthwindEntities.GetContext().SaveChanges();
                    MessageBox.Show("Данные удалены !");

                    DGridClient.ItemsSource = NorthwindEntities.GetContext().Employees.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            updateEmployee();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //updateEmployee();
        }

        private void DpBirthDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            updateEmployee();
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            // Создаем диалог сохранения файла
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Текстовый файл (*.txt)|*.txt|Файл CSV (*.csv)|*.csv";
            saveDialog.FileName = "employees.txt";

            // Если пользователь выбрал место сохранения и нажал ОК
            if (saveDialog.ShowDialog() == true)
            {
                string extension = System.IO.Path.GetExtension(saveDialog.FileName);
                // Создаем экземпляр StreamWriter для записи данных в файл
                using (StreamWriter writer = new StreamWriter(saveDialog.FileName, false, Encoding.UTF8))
                {
                    // Записываем заголовок таблицы
                    writer.WriteLine($"{ "ID",-5 }| { "Фамилия",-15 }| { "Имя",-10 }| { "Должность",-20 }| { "Дата рождения",-15 }");

                    // Записываем данные о сотрудниках
                    foreach (Employee employee in DGridClient.ItemsSource)
                    {
                        if (extension == ".csv")
                        {
                            writer.WriteLine($"{employee.EmployeeID,-5 }| {employee.LastName,-15 }| {employee.FirstName,-10 }| {employee.City,-20 }| {employee.BirthDate:dd.MM.yyyy}");
                        }
                        else if (extension == ".txt")
                        {
                            writer.WriteLine($"{employee.EmployeeID,-5 }| {employee.LastName,-15 }| {employee.FirstName,-10 }| {employee.City,-20 }| {employee.BirthDate:dd.MM.yyyy, -15}");
                        }
                    }
                }

                MessageBox.Show("Данные успешно экспортированы!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            // Создаем диалог открытия файла
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстовый файл (*.txt)|*.txt";

            // Если пользователь выбрал файл и нажал ОК
            if (openFileDialog.ShowDialog() == true)
            {
                // Создаем список для хранения данных
                List<Employee> employees = new List<Employee>();

                // Создаем экземпляр StreamReader для чтения данных из файла
                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    // Пропускаем заголовок таблицы
                    reader.ReadLine();

                    // Читаем данные о сотрудниках
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] fields = line.Split('\t');
                        int employeeID = int.Parse(fields[0]);
                        string lastName = fields[1];
                        string firstName = fields[2];
                        DateTime birthDate = DateTime.ParseExact(fields[3], "dd.MM.yyyy", CultureInfo.InvariantCulture);
                        string city = fields[4];
                        string address = fields[5];

                        Employee employee = new Employee(employeeID, lastName, firstName, birthDate, city, address);
                        employees.Add(employee);
                    }
                }

                // Сохраняем данные в базу данных
                using (var db = new NorthwindEntities())
                {
                    db.Employees.AddRange(employees);
                    db.SaveChanges();
                }

                // Отображаем данные в DataGrid
                DGridClient.ItemsSource = employees;
            }
        }
    }
}
