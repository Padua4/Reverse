using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Reverse.Data;
using Reverse.Models;

namespace Reverse.Forms.FormsProducao
{
    public partial class ProducaoForm : Form
    {
        private readonly int usuarioId;
        private ProducaoRepository repository;

        private ProducaoSolicitacao solicitacaoAtual;
        private List<ProducaoMaterial> materiaisTemporarios;

        public ProducaoForm(int _usuarioId)
        {
            InitializeComponent();
            usuarioId = _usuarioId;

            repository = new ProducaoRepository();
            materiaisTemporarios = new List<ProducaoMaterial>();

            ConfigurarFormulario();
            CarregarDados();
        }

        #region Configuração Inicial

        private void ConfigurarFormulario()
        {
            dtpData.Format = DateTimePickerFormat.Short;
            dtpData.Value = DateTime.Today;

            dtpHora.Format = DateTimePickerFormat.Time;
            dtpHora.ShowUpDown = true;
            dtpHora.Value = DateTime.Now;

            dtpDataFinalizacao.Format = DateTimePickerFormat.Short;
            dtpDataFinalizacao.Value = DateTime.Today;

            dtpHoraFinal.Format = DateTimePickerFormat.Time;
            dtpHoraFinal.ShowUpDown = true;
            dtpHoraFinal.Value = DateTime.Now;

            ConfigurarGridMateriais();

            ConfigurarGridProducao();

            HabilitarCamposInicio(false);
            HabilitarCamposFinalizacao(false);
            btnSalvar.Enabled = false;
        }

        private void ConfigurarGridMateriais()
        {
            dgvMaterial.AllowUserToAddRows = false;
            dgvMaterial.AllowUserToDeleteRows = false;
            dgvMaterial.MultiSelect = false;
            dgvMaterial.AutoGenerateColumns = false;
            dgvMaterial.Columns.Clear();

            dgvMaterial.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvMaterial.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvMaterial.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvMaterial.BackgroundColor = Color.FromArgb(250, 250, 252);
            dgvMaterial.BorderStyle = BorderStyle.FixedSingle;
            dgvMaterial.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvMaterial.GridColor = Color.FromArgb(230, 230, 235);
            dgvMaterial.RowHeadersVisible = false;
            dgvMaterial.EnableHeadersVisualStyles = false;

            dgvMaterial.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvMaterial.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvMaterial.ColumnHeadersHeight = 40;
            dgvMaterial.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(72, 126, 176); // Azul mais claro para diferenciar
            dgvMaterial.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvMaterial.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 5, 0, 5);

            dgvMaterial.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgvMaterial.RowsDefaultCellStyle.BackColor = Color.White;

            dgvMaterial.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgvMaterial.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvMaterial.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvMaterial.ColumnHeadersDefaultCellStyle.BackColor;
            dgvMaterial.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgvMaterial.DefaultCellStyle.Padding = new Padding(5, 8, 5, 8);

            dgvMaterial.RowTemplate.Height = 40;
            dgvMaterial.RowTemplate.MinimumHeight = 38;

            dgvMaterial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaterialId",
                DataPropertyName = "MaterialId",
                Visible = false
            });

            var leftStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F),
                Padding = new Padding(8, 0, 0, 0)
            };

            var centerStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F),
                Padding = new Padding(2)
            };

            var editableStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                BackColor = Color.FromArgb(255, 255, 240),
                ForeColor = Color.Black
            };

            dgvMaterial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Material",
                HeaderText = "MATERIAL",
                DataPropertyName = "MaterialNome",
                Width = 200,
                DefaultCellStyle = leftStyle,
                ReadOnly = true,
                HeaderCell = {
            Style = {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            }
        }
            });

            dgvMaterial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Valorizacao",
                HeaderText = "VALORIZAÇÃO",
                DataPropertyName = "EstrelasFormatadas",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI Emoji", 11F),
                    Padding = new Padding(2)
                },
                ReadOnly = true,
                HeaderCell = {
            Style = {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            }
        }
            });

            dgvMaterial.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Bags",
                HeaderText = "BAGS",
                DataPropertyName = "QtdBags",
                Width = 100,
                DefaultCellStyle = editableStyle,
                ReadOnly = false,
                HeaderCell = {
            Style = {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            }
        }
            });

            dgvMaterial.EditingControlShowing += (sender, e) =>
            {
                if (dgvMaterial.CurrentCell.ColumnIndex == dgvMaterial.Columns["Bags"].Index)
                {
                    if (e.Control is TextBox textBox)
                    {
                        textBox.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                        textBox.ForeColor = Color.FromArgb(30, 57, 91);
                        textBox.BackColor = Color.FromArgb(255, 255, 220);
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                        textBox.TextAlign = HorizontalAlignment.Center;
                    }
                }
            };

            dgvMaterial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dgvMaterial.Columns["Material"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMaterial.Columns["Material"].FillWeight = 50;
            dgvMaterial.Columns["Valorizacao"].FillWeight = 30;
            dgvMaterial.Columns["Bags"].FillWeight = 20;

            dgvMaterial.CellMouseEnter += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    dgvMaterial.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);

                    if (e.ColumnIndex == dgvMaterial.Columns["Bags"].Index)
                    {
                        dgvMaterial.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(255, 255, 200);
                    }
                }
            };

            dgvMaterial.CellMouseLeave += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (e.RowIndex % 2 == 0)
                        dgvMaterial.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    else
                        dgvMaterial.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);

                    if (e.ColumnIndex == dgvMaterial.Columns["Bags"].Index && !dgvMaterial.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected)
                    {
                        dgvMaterial.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(255, 255, 240);
                    }
                }
            };

            dgvMaterial.SelectionChanged += (sender, e) =>
            {
                if (dgvMaterial.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvMaterial.Rows)
                    {
                        row.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
                    }
                    dgvMaterial.SelectedRows[0].DefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

                    if (dgvMaterial.SelectedRows.Count > 0)
                    {
                        int rowIndex = dgvMaterial.SelectedRows[0].Index;
                        if (rowIndex >= 0)
                        {
                            dgvMaterial.Rows[rowIndex].Cells["Bags"].Style.BackColor = Color.FromArgb(255, 255, 200);
                        }
                    }
                }
            };

            dgvMaterial.CellValidating += DgvMaterial_CellValidating;
            dgvMaterial.CellEndEdit += DgvMaterial_CellEndEdit;

            dgvMaterial.CellBeginEdit += (sender, e) =>
            {
                if (dgvMaterial.Columns[e.ColumnIndex].Name == "Bags")
                {
                    dgvMaterial.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(255, 255, 180);
                    dgvMaterial.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Black;
                    dgvMaterial.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                }
            };

            dgvMaterial.CellEndEdit += (sender, e) =>
            {
                DgvMaterial_CellEndEdit(sender, e);

                if (dgvMaterial.Columns[e.ColumnIndex].Name == "Bags")
                {
                    dgvMaterial.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.FromArgb(255, 255, 240);
                    dgvMaterial.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Black;
                    dgvMaterial.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                }
            };

            dgvMaterial.ScrollBars = ScrollBars.Both;
        }

        private void ConfigurarGridProducao()
        {
            dgvProducao.AllowUserToAddRows = false;
            dgvProducao.AllowUserToDeleteRows = false;
            dgvProducao.MultiSelect = false;
            dgvProducao.AutoGenerateColumns = false;
            dgvProducao.ReadOnly = true;
            dgvProducao.Columns.Clear();

            dgvProducao.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvProducao.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvProducao.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvProducao.BackgroundColor = Color.FromArgb(250, 250, 252);
            dgvProducao.BorderStyle = BorderStyle.FixedSingle;
            dgvProducao.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvProducao.GridColor = Color.FromArgb(230, 230, 235);
            dgvProducao.RowHeadersVisible = false;
            dgvProducao.EnableHeadersVisualStyles = false;

            dgvProducao.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvProducao.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvProducao.ColumnHeadersHeight = 51;
            dgvProducao.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgvProducao.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProducao.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 5, 0, 5);

            dgvProducao.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
            dgvProducao.RowsDefaultCellStyle.BackColor = Color.White;

            dgvProducao.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 237, 255);
            dgvProducao.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvProducao.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvProducao.ColumnHeadersDefaultCellStyle.BackColor;
            dgvProducao.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

            dgvProducao.DefaultCellStyle.Padding = new Padding(3, 5, 3, 5);

            dgvProducao.RowTemplate.Height = 36;
            dgvProducao.RowTemplate.MinimumHeight = 35;

            dgvProducao.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SolicitacaoId",
                DataPropertyName = "SolicitacaoId",
                Visible = false
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaterialId",
                DataPropertyName = "MaterialId",
                Visible = false
            });

            var centerStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(2)
            };

            var leftStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5F),
                Padding = new Padding(5, 0, 0, 0)
            };

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NumeroSolicitacao",
                HeaderText = "Nº SOLICITAÇÃO",
                DataPropertyName = "NumeroSolicitacao",
                Width = 100,
                DefaultCellStyle = centerStyle,
                HeaderCell = {
            Style = {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            }
        }
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataInicio",
                HeaderText = "DATA INÍCIO",
                DataPropertyName = "DataInicioFormatada",
                Width = 100,
                DefaultCellStyle = centerStyle
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HoraInicio",
                HeaderText = "HORA INÍCIO",
                DataPropertyName = "HoraInicioFormatada",
                Width = 90,
                DefaultCellStyle = centerStyle
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataFinalizacao",
                HeaderText = "DATA FIM",
                DataPropertyName = "DataFinalizacaoFormatada",
                Width = 100,
                DefaultCellStyle = centerStyle
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HoraFinalizacao",
                HeaderText = "HORA FIM",
                DataPropertyName = "HoraFinalizacaoFormatada",
                Width = 90,
                DefaultCellStyle = centerStyle
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QtdFuncionariosInicio",
                HeaderText = "FUNC. INÍCIO",
                DataPropertyName = "QtdFuncionariosInicio",
                Width = 95,
                DefaultCellStyle = centerStyle
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QtdFuncionariosFinal",
                HeaderText = "FUNC. FINAL",
                DataPropertyName = "QtdFuncionariosFinal",
                Width = 95,
                DefaultCellStyle = centerStyle
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "BagsEntrada",
                HeaderText = "BAGS ENTRADA",
                DataPropertyName = "BagsEntrada",
                Width = 105,
                DefaultCellStyle = centerStyle
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaterialFeito",
                HeaderText = "MATERIAL PRODUZIDO",
                DataPropertyName = "MaterialNome",
                Width = 180,
                DefaultCellStyle = leftStyle
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "QtdBags",
                HeaderText = "BAGS PRODUZIDOS",
                DataPropertyName = "QtdBags",
                Width = 120,
                DefaultCellStyle = centerStyle
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ObservacaoInicial",
                HeaderText = "OBSERVAÇÃO INICIAL",
                DataPropertyName = "ObservacaoInicial",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    WrapMode = DataGridViewTriState.True,
                    Padding = new Padding(5, 5, 5, 5)
                }
            });

            dgvProducao.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ObservacaoFinal",
                HeaderText = "OBSERVAÇÃO FINAL",
                DataPropertyName = "ObservacaoFinal",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Font = new Font("Segoe UI", 9F),
                    WrapMode = DataGridViewTriState.True,
                    Padding = new Padding(5, 5, 5, 5)
                }
            });

            dgvProducao.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dgvProducao.Columns["MaterialFeito"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvProducao.Columns["ObservacaoInicial"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvProducao.Columns["ObservacaoFinal"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvProducao.Columns["MaterialFeito"].FillWeight = 30;
            dgvProducao.Columns["ObservacaoInicial"].FillWeight = 25;
            dgvProducao.Columns["ObservacaoFinal"].FillWeight = 25;

            dgvProducao.ScrollBars = ScrollBars.Both;

            dgvProducao.CellMouseEnter += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    dgvProducao.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
                }
            };

            dgvProducao.CellMouseLeave += (sender, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    if (e.RowIndex % 2 == 0)
                        dgvProducao.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                    else
                        dgvProducao.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 249, 255);
                }
            };

            dgvProducao.SelectionChanged += DgvProducao_SelectionChanged;
            dgvProducao.CellDoubleClick += DgvProducao_CellDoubleClick;

            dgvProducao.SelectionChanged += (sender, e) =>
            {
                if (dgvProducao.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvProducao.Rows)
                    {
                        row.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
                    }
                    dgvProducao.SelectedRows[0].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
                }
            };
        }

        #endregion

        #region Carregamento de Dados

        private void CarregarDados()
        {
            try
            {

                CarregarMateriaisDisponiveis();

                AtualizarGridProducao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvProducao_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                LimparFormulario();

                dgvProducao.Rows[e.RowIndex].Selected = true;

                DataGridViewRow row = dgvProducao.Rows[e.RowIndex];
                int solicitacaoId = Convert.ToInt32(row.Cells["SolicitacaoId"].Value);
                CarregarSolicitacaoParaEdicao(solicitacaoId);
            }
        }

        private void CarregarMateriaisDisponiveis()
        {
            try
            {
                {
                    using (var ctx = new ReverseContext())
                    {
                        var materiais = ctx.Materiais
                            .OrderByDescending(m => m.Valorizacao)
                            .ThenBy(m => m.Nome)
                            .Select(m => new MaterialCombo
                            {
                                Nome = m.Nome,
                                Valorizacao = m.Valorizacao
                            })
                            .ToList();

                        cbMateriais.DataSource = materiais;
                        cbMateriais.DisplayMember = "NomeComEstrelas";
                        cbMateriais.ValueMember = "Nome";
                        cbMateriais.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar materiais: {ex.Message}\n\nVerifique se a tabela Materiais existe no banco de dados.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AtualizarGridProducao()
        {
            List<ProducaoView> producao = repository.CarregarProducaoCompleta();
            dgvProducao.DataSource = null;
            dgvProducao.DataSource = producao;
        }

        #endregion

        #region Eventos de Botões

        private void btnNovaSolicitacao_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            HabilitarCamposInicio(true);
            HabilitarCamposFinalizacao(false);
            btnSalvar.Enabled = true;
            solicitacaoAtual = new ProducaoSolicitacao { UsuarioId = usuarioId };

            txtQtdFuncionarios.Focus();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (solicitacaoAtual == null)
                {
                    MessageBox.Show("Clique em 'Nova Solicitação' para começar.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ValidarCampos())
                    return;

                if (solicitacaoAtual.SolicitacaoId == 0)
                {
                    SalvarInicio();
                }
                else
                {
                    if (solicitacaoAtual.DataFinalizacao.HasValue)
                    {
                        DialogResult result = MessageBox.Show(
                            "Esta solicitação já foi finalizada anteriormente. Deseja atualizar os dados?",
                            "Confirmação",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }

                    SalvarFinalizacao();
                }

                CarregarDados();
                LimparFormulario();
                HabilitarCamposInicio(false);
                HabilitarCamposFinalizacao(false);
                btnSalvar.Enabled = false;

                MessageBox.Show("Dados salvos com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
            HabilitarCamposInicio(false);
            HabilitarCamposFinalizacao(false);
            btnSalvar.Enabled = false;

            if (dgvProducao.SelectedRows.Count > 0)
            {
                dgvProducao.ClearSelection();
            }
        }

        private void btnInserir_Click(object sender, EventArgs e)
        {
            if (cbMateriais.SelectedItem == null)
            {
                MessageBox.Show("Selecione um material.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MaterialCombo materialSelecionado = (MaterialCombo)cbMateriais.SelectedItem;
            string materialNome = materialSelecionado.Nome;
            int valorizacao = materialSelecionado.Valorizacao;

            if (materiaisTemporarios.Any(m => m.MaterialNome == materialNome))
            {
                MessageBox.Show("Este material já foi adicionado.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            materiaisTemporarios.Add(new ProducaoMaterial(materialNome, valorizacao));
            AtualizarGridMateriais();

            cbMateriais.SelectedIndex = -1;
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (dgvMaterial.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecione um material para remover.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Deseja remover este material?", "Confirmação",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int index = dgvMaterial.SelectedRows[0].Index;
                ProducaoMaterial material = materiaisTemporarios[index];

                if (material.MaterialId > 0)
                {
                    repository.RemoverMaterial(material.MaterialId);
                }

                materiaisTemporarios.RemoveAt(index);
                AtualizarGridMateriais();
            }
        }

        #endregion

        #region Eventos de ComboBox e Grid

        private void DgvProducao_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void DgvMaterial_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvMaterial.Columns[e.ColumnIndex].Name == "Bags")
            {
                if (!int.TryParse(e.FormattedValue.ToString(), out int valor) || valor < 0)
                {
                    MessageBox.Show("Digite um número válido.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        private void DgvMaterial_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvMaterial.Columns[e.ColumnIndex].Name == "Bags")
            {
                int index = e.RowIndex;
                int qtdBags = Convert.ToInt32(dgvMaterial.Rows[e.RowIndex].Cells["Bags"].Value);

                materiaisTemporarios[index].QtdBags = qtdBags;
            }
        }

        #endregion

        #region Métodos Auxiliares

        private bool ValidarCampos()
        {
            if (solicitacaoAtual.SolicitacaoId == 0)
            {
                if (string.IsNullOrWhiteSpace(txtQtdFuncionarios.Text) ||
                    !int.TryParse(txtQtdFuncionarios.Text, out int qtdFunc) || qtdFunc <= 0)
                {
                    MessageBox.Show("Informe a quantidade de funcionários.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQtdFuncionarios.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtBag.Text) ||
                    !int.TryParse(txtBag.Text, out int bags) || bags <= 0)
                {
                    MessageBox.Show("Informe a quantidade de bags de entrada.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtBag.Focus();
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtQtdFuncionariosFinal.Text) ||
                    !int.TryParse(txtQtdFuncionariosFinal.Text, out int qtdFuncFinal) || qtdFuncFinal <= 0)
                {
                    MessageBox.Show("Informe a quantidade de funcionários na finalização.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtQtdFuncionariosFinal.Focus();
                    return false;
                }

                if (materiaisTemporarios.Count == 0)
                {
                    MessageBox.Show("Adicione pelo menos um material.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (materiaisTemporarios.Any(m => m.QtdBags <= 0))
                {
                    MessageBox.Show("Todos os materiais devem ter quantidade de bags maior que zero.", "Validação",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private void SalvarInicio()
        {
            solicitacaoAtual.DataInicio = dtpData.Value.Date;
            solicitacaoAtual.HoraInicio = dtpHora.Value.TimeOfDay;
            solicitacaoAtual.QtdFuncionariosInicio = int.Parse(txtQtdFuncionarios.Text);
            solicitacaoAtual.BagsEntrada = int.Parse(txtBag.Text);
            solicitacaoAtual.ObservacaoInicial = txtObsInicio.Text;

            solicitacaoAtual = repository.CriarSolicitacao(solicitacaoAtual);

            MessageBox.Show($"Solicitação #{solicitacaoAtual.NumeroSolicitacao} criada com sucesso!",
                "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SalvarFinalizacao()
        {
            solicitacaoAtual.DataInicio = dtpData.Value.Date;
            solicitacaoAtual.HoraInicio = dtpHora.Value.TimeOfDay;
            solicitacaoAtual.QtdFuncionariosInicio = int.Parse(txtQtdFuncionarios.Text);
            solicitacaoAtual.BagsEntrada = int.Parse(txtBag.Text);
            solicitacaoAtual.ObservacaoInicial = txtObsInicio.Text;

            solicitacaoAtual.DataFinalizacao = dtpDataFinalizacao.Value.Date;
            solicitacaoAtual.HoraFinalizacao = dtpHoraFinal.Value.TimeOfDay;
            solicitacaoAtual.QtdFuncionariosFinal = int.Parse(txtQtdFuncionariosFinal.Text);
            solicitacaoAtual.ObservacaoFinal = txtObsFinalizacao.Text;

            repository.FinalizarSolicitacao(solicitacaoAtual);

            var materiaisExistentes = repository.CarregarMateriaisSolicitacao(solicitacaoAtual.SolicitacaoId);
            var idsTemporarios = new HashSet<int>(materiaisTemporarios.Where(m => m.MaterialId > 0).Select(m => m.MaterialId));

            foreach (var materialExistente in materiaisExistentes)
            {
                if (!idsTemporarios.Contains(materialExistente.MaterialId))
                {
                    repository.RemoverMaterial(materialExistente.MaterialId);
                }
            }

            foreach (var material in materiaisTemporarios)
            {
                material.SolicitacaoId = solicitacaoAtual.SolicitacaoId;
                repository.SalvarMaterial(material);
            }
        }

        private void CarregarSolicitacaoParaEdicao(int solicitacaoId)
        {
            solicitacaoAtual = repository.CarregarSolicitacao(solicitacaoId);

            if (solicitacaoAtual != null)
            {
                txtQtdFuncionarios.Text = solicitacaoAtual.QtdFuncionariosInicio.ToString();
                dtpData.Value = solicitacaoAtual.DataInicio;
                dtpHora.Value = DateTime.Today.Add(solicitacaoAtual.HoraInicio);
                txtBag.Text = solicitacaoAtual.BagsEntrada.ToString();
                txtObsInicio.Text = solicitacaoAtual.ObservacaoInicial;

                txtQtdFuncionariosFinal.Text = solicitacaoAtual.QtdFuncionariosFinal?.ToString() ?? "";
                dtpDataFinalizacao.Value = solicitacaoAtual.DataFinalizacao ?? DateTime.Today;
                dtpHoraFinal.Value = solicitacaoAtual.HoraFinalizacao.HasValue ?
                    DateTime.Today.Add(solicitacaoAtual.HoraFinalizacao.Value) : DateTime.Now;
                txtObsFinalizacao.Text = solicitacaoAtual.ObservacaoFinal ?? "";

                materiaisTemporarios = solicitacaoAtual.Materiais ?? new List<ProducaoMaterial>();
                AtualizarGridMateriais();

                HabilitarCamposInicio(true);
                HabilitarCamposFinalizacao(true);
                btnSalvar.Enabled = true;
            }
        }

        private void AtualizarGridMateriais()
        {
            dgvMaterial.DataSource = null;
            dgvMaterial.DataSource = new BindingList<ProducaoMaterial>(materiaisTemporarios);
        }

        private void LimparFormulario()
        {
            txtQtdFuncionarios.Clear();
            dtpData.Value = DateTime.Today;
            dtpHora.Value = DateTime.Now;
            txtBag.Clear();
            txtObsInicio.Clear();

            txtQtdFuncionariosFinal.Clear();
            dtpDataFinalizacao.Value = DateTime.Today;
            dtpHoraFinal.Value = DateTime.Now;
            txtObsFinalizacao.Clear();

            cbMateriais.SelectedIndex = -1;
            materiaisTemporarios.Clear();
            AtualizarGridMateriais();

            solicitacaoAtual = null;
        }

        private void HabilitarCamposInicio(bool habilitar)
        {
            txtQtdFuncionarios.Enabled = habilitar;
            dtpData.Enabled = habilitar;
            dtpHora.Enabled = habilitar;
            txtBag.Enabled = habilitar;
            txtObsInicio.Enabled = habilitar;
        }

        private void HabilitarCamposFinalizacao(bool habilitar)
        {
            txtQtdFuncionariosFinal.Enabled = habilitar;
            dtpDataFinalizacao.Enabled = habilitar;
            dtpHoraFinal.Enabled = habilitar;
            txtObsFinalizacao.Enabled = habilitar;
            cbMateriais.Enabled = habilitar;
            btnInserir.Enabled = habilitar;
            btnRemover.Enabled = habilitar;
            dgvMaterial.ReadOnly = !habilitar;
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            repository?.Dispose();
            base.OnFormClosing(e);
        }
    }
}