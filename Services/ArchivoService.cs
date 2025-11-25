namespace Facturacion.Services
{
    public class ArchivoService
    {
      
        private const string CONTRASEÑA_ARCHIVO = "archivados";

        public bool ValidarContraseña(string contraseña)
        {
            return contraseña == CONTRASEÑA_ARCHIVO;
        }
    }
}