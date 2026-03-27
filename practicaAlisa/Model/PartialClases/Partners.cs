using System.Linq;

namespace practicaAlisa.Model
{
    public partial class Partners
    {
        public string SizeDiscount
        {
            get
            {
                decimal totalSales = GetTotalSales();

                if (totalSales > 10000 && totalSales <= 50000)
                    return "5%";
                else if (totalSales > 50000 && totalSales <= 300000)
                    return "10%";
                else if (totalSales > 300000)
                    return "15%";
                else
                    return "0%";
            }
        }

        public decimal GetTotalSales()
        {
            var sales = ConnectionClass.comfortEntities.SaleHistory
                .Where(sh => sh.SalePoint.Id_partner == this.Id_partner)
                .ToList();

            decimal total = 0;
            foreach (var sale in sales)
            {
                total += sale.Amount ?? 0;
            }
            return total;
        }

        public decimal GetDiscountPercentage()
        {
            decimal totalSales = GetTotalSales();

            if (totalSales > 10000 && totalSales <= 50000)
                return 5;
            else if (totalSales > 50000 && totalSales <= 300000)
                return 10;
            else if (totalSales > 300000)
                return 15;
            else
                return 0;
        }
    }
}