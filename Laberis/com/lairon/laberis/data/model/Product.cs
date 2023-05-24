using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;

namespace Laberis.com.lairon.laberis.server.model
{
    public class Product
    {
        [JsonInclude] public Int64 id { get; set; }
        [JsonInclude] public string title { get; set; }
        [JsonInclude] public String description { get; set; }
        [JsonInclude] public decimal price { get; set; }
        
        public BitmapSource image { get; set; }
        
        public ProductType productType { get; set; }

        [JsonInclude] public Dictionary<string, string> specifications;
        
        [JsonInclude]
        public string type
        {
            set
            {
                ProductType outType;
                ProductType.TryParse(value, out outType);
                productType = outType;
            }
        }

       
        

        public string titleWithType
        {
            get { return typeToRus() + " | " + title; }
        }

        public string descriptionCutted
        {
            get { return description.Length > 100 ? description.Substring(0, 100) + "..." : description; }
        }

        public string priceFormated
        {
            get { return price + " ₽"; }
        }

        public string typeToRus()
        {
            switch (productType)
            {
                case ProductType.ORM:
                    return "Оперативная память";
                case ProductType.DISK:
                    return "Жесткий диск";
                case ProductType.FRAME:
                    return "Корпус";
                case ProductType.COOLER:
                    return "Охлаждение";
                case ProductType.MONITOR:
                    return "Монитор";
                case ProductType.POWER_UNIT:
                    return "Блок питания";
                case ProductType.PROCESSOR:
                    return "Процессор";
                case ProductType.VIDEO_CARD:
                    return "Видео карта";
                case ProductType.MOTHER_BOARD:
                    return "Материнская плата";
            }

            return "";
        }


        public string specificationsKeysText
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (KeyValuePair<string,string> keyValuePair in specifications)
                {
                builder.Append(keyValuePair.Key + ":\n");
                }

                return builder.ToString();
            }
        }
        
        public string specificationsValuesText
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach (KeyValuePair<string,string> keyValuePair in specifications)
                {
                    builder.Append(keyValuePair.Value + "\n");
                }

                return builder.ToString();
            }
        }
        
        
        public Product()
        {
        }
    }
}