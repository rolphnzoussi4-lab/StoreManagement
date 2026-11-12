using System.Drawing;
using System.Windows.Forms;
using StoreManagement.Application.Abstractions.Persistence;
using StoreManagement.Domain.Entities;
using StoreManagement.Infrastructure.Persistence.Sqlite;
using StoreManagement.Infrastructure.Persistence.Sqlite.Repositories;
using StoreManagement.Presentation.Theme;

namespace StoreManagement.Presentation.Views;

public sealed class CategoryForm : Form
{
    private readonly SQLiteConnectionFactory _connections;
    private readonly ICategoryRepository _categories;
    private readonly DataGridView _grid;

    public CategoryForm()
    {
        Text = "Catégories";
        BackColor = StorePalette.Background;
        Font = StorePalette.DefaultFont;
        _connections = new SQLiteConnectionFactory(Path.Combine(AppContext.BaseDirectory, "store.db"));
        _categories = new CategoryRepository(_connections);

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 58, Padding = new Padding(8) };
        var add = new Controls.ModernButton("Ajouter une catégorie", true);
        add.Click += async (_, _) => await AddCategoryAsync();
        var refresh = new Controls.ModernButton("Actualiser");
        refresh.Click += async (_, _) => await LoadAsync();
        toolbar.Controls.Add(add);
        toolbar.Controls.Add(refresh);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = StorePalette.Surface,
            BorderStyle = BorderStyle.None
        };
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "CategoryId" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nom", DataPropertyName = "Name" });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Description", DataPropertyName = "Description" });

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1, Padding = new Padding(8) };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(toolbar, 0, 0);
        layout.Controls.Add(_grid, 0, 1);
        Controls.Add(layout);
        Shown += async (_, _) => await InitializeAndLoadAsync();
    }

    private async Task InitializeAndLoadAsync()
    {
        try
        {
            await new DatabaseInitializer(_connections).InitializeAsync();
            await LoadAsync();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task LoadAsync()
    {
        try { _grid.DataSource = (await _categories.GetAllAsync()).ToList(); }
        catch (Exception ex) { ShowError(ex); }
    }

    private async Task AddCategoryAsync()
    {
        using var dialog = new Form { Text = "Nouvelle catégorie", Width = 420, Height = 230, StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
        var name = new TextBox { PlaceholderText = "Nom de la catégorie", Dock = DockStyle.Top, Margin = new Padding(12) };
        var description = new TextBox { PlaceholderText = "Description (facultative)", Dock = DockStyle.Top, Margin = new Padding(12) };
        var save = new Controls.ModernButton("Enregistrer", true) { DialogResult = DialogResult.OK, Anchor = AnchorStyles.Right };
        var cancel = new Controls.ModernButton("Annuler") { DialogResult = DialogResult.Cancel, Anchor = AnchorStyles.Right };
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 54, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        buttons.Controls.Add(cancel);
        buttons.Controls.Add(save);
        dialog.Controls.Add(buttons);
        dialog.Controls.Add(description);
        dialog.Controls.Add(name);
        dialog.AcceptButton = save;
        dialog.CancelButton = cancel;
        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            var category = new Category { Name = name.Text.Trim(), Description = string.IsNullOrWhiteSpace(description.Text) ? null : description.Text.Trim() };
            category.Validate();
            await _categories.AddAsync(category);
            await LoadAsync();
        }
        catch (Exception ex) { ShowError(ex); }
    }

    private void ShowError(Exception ex) => MessageBox.Show(ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
