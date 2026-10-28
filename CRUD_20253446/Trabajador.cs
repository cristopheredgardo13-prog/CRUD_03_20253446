using SQLite;
namespace CRUD_20253446;
[Table("trabajador")]
public class Trabajador
{
 [PrimaryKey, AutoIncrement] public int Id { get; set; }
 public string Nombre { get; set; } = "";
 public decimal Sueldo { get; set; }
 public decimal Aumento { get; set; }
 public decimal NuevoSueldo { get; set; }
 [Ignore] public string Detalle => $"Sueldo: ${Sueldo:0.00} · Aumento (10 %): ${Aumento:0.00}";
 [Ignore] public string TotalTexto => $"Nuevo sueldo: ${NuevoSueldo:0.00}";
}
