using System.Windows.Forms;

namespace KRingForm
{
    public static class FormArranger
    {
        public static void HideBehind(Form hiding, Form blocking)
        {
            hiding.Enabled = false;
            blocking.Show();
            hiding.Enabled = true;
        }
    }
}
