namespace TestGithub.Core
{
    /// <summary>
    /// Servicio de cálculo aritmético y validaciones para pruebas de calidad y cobertura.
    /// </summary>
    public class CalculadoraService
    {
        /// <summary>
        /// Obtiene el número total de operaciones ejecutadas.
        /// </summary>
        public int TotalOperaciones { get; private set; }

        /// <summary>
        /// Suma dos enteros.
        /// </summary>
        public int Sumar(int a, int b)
        {
            TotalOperaciones++;
            return a + b;
        }

        /// <summary>
        /// Resta dos enteros.
        /// </summary>
        public int Restar(int a, int b)
        {
            TotalOperaciones++;
            return a - b;
        }

        /// <summary>
        /// Multiplica dos enteros.
        /// </summary>
        public int Multiplicar(int a, int b)
        {
            TotalOperaciones++;
            return a * b;
        }

        /// <summary>
        /// Divide dos números. Lanza excepción si el divisor es cero.
        /// </summary>
        public double Dividir(double a, double b)
        {
            TotalOperaciones++;
            return b == 0 ? throw new DivideByZeroException("El divisor no puede ser cero.") : a / b;
        }

        /// <summary>
        /// Clasifica el número según su signo.
        /// </summary>
        public string ClasificarNumero(int numero)
        {
            TotalOperaciones++;
            return numero switch
            {
                > 0 => "Positivo",
                < 0 => "Negativo",
                _ => "Cero"
            };
        }
    }
}
