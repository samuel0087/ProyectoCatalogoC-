using dominio;
using negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Configuration;
using System.IO;

namespace CatalogoApp
{
    public partial class frmArticulo : Form
    {
        private Articulo articulo = null;
        private OpenFileDialog archivo = null;

        public frmArticulo()
        {
            InitializeComponent();
        }

        public frmArticulo(Articulo articulo)
        {
            this.articulo = articulo;
            InitializeComponent();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmArticulo_Load(object sender, EventArgs e)
        {
            cargarFormulario();
        }

        public void cargarFormulario()
        {
            MarcaNegocio mNegocio = new MarcaNegocio();
            CategoriaNegocio cNegocio = new CategoriaNegocio();

            cboMarca.DataSource = mNegocio.listar();
            cboMarca.ValueMember = "IdMarca";
            cboMarca.DisplayMember = "Nombre";
  

            cboCategoria.DataSource = cNegocio.listar();
            cboCategoria.ValueMember = "IdCategoria";
            cboCategoria.DisplayMember = "Nombre";

            if (articulo != null )
            {
                Text = "Editar articulo";

                txtCodigo.Text = articulo.Codigo;
                txtNombre.Text = articulo.Nombre;
                txtDescripcion.Text = articulo.Descripcion;
                cboMarca.SelectedValue = articulo.Marca.IdMarca;
                cboCategoria.SelectedValue = articulo.Categoria.IdCategoria;
                txtImagen.Text = articulo.UrlImagen;
                txtPrecio.Text = articulo.Precio.ToString();
                cargarImagen(articulo.UrlImagen);

            }

        }

        public void cargarImagen(string url)
        {
            try
            {
                liberarImagen();

                if (url.ToUpper().Contains("HTTP"))
                {
                    pbxArticulo.Load(url);
                }
                else
                {
                    FileStream fs = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    pbxArticulo.Image = Image.FromStream(fs);
                    fs.Close();
                    fs.Dispose();
                }

            }
            catch
            {
                pbxArticulo.Load("https://cdn-icons-png.flaticon.com/512/4154/4154438.png");
            }
        }

        public void liberarImagen()
        {
            if (pbxArticulo.Image != null)
            {
                pbxArticulo.Image.Dispose();
                pbxArticulo.Image = null;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            ArticuloNegocio aNegocio = new ArticuloNegocio();

            try
            {



                if (string.IsNullOrEmpty(txtImagen.Text))
                {

                    DialogResult result = MessageBox.Show("No se cargo ninguna imagen ¿Desea continuar de todas formas?", "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        return;
                    }

                }

                if (!validarFormulario())
                {
                    MessageBox.Show("Existen campos obligatorios sin completar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(articulo == null)
                {
                    articulo = new Articulo();
                }

                articulo.Codigo = txtCodigo.Text;
                articulo.Nombre = txtNombre.Text;
                articulo.Descripcion = txtDescripcion.Text;
                articulo.Marca = (Marca)cboMarca.SelectedItem;
                articulo.Categoria = (Categoria)cboCategoria.SelectedItem;
                articulo.Precio = decimal.Parse(txtPrecio.Text);
                articulo.UrlImagen = txtImagen.Text;

                if(articulo.IdArticulo != 0)
                {
                    DialogResult resultado = MessageBox.Show("Esta seguro de modificar este articulo?", "Modificando", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if(resultado == DialogResult.Yes)
                    {
                        guardarImagenLocal();
                        aNegocio.modificar(articulo);
                        MessageBox.Show("Modificado exitosamente");
                        Close();
                    }
                    
                }
                else
                {
                    DialogResult resultado = MessageBox.Show("Esta seguro de Guardar este articulo?", "Agregando", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (resultado == DialogResult.Yes)
                    {
                        guardarImagenLocal();
                        aNegocio.agregar(articulo);
                        MessageBox.Show("Agregado exitosamente");
                        Close();
                    }
                }

               
            }
            catch(Exception ex)
            {
                MessageBox.Show("No se pudo guardar en este momento, intentelo mas tarde: " + ex);
            }
        }

        private void txtImagen_TextChanged(object sender, EventArgs e)
        {
            cargarImagen(txtImagen.Text);
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg;|png|*.png";

            if(archivo.ShowDialog() == DialogResult.OK)
            {
                txtImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);
            }
        }

        private void guardarImagenLocal()
        {
            try
            {
                //Se guarda la imagen localmente
                if (archivo != null && !(txtImagen.Text.ToUpper().Contains("HTTP")))
                {
                    string carpeta = ConfigurationManager.AppSettings["folder-articulos"];

                    if (!Directory.Exists(carpeta))
                    {
                        Directory.CreateDirectory(carpeta);
                    }

                    string rutaDestino = Path.Combine(carpeta, archivo.SafeFileName);

                    if (File.Exists(rutaDestino))
                    {
                        DialogResult result = MessageBox.Show("El nombre del archivo  ya existe. ¿Desea reemplazarlo?", "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }

                    liberarImagen(); //Libera imagen para poder cargarla


                    File.Copy(archivo.FileName, rutaDestino, true);
                    articulo.UrlImagen = rutaDestino; //guarda ruta de la carpeta destino antes de guardarla en la db
                    
                }
            }
            catch
            {
                MessageBox.Show("Error al cargar la imagen localmente");
            }

        }

        public bool validarFormulario()
        {
            bool estado = true;
            lblCodigoError.Text = string.Empty;
            lblNombreError.Text = string.Empty;
            lblDescripcionError.Text = string.Empty;
            lblPrecioError.Text = string.Empty;


            if (string.IsNullOrEmpty(txtCodigo.Text))
            {
                lblCodigoError.Text = "*Campo obligatorio";
                estado = false;
            }

            if(string.IsNullOrEmpty(txtNombre.Text))
            {
                lblNombreError.Text = "*Campo obligatorio";
                estado = false;
            }

            if(string.IsNullOrEmpty (txtPrecio.Text))
            {
                lblPrecioError.Text = "*Campo obligatorio";
                estado = false;
            }
            
            return estado;
        }



        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            if (e.KeyChar == ',' && ((TextBox)sender).Text.Contains(","))
            {
                e.Handled = true;
            }
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsLetterOrDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled= true;
            }

            e.KeyChar = char.ToUpper(e.KeyChar);
        }
    }
}
