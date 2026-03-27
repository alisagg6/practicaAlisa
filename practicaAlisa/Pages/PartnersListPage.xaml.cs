using practicaAlisa.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace practicaAlisa.Pages
{
    public partial class PartnersListPage : Page
    {
        public PartnersListPage()
        {
            InitializeComponent();
            LoadPartners();
        }

        public void LoadPartners()
        {
            PartnersLW.ItemsSource = ConnectionClass.comfortEntities.Partners.ToList();
            StatusText.Text = $"Всего: {ConnectionClass.comfortEntities.Partners.Count()}";
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = SearchBox.Text.ToLower();
            var all = ConnectionClass.comfortEntities.Partners.ToList();
            var filtered = all.Where(p => p.NamePartner != null && p.NamePartner.ToLower().Contains(search)).ToList();
            PartnersLW.ItemsSource = filtered;
            StatusText.Text = $"Найдено: {filtered.Count}";
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string search = SearchBox.Text.ToLower();
            var all = ConnectionClass.comfortEntities.Partners.ToList();
            var filtered = all.Where(p => p.NamePartner != null && p.NamePartner.ToLower().Contains(search)).ToList();

            string sort = (SortComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (sort == "По имени (А-Я)")
                filtered = filtered.OrderBy(p => p.NamePartner).ToList();
            else if (sort == "По имени (Я-А)")
                filtered = filtered.OrderByDescending(p => p.NamePartner).ToList();
            else if (sort == "По рейтингу ↑")
                filtered = filtered.OrderBy(p => p.Raiting).ToList();
            else if (sort == "По рейтингу ↓")
                filtered = filtered.OrderByDescending(p => p.Raiting).ToList();

            PartnersLW.ItemsSource = filtered;
            StatusText.Text = $"Найдено: {filtered.Count}";
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PartnersAddEditPage(new Partners()));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var selPartner = PartnersLW.SelectedItem as Partners;
            if (selPartner != null)
            {
                NavigationService.Navigate(new PartnersAddEditPage(selPartner));
            }
            else
            {
                MessageBox.Show("Не выбран партнер для редактирования");
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selPartner = PartnersLW.SelectedItem as Partners;
            if (selPartner == null)
            {
                MessageBox.Show("Выберите партнера");
                return;
            }

            var result = MessageBox.Show($"Удалить {selPartner.NamePartner}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                ConnectionClass.comfortEntities.Partners.Remove(selPartner);
                ConnectionClass.comfortEntities.SaveChanges();
                LoadPartners();
                MessageBox.Show("Удалено");
            }
        }

        private void HistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var selPartner = PartnersLW.SelectedItem as Partners;
            if (selPartner != null)
            {
                NavigationService.Navigate(new SaleHistoryPage(selPartner.Id_partner, selPartner.NamePartner));
            }
            else
            {
                MessageBox.Show("Выберите партнера");
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}