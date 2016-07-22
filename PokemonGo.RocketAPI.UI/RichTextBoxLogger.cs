using PokemonGo.RocketAPI.Logging;
using System;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.UI
{
    public class RichTextBoxLogger : ILogger
    {
        private LogLevel maxLogLevel;
        private RichTextBox richTextBox;

        private delegate void AppendTextDelegate(RichTextBox richTextBox, string text);

        /// <summary>
        /// To create a ConsoleLogger, we must define a maximum log level.
        /// All levels above won't be logged.
        /// </summary>
        /// <param name="maxLogLevel"></param>
        public RichTextBoxLogger(LogLevel maxLogLevel, RichTextBox richTextBox)
        {
            this.maxLogLevel = maxLogLevel;
            this.richTextBox = richTextBox;
        }

        public void Write(string message, LogLevel level = LogLevel.Info)
        {
            if (level > maxLogLevel)
                return;

            richTextBox.Invoke(new AppendTextDelegate(AppendText), new object[] { richTextBox, message });
        }

        private void AppendText(RichTextBox richTextBox, string text)
        {
            richTextBox.AppendText($"[{ DateTime.Now.ToString("HH:mm:ss")}] { text}" + Environment.NewLine);
            richTextBox.ScrollToCaret();
        }
    }
}
