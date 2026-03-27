using practicaAlisa.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace practicaAlisa.Pages
{
    public partial class PartnersAddEditPage : Page
    {
        Partners partners;
        bool isEditing;

        public PartnersAddEditPage(Partners _partners)
        {
            InitializeComponent();
            partners = _partners;
            isEditing = partners.Id_partner != 0;
            this.DataContext = partners;

            LoadTypes();

            if (isEditing && partners.Id_type.HasValue)
            {
                TypeCB.SelectedValue = partners.Id_type.Value;
            }
        }

        private void LoadTypes()
        {
            try
            {
                TypeCB.ItemsSource = ConnectionClass.comfortEntities.TypeOfBusiness.ToList();
                TypeCB.DisplayMemberPath = "NameBusiness";
                TypeCB.SelectedValuePath = "Id_type";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(partners.NamePartner))
                {
                    MessageBox.Show("Введите наименование партнера!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (TypeCB.SelectedItem == null)
                {
                    MessageBox.Show("Выберите тип партнера!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(partners.Phone))
                {
                    MessageBox.Show("Введите телефон!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (partners.Raiting.HasValue && (partners.Raiting < 0 || partners.Raiting > 5))
                {
                    MessageBox.Show("Рейтинг должен быть от 0 до 5!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                partners.Id_type = (int)TypeCB.SelectedValue;

                if (!isEditing)
                {
                    ConnectionClass.comfortEntities.Partners.Add(partners);
                    MessageBox.Show("Партнер успешно добавлен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Данные партнера успешно обновлены!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                ConnectionClass.comfortEntities.SaveChanges();
                NavigationService.Navigate(new PartnersListPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CanselBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}