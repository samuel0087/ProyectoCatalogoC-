using CatalogoApp;
using System;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new frmPrincipal()); // Reemplaza TuFormularioPrincipal con el nombre de tu formulario principal
    }
}
