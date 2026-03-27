using practicaAlisa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace practicaAlisa.Pages
{
    public partial class PartnersListPage : Page
    {
        private List<Partners> allPartners;
        private string currentSearch = "";
        private string currentSort = "По наименованию (А-Я)";

        public PartnersListPage()
        {
            InitializeComponent();
            LoadPartners();
        }

        private void LoadPartners()
        {
            try
            {
                allPartners = ConnectionClass.comfortEntities.Partners.ToList();
                ApplyFilterAndSort();
                UpdateStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки партнеров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilterAndSort()
        {
            if (allPartners == null) return;

            var filtered = allPartners.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(currentSearch))
            {
                filtered = filtered.Where(p =>
                    (p.NamePartner != null && p.NamePartner.ToLower().Contains(currentSearch.ToLower())) ||
                    (p.Phone != null && p.Phone.Contains(currentSearch)) ||
                    (p.Email != null && p.Email.ToLower().Contains(currentSearch.ToLower())) ||
                    (p.INN != null && p.INN.Contains(currentSearch)) ||
                    (p.SurnameDirector != null && p.SurnameDirector.ToLower().Contains(currentSearch.ToLower()))
                );
            }

            switch (currentSort)
            {
                case "По наименованию (А-Я)":
                    filtered = filtered.OrderBy(p => p.NamePartner);
                    break;
                case "По наименованию (Я-А)":
                    filtered = filtered.OrderByDescending(p => p.NamePartner);
                    break;
                case "По рейтингу ↑":
                    filtered = filtered.OrderBy(p => p.Raiting);
                    break;
                case "По рейтингу ↓":
                    filtered = filtered.OrderByDescending(p => p.Raiting);
                    break;
                case "По скидке ↑":
                    filtered = filtered.OrderBy(p => p.GetDiscountPercentage());
                    break;
                case "По скидке ↓":
                    filtered = filtered.OrderByDescending(p => p.GetDiscountPercentage());
                    break;
                default:
                    filtered = filtered.OrderBy(p => p.NamePartner);
                    break;
            }

            PartnersLV.ItemsSource = filtered.ToList();
        }

        private void UpdateStatus()
        {
            int count = PartnersLV.Items.Count;
            StatusText.Text = $"Найдено партнеров: {count} из {allPartners?.Count ?? 0}";
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearch = SearchBox.Text;
            ApplyFilterAndSort();
            UpdateStatus();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedItem != null)
            {
                currentSort = (SortComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                ApplyFilterAndSort();
                UpdateStatus();
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartnersAddEditPage(new Partners()));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = PartnersLV.SelectedItem as Partners;
            if (selectedPartner != null)
            {
                NavigationService.Navigate(new PartnersAddEditPage(selectedPartner));
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите партнера для редактирования.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedPartner = PartnersLV.SelectedItem as Partners;
            if (selectedPartner == null)
            {
                MessageBox.Show("Пожалуйста, выберите партнера для удаления.",
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Вы действительно хотите удалить партнера \"{selectedPartner.NamePartner}\"?\n\n" +
                "ВНИМАНИЕ! Будут удалены все связанные данные.",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var salePoints = ConnectionClass.comfortEntities.SalePoint
                        .Where(sp => sp.Id_partner == selectedPartner.Id_partner)
                        .ToList();

                    foreach (var point in salePoints)
                    {
                        var sales = ConnectionClass.comfortEntities.SaleHistory
                            .Where(sh => sh.Id_point == point.Id_point)
                            .ToList();
                        foreach (var sale in sales)
                        {
                            ConnectionClass.comfortEntities.SaleHistory.Remove(sale);
                        }
                        ConnectionClass.comfortEntities.SalePoint.Remove(point);
                    }

                    var discounts = ConnectionClass.comfortEntities.DiscountPartners
                        .Where(dp => dp.Id_partner == selectedPartner.Id_partner)
                        .ToList();
                    foreach (var discount in discounts)
                    {
                        ConnectionClass.comfortEntities.DiscountPartners.Remove(discount);
                    }

                    var requests = ConnectionClass.comfortEntities.Request
                        .Where(r => r.Id_partner == selectedPartner.Id_partner)
                        .ToList();

                    foreach (var request in requests)
                    {
                        var details = ConnectionClass.comfortEntities.RequestDetails
                            .Where(rd => rd.Id_request == request.Id_request)
                            .ToList();
                        foreach (var detail in details)
                        {
                            ConnectionClass.comfortEntities.RequestDetails.Remove(detail);
                        }
                        ConnectionClass.comfortEntities.Request.Remove(request);
                    }

                    ConnectionClass.comfortEntities.Partners.Remove(selectedPartner);
                    ConnectionClass.comfortEntities.SaveChanges();

                    MessageBox.Show("Партнер успешно удален.", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadPartners();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении партнера: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadPartners();
            SearchBox.Text = "";
            SortComboBox.SelectedIndex = 0;
            currentSearch = "";
            currentSort = "По наименованию (А-Я)";
            MessageBox.Show("Список обновлен.", "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowHistory_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.Tag != null)
            {
                int partnerId = (int)button.Tag;
                var partner = allPartners?.FirstOrDefault(p => p.Id_partner == partnerId);
                if (partner != null)
                {
                    NavigationService.Navigate(new SaleHistoryPage(partnerId, partner.NamePartner));
                }
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                CurrentUser.User = null;
                NavigationService.Navigate(new LoginPage());
            }
        }
    }
}