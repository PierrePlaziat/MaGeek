namespace MageekFrontWpf.Framework.BaseMvvm
{

    public abstract class DocumentInitArgs
    {

        public string documentId = null;
        public string documentTitle = null;
        public bool validated = false;

    }

    public interface IDocument
    {

        public abstract void Initialize(DocumentInitArgs args);

    }

}