using Capa3_Dominio.ModuloPrincipal;
using Capa3_Dominio.ModuloPrincipal.Entidad;
using Capa3_Dominio.ModuloPrincipal.Entidades;
using Capa3_Dominio.ModuloPrincipal.TransferenciaDatos;
using Capa4_Persistencia.SqlServer.ModuloBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capa4_Persistencia.SqlServer.ModuloPrincipal
{
    public class ConsultaSQL
    {

        private AccesoSQLServer accesoSQLServer;

        public ConsultaSQL(AccesoSQLServer accesoSQLServer)
        {
            this.accesoSQLServer = accesoSQLServer;
        }

        public virtual void CrearConsulta(Consulta consulta)
        {
            string procedimientoSQL = "pro_Crear_Consulta";
            try
            {
                SqlCommand comandoSQL = accesoSQLServer.ObtenerComandoDeProcedimiento(procedimientoSQL);
                comandoSQL.Parameters.Add(new SqlParameter("@consultaCodigo", consulta.ConsultaCodigo));
                comandoSQL.Parameters.Add(new SqlParameter("@consultacitaCodigo", consulta.Cita.CitaCodigo));
                comandoSQL.Parameters.Add(new SqlParameter("@consultaFechaHoraFinal", consulta.ConsultaFechaHoraFinal));

                comandoSQL.Parameters.Add(new SqlParameter("@medicoCodigo", consulta.Medico.MedicoCodigo));
                comandoSQL.Parameters.Add(new SqlParameter("@tipoConsultaCodigo", consulta.TipoConsulta.TipoConsultaCodigo));
                comandoSQL.Parameters.Add(new SqlParameter("@pacienteCodigo", consulta.Paciente.PacienteCodigo));
                comandoSQL.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new Exception($"Error al guardar la consulta: {ex.Message}"); 
            }
        }



        public List<Consulta> ListarConsultas()
        {
            List<Consulta> consultas = new List<Consulta>();
            string procedimientoSQL = "pro_Listar_ConsultaConEspecialidad1";

            try
            {
                SqlCommand comandoSQL = accesoSQLServer.ObtenerComandoDeProcedimiento(procedimientoSQL);

                using (SqlDataReader resultadoSQL = comandoSQL.ExecuteReader())
                {
                    while (resultadoSQL.Read())
                    {
                        Consulta consulta = new Consulta()
                        {
                            ConsultaCodigo = resultadoSQL.GetString(0),
                            ConsultaFechaHoraFinal = resultadoSQL.IsDBNull(4) ? (DateTime?)null : resultadoSQL.GetDateTime(4),
                            Cita = new Cita()
                            {
                                CitaCodigo = resultadoSQL.GetString(1),
                                CitaFechaHora = resultadoSQL.GetDateTime(2),
                                CitaEstado = resultadoSQL.GetString(3)
                            },
                            Paciente = new Paciente()
                            {
                                PacienteCodigo = resultadoSQL.GetString(5),
                                PacienteNombreCompleto = resultadoSQL.GetString(6),
                            },
                            Medico = new Medico()
                            {
                                MedicoCodigo = resultadoSQL.GetString(7),
                                MedicoNombre = resultadoSQL.GetString(8),
                                MedicoApellido = resultadoSQL.GetString(9),
                                Especialidad = new Especialidad()
                                {
                                    EspecialidadCodigo = resultadoSQL.GetString(12),
                                    EspecialidadNombre = resultadoSQL.GetString(13)
                                }
                            },
                            TipoConsulta = new TipoConsulta()
                            {
                                TipoConsultaCodigo = resultadoSQL.GetString(10)
                            },
                            HistoriaClinica = new HistoriaClinica()
                            {
                                HistorialClinicoCodigo = resultadoSQL.GetString(11),
                            }
                        };

                        consultas.Add(consulta);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al listar consultas: {ex.Message}", ex);
            }

            return consultas;
        }

        public Consulta ObtenerConsultaPorCodigo1(string consultaCodigo)
        {
            Consulta consulta = null;
            string procedimientoSQL = "pro_Obtener_ConsultaPorCodigo";

            try
            {
                SqlCommand comandoSQL = accesoSQLServer.ObtenerComandoDeProcedimiento(procedimientoSQL);
                comandoSQL.Parameters.Add(new SqlParameter("@consultaCodigo", consultaCodigo));

                using (SqlDataReader resultadoSQL = comandoSQL.ExecuteReader())
                {
                    if (resultadoSQL.Read())
                    {
                        consulta = new Consulta()
                        {
                            ConsultaCodigo = resultadoSQL.GetString(0),
                            ConsultaFechaHoraFinal = resultadoSQL.IsDBNull(4) ? (DateTime?)null : resultadoSQL.GetDateTime(4),

                            Cita = new Cita()
                            {
                                CitaCodigo = resultadoSQL.GetString(1),
                                CitaFechaHora = resultadoSQL.GetDateTime(2),
                                CitaEstado = resultadoSQL.GetString(3)
                            },
                            Paciente = new Paciente()
                            {
                                PacienteCodigo = resultadoSQL.GetString(5),
                                PacienteNombreCompleto = resultadoSQL.GetString(6)
                            },
                            Medico = new Medico()
                            {
                                MedicoCodigo = resultadoSQL.GetString(7),
                                MedicoNombre = resultadoSQL.GetString(8),
                                MedicoApellido = resultadoSQL.GetString(9)
                            },
                            TipoConsulta = new TipoConsulta()
                            {
                                TipoConsultaCodigo = resultadoSQL.GetString(10)
                            },
                            HistoriaClinica = new HistoriaClinica()
                            {
                                HistorialClinicoCodigo = resultadoSQL.GetString(11)
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la consulta con código {consultaCodigo}: {ex.Message}", ex);
            }

            return consulta;
        }

        /*public ConsultaCompletaDTO ObtenerConsultaCompleta(string consultaCodigo)
        {
            // Repositorios auxiliares
            DetallesConsultaSQL detallesSQL = new DetallesConsultaSQL(accesoSQLServer);
            DiagnosticoSQL diagnosticoSQL = new DiagnosticoSQL(accesoSQLServer);
            RecetaMedicaSQL recetaSQL = new RecetaMedicaSQL(accesoSQLServer);

            // Paso 1: obtener la consulta base
            Consulta consulta = ObtenerConsultaPorCodigo(consultaCodigo);

            // Paso 2: obtener relaciones
            DetallesConsulta detalles = detallesSQL.ObtenerPorConsulta(consultaCodigo);
            var diagnosticos = diagnosticoSQL.ObtenerPorConsulta(consultaCodigo);
            var recetas = recetaSQL.ObtenerPorConsulta(consultaCodigo);

            return new ConsultaCompletaDTO
            {
                Consulta = consulta,
                DetallesConsulta = detalles,
                Diagnosticos = diagnosticos,
                Recetas = recetas
            };
        }*/


        private Consulta ObtenerConsultaPorCodigo(string consultaCodigo)
        {
            string procedimientoSQL = "pro_Obtener_Consulta_PorCodigo";
            SqlCommand comandoSQL = accesoSQLServer.ObtenerComandoDeProcedimiento(procedimientoSQL);
            comandoSQL.Parameters.Add(new SqlParameter("@consultaCodigo", consultaCodigo));

            using (SqlDataReader reader = comandoSQL.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new Consulta
                    {
                        ConsultaCodigo = reader.GetString(0),
                        ConsultaFechaHoraFinal = reader.IsDBNull(1) ? (DateTime?)null : reader.GetDateTime(1),
                        Paciente = new Paciente
                        {
                            PacienteCodigo = reader.GetString(2),
                            PacienteNombreCompleto = reader.GetString(3)
                        },
                        Medico = new Medico
                        {
                            MedicoCodigo = reader.GetString(4),
                            MedicoNombre = reader.GetString(5),
                            MedicoApellido = reader.GetString(6)
                        }
                    };
                }
            }
            return null;
        }

        public ConsultaCompletaDTO ObtenerConsultaCompleta(string consultaCodigo)
        {
            string procedimientoSQL = "pro_Obtener_ConsultaCompleta";
            ConsultaCompletaDTO dto = new ConsultaCompletaDTO();

            using (SqlCommand comandoSQL = accesoSQLServer.ObtenerComandoDeProcedimiento(procedimientoSQL))
            {
                comandoSQL.Parameters.AddWithValue("@consultaCodigo", consultaCodigo);

                using (SqlDataReader reader = comandoSQL.ExecuteReader())
                {
                    // 1. Consulta principal
                    if (reader.Read())
                    {
                        dto.Consulta = new Consulta
                        {
                            ConsultaCodigo = reader.GetString(0),
                            Cita = new Cita
                            {
                                CitaCodigo = reader.GetString(1),
                                CitaFechaHora = reader.GetDateTime(2),
                                CitaEstado = reader.GetString(3)
                            },
                            ConsultaFechaHoraFinal = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                            Paciente = new Paciente
                            {
                                PacienteCodigo = reader.GetString(5),
                                PacienteNombreCompleto = reader.GetString(6)
                            },
                            Medico = new Medico
                            {
                                MedicoCodigo = reader.GetString(7),
                                MedicoNombre = reader.GetString(8),
                                MedicoApellido = reader.GetString(9)
                            },
                            TipoConsulta = new TipoConsulta
                            {
                                TipoConsultaCodigo = reader.GetString(10)
                            },
                            HistoriaClinica = new HistoriaClinica
                            {
                                HistorialClinicoCodigo = reader.GetString(11)
                            }
                        };
                    }

                    // 2. Detalles
                    reader.NextResult();
                    dto.DetallesConsulta = new List<DetallesConsulta>();
                    while (reader.Read())
                    {
                        dto.DetallesConsulta.Add(new DetallesConsulta
                        {
                            DetallesConsultaCodigo1 = reader.GetString(0),
                            DetallesConsultaHistoriaEnfermedad1 = reader.GetString(1),
                            DetallesConsultaRevisiones1 = reader.GetString(2),
                            DetallesConsultaEvaluacionPsico1 = reader.GetString(3),
                            DetallesConsultaMotivoConsulta1 = reader.GetString(4),
                            Consulta = dto.Consulta
                        });
                    }

                    // 3. Diagnósticos
                    reader.NextResult();
                    dto.Diagnosticos = new List<Diagnostico>();
                    while (reader.Read())
                    {
                        dto.Diagnosticos.Add(new Diagnostico
                        {
                            DiagnosticoCodigo = reader.GetString(0),
                            DiagnosticoDescripcion = reader.GetString(1),
                            Consulta = dto.Consulta
                        });
                    }

                    // 4. Recetas
                    reader.NextResult();
                    dto.Recetas = new List<RecetaMedica>();
                    while (reader.Read())
                    {
                        dto.Recetas.Add(new RecetaMedica
                        {
                            RecetaCodigo = reader.GetString(0),
                            RecetaDescripcion = reader.GetString(1),
                            RecetaTratamiento = reader.GetString(2),
                            RecetaRecomendaciones = reader.GetString(3),
                            Consulta = dto.Consulta
                        });
                    }
                }
            }

            return dto;
        }





    }
}
