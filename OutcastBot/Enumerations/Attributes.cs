using System;

namespace OutcastBot.Enumerations
{
    class Attributes
    {
        public class MasteryInfoAttribute : Attribute
        {
            public string ImageUrl { get; private set; }

            internal MasteryInfoAttribute(string imageUrl)
            {
                ImageUrl = imageUrl;
            }
        }
    }
}
