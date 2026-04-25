    using SQLite;

    namespace FeedAct.Entities
    {
        [Table("Comments")]
        public class Comment
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public int PostId { get; set; }   
            public string Text { get; set; } = string.Empty;
        }
    }