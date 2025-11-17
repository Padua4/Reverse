using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Configuration;

namespace Reverse.Forms.FormsRH
{
    public partial class FormParticipantesAptos : Form
    {
        private string strConn = ConfigurationManager
        .ConnectionStrings["ReverseDB"]
        .ConnectionString;

        public FormParticipantesAptos(int _usuarioId)
        {
            InitializeComponent();
            PopularCombos();
            dgvAptos.DataBindingComplete += DgvAptos_DataBindingComplete;
            dgvAptos.RowsAdded += DgvAptos_RowsAdded;

            CarregarGrid();
        }

        private void PopularCombos()
        {
            cbbCategoria.Items.AddRange(new object[]
            {
                "Administrativo","Area Tecnica","Compras","Limpeza","Logistica",
                "Manutenção","Motorista","Produção","Segurança","T.I","Vigilante","Outros"
            });

            cbbCNH.Items.AddRange(new object[]
            {
                "Sim - categoria A","Sim - categoria B","Sim - categoria A/B",
                "Não possuo","Em andamento","Outra categoria"
            });
        }

        private void DgvAptos_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ColorirTodasAsLinhas();
        }

        private void DgvAptos_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                if (i >= 0 && i < dgvAptos.Rows.Count) ColorirLinha(dgvAptos.Rows[i]);
        }

        private void ColorirTodasAsLinhas()
        {
            foreach (DataGridViewRow row in dgvAptos.Rows)
                if (!row.IsNewRow) ColorirLinha(row);
        }

        private void CarregarGrid(string filtroNome = "", string categoria = "")
        {
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();
                string sql = @"SELECT Id, NomeCandidato, Categoria, DataCadastro, Interesse, Situacao
               FROM Curriculos
               WHERE Apto = 1";

                if (!string.IsNullOrWhiteSpace(filtroNome))
                    sql += " AND NomeCandidato LIKE @nome";

                if (!string.IsNullOrWhiteSpace(categoria))
                    sql += " AND Categoria = @cat";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (!string.IsNullOrWhiteSpace(filtroNome))
                        cmd.Parameters.AddWithValue("@nome", "%" + filtroNome + "%");

                    if (!string.IsNullOrWhiteSpace(categoria))
                        cmd.Parameters.AddWithValue("@cat", categoria);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgvAptos.DataSource = dt;

                        dgvAptos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        dgvAptos.AllowUserToAddRows = false;

                        dgvAptos.DefaultCellStyle.ForeColor = Color.Black;
                        dgvAptos.DefaultCellStyle.BackColor = Color.White;
                        dgvAptos.DefaultCellStyle.SelectionBackColor = Color.DarkBlue;
                        dgvAptos.DefaultCellStyle.SelectionForeColor = Color.White;

                        if (dgvAptos.Columns.Contains("Id"))
                            dgvAptos.Columns["Id"].Visible = false;
                        if (dgvAptos.Columns.Contains("Interesse"))
                            dgvAptos.Columns["Interesse"].Visible = false;
                        if (dgvAptos.Columns.Contains("Situacao"))
                            dgvAptos.Columns["Situacao"].Visible = false;

                    }
                }
            }
        }

        private void txtFiltroNome_TextChanged(object sender, EventArgs e) =>
            CarregarGrid(txtFiltroNome.Text, cbbCategoria.Text);

        private void cbbCategoria_SelectedIndexChanged(object sender, EventArgs e) =>
            CarregarGrid(txtFiltroNome.Text, cbbCategoria.Text);

        private void btnLimparFiltro_Click(object sender, EventArgs e)
        {
            txtFiltroNome.Clear();
            cbbCategoria.SelectedIndex = -1;
            CarregarGrid();
        }

        private void dgvAptos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAptos.CurrentRow == null) return;

            var cellValue = dgvAptos.CurrentRow.Cells["Id"].Value;
            if (cellValue == null || cellValue == DBNull.Value) return;

            int id = Convert.ToInt32(cellValue);

            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("SELECT *, Situacao FROM Curriculos WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            txtNome.Text = dr["NomeCandidato"].ToString();
                            dtpDataN.Value = dr["DataNascimento"] != DBNull.Value ? (DateTime)dr["DataNascimento"] : DateTime.Now;
                            nudIdade.Value = dr["Idade"] != DBNull.Value ? Convert.ToDecimal(dr["Idade"]) : 0;
                            txtRG.Text = dr["RG"].ToString();
                            txtCPF.Text = dr["CPF"].ToString();
                            txtTel.Text = dr["Telefone"].ToString();
                            txtEmail.Text = dr["Email"].ToString();
                            txtTrab.Text = dr["TrabalhaAtualmente"].ToString();
                            txtBen.Text = dr["Beneficio"].ToString();
                            txtUltSal.Text = dr["UltimoSalario"].ToString();
                            txtPreSal.Text = dr["PretensaoSalarial"].ToString();
                            cbbCNH.Text = dr["CNH"].ToString();
                            txtEsc.Text = dr["Escolaridade"].ToString();
                            txtIntTrab.Text = dr["PorqueInteresse"].ToString();
                            txtQuali.Text = dr["Qualidades"].ToString();
                            txtPressTrab.Text = dr["LidaPressao"].ToString();
                            txtConfTrab.Text = dr["ConflitosEquipe"].ToString();
                            txtMotTrab.Text = dr["Motivacao"].ToString();
                            txtSabDLD.Text = dr["OQueSabeEmpresa"].ToString();
                            txtConDLD.Text = dr["ConheciaEmpresa"].ToString();

                            // Já pega a situação aqui
                            string situacao = dr["Situacao"]?.ToString();
                            bool contratado = string.Equals(situacao, "Contratado", StringComparison.OrdinalIgnoreCase);
                            SetCamposEditaveis(!contratado);
                        }
                    }
                }
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (dgvAptos.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvAptos.CurrentRow.Cells["Id"].Value);

            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();
                string sql = @"UPDATE Curriculos SET
            NomeCandidato=@Nome, DataNascimento=@DataN, Idade=@Idade, RG=@RG, CPF=@CPF, Telefone=@Tel, Email=@Email,
            TrabalhaAtualmente=@Trab, Beneficio=@Ben, UltimoSalario=@UltSal, PretensaoSalarial=@PreSal, CNH=@CNH,
            Escolaridade=@Esc, PorqueInteresse=@IntTrab, Qualidades=@Quali, LidaPressao=@PressTrab,
            ConflitosEquipe=@ConfTrab, Motivacao=@MotTrab, OQueSabeEmpresa=@SabDLD, ConheciaEmpresa=@ConDLD
            WHERE Id=@Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Texto
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Nome", txtNome.Text);
                    cmd.Parameters.AddWithValue("@DataN", dtpDataN.Value);
                    cmd.Parameters.AddWithValue("@Idade", SafeInt(nudIdade.Value));
                    cmd.Parameters.AddWithValue("@RG", txtRG.Text);
                    cmd.Parameters.AddWithValue("@CPF", txtCPF.Text);
                    cmd.Parameters.AddWithValue("@Tel", txtTel.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);

                    // Trabalha atualmente (bit)
                    bool? trabBit = ToBool(txtTrab.Text);
                    cmd.Parameters.Add("@Trab", SqlDbType.Bit).Value = (object)trabBit ?? DBNull.Value;

                    cmd.Parameters.AddWithValue("@Ben", txtBen.Text);

                    // Decimais com conversão segura
                    cmd.Parameters.Add("@UltSal", SqlDbType.Decimal).Value = SafeDecimal(txtUltSal.Text);
                    cmd.Parameters.Add("@PreSal", SqlDbType.Decimal).Value = SafeDecimal(txtPreSal.Text);

                    cmd.Parameters.AddWithValue("@CNH", cbbCNH.Text);
                    cmd.Parameters.AddWithValue("@Esc", txtEsc.Text);
                    cmd.Parameters.AddWithValue("@IntTrab", txtIntTrab.Text);
                    cmd.Parameters.AddWithValue("@Quali", txtQuali.Text);
                    cmd.Parameters.AddWithValue("@PressTrab", txtPressTrab.Text);
                    cmd.Parameters.AddWithValue("@ConfTrab", txtConfTrab.Text);
                    cmd.Parameters.AddWithValue("@MotTrab", txtMotTrab.Text);
                    cmd.Parameters.AddWithValue("@SabDLD", txtSabDLD.Text);
                    cmd.Parameters.AddWithValue("@ConDLD", txtConDLD.Text);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Informações salvas com sucesso!");
            CarregarGrid(txtFiltroNome.Text, cbbCategoria.Text);
        }

        private int SafeInt(object valor)
        {
            try
            {
                return Convert.ToInt32(valor);
            }
            catch { return 0; }
        }

        private decimal SafeDecimal(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return 0m;

            // Remove símbolo de moeda e espaços
            string limpo = texto.Replace("R$", "").Trim();

            // Troca vírgula por ponto se necessário
            limpo = limpo.Replace(".", "").Replace(",", ".");

            if (decimal.TryParse(limpo, System.Globalization.NumberStyles.Any,
                                 System.Globalization.CultureInfo.InvariantCulture, out decimal resultado))
            {
                return resultado;
            }

            return 0m; // fallback
        }

        private bool? ToBool(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return null;
            var t = texto.Trim().ToLowerInvariant();
            if (t == "sim") return true;
            if (t == "não" || t == "nao") return false;
            return null;
        }

        private void MarcarInteresse(string interesse, string situacao)
        {
            if (dgvAptos.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvAptos.CurrentRow.Cells["Id"].Value);

            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE Curriculos SET Interesse=@interesse, Situacao=@situacao WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@interesse", interesse);
                    cmd.Parameters.AddWithValue("@situacao", situacao);
                    cmd.ExecuteNonQuery();
                }
            }

            CarregarGrid(txtFiltroNome.Text, cbbCategoria.Text);
        }

        private void btnInteresseVermelho_Click(object sender, EventArgs e)
            => MarcarInteresse("Vermelho", "Menos interesse");

        private void btnInteresseAmarelo_Click(object sender, EventArgs e)
            => MarcarInteresse("Amarelo", "Aguardando candidato");

        private void btnInteresseVerde_Click(object sender, EventArgs e)
            => MarcarInteresse("Verde", "Maior interesse");


        private void btnInapto_Click(object sender, EventArgs e)
        {
            if (dgvAptos.CurrentRow == null) return;

            string motivo = PromptMotivo();
            if (string.IsNullOrWhiteSpace(motivo)) return; // cancelou a operação

            int id = Convert.ToInt32(dgvAptos.CurrentRow.Cells["Id"].Value);

            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();

                // Marca como "Descartado" e registra motivo
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE Curriculos
                       SET Situacao = @sit,
                           MotivoCancelamento = @mot,
                           DataCancelamento = GETDATE(),
                           Apto = 0
                     WHERE Id = @Id;", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@mot", motivo);
                    cmd.Parameters.AddWithValue("@sit", "Descartado");
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Contrato cancelado e candidato movido para 'Descartado'.");
            CarregarGrid(txtFiltroNome.Text, cbbCategoria.Text);
        }

        private string PromptMotivo()
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Motivo do cancelamento",
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = "Informe o motivo:" };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340 };
            Button confirmation = new Button() { Text = "OK", Left = 280, Width = 80, Top = 90, DialogResult = DialogResult.OK };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : string.Empty;
        }

        private void btnContratar_Click(object sender, EventArgs e)
        {
            if (dgvAptos.CurrentRow == null) return;

            var confirm = MessageBox.Show(
                "Tem certeza que deseja contratar este candidato? As informações ficarão bloqueadas para edição.",
                "Confirmar Contratação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes) return;

            int id = Convert.ToInt32(dgvAptos.CurrentRow.Cells["Id"].Value);

            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE Curriculos
                       SET Situacao = @sit,
                           DataContratacao = GETDATE(),
                           Apto = 0,
                           Contratado = 1
                     WHERE Id = @Id;", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@sit", "Contratado");
                    cmd.ExecuteNonQuery();
                }
            }

            SetCamposEditaveis(false);
            dgvAptos.CurrentRow.ReadOnly = true;
            dgvAptos.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
            dgvAptos.CurrentRow.DefaultCellStyle.ForeColor = Color.DimGray;

            MessageBox.Show("Candidato contratado. Edição bloqueada.");
        }

        private void SetCamposEditaveis(bool habilitar)
        {
            txtNome.ReadOnly = !habilitar;
            dtpDataN.Enabled = habilitar;
            nudIdade.Enabled = habilitar;
            txtRG.ReadOnly = !habilitar;
            txtCPF.ReadOnly = !habilitar;
            txtTel.ReadOnly = !habilitar;
            txtEmail.ReadOnly = !habilitar;
            txtTrab.ReadOnly = !habilitar;
            txtBen.ReadOnly = !habilitar;
            txtUltSal.ReadOnly = !habilitar;
            txtPreSal.ReadOnly = !habilitar;
            cbbCNH.Enabled = habilitar;
            txtEsc.ReadOnly = !habilitar;
            txtIntTrab.ReadOnly = !habilitar;
            txtQuali.ReadOnly = !habilitar;
            txtPressTrab.ReadOnly = !habilitar;
            txtConfTrab.ReadOnly = !habilitar;
            txtMotTrab.ReadOnly = !habilitar;
            txtSabDLD.ReadOnly = !habilitar;
            txtConDLD.ReadOnly = !habilitar;
        }

        private void AplicarBloqueioPorSituacao(int id)
        {
            string situacao = null;

            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT Situacao FROM Curriculos WHERE Id = @Id;", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    var result = cmd.ExecuteScalar();
                    situacao = result?.ToString();
                }
            }

            bool contratado = string.Equals(situacao, "Contratado", StringComparison.OrdinalIgnoreCase);
            SetCamposEditaveis(!contratado);

            if (dgvAptos.CurrentRow != null)
            {
                dgvAptos.CurrentRow.ReadOnly = contratado;
                if (contratado)
                {
                    dgvAptos.CurrentRow.DefaultCellStyle.BackColor = Color.LightGray;
                    dgvAptos.CurrentRow.DefaultCellStyle.ForeColor = Color.DimGray;
                }
            }
        }

        private void ColorirLinha(DataGridViewRow row)
        {
            var s = Convert.ToString(row.Cells["Situacao"].Value)?.Trim().ToLowerInvariant();

            Color bg = Color.White;
            Color fg = Color.Black;
            var font = dgvAptos.Font;

            // Aceita variações para evitar erro por digitação
            if (s == "maior interesse" || s == "verde")
            {
                bg = Color.FromArgb(0, 200, 0);
                fg = Color.White;
                font = new Font(dgvAptos.Font, FontStyle.Bold);
            }
            else if (s == "aguardando candidato" || s == "amarelo")
            {
                bg = Color.FromArgb(255, 230, 0);
                fg = Color.Black;
                font = new Font(dgvAptos.Font, FontStyle.Bold);
            }
            else if (s == "menos interesse" || s == "menor interesse" || s == "vermelho")
            {
                bg = Color.FromArgb(255, 80, 80);
                fg = Color.White;
                font = new Font(dgvAptos.Font, FontStyle.Bold);
            }

            row.DefaultCellStyle.BackColor = bg;
            row.DefaultCellStyle.ForeColor = fg;
            row.DefaultCellStyle.SelectionBackColor = bg;   // mantém a cor quando selecionado
            row.DefaultCellStyle.SelectionForeColor = fg;
            row.DefaultCellStyle.Font = font;
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
