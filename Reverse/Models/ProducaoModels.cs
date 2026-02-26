using System;
using System.Collections.Generic;

namespace Reverse.Models
{
    /// <summary>
    /// Modelo de Solicitação de Produção
    /// </summary>
    public class ProducaoSolicitacao
    {
        public int SolicitacaoId { get; set; }
        public int NumeroSolicitacao { get; set; }
        public int UsuarioId { get; set; }

        // Dados de Início
        public DateTime DataInicio { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public int QtdFuncionariosInicio { get; set; }
        public int BagsEntrada { get; set; }
        public string ObservacaoInicial { get; set; }

        // Dados de Finalização
        public DateTime? DataFinalizacao { get; set; }
        public TimeSpan? HoraFinalizacao { get; set; }
        public int? QtdFuncionariosFinal { get; set; }
        public string ObservacaoFinal { get; set; }

        // Controle
        public char StatusSolicitacao { get; set; } // A=Aberta, F=Finalizada

        // Lista de materiais associados
        public List<ProducaoMaterial> Materiais { get; set; }

        public ProducaoSolicitacao()
        {
            Materiais = new List<ProducaoMaterial>();
            DataInicio = DateTime.Today;
            HoraInicio = DateTime.Now.TimeOfDay;
            StatusSolicitacao = 'A';
        }

        public bool EstaFinalizada()
        {
            return StatusSolicitacao == 'F';
        }

        public override string ToString()
        {
            return $"Solicitação #{NumeroSolicitacao}";
        }
    }

    /// <summary>
    /// Modelo de Material de Produção com Valorização
    /// </summary>
    public class ProducaoMaterial
    {
        public int MaterialId { get; set; }
        public int SolicitacaoId { get; set; }
        public string MaterialNome { get; set; }
        public int QtdBags { get; set; }
        public int Valorizacao { get; set; } // 1 a 5 estrelas

        public ProducaoMaterial()
        {
            MaterialId = 0;
            QtdBags = 0;
            Valorizacao = 1;
        }

        public ProducaoMaterial(string materialNome, int valorizacao)
        {
            MaterialId = 0;
            MaterialNome = materialNome;
            QtdBags = 0;
            Valorizacao = valorizacao;
        }

        /// <summary>
        /// Retorna as estrelas formatadas (⭐)
        /// </summary>
        public string EstrelasFormatadas
        {
            get
            {
                return new string('⭐', Valorizacao);
            }
        }
    }

    /// <summary>
    /// Modelo para exibição na grid de produção (view completa)
    /// </summary>
    public class ProducaoView
    {
        public int SolicitacaoId { get; set; }
        public int NumeroSolicitacao { get; set; }
        public DateTime DataInicio { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public TimeSpan? HoraFinalizacao { get; set; }
        public int QtdFuncionariosInicio { get; set; }
        public int? QtdFuncionariosFinal { get; set; }
        public int BagsEntrada { get; set; }
        public int MaterialId { get; set; }
        public string MaterialNome { get; set; }
        public int QtdBags { get; set; }
        public string ObservacaoInicial { get; set; }
        public string ObservacaoFinal { get; set; }

        // Propriedades formatadas para exibição
        public string DataInicioFormatada => DataInicio.ToString("dd/MM/yyyy");
        public string HoraInicioFormatada => HoraInicio.ToString(@"hh\:mm");
        public string DataFinalizacaoFormatada => DataFinalizacao?.ToString("dd/MM/yyyy") ?? "";
        public string HoraFinalizacaoFormatada => HoraFinalizacao?.ToString(@"hh\:mm") ?? "";
    }

    /// <summary>
    /// Modelo para o ComboBox de Materiais com Valorização
    /// </summary>
    public class MaterialCombo
    {
        public string Nome { get; set; }
        public int Valorizacao { get; set; }

        public string NomeComEstrelas
        {
            get
            {
                string estrelas = new string('⭐', Valorizacao);
                return $"{Nome} {estrelas}";
            }
        }

        public override string ToString()
        {
            return NomeComEstrelas;
        }
    }
}