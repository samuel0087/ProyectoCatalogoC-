using dominio;
using negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CatalogoApp
{
    public partial class frmPrincipal : Form
    {
        private List<Articulo> listaArticulos;

        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            cargar();
        }

        private void cargar()
        {
            ArticuloNegocio aNegocio = new ArticuloNegocio();
            listaArticulos = aNegocio.listar();
            cargarLista(listaArticulos);
            cargarImagen(listaArticulos[0].UrlImagen);

            lblCampo.Visible = false;
            lblCriterio.Visible = false;
            cboCriterio.Visible = false;
            cboCampo.Visible = false;

        }

        private void cargarCampos()
        {
            cboCampo.Items.Clear();
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripcion");
            cboCampo.Items.Add("Marca");
            cboCampo.Items.Add("Categoria");
            cboCampo.Items.Add("Precio");

            cboCampo.SelectedItem = -1;
            cboCampo.Text = string.Empty;

            cboCampo.Items.Add("Codigo");
            cboCriterio.Enabled = false;
        }
        

        public void cargarLista(List<Articulo> lista)
        {
            dgvArticulos.DataSource = null;
            dgvArticulos.DataSource = lista;
            dgvArticulos.Columns["UrlImagen"].Visible = false;
            dgvArticulos.Columns["IdArticulo"].Visible = false;
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
            if(pbxArticulo.Image != null)
            {
                pbxArticulo.Image.Dispose();
                pbxArticulo.Image = null;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmArticulo formulario = new frmArticulo();
            formulario.ShowDialog();
            cargar();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado = null;
            try
            {
                if(dgvArticulos.CurrentRow != null)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                    liberarImagen();

                    frmArticulo formulario = new frmArticulo(seleccionado);
                    formulario.ShowDialog();
                }                
            }
            catch
            {
                MessageBox.Show("No se pudo seleccionar correctamente el articulo, intentelo nuevamente");
            }
            cargar();

        }

        private void dgvArticulos_SelectionChanged(object sender, EventArgs e)
        {
            Articulo seleccionado;

            try
            {
                if(dgvArticulos.CurrentRow != null)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;
                    cargarImagen(seleccionado.UrlImagen);
                }
            }
            catch
            {
                MessageBox.Show("Error al seleccionar articulo");
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;
            ArticuloNegocio aNegocio = new ArticuloNegocio();

            try
            {
                if (dgvArticulos.CurrentRow != null)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

                    DialogResult result = MessageBox.Show("¿Desea eliminar este articulo de forma definitiva?", "Eliminando...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        aNegocio.eliminar(seleccionado.IdArticulo);
                        MessageBox.Show("Articulo eliminado correctamente", "Exito");
                        cargar();
                    }
                }

            }
            catch
            {
                MessageBox.Show("Error al seleccionar articulo");
            }
        }

        private void btnDetalles_Click(object sender, EventArgs e)
        {
            Articulo seleccionado;

            try
            {
                if(dgvArticulos.CurrentRow != null)
                {
                    seleccionado = (Articulo)dgvArticulos.CurrentRow.DataBoundItem;

                    frmDetalle frmDetalle = new frmDetalle(seleccionado);
                    frmDetalle.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Seleccione un elemento por favor");
                }
            }
            catch
            {
                MessageBox.Show("No se pudo seleccionar el articulo, intentelo nuevamente mas tarde");
            }
        }

        private void btnBusquuedaAvanzada_Click(object sender, EventArgs e)
        {
            if (!ckbAvanzado.Checked)
            {
                busquedaSimple();
            }
            else
            {
                busquedaAvanzada();
            }


        }

        public void busquedaSimple()
        {
            List<Articulo> listaFiltrada;
            string filtro = txtBuscar.Text;

            try
            {
                if (filtro.Length > 2)
                {
                    liberarImagen();
                    listaFiltrada = listaArticulos.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) ||
                                                                x.Descripcion.ToUpper().Contains(filtro.ToUpper()) ||
                                                                x.Marca.Nombre.ToUpper().Contains(filtro.ToUpper()) ||
                                                                x.Categoria.Nombre.ToUpper().Contains(filtro.ToUpper())
                                                           );
                }
                else
                {
                    listaFiltrada = listaArticulos;
                }

                cargarLista(listaFiltrada);
            }
            catch
            {
                MessageBox.Show("Error al buscar Articulo");
            }
        }

        public void busquedaAvanzada()
        {
            ArticuloNegocio aNegocio = new ArticuloNegocio();
            List<Articulo> listaFiltrada = new List<Articulo>();
            //Capturar campos de busqueda

            try
            {

                if (!validarFiltros(txtBuscar.Text.Trim()))
                {
                    return;
                }

                string campo = cboCampo.Text;
                string criterio = cboCriterio.Text;
                string filtro = txtBuscar.Text.Trim();

                //realizar busqueda

                listaFiltrada = aNegocio.busquedaAvanzada(campo, criterio, filtro);

                if(listaFiltrada.Count == 0)
                {
                    MessageBox.Show("No se encontro resultado que coincidan con el filtro solicitado");
                    listaFiltrada = aNegocio.listar();
                }

                cargarLista(listaFiltrada);
            }
            catch
            {
                MessageBox.Show("No se pudo realizar la busqueda");
            }

            //mostrar resultados

        }

        private void ckbAvanzado_CheckedChanged(object sender, EventArgs e)
        {
            bool estado = false;

            if(ckbAvanzado.Checked)
            {              
                estado = true;
            }

            cargarCampos();

            lblCampo.Visible = estado;
            lblCriterio.Visible = estado;
            cboCriterio.Visible = estado;
            cboCampo.Visible = estado;



        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cboCriterio.Enabled = true;
                txtBuscar.Enabled = true;
                txtBuscar.Text = "";

                if(cboCriterio.DataSource != null )
                {
                    cboCriterio.DataSource = null;
                }
                else
                {
                    cboCriterio.Items.Clear();
                }

                switch(cboCampo.SelectedItem.ToString())
                {
                    case "Precio":
                        cboCriterio.Items.Add("Mayor que");
                        cboCriterio.Items.Add("Menor que");
                        break;

                    case "Marca":
                        MarcaNegocio mNegocio = new MarcaNegocio();
                        cboCriterio.DataSource = mNegocio.listar();
                        txtBuscar.Enabled = false;
                        break;

                    case "Categoria":
                        CategoriaNegocio cNegocio = new CategoriaNegocio();
                        cboCriterio.DataSource = cNegocio.listar();
                        txtBuscar.Enabled = false;
                        break;

                    default:
                        cboCriterio.Items.Add("Comienza con");
                        cboCriterio.Items.Add("Contiene");
                        cboCriterio.Items.Add("Termina con");
                        break;

                }

                cboCriterio.SelectedItem = -1;
                cboCriterio.Text = string.Empty;
            }
            catch
            {
                MessageBox.Show("Error al seleccionar campo de busqueda");
            }
            
        }

        private bool validarFiltros(string cadena)
        {

            try
            {
                if (cboCampo.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione un Campo de busqueda por favor");
                    return false;
                }

                if(cboCriterio.SelectedIndex < 0)
                {
                    MessageBox.Show("Seleccione un criterio de busqueda por favor");
                    return false;
                }

                if(cboCampo.SelectedItem.ToString() == "Precio")
                {
                    if (string.IsNullOrEmpty(cadena))
                    {
                        MessageBox.Show("Ingrese un filtro numerico para poder realizar la busqueda");
                        return false;
                    }

                }


                return true;
            }
            catch
            {
                MessageBox.Show("No se pudo realizar busqueda");
                return false;
            }

           


        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!ckbAvanzado.Checked)
            {
                return;
            }

            if(cboCampo.SelectedItem != null)
            {
                if (cboCampo.SelectedItem.ToString() == "Precio")
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
            }

            

        }
    }
}
