using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dominio;
using datos;
namespace negocio
{
    public class MarcaNegocio
    {

        public List<Marca> listar()
        {
            List<Marca> lista = new List<Marca>();
            AccesoDatos datos = new AccesoDatos();
            string query = "  SELECT Id, Descripcion FROM MARCAS";

            try
            {
                datos.setearConsulta(query);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Marca aux = new Marca();
                    aux.IdMarca = datos.Lector["Id"] is DBNull ? 0 : (int)datos.Lector["Id"];
                    aux.Nombre = datos.Lector["Descripcion"] is DBNull ? "" : (string)datos.Lector["Descripcion"];

                    lista.Add(aux);
                }

                return lista;
            }
            catch(Exception e) 
            {
                throw e;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
