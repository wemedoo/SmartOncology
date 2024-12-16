using System.Collections.Generic;

namespace Classes
{
    public class Chapter
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public Chapter()
        {

        }

        public Chapter(string n, int i)
        {
            Name = n;
            Id = i;
        }
    }
}
