using System;
using System.Linq;
using Xunit;

namespace Possible.Math.Test
{
	public class PPMT
	{
		[Fact]
		public void PPMT_Vindo_Cotizador()
		{
			var resultadoDecimal = System.Math.Round(Possible.Math.Financial.PPmt(0.025M/12, 1,24, -1000, 0),2);

			
			var resultadoDouble = System.Math.Round(Possible.Math.Financial.PPmt(0.025/12, 1,24, -1000, 0),2);

			Assert.Equal((decimal)resultadoDouble,resultadoDecimal);
			Assert.Equal(resultadoDouble,(double)resultadoDecimal);
			Assert.Equal(40.68M, resultadoDecimal);
		}
	}
}
