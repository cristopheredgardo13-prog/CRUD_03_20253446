namespace CRUD_20253446;
public partial class MainPage : ContentPage
{
 private readonly LocalDbService _db;
 private int _editingId;
 private bool _busy;
 public MainPage(LocalDbService db) { InitializeComponent(); _db = db; }
 protected override async void OnAppearing() { base.OnAppearing(); await RunAsync(RefreshAsync); }
 private async Task RefreshAsync()
 {
  var items = await _db.GetAllAsync();
  Records.ItemsSource = items;
  StatusLabel.Text = $"Trabajadores guardados: {items.Count}";
 }
 private void SalaryChanged(object? sender, TextChangedEventArgs e)
 {
  if (PriceLabel is null || TotalLabel is null) return;
  if (CalculadoraSueldos.TrySueldo(SalaryEntry.Text, out var sueldo))
  {
   var aumento = decimal.Round(sueldo * 0.10m, 2, MidpointRounding.AwayFromZero);
   PriceLabel.Text = $"Aumento (10 %): ${aumento:0.00}";
   TotalLabel.Text = $"Nuevo sueldo: ${sueldo + aumento:0.00}";
  }
  else { PriceLabel.Text = "Aumento (10 %): —"; TotalLabel.Text = "Nuevo sueldo: —"; }
 }
 // Centraliza los errores y evita operaciones simultáneas por doble pulsación.
 private async Task RunAsync(Func<Task> action)
 {
  if (_busy) return;
  _busy = true; SaveButton.IsEnabled = false; CancelButton.IsEnabled = false; Records.IsEnabled = false;
  try { await action(); }
  catch (ArgumentException ex) { await DisplayAlert("Revisa los datos", ex.Message, "Aceptar"); }
  catch (Exception ex) { await DisplayAlert("No se pudo completar", ex.Message, "Aceptar"); }
  finally { _busy = false; SaveButton.IsEnabled = true; CancelButton.IsEnabled = true; Records.IsEnabled = true; }
 }
 private async void SaveClicked(object? sender, EventArgs e) => await RunAsync(async () =>
 {
  if (!CalculadoraSueldos.TrySueldo(SalaryEntry.Text, out var sueldo))
   throw new ArgumentException("Escribe un sueldo positivo de hasta 1,000,000,000 con máximo dos decimales; no uses separadores de miles.");
  var item = CalculadoraSueldos.Crear(NameEntry.Text, sueldo, _editingId);
  if (_editingId == 0) await _db.CreateAsync(item); else await _db.UpdateAsync(item);
  ResetForm(); await RefreshAsync();
 });
 private async void RecordSelected(object? sender, SelectionChangedEventArgs e)
 {
  if (e.CurrentSelection.FirstOrDefault() is not Trabajador item) return;
  Records.SelectedItem = null;
  await RunAsync(async () =>
  {
   var action = await DisplayActionSheet("Trabajador #" + item.Id, "Cancelar", "Eliminar", "Editar");
   if (action == "Editar")
   {
    _editingId = item.Id; NameEntry.Text = item.Nombre;
    SalaryEntry.Text = item.Sueldo.ToString(System.Globalization.CultureInfo.InvariantCulture);
    ModeLabel.Text = $"Editando trabajador #{item.Id}"; SaveButton.Text = "Actualizar"; CancelButton.Text = "Cancelar edición";
   }
   else if (action == "Eliminar" && await DisplayAlert("Eliminar trabajador", $"¿Eliminar al trabajador {item.Nombre}?", "Eliminar", "Cancelar"))
   {
    await _db.DeleteAsync(item); if (_editingId == item.Id) ResetForm(); await RefreshAsync();
   }
  });
 }
 private void CancelClicked(object? sender, EventArgs e) => ResetForm();
 private void ResetForm()
 {
  _editingId = 0; NameEntry.Text = ""; SalaryEntry.Text = "";
  PriceLabel.Text = "Aumento (10 %): —"; TotalLabel.Text = "Nuevo sueldo: —";
  ModeLabel.Text = "Nuevo trabajador"; SaveButton.Text = "Guardar"; CancelButton.Text = "Limpiar";
 }
}
