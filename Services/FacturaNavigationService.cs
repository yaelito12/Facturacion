// Services/FacturaNavigationService.cs
namespace Facturacion.Services
{
    public class FacturaNavigationService
    {
        private readonly HashSet<string> _tokensValidos = new();
        private readonly HashSet<int> _facturasAutorizadas = new();

        public string GenerarTokenNavegacion(int facturaId)
        {
            var token = Guid.NewGuid().ToString();
            _tokensValidos.Add($"{facturaId}_{token}");
            return token;
        }

        public bool ValidarYConsumir(int facturaId, string? token)
        {
            // Si ya está autorizada en esta sesión, permitir acceso
            if (_facturasAutorizadas.Contains(facturaId))
                return true;

            if (string.IsNullOrEmpty(token))
                return false;

            var key = $"{facturaId}_{token}";
            if (_tokensValidos.Contains(key))
            {
                _tokensValidos.Remove(key); // Consumir el token
                _facturasAutorizadas.Add(facturaId); // Autorizar para toda la sesión
                return true;
            }

            return false;
        }

        public void RevocarAutorizacion(int facturaId)
        {
            _facturasAutorizadas.Remove(facturaId);
        }

        public void LimpiarTokens()
        {
            _tokensValidos.Clear();
            _facturasAutorizadas.Clear();
        }
    }
}