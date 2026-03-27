using practicaAlisa.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace practicaAlisa.Pages
{
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
            LoadComboxes();
        }

        private void LoadComboxes()
        {
            try
            {
                familyStatusCombo.ItemsSource = ConnectionClass.comfortEntities.FamilyStatus.ToList();
                healthCombo.ItemsSource = ConnectionClass.comfortEntities.Health.ToList();
                positionCombo.ItemsSource = ConnectionClass.comfortEntities.Role.ToList();

                if (familyStatusCombo.Items.Count > 0)
                    familyStatusCombo.SelectedIndex = 0;
                if (healthCombo.Items.Count > 0)
                    healthCombo.SelectedIndex = 0;
                if (positionCombo.Items.Count > 0)
                    positionCombo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справочников: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void registration_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(login.Text) ||
                string.IsNullOrWhiteSpace(password.Password) ||
                string.IsNullOrWhiteSpace(surname.Text) ||
                string.IsNullOrWhiteSpace(name.Text) ||
                birthday.SelectedDate == null ||
                string.IsNullOrWhiteSpace(passportSeries.Text) ||
                string.IsNullOrWhiteSpace(passportNumber.Text) ||
                familyStatusCombo.SelectedItem == null ||
                healthCombo.SelectedItem == null ||
                positionCombo.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля!", "Ошибка регистрации",
                          MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Password != confirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка регистрации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var allLogins = ConnectionClass.comfortEntities.Logins.ToList();
                var existingLogin = allLogins.FirstOrDefault(l => l.Login == login.Text.Trim());

                if (existingLogin != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!",
                        "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    login.Focus();
                    return;
                }

                var allEmployees = ConnectionClass.comfortEntities.Employee.ToList();
                var existingEmployee = allEmployees.FirstOrDefault(emp =>
                    emp.PassportSeria == passportSeries.Text.Trim() &&
                    emp.PassportNumber == passportNumber.Text.Trim());

                if (existingEmployee != null)
                {
                    MessageBox.Show("Сотрудник с такими паспортными данными уже зарегистрирован!",
                        "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var selectedFamily = (FamilyStatus)familyStatusCombo.SelectedItem;
                var selectedHealth = (Health)healthCombo.SelectedItem;
                var selectedRole = (Role)positionCombo.SelectedItem;

                Employee employeeObj = new Employee
                {
                    Surname = surname.Text.Trim(),
                    Name = name.Text.Trim(),
                    Patronumic = string.IsNullOrWhiteSpace(patronymic.Text) ? null : patronymic.Text.Trim(),
                    Birthday = birthday.SelectedDate.Value,
                    PassportSeria = passportSeries.Text.Trim(),
                    PassportNumber = passportNumber.Text.Trim(),
                    Id_family = selectedFamily.Id_status,
                    Id_health = selectedHealth.Id_health,
                    Id_role = selectedRole.Id_role,
                    BankDetails = "Не указаны"
                };

                ConnectionClass.comfortEntities.Employee.Add(employeeObj);
                ConnectionClass.comfortEntities.SaveChanges();

                Logins loginObj = new Logins
                {
                    Login = login.Text.Trim(),
                    Password = password.Password.Trim(),
                    Id_user = employeeObj.Id_employee
                };

                ConnectionClass.comfortEntities.Logins.Add(loginObj);
                ConnectionClass.comfortEntities.SaveChanges();

                MessageBox.Show($"Регистрация успешно завершена!\n\n" +
                    $"Сотрудник: {employeeObj.Surname} {employeeObj.Name}\n" +
                    $"Логин: {loginObj.Login}\n\n" +
                    "Теперь вы можете войти в систему.",
                    "Успешная регистрация",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new LoginPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}",
                    "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}