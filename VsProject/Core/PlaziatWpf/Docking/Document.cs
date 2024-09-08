namespace PlaziatWpf.Docking
{

    /// <summary>
    /// Interface for dockable documents 
    /// Implement it for app needs
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// Called after control initialization 
        /// to open the specified document
        /// </summary>
        /// <param name="arguments"></param>
        public abstract void OpenDocument(AbstractDocumentArguments arguments);

    }

    /// <summary>
    /// Allows to transfer some data when opening a dockable document
    /// Derive it for app needs
    /// </summary>
    public abstract class AbstractDocumentArguments
    {
        /// <summary>
        /// Prevent to open the same document twice
        /// </summary>
        public string? documentId;
        /// <summary>
        /// Showed in the tab header
        /// </summary>
        public string documentTitle = string.Empty;
        /// <summary>
        /// Check arguments integrity
        /// </summary>
        public bool validated = false;

    }

}