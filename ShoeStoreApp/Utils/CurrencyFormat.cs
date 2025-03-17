namespace ShoeStoreApp.Utils
{
    public class CurrencyFormat
    {
        public static string Format(long number)
        {
            return number.ToString("#,##0") + "₫";
        }
    }
}
