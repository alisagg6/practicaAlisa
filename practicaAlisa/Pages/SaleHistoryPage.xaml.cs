using practicaAlisa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace practicaAlisa.Pages
{
    public partial class SaleHistoryPage : Page
    {
        private int partnerId;
        private string partnerName;

        public class SaleHistoryItem
        {
            public DateTime SaleDate { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal Amount { get; set; }
            public string PointName { get; set; }
        }

        public SaleHistoryPage(int partnerId, string partnerName)
        {
            InitializeComponent();
            this.partnerId = partnerId;
            this.partnerName = partnerName;
            LoadSaleHistory();
        }

        private void LoadSaleHistory()
        {
            try
            {
                var salesList = ConnectionClass.comfortEntities.SaleHistory
                    .Where(sh => sh.SalePoint.Id_partner == partnerId)
                    .ToList();

                var sales = salesList.Select(sh => new SaleHistoryItem
                {
                    SaleDate = DateTime.Now,
                    ProductName = sh.Products != null ? sh.Products.NameProduct.ToString() : "",
                    Quantity = sh.Quantity ?? 0,
                    Amount = sh.Amount ?? 0,
                    PointName = sh.SalePoint != null ? sh.SalePoint.NamePoint.ToString() : ""
                }).OrderByDescending(sh => sh.SaleDate).ToList();

                decimal totalSales = sales.Sum(s => s.Amount);

                decimal discountPercentage = 0;
                if (totalSales > 10000 && totalSales <= 50000)
                    discountPercentage = 5;
                else if (totalSales > 50000 && totalSales <= 300000)
                    discountPercentage = 10;
                else if (totalSales > 300000)
                    discountPercentage = 15;

                PartnerNameText.Text = $"Партнер: {partnerName}";
                TotalSalesText.Text = $"Общая сумма продаж: {totalSales:N2} руб.";
                DiscountText.Text = $"Текущая скидка: {discountPercentage}%";

                SalesGrid.ItemsSource = sales;

                if (sales.Count == 0)
                {
                    MessageBox.Show("У данного партнера пока нет истории продаж.",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории продаж: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}