using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using practicaAlisa.Model;

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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                Employee employeeObj = new Employee
                {
                    Surname = surname.Text.Trim(),
                    Name = name.Text.Trim(),
                    Patronumic = string.IsNullOrWhiteSpace(patronymic.Text) ? " " : patronymic.Text.Trim(),
                    Birthday = birthday.SelectedDate.Value,
                    PassportSeria = passportSeries.Text.Trim(),
                    PassportNumber = passportNumber.Text.Trim(),
                    Id_family = (int)familyStatusCombo.SelectedValue,
                    Id_health = (int)healthCombo.SelectedValue,
                    Id_role = (int)positionCombo.SelectedValue
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

                MessageBox.Show("Данные успешно добавлены", "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                NavigationService.Navigate(new LoginPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка регистрации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}