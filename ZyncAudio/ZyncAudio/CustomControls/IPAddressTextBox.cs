using System.Windows.Forms;

namespace ZyncAudio.CustomControls
{
    public class IPAddressTextBox : TextBox
    {

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassName = "SysIPAddress32";
                return cp;
            }
        }

    }
}
