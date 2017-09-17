namespace Intent.Modules.VisualStudio.Projects.Parsing
{
    public class Bookmark
    {
        public int Position { get; internal set; }
        public string Token { get; }

        public Bookmark(string token, int position)
        {
            Token = token;
            Position = position;
        }
    }
}
