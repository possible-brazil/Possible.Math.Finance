using System;
using System.Linq;
using Xunit;

namespace Possible.Math.Test
{
	public class PMT
	{
		[Fact]
		public void PMT_Vindo_Cotizador()
		{
			var resultadoDecimal = System.Math.Round(Possible.Math.Financial.Pmt(0.025M/12, 10, -1000, 0),2);

			
			var resultadoDouble = System.Math.Round(Possible.Math.Financial.Pmt(0.025/12, 10, -1000, 0),2);

			Assert.Equal((decimal)resultadoDouble,resultadoDecimal);
			Assert.Equal(resultadoDouble,(double)resultadoDecimal);
			Assert.Equal(101.15M, resultadoDecimal);
		}
	}
}
