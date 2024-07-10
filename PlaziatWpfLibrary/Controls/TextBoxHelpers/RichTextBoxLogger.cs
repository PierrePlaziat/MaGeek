using Application = System.Windows.Application;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace PlaziatWpf.Controls.TextBoxHelpers
{

    public class RichTextBoxLogger
    {

        readonly RichTextBox logBox;

        public RichTextBoxLogger(RichTextBox logBox)
        {
            this.logBox = logBox;
        }

        public void Log(string log, string color = "#999999")
        {
            LogAsync(log, color).ConfigureAwait(false);
        }

        public async Task LogAsync(string log, string color = "#999999")
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() => {
                    logBox.AppendText(log /*+ "\n"*/, color);
                    logBox.ScrollToEnd();
                    return Task.CompletedTask;
                });
            });
        }

    }

}
