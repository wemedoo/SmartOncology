namespace Classes
{
    public class Page
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public Page()
        {
        }

        public Page(string n, int i)
        {
            Name = n;
            Id = i;
        }
    }
}
