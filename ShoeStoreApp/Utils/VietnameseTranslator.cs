using Microsoft.Build.Logging;

namespace ShoeStoreApp.Utils
{
    public class VietnameseTranslator
    {
        Dictionary<String, String> _dict;
        public VietnameseTranslator() { 
            _dict = new Dictionary<String, String>
            {
                { "men", "nam" },
                { "women", "nữ" },
                { "unisex", "unisex" },
                { "basketball", "bóng rổ" },
                { "sneaker", "sneaker" },
                { "football", "bóng đá" },
                { "black", "đen" },
                { "white", "trắng" },
                { "orange", "cam" },
                { "pink", "hồng" },
                { "red", "đỏ" },
                { "gray", "xám" },
                { "blue", "xanh dương" },
                { "green", "xanh lục" },
                { "yellow", "edit" }
            };
        }
        public string Translate(string word)
        {
            if (_dict.ContainsKey(word))
            {
                return _dict[word];
            }
            return word;
        }
    }
}
