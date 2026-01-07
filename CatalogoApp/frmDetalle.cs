using dominio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CatalogoApp
{
    public partial class frmDetalle : Form
    {
        Articulo articulo;
        public frmDetalle()
        {
            InitializeComponent();
        }

        public frmDetalle(Articulo articulo)
        {
            this.articulo = articulo;
            InitializeComponent();
        }

        private void frmDetalle_Load(object sender, EventArgs e)
        {
            cargar();
        }

        private void cargar()
        {
            try
            {
                if(articulo != null)
                {
                    cargarImagen(articulo.UrlImagen);
                    lblNombre.Text = articulo.Nombre;
                    lblPrecio.Text = "$" + (articulo.Precio).ToString();
                    lblDescripcion.Text = "Descripcion: " + articulo.Descripcion;
                    lblMarca.Text = "Marca: " + articulo.Marca.ToString();
                    lblCategoria.Text = "Categoria: " + articulo.Categoria.ToString();
                }
            }
            catch
            {
                MessageBox.Show("Error al cargar articulo");
            }
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxArticulo.Load(imagen);
            }
            catch
            {
                pbxArticulo.Load("https://cdn-icons-png.flaticon.com/512/4154/4154438.png");
            }
        }
    }
}
