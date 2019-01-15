using Xunit;

namespace Possible.Math.Test
{
	public class IRR
	{
		[Fact]
		public void IRR_Vindo_Cotizador()
		{
			var arrDouble = new[]
			{
				-91045.53,
				3692.25,
				52110,
				2287.5,
				49822.5
			};

			var resultadoDouble = System.Math.Round((System.Math.Pow(1 +Possible.Math.Financial.IRR(ref arrDouble, 0.001),2)- 1) * 100, 2);
			
			var arrDecimal = new[]
			{
				-91045.53M,
				3692.25M,
				52110M,
				2287.5M,
				49822.5M
			};

			var resultadoDecimal = System.Math.Round(
				( (decimal)
					System.Math.Pow(
						(1.0 +
						 (double)Possible.Math.Financial.IRR(
							 ref arrDecimal, 
							 0.001M)
						 )
						,2.0)
					- 1) 
				* 100
			, 2);

			
			Assert.Equal((decimal)resultadoDouble,resultadoDecimal);
			Assert.Equal(resultadoDouble,(double)resultadoDecimal);
			Assert.Equal(12.53M, resultadoDecimal);
		}
	}
}
