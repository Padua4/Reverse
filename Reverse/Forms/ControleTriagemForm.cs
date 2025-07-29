using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Reverse
{
    public partial class ControleTriagemForm : Form
    {
        private readonly string _usuario;
        private const string ConnectionString =
            @"Server=.\SQLEXPRESS;Database=Reverse;Trusted_Connection=True;";

        public ControleTriagemForm(string usuario)
        {
            InitializeComponent();
            _usuario = usuario;

            // configurações iniciais do DateTimePicker
            dtpFiltroEmissao.Format = DateTimePickerFormat.Custom;
            dtpFiltroEmissao.CustomFormat = " ";         // não mostra data até o filtro ser ativado
            dtpFiltroEmissao.Value = DateTime.Today;

            // associa o CheckBox externo
            chkFiltroEmissao.Checked = false;
            chkFiltroEmissao.CheckedChanged += chkFiltroEmissao_CheckedChanged;
        }

        private void ControleTriagemForm_Load(object sender, EventArgs e)
        {
            lblGreeting.Text = $"Bem vindo ao Controle de Triagem, {_usuario}!";
            CarregarProdutosNoGrid();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            var main = Application.OpenForms
                                  .OfType<MainForm>()
                                  .FirstOrDefault();
            if (main != null)
                main.Show();

            this.Close();
        }

        private void btnImportXml_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog
            {
                Filter = "NFe XML (*.xml)|*.xml",
                Multiselect = true
            })
            {
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                int totalArquivos = dlg.FileNames.Length;
                int totalItens = 0;
                int totalNovos = 0;

                foreach (var arquivo in dlg.FileNames)
                {
                    NfeProc nfe;
                    try
                    {
                        nfe = CarregarNfeDoArquivo(arquivo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Erro ao ler '{Path.GetFileName(arquivo)}':\n{ex.Message}",
                            "Importação",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        continue;
                    }

                    string numeroNF = nfe.NFe.InfNFe.Ide.Numero;
                    DateTime emissaoDt = DateTime.Parse(nfe.NFe.InfNFe.Ide.DataHoraEmissao);

                    foreach (var det in nfe.NFe.InfNFe.Detalhes)
                    {
                        totalItens++;
                        if (SalvarProduto(det.Produto, numeroNF, emissaoDt))
                            totalNovos++;
                    }
                }

                CarregarProdutosNoGrid();

                MessageBox.Show(
                    $"Arquivos processados: {totalArquivos}\n" +
                    $"Itens encontrados: {totalItens}\n" +
                    $"Novos cadastrados: {totalNovos}",
                    "Importação Concluída",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                btnStartTriagem.Enabled = dgvNotas.Rows.Count > 0;
            }
        }

        private void btnStartTriagem_Click(object sender, EventArgs e)
        {
            var triagemForm = new TriagemForm
            {
                StartPosition = FormStartPosition.CenterScreen
            };

            Hide();
            triagemForm.FormClosed += (s, args) => Close();
            triagemForm.Show();
        }

        private NfeProc CarregarNfeDoArquivo(string path)
        {
            var serializer = new XmlSerializer(typeof(NfeProc));
            using (var stream = File.OpenRead(path))
            {
                return (NfeProc)serializer.Deserialize(stream);
            }
        }

        private bool SalvarProduto(Produto prod, string numeroNF, DateTime dataEmissao)
        {
            const string sql = @"
INSERT INTO Produtos (
    CodigoBarras,
    SKU,
    Descricao,
    Valor,
    NumeroNF,
    DataEmissao
) VALUES (
    @codBarras,
    @sku,
    @desc,
    @valor,
    @numNF,
    @dataEmis
);";

            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@codBarras", prod.CodigoBarras ?? "");
                cmd.Parameters.AddWithValue("@sku", prod.Codigo);
                cmd.Parameters.AddWithValue("@desc", prod.Descricao ?? "");
                cmd.Parameters.AddWithValue("@valor", prod.Valor);
                cmd.Parameters.AddWithValue("@numNF", numeroNF);
                cmd.Parameters.AddWithValue("@dataEmis", dataEmissao);

                try
                {
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex) when (ex.Number == 2627)
                {
                    // já existe (PK)
                    return false;
                }
            }
        }

        private void CarregarProdutosNoGrid()
        {
            dgvNotas.Rows.Clear();

            const string sql = @"
SELECT 
    NumeroNF,
    DataEmissao,
    CodigoBarras,
    SKU,
    Descricao,
    Valor
FROM Produtos;";

            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        dgvNotas.Rows.Add(
                            dr["NumeroNF"],
                            dr.GetDateTime(dr.GetOrdinal("DataEmissao")),
                            dr["CodigoBarras"],
                            dr["SKU"],
                            dr["Descricao"],
                            dr.GetDecimal(dr.GetOrdinal("Valor"))
                        );
                    }
                }
            }
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            var sql = @"
SELECT 
    NumeroNF,
    DataEmissao,
    CodigoBarras,
    SKU,
    Descricao,
    Valor
FROM Produtos
WHERE 1=1";
            var parametros = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(txtFiltroCodBarras.Text))
            {
                sql += " AND CodigoBarras = @codBarras";
                parametros.Add(new SqlParameter("@codBarras", txtFiltroCodBarras.Text.Trim()));
            }

            // só aplica filtro de data se o CheckBox estiver marcado
            if (chkFiltroEmissao.Checked)
            {
                sql += " AND CAST(DataEmissao AS DATE) = @dataEmis";
                parametros.Add(new SqlParameter("@dataEmis", dtpFiltroEmissao.Value.Date));
            }

            if (!string.IsNullOrWhiteSpace(txtFiltroDescricao.Text))
            {
                sql += " AND Descricao LIKE @desc";
                parametros.Add(new SqlParameter("@desc", "%" + txtFiltroDescricao.Text.Trim() + "%"));
            }

            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddRange(parametros.ToArray());
                cn.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    dgvFiltrados.Rows.Clear();
                    while (dr.Read())
                    {
                        dgvFiltrados.Rows.Add(
                            dr["NumeroNF"],
                            dr.GetDateTime(dr.GetOrdinal("DataEmissao")),
                            dr["CodigoBarras"],
                            dr["SKU"],
                            dr["Descricao"],
                            dr.GetDecimal(dr.GetOrdinal("Valor"))
                        );
                    }
                }
            }

            decimal total = 0;
            foreach (DataGridViewRow row in dgvFiltrados.Rows)
                total += Convert.ToDecimal(row.Cells["Valor"].Value);

            lblTotal.Text = $"Total: {total:C2}";
        }

        private void btnLimparFiltro_Click(object sender, EventArgs e)
        {
            txtFiltroCodBarras.Clear();
            txtFiltroDescricao.Clear();
            chkFiltroEmissao.Checked = false;
            dtpFiltroEmissao.Value = DateTime.Today;
            dgvFiltrados.Rows.Clear();
            lblTotal.Text = "Total: R$ 0,00";
        }

        private void chkFiltroEmissao_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFiltroEmissao.Checked)
                dtpFiltroEmissao.CustomFormat = "dd/MM/yyyy";
            else
                dtpFiltroEmissao.CustomFormat = " ";
        }

        
        private void dgvNotas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dtpFiltroEmissao_ValueChanged(object sender, EventArgs e)
        {
            
        }
    }
}
