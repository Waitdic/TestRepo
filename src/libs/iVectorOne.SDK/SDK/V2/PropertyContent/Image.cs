namespace iVectorOne.SDK.V2.PropertyContent
{
    public class Image
    {
        public string? SourceURL { get; set; }
        public string? URL { get; set; }
        public string? Title { get; set; }
        public int Sequence { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public Image()
        {

        }

        public Image(string? sourceUrl, string? Url, string? title, int sequence, int height, int width)
        {
            this.SourceURL = sourceUrl;
            this.URL = Url;
            this.Title = title;
            this.Sequence = sequence;
            this.Height = height;
            this.Width = width;
        }
    }
}