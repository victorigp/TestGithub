using TestGithub.Core;

namespace TestGithub.Test
{
    /// <summary>
    /// Pruebas unitarias para verificar CalculadoraService y medir cobertura con Coverlet.
    /// </summary>
    public class CalculadoraServiceTests
    {
        private readonly CalculadoraService _servicio = new();

        /// <summary>
        /// Verifica que la suma de dos enteros devuelva el resultado correcto.
        /// </summary>
        [Fact]
        public void Sumar_DosNumeros_DevuelveSumaCorrecta()
        {
            int resultado = _servicio.Sumar(3, 5);
            Assert.Equal(8, resultado);
            Assert.Equal(1, _servicio.TotalOperaciones);
        }

        /// <summary>
        /// Verifica que la resta de dos enteros devuelva el resultado correcto.
        /// </summary>
        [Fact]
        public void Restar_DosNumeros_DevuelveRestaCorrecta()
        {
            int resultado = _servicio.Restar(10, 4);
            Assert.Equal(6, resultado);
        }

        /// <summary>
        /// Verifica que dividir por cero lance la excepción DivideByZeroException.
        /// </summary>
        [Fact]
        public void Dividir_PorCero_LanzaDivideByZeroException() =>
            _ = Assert.Throws<DivideByZeroException>(() => _servicio.Dividir(10, 0));

        /// <summary>
        /// Verifica que la división de dos números válidos devuelva el cociente correcto.
        /// </summary>
        [Fact]
        public void Dividir_NumerosValidos_DevuelveCociente()
        {
            double resultado = _servicio.Dividir(10, 2);
            Assert.Equal(5.0, resultado);
        }

        /// <summary>
        /// Verifica la clasificación de números positivos y negativos.
        /// </summary>
        [Theory]
        [InlineData(10, "Positivo")]
        [InlineData(-5, "Negativo")]
        public void ClasificarNumero_PositivoYNegativo_DevuelveEtiquetaCorrecta(int numero, string esperado)
        {
            string resultado = _servicio.ClasificarNumero(numero);
            Assert.Equal(esperado, resultado);
        }
    }
}
