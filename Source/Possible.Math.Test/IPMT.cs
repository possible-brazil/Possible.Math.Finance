using System;
using System.Linq;
using Xunit;

namespace Possible.Math.Test
{
	public class IPMT
	{
		[Fact]
		public void IPmt_Ok()
		{
			var resultadoDecimal = System.Math.Round(Possible.Math.Financial.IPmt(0.025M/12, 1,24, -10000, 0),2);

			
			var resultadoDouble = System.Math.Round(Possible.Math.Financial.IPmt(0.025/12, 1,24, -10000, 0),2);

			Assert.Equal((decimal)resultadoDouble,resultadoDecimal);
			Assert.Equal(resultadoDouble,(double)resultadoDecimal);
			Assert.Equal(20.83M, resultadoDecimal);
		}
	}
}
