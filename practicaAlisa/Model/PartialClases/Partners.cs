using System.Linq;

namespace practicaAlisa.Model
{
    public partial class Partners
    {
        public string SizeDiscount
        {
            get
            {
                decimal? sum = ConnectionClass.comfortEntities.SaleHistory
                    .Where(sh => sh.SalePoint.Id_partner == this.Id_partner)
                    .Sum(sh => sh.Amount);

                if (sum > 10000 && sum < 50000)
                    return "5%";
                else if (sum >= 50000 && sum < 300000)
                    return "10%";
                else if (sum >= 300000)
                    return "15%";
                else
                    return null;
            }
        }
    }
}