using Reverse.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Reverse.Forms.FormsLogin
{
    public partial class FormConfigU : Form
    {
        private Usuario _usuarioSelecionado;
        private BindingList<PermissaoGridItem> _permissoes = new BindingList<PermissaoGridItem>();
        private static List<FormInfo> _formsCache = null;

        public FormConfigU()
        {
            InitializeComponent();
        }

        // Classe para armazenar informações do Form
        public class FormInfo
        {
            public string Nome { get; set; }
            public string Categoria { get; set; }
        }

        public class PermissaoGridItem
        {
            public string FormName { get; set; }
        }

        private void FormConfigU_Load(object sender, EventArgs e)
        {
            CarregarUsuarios();
            CarregarForms();

            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPermissao.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPermissao.DataSource = _permissoes;

            if (dgvUsuarios.Columns.Contains("Id"))
                dgvUsuarios.Columns["Id"].Visible = false;

            foreach (var grid in new[] { dgvUsuarios, dgvPermissao })
            {
                grid.DefaultCellStyle.ForeColor = Color.Black;
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
                grid.RowsDefaultCellStyle.BackColor = Color.White;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            }

            tvForms.KeyDown += TvForms_KeyDown;

            tvForms.NodeMouseDoubleClick += TvForms_NodeMouseDoubleClick;
        }

        private void CarregarUsuarios()
        {
            using (var ctx = new ReverseContext())
            {
                var usuarios = ctx.Usuarios
                    .Select(u => new { u.Id, u.UsuarioNome, u.Setor, u.DataCadastro })
                    .ToList();

                dgvUsuarios.DataSource = usuarios;
            }
        }

        public static class PermissaoHelper
        {
            public static bool TemPermissao(int usuarioId, string formName)
            {
                using (var ctx = new ReverseContext())
                {
                    return ctx.Permissoes.Any(p => p.UsuarioId == usuarioId &&
                                                   p.FormName == formName &&
                                                   p.PodeAcessar);
                }
            }
        }

        private void TvForms_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                AdicionarFormSelecionado();
            }
        }

        private void TvForms_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                AdicionarFormSelecionado();
            }
        }

        private void AdicionarFormSelecionado()
        {
            if (_usuarioSelecionado == null)
            {
                MessageBox.Show("Selecione um usuário primeiro.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tvForms.SelectedNode == null || tvForms.SelectedNode.Tag == null)
            {
                MessageBox.Show("Selecione um formulário (não uma categoria).", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string formName = tvForms.SelectedNode.Tag.ToString();

            if (_permissoes.Any(p => p.FormName == formName))
            {
                MessageBox.Show($"O formulário '{formName}' já está adicionado.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _permissoes.Add(new PermissaoGridItem { FormName = formName });

            MessageBox.Show($"Formulário '{formName}' adicionado! Não esqueça de SALVAR.",
                            "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void CarregarForms()
        {
            if (_formsCache == null)
            {
                _formsCache = typeof(MainForm).Assembly
                    .GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(Form)) &&
                               !t.IsAbstract &&
                               !t.Name.Contains("Designer") &&
                               t.Namespace != null)
                    .Select(t => new FormInfo
                    {
                        Nome = t.Name,
                        Categoria = ObterCategoria(t.Namespace)
                    })
                    .OrderBy(f => f.Categoria)
                    .ThenBy(f => f.Nome)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"=== TOTAL DE FORMS DETECTADOS: {_formsCache.Count} ===");
                foreach (var form in _formsCache)
                {
                    System.Diagnostics.Debug.WriteLine($"Form: {form.Nome} | Categoria: {form.Categoria}");
                }
            }

            tvForms.BeginUpdate();
            try
            {
                tvForms.Nodes.Clear();
                tvForms.ForeColor = Color.White;
                tvForms.BackColor = Color.FromArgb(45, 45, 48);

                var grupos = _formsCache.GroupBy(f => f.Categoria);
                foreach (var grupo in grupos)
                {
                    TreeNode nodeCategoria = new TreeNode($"{grupo.Key} ({grupo.Count()})")
                    {
                        ForeColor = Color.White,
                        NodeFont = new Font(tvForms.Font, FontStyle.Bold),
                        BackColor = ObterCorCategoria(grupo.Key),
                        Tag = null
                    };

                    foreach (var form in grupo)
                    {
                        TreeNode nodeForm = new TreeNode(form.Nome)
                        {
                            Tag = form.Nome,
                            ForeColor = Color.White
                        };
                        nodeCategoria.Nodes.Add(nodeForm);
                    }

                    tvForms.Nodes.Add(nodeCategoria);
                    nodeCategoria.Expand();
                }
            }
            finally
            {
                tvForms.EndUpdate();
            }
        }

        private string ObterCategoria(string nomeCompleto)
        {
            if (string.IsNullOrEmpty(nomeCompleto)) return "Outros";

            var partes = nomeCompleto.Split('.');

            for (int i = 0; i < partes.Length; i++)
            {
                if (partes[i] == "Forms" && i + 1 < partes.Length)
                {
                    string categoria = partes[i + 1];

                    if (categoria.StartsWith("Forms", StringComparison.OrdinalIgnoreCase))
                    {
                        return categoria;
                    }
                }
            }

            if (nomeCompleto.StartsWith("Reverse.Forms") && partes.Length == 2)
            {
                return "Forms";
            }

            return "Outros";
        }

        private Color ObterCorCategoria(string categoria)
        {
            // Cores mais escuras para texto branco ficar legível
            switch (categoria.ToLower())
            {
                case "formstriagem":
                    return Color.FromArgb(180, 140, 0); // Amarelo escuro
                case "formslogin":
                    return Color.FromArgb(30, 90, 150); // Azul escuro
                case "formscomercial":
                    return Color.FromArgb(0, 120, 120); // Ciano escuro
                case "formsexpedicao":
                    return Color.FromArgb(150, 80, 0); // Laranja escuro
                case "formsfinanceiro":
                    return Color.FromArgb(150, 50, 50); // Vermelho escuro
                case "formsrh":
                    return Color.FromArgb(100, 100, 100); // Cinza escuro
                case "formsestoque":
                    return Color.FromArgb(40, 120, 40); // Verde escuro
                case "formsrelatorios":
                    return Color.FromArgb(90, 50, 130); // Roxo escuro
                case "formsproducao":
                    return Color.FromArgb(120, 80, 40); // Marrom escuro
                case "formsvendas":
                    return Color.FromArgb(120, 0, 120); // Magenta escuro
                case "formscompras":
                    return Color.FromArgb(0, 100, 0); // Verde musgo
                default:
                    return Color.FromArgb(80, 80, 80); // Cinza padrão
            }
        }

        private void dgvForms_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dgvForms = (DataGridView)sender;
            if (dgvForms.Rows[e.RowIndex].DataBoundItem is FormInfo form)
            {
                Color corFundo = ObterCorCategoria(form.Categoria);
                dgvForms.Rows[e.RowIndex].DefaultCellStyle.BackColor = corFundo;
                dgvForms.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White; // TEXTO BRANCO

                // Cor de seleção um pouco mais clara
                dgvForms.Rows[e.RowIndex].DefaultCellStyle.SelectionBackColor =
                    Color.FromArgb(
                        Math.Min(255, corFundo.R + 40),
                        Math.Min(255, corFundo.G + 40),
                        Math.Min(255, corFundo.B + 40)
                    );
                dgvForms.Rows[e.RowIndex].DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.CurrentRow == null) return;

            int usuarioId = (int)dgvUsuarios.CurrentRow.Cells["Id"].Value;

            using (var ctx = new ReverseContext())
            {
                _usuarioSelecionado = ctx.Usuarios.Find(usuarioId);

                _permissoes.Clear();

                var permissoes = ctx.Permissoes
                    .Where(p => p.UsuarioId == usuarioId && p.PodeAcessar)
                    .Select(p => p.FormName)
                    .ToList();

                foreach (var formName in permissoes)
                {
                    _permissoes.Add(new PermissaoGridItem { FormName = formName });
                }
            }
        }

        private void btnAdicionar_Click(object sender, EventArgs e)
        {
            AdicionarFormSelecionado();
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (dgvPermissao.CurrentRow == null || _usuarioSelecionado == null) return;

            string formName = dgvPermissao.CurrentRow.Cells["FormName"].Value.ToString();
            var item = _permissoes.FirstOrDefault(p => p.FormName == formName);

            if (item != null)
            {
                _permissoes.Remove(item);

                using (var ctx = new ReverseContext())
                {
                    var permissao = ctx.Permissoes
                        .FirstOrDefault(p => p.UsuarioId == _usuarioSelecionado.Id &&
                                           p.FormName == formName);

                    if (permissao != null)
                    {
                        ctx.Permissoes.Remove(permissao);
                        ctx.SaveChanges();
                    }
                }
            }
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            if (_usuarioSelecionado == null) return;

            using (var ctx = new ReverseContext())
            {
                var user = ctx.Usuarios.Find(_usuarioSelecionado.Id);
                if (user != null)
                {
                    var permissoes = ctx.Permissoes.Where(p => p.UsuarioId == user.Id);
                    ctx.Permissoes.RemoveRange(permissoes);

                    ctx.Usuarios.Remove(user);
                    ctx.SaveChanges();
                }

                CarregarUsuarios();
                dgvPermissao.DataSource = null;
                _permissoes.Clear();
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (_usuarioSelecionado == null) return;

            using (var ctx = new ReverseContext())
            {
                var usuario = ctx.Usuarios.Find(_usuarioSelecionado.Id);

                // OTIMIZAÇÃO: Buscar todas as permissões existentes de uma vez
                var permissoesExistentes = ctx.Permissoes
                    .Where(p => p.UsuarioId == usuario.Id)
                    .Select(p => p.FormName)
                    .ToHashSet();

                foreach (var item in _permissoes)
                {
                    if (!permissoesExistentes.Contains(item.FormName))
                    {
                        ctx.Permissoes.Add(new Permissao
                        {
                            UsuarioId = usuario.Id,
                            FormName = item.FormName,
                            PodeAcessar = true
                        });
                    }
                }

                ctx.SaveChanges();
            }

            MessageBox.Show("Permissões salvas com sucesso!");
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}