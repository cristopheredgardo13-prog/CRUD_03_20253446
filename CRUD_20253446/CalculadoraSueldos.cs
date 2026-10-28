using System.Globalization;
namespace CRUD_20253446;
// Ejercicio 28: aumento del 10 % para cada trabajador registrado.
public static class CalculadoraSueldos
{
 public static bool TrySueldo(string? texto, out decimal sueldo) =>
  decimal.TryParse(texto?.Trim().Replace(',', '.'), NumberStyles.AllowDecimalPoint,
   CultureInfo.InvariantCulture, out sueldo) && sueldo > 0 && sueldo <= 1000000000m
   && decimal.Round(sueldo, 2) == sueldo;
 public static Trabajador Crear(string? nombre, decimal sueldo, int id = 0)
 {
  var limpio = nombre?.Trim() ?? "";
  if (string.IsNullOrWhiteSpace(limpio) || limpio.Length > 120)
   throw new ArgumentException("Escribe el nombre del trabajador (máximo 120 caracteres).");
  if (sueldo <= 0 || sueldo > 1000000000m || decimal.Round(sueldo,2) != sueldo)
   throw new ArgumentException("Escribe un sueldo positivo con un máximo de dos decimales (hasta 1,000,000,000).");
  var aumento = decimal.Round(sueldo * 0.10m, 2, MidpointRounding.AwayFromZero);
  return new Trabajador {Id=id, Nombre=limpio, Sueldo=sueldo, Aumento=aumento, NuevoSueldo=sueldo+aumento};
 }
}
