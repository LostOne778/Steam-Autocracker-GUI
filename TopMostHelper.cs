using System.Drawing;
using System.Windows.Forms;

namespace APPID
{
    /// <summary>
    /// Child dialogs must be TopMost, otherwise they render BEHIND windows that
    /// are themselves TopMost (the batch processor, or the main window when the
    /// stay-on-top pin is enabled) and become unreachable - which locks the app.
    /// Every form the app opens should go through this before being shown.
    /// </summary>
    public static class TopMostHelper
    {
        /// <summary>
        /// Forces the form above everything and keeps it there once shown.
        /// Usage: myForm.AsTopMost().ShowDialog(this);
        /// </summary>
        public static T AsTopMost<T>(this T form) where T : Form
        {
            if (form == null) return form;

            form.TopMost = true;
            form.Shown += (s, e) =>
            {
                try
                {
                    form.TopMost = true;
                    form.BringToFront();
                    form.Activate();
                }
                catch { }
            };
            return form;
        }

        /// <summary>
        /// MessageBox that can't hide behind a TopMost window. A plain
        /// MessageBox.Show() with no owner renders behind the batch/pinned windows
        /// and blocks the app, so it is shown owned by an invisible TopMost form.
        /// Argument order matches MessageBox.Show(text, caption, buttons, icon).
        /// </summary>
        public static DialogResult ShowMessage(string text, string caption = "",
            MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            using (var owner = new Form
            {
                TopMost = true,
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Location = new Point(-4000, -4000),
                Size = new Size(1, 1)
            })
            {
                owner.Show();
                try { return MessageBox.Show(owner, text, caption, buttons, icon); }
                finally { owner.Hide(); }
            }
        }
    }
}
