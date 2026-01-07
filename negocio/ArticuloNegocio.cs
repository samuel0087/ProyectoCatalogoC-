using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dominio;
using datos;
using System.Collections;

namespace negocio
{
    public class ArticuloNegocio
    {
        public List<Articulo> listar()
        {
            List<Articulo> lista = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            string query = "SELECT a.Id, a.Codigo, a.Nombre, a.Descripcion, m.Id AS IDMarca, m.Descripcion AS Marca, c.Id AS IDCategoria, c.Descripcion AS Categoria, a.ImagenUrl, a.Precio "+
                           "FROM ARTICULOS a " +
                           "INNER JOIN MARCAS m ON m.Id = a.IdMarca "+
                           "INNER JOIN CATEGORIAS c ON c.Id = a.IdCategoria ";

            try
            {
                datos.setearConsulta(query);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.IdArticulo = datos.Lector["Id"] is DBNull ? 0 : (int)datos.Lector["Id"];
                    aux.Codigo = datos.Lector["Codigo"] is DBNull ? "" : (string)datos.Lector["Codigo"];
                    aux.Nombre = datos.Lector["Nombre"] is DBNull ? "" : (string)datos.Lector["Nombre"];
                    aux.Descripcion = datos.Lector["Descripcion"] is DBNull ? "" : (string)datos.Lector["Descripcion"];
                    aux.UrlImagen = datos.Lector["ImagenUrl"] is DBNull ? "" : (string)datos.Lector["ImagenUrl"];
                    aux.Precio = datos.Lector["Precio"] is DBNull ? 0 : Math.Round((decimal)datos.Lector["Precio"], 2);

                    aux.Marca = new Marca();
                    aux.Marca.IdMarca = datos.Lector["IDMarca"] is DBNull ? 0 : (int)datos.Lector["IDMarca"];
                    aux.Marca.Nombre = datos.Lector["Marca"] is DBNull ? "" : (string)datos.Lector["Marca"];

                    aux.Categoria = new Categoria();
                    aux.Categoria.IdCategoria = datos.Lector["IDCategoria"] is DBNull ? 0 : (int)datos.Lector["IDCategoria"];
                    aux.Categoria.Nombre = datos.Lector["Categoria"] is DBNull ? "" : (string)datos.Lector["Categoria"];

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<Articulo> busquedaAvanzada(string campo, string criterio, string filtro)
        {
            List<Articulo> listaFiltrada = new List<Articulo>();
            AccesoDatos datos = new AccesoDatos();
            string query = "SELECT a.Id, a.Codigo, a.Nombre, a.Descripcion, m.Id AS IDMarca, m.Descripcion AS Marca, c.Id AS IDCategoria, c.Descripcion AS Categoria, a.ImagenUrl, a.Precio " +
                           "FROM ARTICULOS a " +
                           "INNER JOIN MARCAS m ON m.Id = a.IdMarca " +
                           "INNER JOIN CATEGORIAS c ON c.Id = a.IdCategoria WHERE ";

            try
            {
                switch (campo)
                {
                    case "Precio":

                        string filtroNmerico = filtro.Replace(',', '.');

                        if(criterio == "Mayor que")
                        {
                            query += "a.Precio > " + filtroNmerico;
                        }
                        else
                        {
                            query += "a.Precio < " + filtroNmerico;
                        }

                        break;

                    case "Categoria":
                        query += " c.Descripcion = '" + criterio + "'";
                        break;

                    case "Marca":
                        query += " m.Descripcion = '" + criterio + "'";
                        break;

                    default:

                        query += "a." + campo + " LIKE ";

                        if(criterio == "Contiene")
                        {
                            query += "'%" + filtro + "%'";
                        }
                        else if (criterio == "Comienza con")
                        {
                            query += "'" + filtro + "%'";
                        }
                        else
                        {
                            query += "'%" + filtro + "'";
                        }


                        break;
                }

                datos.setearConsulta(query);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Articulo aux = new Articulo();
                    aux.IdArticulo = datos.Lector["Id"] is DBNull ? 0 : (int)datos.Lector["Id"];
                    aux.Codigo = datos.Lector["Codigo"] is DBNull ? "" : (string)datos.Lector["Codigo"];
                    aux.Nombre = datos.Lector["Nombre"] is DBNull ? "" : (string)datos.Lector["Nombre"];
                    aux.Descripcion = datos.Lector["Descripcion"] is DBNull ? "" : (string)datos.Lector["Descripcion"];
                    aux.UrlImagen = datos.Lector["ImagenUrl"] is DBNull ? "" : (string)datos.Lector["ImagenUrl"];
                    aux.Precio = datos.Lector["Precio"] is DBNull ? 0 : Math.Round((decimal)datos.Lector["Precio"], 2);

                    aux.Marca = new Marca();
                    aux.Marca.IdMarca = datos.Lector["IDMarca"] is DBNull ? 0 : (int)datos.Lector["IDMarca"];
                    aux.Marca.Nombre = datos.Lector["Marca"] is DBNull ? "" : (string)datos.Lector["Marca"];

                    aux.Categoria = new Categoria();
                    aux.Categoria.IdCategoria = datos.Lector["IDCategoria"] is DBNull ? 0 : (int)datos.Lector["IDCategoria"];
                    aux.Categoria.Nombre = datos.Lector["Categoria"] is DBNull ? "" : (string)datos.Lector["Categoria"];

                    listaFiltrada.Add(aux);
                }

                return listaFiltrada;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
        }

        public void eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            string query = "Delete From ARTICULOS Where Id = @id";

            try
            {
                datos.setearConsulta(query);
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();

            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Articulo articulo)
        {
            AccesoDatos datos = new AccesoDatos();
            string query = "Update Articulos Set Codigo = @codigo, Nombre = @nombre, Descripcion = @descripcion, IdMarca = @marca, IdCategoria = @marca, ImagenUrl = @imagen, Precio = @precio Where Id = @id ";

            try
            {
                datos.setearConsulta(query);
                datos.setearParametro("@id", articulo.IdArticulo);
                datos.setearParametro("@codigo", articulo.Codigo);
                datos.setearParametro("@nombre", articulo.Nombre);
                datos.setearParametro("@descripcion", articulo.Descripcion);
                datos.setearParametro("@marca", articulo.Marca.IdMarca);
                datos.setearParametro("@categoria", articulo.Categoria.IdCategoria);
                datos.setearParametro("@imagen", articulo.UrlImagen);
                datos.setearParametro("@precio", articulo.Precio);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void agregar(Articulo articulo)
        {
            AccesoDatos datos = new AccesoDatos();
            string query = "Insert Into Articulos(Codigo, Nombre, Descripcion, IdMarca, IdCategoria, ImagenUrl, Precio)" +
                           " Values (@codigo, @nombre, @descripcion, @marca, @categoria, @imagen, @precio);";

            try
            {
                datos.setearConsulta(query);
                datos.setearParametro("@codigo", articulo.Codigo);
                datos.setearParametro("@nombre", articulo.Nombre);
                datos.setearParametro("@descripcion", articulo.Descripcion);
                datos.setearParametro("@marca", articulo.Marca.IdMarca);
                datos.setearParametro("@categoria", articulo.Categoria.IdCategoria);
                datos.setearParametro("@imagen", articulo.UrlImagen);
                datos.setearParametro("@precio", articulo.Precio);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

    }

}
