using System;

namespace OutcastBot.Enumerations
{
    class Attributes
    {
        public class MasteryInfoAttribute : Attribute
        {
            public string ImageUrl { get; private set; }
            public int Color { get; private set; }

            internal MasteryInfoAttribute(string imageUrl, int color)
            {
                ImageUrl = imageUrl;
                Color = color;
            }
        }
    }
}
