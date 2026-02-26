using Reverse.Helpers;
using Reverse.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Reverse.Data
{
    /// <summary>
    /// Repository para acesso aos dados de Produção
    /// Otimizado para múltiplos usuários simultâneos
    /// </summary>
    public class ProducaoRepository : IDisposable
    {
        private readonly string connectionString;
        private SqlConnection connection;

        public ProducaoRepository()
        {
            // Obtém connection string do App.config
            connectionString = ConnectionHelper.GetConnectionString();
        }

        private SqlConnection GetConnection()
        {
            if (connection == null)
            {
                connection = new SqlConnection(connectionString);
            }

            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }

        /// <summary>
        /// Cria uma nova solicitação de produção
        /// </summary>
        public ProducaoSolicitacao CriarSolicitacao(ProducaoSolicitacao solicitacao)
        {
            using (SqlCommand cmd = new SqlCommand("sp_CriarSolicitacao", GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 30;

                // Parâmetros de entrada
                cmd.Parameters.AddWithValue("@UsuarioId", solicitacao.UsuarioId);
                cmd.Parameters.AddWithValue("@DataInicio", solicitacao.DataInicio);
                cmd.Parameters.AddWithValue("@HoraInicio", solicitacao.HoraInicio);
                cmd.Parameters.AddWithValue("@QtdFuncionariosInicio", solicitacao.QtdFuncionariosInicio);
                cmd.Parameters.AddWithValue("@BagsEntrada", solicitacao.BagsEntrada);
                cmd.Parameters.AddWithValue("@ObservacaoInicial",
                    string.IsNullOrWhiteSpace(solicitacao.ObservacaoInicial) ? (object)DBNull.Value : solicitacao.ObservacaoInicial);

                // Parâmetros de saída
                SqlParameter paramSolicitacaoId = new SqlParameter("@SolicitacaoId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramSolicitacaoId);

                SqlParameter paramNumeroSolicitacao = new SqlParameter("@NumeroSolicitacao", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(paramNumeroSolicitacao);

                cmd.ExecuteNonQuery();

                solicitacao.SolicitacaoId = (int)paramSolicitacaoId.Value;
                solicitacao.NumeroSolicitacao = (int)paramNumeroSolicitacao.Value;
                solicitacao.StatusSolicitacao = 'A';

                return solicitacao;
            }
        }

        /// <summary>
        /// Finaliza uma solicitação existente
        /// </summary>
        public void FinalizarSolicitacao(ProducaoSolicitacao solicitacao)
        {
            using (SqlCommand cmd = new SqlCommand("sp_FinalizarSolicitacao", GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 30;

                cmd.Parameters.AddWithValue("@SolicitacaoId", solicitacao.SolicitacaoId);
                cmd.Parameters.AddWithValue("@DataFinalizacao", solicitacao.DataFinalizacao ?? DateTime.Today);
                cmd.Parameters.AddWithValue("@HoraFinalizacao", solicitacao.HoraFinalizacao ?? DateTime.Now.TimeOfDay);
                cmd.Parameters.AddWithValue("@QtdFuncionariosFinal", solicitacao.QtdFuncionariosFinal ?? 0);
                cmd.Parameters.AddWithValue("@ObservacaoFinal",
                    string.IsNullOrWhiteSpace(solicitacao.ObservacaoFinal) ? (object)DBNull.Value : solicitacao.ObservacaoFinal);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Busca todas as solicitações abertas
        /// </summary>
        public List<ProducaoSolicitacao> BuscarSolicitacoesAbertas()
        {
            List<ProducaoSolicitacao> lista = new List<ProducaoSolicitacao>();

            using (SqlCommand cmd = new SqlCommand("sp_BuscarSolicitacoesAbertas", GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 30;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ProducaoSolicitacao
                        {
                            SolicitacaoId = reader.GetInt32(0),
                            NumeroSolicitacao = reader.GetInt32(1),
                            DataInicio = reader.GetDateTime(2),
                            HoraInicio = (TimeSpan)reader.GetValue(3),
                            QtdFuncionariosInicio = reader.GetInt32(4),
                            BagsEntrada = reader.GetInt32(5),
                            ObservacaoInicial = reader.IsDBNull(6) ? null : reader.GetString(6),
                            StatusSolicitacao = 'A'
                        });
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Carrega uma solicitação específica com seus materiais
        /// </summary>
        public ProducaoSolicitacao CarregarSolicitacao(int solicitacaoId)
        {
            ProducaoSolicitacao solicitacao = null;

            string sql = @"SELECT SolicitacaoId, NumeroSolicitacao, UsuarioId, DataInicio, HoraInicio, 
                                  QtdFuncionariosInicio, BagsEntrada, ObservacaoInicial,
                                  DataFinalizacao, HoraFinalizacao, QtdFuncionariosFinal, 
                                  ObservacaoFinal, StatusSolicitacao
                           FROM ProducaoSolicitacao
                           WHERE SolicitacaoId = @SolicitacaoId";

            using (SqlCommand cmd = new SqlCommand(sql, GetConnection()))
            {
                cmd.Parameters.AddWithValue("@SolicitacaoId", solicitacaoId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        solicitacao = new ProducaoSolicitacao
                        {
                            SolicitacaoId = reader.GetInt32(0),
                            NumeroSolicitacao = reader.GetInt32(1),
                            UsuarioId = reader.GetInt32(2),
                            DataInicio = reader.GetDateTime(3),
                            HoraInicio = (TimeSpan)reader.GetValue(4),
                            QtdFuncionariosInicio = reader.GetInt32(5),
                            BagsEntrada = reader.GetInt32(6),
                            ObservacaoInicial = reader.IsDBNull(7) ? null : reader.GetString(7),
                            DataFinalizacao = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                            HoraFinalizacao = reader.IsDBNull(9) ? (TimeSpan?)null : (TimeSpan)reader.GetValue(9),
                            QtdFuncionariosFinal = reader.IsDBNull(10) ? (int?)null : reader.GetInt32(10),
                            ObservacaoFinal = reader.IsDBNull(11) ? null : reader.GetString(11),
                            StatusSolicitacao = reader.GetString(12)[0]
                        };
                    }
                }
            }

            // Carrega os materiais
            if (solicitacao != null)
            {
                solicitacao.Materiais = CarregarMateriaisSolicitacao(solicitacaoId);
            }

            return solicitacao;
        }

        /// <summary>
        /// Carrega os materiais de uma solicitação
        /// </summary>
        public List<ProducaoMaterial> CarregarMateriaisSolicitacao(int solicitacaoId)
        {
            List<ProducaoMaterial> lista = new List<ProducaoMaterial>();

            using (SqlCommand cmd = new SqlCommand("sp_CarregarMateriaisSolicitacao", GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SolicitacaoId", solicitacaoId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ProducaoMaterial
                        {
                            MaterialId = reader.GetInt32(0),
                            SolicitacaoId = solicitacaoId,
                            MaterialNome = reader.GetString(1),
                            QtdBags = reader.GetInt32(2),
                            Valorizacao = reader.GetInt32(3)
                        });
                    }
                }
            }

            return lista;
        }

        /// <summary>
        /// Salva ou atualiza um material
        /// </summary>
        public int SalvarMaterial(ProducaoMaterial material)
        {
            using (SqlCommand cmd = new SqlCommand("sp_SalvarMaterial", GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 30;

                SqlParameter paramMaterialId = new SqlParameter("@MaterialId", SqlDbType.Int)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = material.MaterialId
                };
                cmd.Parameters.Add(paramMaterialId);

                cmd.Parameters.AddWithValue("@SolicitacaoId", material.SolicitacaoId);
                cmd.Parameters.AddWithValue("@MaterialNome", material.MaterialNome);
                cmd.Parameters.AddWithValue("@QtdBags", material.QtdBags);
                cmd.Parameters.AddWithValue("@Valorizacao", material.Valorizacao);

                cmd.ExecuteNonQuery();

                material.MaterialId = (int)paramMaterialId.Value;
                return material.MaterialId;
            }
        }

        /// <summary>
        /// Remove um material
        /// </summary>
        public void RemoverMaterial(int materialId)
        {
            using (SqlCommand cmd = new SqlCommand("sp_RemoverMaterial", GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaterialId", materialId);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Carrega a produção completa para exibição na grid principal
        /// </summary>
        public List<ProducaoView> CarregarProducaoCompleta(DateTime? dataInicio = null, DateTime? dataFim = null)
        {
            List<ProducaoView> lista = new List<ProducaoView>();

            using (SqlCommand cmd = new SqlCommand("sp_CarregarProducaoCompleta", GetConnection()))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 30;

                cmd.Parameters.AddWithValue("@DataInicio", dataInicio.HasValue ? (object)dataInicio.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@DataFim", dataFim.HasValue ? (object)dataFim.Value : DBNull.Value);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ProducaoView
                        {
                            SolicitacaoId = reader.GetInt32(0),
                            NumeroSolicitacao = reader.GetInt32(1),
                            DataInicio = reader.GetDateTime(2),
                            HoraInicio = (TimeSpan)reader.GetValue(3),
                            DataFinalizacao = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                            HoraFinalizacao = reader.IsDBNull(5) ? (TimeSpan?)null : (TimeSpan)reader.GetValue(5),
                            QtdFuncionariosInicio = reader.GetInt32(6),
                            QtdFuncionariosFinal = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                            BagsEntrada = reader.GetInt32(8),
                            MaterialId = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                            MaterialNome = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            QtdBags = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                            ObservacaoInicial = reader.IsDBNull(12) ? null : reader.GetString(12),
                            ObservacaoFinal = reader.IsDBNull(13) ? null : reader.GetString(13)
                        });
                    }
                }
            }
            return lista.OrderByDescending(p => p.NumeroSolicitacao).ToList();
        }

        public void Dispose()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}