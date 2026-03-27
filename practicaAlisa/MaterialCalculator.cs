using practicaAlisa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace practicaAlisa
{
    public static class MaterialCalculator
    {
        public static int CalculateRequiredMaterial(int productTypeId, int materialTypeId,
            int quantity, double param1, double param2)
        {
            try
            {
                if (quantity <= 0 || param1 <= 0 || param2 <= 0)
                {
                    return -1;
                }

                var productType = ConnectionClass.comfortEntities.ProductType
                    .FirstOrDefault(pt => pt.Id_prodtype == productTypeId);

                if (productType == null || string.IsNullOrEmpty(productType.Coefficient))
                {
                    return -1;
                }

                double productCoefficient;
                if (!double.TryParse(productType.Coefficient, out productCoefficient))
                {
                    return -1;
                }

                var materialType = ConnectionClass.comfortEntities.TypeMaterial
                    .FirstOrDefault(tm => tm.Id_type_material == materialTypeId);

                if (materialType == null)
                {
                    return -1;
                }

                double materialLostPercent = (double)(materialType.LostProcent ?? 0);

                double materialPerUnit = param1 * param2 * productCoefficient;
                double totalMaterial = materialPerUnit * quantity;
                double materialWithWaste = totalMaterial * (1 + materialLostPercent);
                int result = (int)Math.Ceiling(materialWithWaste);

                return result;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int CalculateRequiredMaterial(string productTypeName, string materialTypeName,
            int quantity, double param1, double param2)
        {
            try
            {
                var productType = ConnectionClass.comfortEntities.ProductType
                    .FirstOrDefault(pt => pt.NameProdType == productTypeName);

                if (productType == null)
                {
                    return -1;
                }

                var materialType = ConnectionClass.comfortEntities.TypeMaterial
                    .FirstOrDefault(tm => tm.Name_material == materialTypeName);

                if (materialType == null)
                {
                    return -1;
                }

                return CalculateRequiredMaterial(
                    productType.Id_prodtype,
                    materialType.Id_type_material,
                    quantity, param1, param2);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static void TestCalculation()
        {
            int result = CalculateRequiredMaterial("Гостиные", "Мебельный щит из массива дерева",
                5, 2.5, 1.2);

            Console.WriteLine($"Требуется материала: {result} единиц");
        }
    }
}