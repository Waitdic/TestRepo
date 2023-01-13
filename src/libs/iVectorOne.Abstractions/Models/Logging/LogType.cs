namespace iVectorOne.Models.Logging
{
    /// <summary>Enum representing types of API database logs</summary>
    public enum LogType
    {
        /// <summary>A log for a search request/response</summary>
        Search = 0,

        /// <summary>A log for a pre book request/response</summary>
        Prebook = 1,

        /// <summary>A log for a book request/response</summary>
        Book = 2,

        /// <summary>A log for a cancel request/response</summary>
        Cancel = 3,

        /// <summary>A log for a pre cancel request/response</summary>
        Precancel = 4
    }
}