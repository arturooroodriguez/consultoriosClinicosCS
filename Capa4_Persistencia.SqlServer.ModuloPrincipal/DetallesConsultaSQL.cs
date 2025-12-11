using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Capa3_Dominio.ModuloPrincipal;
using Capa3_Dominio.ModuloPrincipal.Entidades;
using Capa4_Persistencia.SqlServer.ModuloBase;

namespace Capa4_Persistencia.SqlServer.ModuloPrincipal
{
    public class DetallesConsultaSQL
    {

        private AccesoSQLServer accesoSQLServer;

        public DetallesConsultaSQL(AccesoSQLServer accesoSQLServer)
        {
            this.accesoSQLServer = accesoSQLServer;
        }

            public void RegistrarDetallesConsulta(DetallesConsulta detallesConsulta)
           {
              string procedimientoSQL = "pro_registrar_DetallesConsulta";
               try
             {
                 SqlCommand comandoSQL = accesoSQLServer.ObtenerComandoDeProcedimiento(procedimientoSQL);
                 comandoSQL.Parameters.Add(new SqlParameter("@DetallesConsultaCodigo", detallesConsulta.DetallesConsultaEvaluacionPsico1));
                 comandoSQL.Parameters.Add(new SqlParameter("@DetallesConsultaHistoriaEnfermedad", detallesConsulta.DetallesConsultaHistoriaEnfermedad1));
                 comandoSQL.Parameters.Add(new SqlParameter("@DetallesConsultaRevisiones", detallesConsulta.DetallesConsultaRevisiones1));
                 comandoSQL.Parameters.Add(new SqlParameter("@DetallesConsultaEvaluacionPsico", detallesConsulta.DetallesConsultaEvaluacionPsico1));
                 comandoSQL.Parameters.Add(new SqlParameter("@DetallesConsultaMotivoConsulta", detallesConsulta.DetallesConsultaMotivoConsulta1));
                 comandoSQL.Parameters.Add(new SqlParameter("@ConsultaCodigo", detallesConsulta.Consulta.ConsultaCodigo));

                comandoSQL.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al registrar los detalles de la consulta: {ex.Message}");
            }


            }
        public DetallesConsulta ObtenerPorConsulta(string consultaCodigo)
        {
            string procedimientoSQL = "pro_Obtener_DetallesConsulta_Por_Consulta";

            SqlCommand comandoSQL = accesoSQLServer.ObtenerComandoDeProcedimiento(procedimientoSQL);
            comandoSQL.Parameters.Add(new SqlParameter("@consultaCodigo", consultaCodigo));

            using (SqlDataReader reader = comandoSQL.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new DetallesConsulta
                    {
                        DetallesConsultaCodigo1 = reader.GetString(0),
                        DetallesConsultaHistoriaEnfermedad1 = reader.IsDBNull(1) ? null : reader.GetString(1),
                        DetallesConsultaRevisiones1 = reader.IsDBNull(2) ? null : reader.GetString(2),
                        DetallesConsultaEvaluacionPsico1 = reader.IsDBNull(3) ? null : reader.GetString(3),
                        DetallesConsultaMotivoConsulta1 = reader.IsDBNull(4) ? null : reader.GetString(4)
                    };
                }
            }
            return null;
        }

    }

}
