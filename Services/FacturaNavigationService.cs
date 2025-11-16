using Microsoft.JSInterop;

namespace Facturacion.Services
{
    public class FacturaNavigationService
    {
        private readonly IJSRuntime _js;
        private readonly Dictionary<int, string> _tokensEnMemoria = new();

        public FacturaNavigationService(IJSRuntime js)
        {
            _js = js;
        }

        public string GenerarTokenNavegacion(int facturaId)
        {
            var token = Guid.NewGuid().ToString();
            _tokensEnMemoria[facturaId] = token;

            Console.WriteLine($"[NavService] Token generado para factura {facturaId}: {token}");

            // Guardar también en sessionStorage de forma asíncrona pero sin esperar
            _ = Task.Run(async () =>
            {
                try
                {
                    await _js.InvokeVoidAsync("sessionStorage.setItem",
                        $"factura_token_{facturaId}", token);
                    Console.WriteLine($"[NavService] Token guardado en sessionStorage para factura {facturaId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NavService] Error guardando en sessionStorage: {ex.Message}");
                }
            });

            return token;
        }

        public async Task<bool> ValidarYAutorizarAsync(int facturaId, string? token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            // Primero verificar en memoria
            if (_tokensEnMemoria.TryGetValue(facturaId, out var tokenGuardado)
                && tokenGuardado == token)
            {
                return true;
            }

            // Si no está en memoria, verificar en sessionStorage
            try
            {
                var tokenStorage = await _js.InvokeAsync<string?>(
                    "eval", $"sessionStorage.getItem('factura_token_{facturaId}')");

                if (!string.IsNullOrEmpty(tokenStorage) && tokenStorage == token)
                {
                    _tokensEnMemoria[facturaId] = token; // Restaurar en memoria
                    return true;
                }
            }
            catch
            {
                // Ignorar errores de JS
            }

            return false;
        }

        public void RevocarAutorizacion(int facturaId)
        {
            _tokensEnMemoria.Remove(facturaId);

            // Eliminar de sessionStorage de forma asíncrona pero sin esperar
            _ = Task.Run(async () =>
            {
                try
                {
                    await _js.InvokeVoidAsync("sessionStorage.removeItem",
                        $"factura_token_{facturaId}");
                }
                catch
                {
                    // Ignorar errores de JS
                }
            });
        }
    }
}