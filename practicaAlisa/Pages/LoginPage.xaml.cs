using practicaAlisa.Model;
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


namespace practicaAlisa.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(login.Text) || string.IsNullOrEmpty(password.Password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var userObj = ConnectionClass.comfortEntities.Logins.ToList().FirstOrDefault(x => x.Login == login.Text.Trim() && x.Password == password.Password.Trim());
                if (userObj == null)
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    CurrentUser.User = userObj;
                    MessageBox.Show("Вы усппешно авторизовались");
                    NavigationService.Navigate(new PartnersListPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к бд: {ex.Message}", "Уведомление",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void registration_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }
    }
}
