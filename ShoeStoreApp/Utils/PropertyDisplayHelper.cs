using Microsoft.CodeAnalysis.Differencing;

namespace ShoeStoreApp.Utils
{
    public class PropertyDisplayHelper
    {
        public static string GetTextForColor(string colorStr)
        {
            VietnameseTranslator translator = new VietnameseTranslator();
            var colors = colorStr.Split('/');
            List<string> results = new List<string>();
            foreach (var color in colors)
            {
                results.Add(Capitalize(translator.Translate(color.Trim())));
            }
            return string.Join(" / ", results);
        }

        public static string Capitalize(string str)
        {
            return str[..1].ToUpper() + str[1..];
        }
    }
}
