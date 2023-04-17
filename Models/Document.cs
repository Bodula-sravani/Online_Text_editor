namespace Text_Editor.Models
{
    public class Document
    {
        public int id { get; set; }

        public string name { get; set; }
        public string content { get; set; }

        public DateTime createdDate { get; set; }

        public DateTime updatedDate { get; set; }

    }
}
