namespace iVectorOne.Repositories
{
    /// <summary>Enum representing types of API database logs</summary>
    public enum LogType
    {
        /// <summary>A log for a pre book request/response</summary>
        Prebook = 0,

        /// <summary>A log for a book request/response</summary>
        Book = 1,

        /// <summary>A log for a cancel request/response</summary>
        Cancel = 2,

        /// <summary>A log for a pre cancel request/response</summary>
        Precancel = 3
    }
}